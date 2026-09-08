using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutReachToursAPI.Data;
using OutReachToursAPI.Models;
using OutReachToursAPI.Services;

namespace OutReachToursAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHotelAvailabilityService _hotelService;
        private readonly IEmailService _emailService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<HotelsController> _logger;

        public HotelsController(
            AppDbContext context, 
            IHotelAvailabilityService hotelService, 
            IEmailService emailService,
            IPaymentService paymentService,
            ILogger<HotelsController> logger)
        {
            _context = context;
            _hotelService = hotelService;
            _emailService = emailService;
            _paymentService = paymentService;
            _logger = logger;
        }


        /// <summary>
        /// Real-time live room availability search via Booking.com / RapidAPI live engine.
        /// </summary>
        [HttpGet("live-search")]
        public async Task<ActionResult<LiveHotelAvailabilityResult>> LiveSearch([FromQuery] LiveAvailabilityQuery query)
        {
            try
            {
                var result = await _hotelService.CheckAvailabilityAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing live hotel search for {CityCode}", query.CityCode);
                return StatusCode(500, new { message = "Failed to query live hotel room availability", error = ex.Message });
            }
        }

        /// <summary>
        /// Quick-select popular travel destinations across Kenya and East Africa.
        /// </summary>
        [HttpGet("destinations")]
        public ActionResult<IEnumerable<DestinationInfo>> GetDestinations()
        {
            return Ok(_hotelService.GetPopularDestinations());
        }

        /// <summary>
        /// Get all active calendar hotel room bookings with budget analysis per client.
        /// </summary>
        [HttpGet("bookings")]
        public async Task<ActionResult> GetBookings()
        {
            var bookings = await _context.HotelBookings
                .OrderBy(b => b.CheckInDate)
                .ToListAsync();

            // Get unique client IDs to fetch budget data
            var clientIds = bookings.Select(b => b.ClientId).Where(id => !string.IsNullOrEmpty(id) && id != "walk-in").Distinct().ToList();
            var clients = await _context.Clients
                .Where(c => clientIds.Contains(c.Id))
                .Select(c => new { c.Id, c.BudgetKES, c.Email })
                .ToListAsync();

            // Calculate total spend per client across all bookings
            var spendByClient = bookings
                .Where(b => !string.IsNullOrEmpty(b.ClientId) && b.ClientId != "walk-in")
                .GroupBy(b => b.ClientId)
                .ToDictionary(g => g.Key, g => g.Sum(b => b.TotalAmountKES));

            var enrichedBookings = bookings.Select(b =>
            {
                var client = clients.FirstOrDefault(c => c.Id == b.ClientId);
                var clientBudget = client?.BudgetKES ?? 0;
                var totalSpent = spendByClient.GetValueOrDefault(b.ClientId, 0);
                var budgetRemaining = clientBudget - totalSpent;

                bool isExpired = false;
                double hoursRemaining = 0;
                if (b.Status == "Provisional Hold" && !string.IsNullOrEmpty(b.HoldExpiryDate))
                {
                    if (DateTime.TryParse(b.HoldExpiryDate, out var expiry))
                    {
                        var diff = expiry - DateTime.UtcNow;
                        hoursRemaining = Math.Round(diff.TotalHours, 1);
                        isExpired = diff.TotalSeconds <= 0;
                    }
                }

                return new
                {
                    b.Id,
                    b.HotelId,
                    b.HotelName,
                    b.RoomTypeName,
                    b.ClientId,
                    b.ClientName,
                    b.TourId,
                    b.TourTitle,
                    b.CheckInDate,
                    b.CheckOutDate,
                    b.RoomsCount,
                    b.GuestsCount,
                    b.Status,
                    b.HoldExpiryDate,
                    b.VoucherNumber,
                    b.RatePerNightKES,
                    b.TotalAmountKES,
                    b.PaidAmountKES,
                    b.PaystackReference,
                    b.PaymentUrl,
                    b.HotelContactEmail,
                    b.HotelNotifiedAt,
                    b.Notes,
                    b.CreatedAt,
                    // Expiry analysis
                    IsExpired = isExpired,
                    HoursRemaining = hoursRemaining,
                    // Budget analysis
                    ClientBudgetKES = clientBudget,
                    ClientTotalSpentKES = totalSpent,
                    BudgetRemainingKES = budgetRemaining,
                    IsOverBudget = clientBudget > 0 && budgetRemaining < 0,
                    ClientEmail = client?.Email
                };
            });

            return Ok(enrichedBookings);
        }

        /// <summary>
        /// Book a hotel room or create a provisional reservation hold for a client.
        /// Sends hold notification email to client informing them of the reservation under their trip package.
        /// </summary>
        [HttpPost("bookings")]
        public async Task<ActionResult<HotelBooking>> CreateBooking(HotelBooking booking)
        {
            if (string.IsNullOrWhiteSpace(booking.Id))
            {
                booking.Id = $"hb_{Guid.NewGuid():N}";
            }

            if (string.IsNullOrWhiteSpace(booking.VoucherNumber))
            {
                booking.VoucherNumber = $"VCH-{DateTime.UtcNow:yyyyMM}-{Random.Shared.Next(1000, 9999)}";
            }

            if (string.IsNullOrWhiteSpace(booking.CreatedAt))
            {
                booking.CreatedAt = DateTime.UtcNow.ToString("o");
            }

            if (booking.Status == "Provisional Hold" && string.IsNullOrWhiteSpace(booking.HoldExpiryDate))
            {
                booking.HoldExpiryDate = DateTime.UtcNow.AddHours(48).ToString("o");
            }

            _context.HotelBookings.Add(booking);
            await _context.SaveChangesAsync();

            // Send hold notification email to client (informing them that reservation is held under their trip package)
            if (!string.IsNullOrEmpty(booking.ClientId) && booking.ClientId != "walk-in")
            {
                var client = await _context.Clients.FindAsync(booking.ClientId);
                if (client != null && !string.IsNullOrEmpty(client.Email))
                {
                    try
                    {
                        var (plain, html) = EmailTemplates.GetHoldNotificationEmail(
                            client.Name, booking.HotelName, booking.RoomTypeName,
                            booking.CheckInDate, booking.CheckOutDate,
                            booking.TotalAmountKES, booking.VoucherNumber);

                        await _emailService.SendEmailAsync(client.Email,
                            $"Room Hold Reserved — {booking.HotelName} ({booking.VoucherNumber})",
                            plain, html);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to send hold notification email for booking {BookingId}", booking.Id);
                    }
                }
            }

            // Create system notification
            var notification = new Notification
            {
                Id = $"n_{Guid.NewGuid():N}",
                Title = "New Hotel Hold",
                Message = $"{booking.Status} created for {booking.ClientName} at {booking.HotelName} (Voucher: {booking.VoucherNumber})",
                Time = DateTime.UtcNow.ToString("o"),
                Read = false
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookings), new { id = booking.Id }, booking);
        }

        /// <summary>
        /// Update booking status (Confirm, Check In, Complete, Cancel).
        /// Sends confirmation email on status change to "Confirmed".
        /// </summary>
        [HttpPatch("bookings/{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(string id, [FromBody] BookingStatusUpdate update)
        {
            var booking = await _context.HotelBookings.FindAsync(id);
            if (booking == null) return NotFound(new { message = "Booking not found" });

            var oldStatus = booking.Status;
            booking.Status = update.Status;

            // If manually confirmed, mark as paid with the full amount
            if (update.Status == "Confirmed" && booking.PaidAmountKES == 0)
            {
                booking.PaidAmountKES = booking.TotalAmountKES;
            }

            await _context.SaveChangesAsync();

            // Send confirmation email if status changed to Confirmed
            if (update.Status == "Confirmed" && oldStatus != "Confirmed")
            {
                if (!string.IsNullOrEmpty(booking.ClientId) && booking.ClientId != "walk-in")
                {
                    var client = await _context.Clients.FindAsync(booking.ClientId);
                    if (client != null && !string.IsNullOrEmpty(client.Email))
                    {
                        try
                        {
                            var (plain, html) = EmailTemplates.GetBookingConfirmedEmail(
                                client.Name, booking.HotelName, booking.RoomTypeName,
                                booking.CheckInDate, booking.CheckOutDate,
                                booking.TotalAmountKES, booking.VoucherNumber);

                            await _emailService.SendEmailAsync(client.Email,
                                $"Booking Confirmed — {booking.HotelName} ({booking.VoucherNumber})",
                                plain, html);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to send confirmation email for booking {BookingId}", booking.Id);
                        }
                    }
                }

                // Create system notification
                var notification = new Notification
                {
                    Id = $"n_{Guid.NewGuid():N}",
                    Title = "Booking Confirmed",
                    Message = $"{booking.HotelName} booking for {booking.ClientName} confirmed (Voucher: {booking.VoucherNumber})",
                    Time = DateTime.UtcNow.ToString("o"),
                    Read = false
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = $"Status updated from '{oldStatus}' to '{update.Status}'", booking });
        }

        /// <summary>
        /// Send official Agency Room Booking Order / Voucher to the partner hotel's reservation desk.
        /// </summary>
        [HttpPost("bookings/{id}/notify-hotel")]
        public async Task<IActionResult> NotifyHotelDesk(string id, [FromBody] NotifyHotelRequest? request)
        {
            var booking = await _context.HotelBookings.FindAsync(id);
            if (booking == null) return NotFound(new { message = "Booking not found" });

            string? recipientEmail = request?.RecipientEmail;

            // If no custom email supplied, look up hotel record
            if (string.IsNullOrWhiteSpace(recipientEmail) && !string.IsNullOrEmpty(booking.HotelId))
            {
                var hotel = await _context.Hotels.FindAsync(booking.HotelId);
                recipientEmail = !string.IsNullOrWhiteSpace(hotel?.ReservationDesk) 
                    ? hotel.ReservationDesk 
                    : hotel?.ContactEmail;
            }

            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                return BadRequest(new { message = "Recipient hotel desk email is required. Please specify a destination email address." });
            }

            try
            {
                var (plain, html) = EmailTemplates.GetHotelDeskBookingOrderEmail(
                    booking.HotelName, booking.VoucherNumber, booking.ClientName,
                    booking.GuestsCount, booking.RoomsCount, booking.RoomTypeName,
                    booking.CheckInDate, booking.CheckOutDate, booking.TotalAmountKES,
                    booking.Notes);

                await _emailService.SendEmailAsync(recipientEmail,
                    $"Agency Booking Order — {booking.HotelName} ({booking.VoucherNumber})",
                    plain, html);

                booking.HotelContactEmail = recipientEmail;
                booking.HotelNotifiedAt = DateTime.UtcNow.ToString("o");
                await _context.SaveChangesAsync();

                // Create system notification
                _context.Notifications.Add(new Notification
                {
                    Id = $"n_{Guid.NewGuid():N}",
                    Title = "Hotel Desk Order Sent",
                    Message = $"Agency booking order for {booking.ClientName} at {booking.HotelName} sent to {recipientEmail} (Voucher: {booking.VoucherNumber})",
                    Time = DateTime.UtcNow.ToString("o"),
                    Read = false
                });
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Agency booking order sent to {recipientEmail}", booking });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send hotel desk order email for booking {BookingId}", booking.Id);
                return StatusCode(500, new { message = "Failed to send hotel desk order email", error = ex.Message });
            }
        }

        /// <summary>
        /// Extend a provisional room hold by additional hours (default 24h).
        /// </summary>
        [HttpPost("bookings/{id}/extend-hold")]
        public async Task<IActionResult> ExtendHold(string id, [FromBody] ExtendHoldRequest? request)
        {
            var booking = await _context.HotelBookings.FindAsync(id);
            if (booking == null) return NotFound(new { message = "Booking not found" });

            int hours = request?.AdditionalHours > 0 ? request.AdditionalHours : 24;

            DateTime baseDate = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(booking.HoldExpiryDate) && DateTime.TryParse(booking.HoldExpiryDate, out var currentExpiry))
            {
                if (currentExpiry > DateTime.UtcNow)
                {
                    baseDate = currentExpiry;
                }
            }

            var newExpiry = baseDate.AddHours(hours);
            booking.HoldExpiryDate = newExpiry.ToString("o");
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Hold extended by {hours} hours. New expiry: {newExpiry:yyyy-MM-dd HH:mm} UTC", booking });
        }

        /// <summary>
        /// Cancel or release a hotel room hold.
        /// </summary>
        [HttpDelete("bookings/{id}")]
        public async Task<IActionResult> DeleteBooking(string id)
        {
            var booking = await _context.HotelBookings.FindAsync(id);
            if (booking == null) return NotFound(new { message = "Booking not found" });

            _context.HotelBookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Get saved contracted partner properties.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hotel>>> GetHotels()
        {
            return await _context.Hotels
                .OrderBy(h => h.Destination)
                .ThenBy(h => h.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Add or register a partner hotel in the system database.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Hotel>> CreateHotel(Hotel hotel)
        {
            if (string.IsNullOrWhiteSpace(hotel.Id))
            {
                hotel.Id = $"h_{Guid.NewGuid():N}";
            }

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHotels), new { id = hotel.Id }, hotel);
        }
    }

    /// <summary>
    /// DTO for booking status updates.
    /// </summary>
    public class BookingStatusUpdate
    {
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for notifying hotel desk.
    /// </summary>
    public class NotifyHotelRequest
    {
        public string? RecipientEmail { get; set; }
    }

    /// <summary>
    /// DTO for extending a room hold.
    /// </summary>
    public class ExtendHoldRequest
    {
        public int AdditionalHours { get; set; } = 24;
    }
}
