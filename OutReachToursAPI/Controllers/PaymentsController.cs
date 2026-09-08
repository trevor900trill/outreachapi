using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutReachToursAPI.Data;
using OutReachToursAPI.Models;
using OutReachToursAPI.Services;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OutReachToursAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            AppDbContext context,
            IPaymentService paymentService,
            IEmailService emailService,
            IConfiguration config,
            ILogger<PaymentsController> logger)
        {
            _context = context;
            _paymentService = paymentService;
            _emailService = emailService;
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// Paystack webhook endpoint. Receives charge.success events and auto-confirms hotel bookings.
        /// Configure this URL in Paystack Dashboard → Settings → Webhooks.
        /// </summary>
        [HttpPost("webhook")]
        public async Task<IActionResult> PaystackWebhook()
        {
            // Read the raw body
            string requestBody;
            using (var reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            // Verify Paystack signature
            var secretKey = _config["Paystack:SecretKey"];
            if (!string.IsNullOrEmpty(secretKey))
            {
                var paystackSignature = Request.Headers["x-paystack-signature"].FirstOrDefault();
                if (!string.IsNullOrEmpty(paystackSignature))
                {
                    using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey));
                    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(requestBody));
                    var computedSignature = BitConverter.ToString(computedHash).Replace("-", "").ToLower();

                    if (computedSignature != paystackSignature)
                    {
                        _logger.LogWarning("Invalid Paystack webhook signature");
                        return Unauthorized("Invalid signature");
                    }
                }
            }

            try
            {
                var payload = JsonDocument.Parse(requestBody);
                var eventType = payload.RootElement.GetProperty("event").GetString();

                if (eventType == "charge.success")
                {
                    var data = payload.RootElement.GetProperty("data");
                    var reference = data.GetProperty("reference").GetString();
                    var amountKobo = data.GetProperty("amount").GetInt64();
                    var amountKES = amountKobo / 100.0; // Paystack sends amount in kobo/cents

                    _logger.LogInformation("Paystack charge.success received for reference: {Reference}, amount: {Amount}", reference, amountKES);

                    // Find the booking by voucher number (used as Paystack reference)
                    var booking = await _context.HotelBookings
                        .FirstOrDefaultAsync(b => b.VoucherNumber == reference || b.PaystackReference == reference);

                    if (booking != null)
                    {
                        booking.Status = "Confirmed";
                        booking.PaidAmountKES = amountKES;
                        booking.PaystackReference = reference;

                        await _context.SaveChangesAsync();

                        _logger.LogInformation("Booking {BookingId} auto-confirmed via Paystack payment. Voucher: {Voucher}", booking.Id, booking.VoucherNumber);

                        // Create system notification
                        var notification = new Notification
                        {
                            Id = $"n_{Guid.NewGuid():N}",
                            Title = "Payment Received",
                            Message = $"Payment of KES {amountKES:N0} received for {booking.HotelName} (Voucher: {booking.VoucherNumber}). Booking auto-confirmed.",
                            Time = DateTime.UtcNow.ToString("o"),
                            Read = false
                        };
                        _context.Notifications.Add(notification);
                        await _context.SaveChangesAsync();

                        // Send confirmation email to client
                        if (!string.IsNullOrEmpty(booking.ClientId) && booking.ClientId != "walk-in")
                        {
                            var client = await _context.Clients.FindAsync(booking.ClientId);
                            if (client != null && !string.IsNullOrEmpty(client.Email))
                            {
                                var (plain, html) = EmailTemplates.GetBookingConfirmedEmail(
                                    client.Name, booking.HotelName, booking.RoomTypeName,
                                    booking.CheckInDate, booking.CheckOutDate,
                                    booking.TotalAmountKES, booking.VoucherNumber);

                                await _emailService.SendEmailAsync(client.Email,
                                    $"Booking Confirmed — {booking.HotelName} ({booking.VoucherNumber})",
                                    plain, html);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No booking found for Paystack reference: {Reference}", reference);
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Paystack webhook");
                return Ok(); // Always return 200 to Paystack to avoid retries
            }
        }

        /// <summary>
        /// Generate a Paystack payment link for a specific hotel booking.
        /// </summary>
        [HttpPost("initialize/{bookingId}")]
        public async Task<ActionResult> InitializePayment(string bookingId)
        {
            var booking = await _context.HotelBookings.FindAsync(bookingId);
            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            var client = await _context.Clients.FindAsync(booking.ClientId);
            var clientEmail = client?.Email ?? "guest@outreachtours.com";

            try
            {
                // Use voucher number as the payment reference so webhook can find the booking
                var paymentUrl = await _paymentService.CreatePaymentLinkAsync(
                    clientEmail, (int)booking.TotalAmountKES, booking.VoucherNumber);

                if (!string.IsNullOrEmpty(paymentUrl))
                {
                    booking.PaymentUrl = paymentUrl;
                    booking.PaystackReference = booking.VoucherNumber;
                    await _context.SaveChangesAsync();

                    return Ok(new { paymentUrl, reference = booking.VoucherNumber });
                }

                return StatusCode(500, new { message = "Failed to generate payment link" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Paystack payment for booking {BookingId}", bookingId);
                return StatusCode(500, new { message = "Payment initialization failed", error = ex.Message });
            }
        }
    }
}
