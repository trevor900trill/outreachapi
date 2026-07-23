using System.Net;
using System.Net.Mail;
using Paystack.Net.SDK.Transactions;

namespace OutReachToursAPI.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email. The segment parameter determines which sender account to use:
        /// "Ultra Luxury" uses concierge@outreachtours.com, anything else uses journeys@outreachtours.com.
        /// If segment is null, defaults to journeys@.
        /// </summary>
        Task SendEmailAsync(string toEmail, string subject, string plainTextContent, string htmlContent, string? segment = null);
    }

    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string plainTextContent, string htmlContent, string? segment = null)
        {
            var host = _config["Smtp:Host"];
            var port = int.Parse(_config["Smtp:Port"] ?? "465");
            var enableSsl = bool.Parse(_config["Smtp:EnableSsl"] ?? "true");

            // Select sender credentials based on tour segment
            string fromEmail, fromPassword, fromDisplayName;

            if (string.Equals(segment, "Ultra Luxury", StringComparison.OrdinalIgnoreCase))
            {
                fromEmail = _config["Smtp:Concierge:Email"] ?? "concierge@outreachtours.com";
                fromPassword = _config["Smtp:Concierge:Password"] ?? "";
                fromDisplayName = "Outreach Tours Concierge";
            }
            else
            {
                fromEmail = _config["Smtp:Journeys:Email"] ?? "journeys@outreachtours.com";
                fromPassword = _config["Smtp:Journeys:Password"] ?? "";
                fromDisplayName = "Outreach Tours Journeys";
            }

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(fromPassword))
            {
                _logger.LogWarning("SMTP is not configured. Email simulated: [{Subject}] to [{Email}] from [{From}]", subject, toEmail, fromEmail);
                return;
            }

            try
            {
                using var message = new MailMessage();
                message.From = new MailAddress(fromEmail, fromDisplayName);
                message.To.Add(new MailAddress(toEmail));
                message.Subject = subject;
                message.Body = htmlContent;
                message.IsBodyHtml = true;

                // Add plain-text alternative view for email clients that prefer it
                if (!string.IsNullOrEmpty(plainTextContent))
                {
                    var plainView = AlternateView.CreateAlternateViewFromString(plainTextContent, null, "text/plain");
                    var htmlView = AlternateView.CreateAlternateViewFromString(htmlContent, null, "text/html");
                    message.AlternateViews.Add(plainView);
                    message.AlternateViews.Add(htmlView);
                    message.Body = null; // Let the alternate views handle it
                }

                using var client = new SmtpClient(host, port);
                client.Credentials = new NetworkCredential(fromEmail, fromPassword);
                client.EnableSsl = enableSsl;

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent: [{Subject}] to [{Email}] from [{From}]", subject, toEmail, fromEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email [{Subject}] to [{Email}]", subject, toEmail);
            }
        }
    }

    public interface IPaymentService
    {
        Task<string> CreatePaymentLinkAsync(string email, int amountKes, string reference);
    }

    public class PaystackPaymentService : IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<PaystackPaymentService> _logger;

        public PaystackPaymentService(IConfiguration config, ILogger<PaystackPaymentService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<string> CreatePaymentLinkAsync(string email, int amountKes, string reference)
        {
            var secretKey = _config["Paystack:SecretKey"];
            if (string.IsNullOrEmpty(secretKey) || secretKey == "YOUR_PAYSTACK_SECRET")
            {
                _logger.LogWarning("Paystack Secret Key is missing. Simulating payment link for {Reference}", reference);
                return $"https://paystack.com/pay/simulated_link_{reference}";
            }

            try
            {
                // Paystack amount is in kobo/cents. For KES, multiply by 100
                var api = new PaystackTransaction(secretKey);
                var response = await api.InitializeTransaction(email, amountKes * 100);

                if (response.status)
                {
                    return response.data.authorization_url;
                }

                _logger.LogError("Failed to generate Paystack link: {Message}", response.message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Paystack API call failed for {Reference}", reference);
            }

            return string.Empty;
        }
    }
}
