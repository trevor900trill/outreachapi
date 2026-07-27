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

    public class ResendEmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<ResendEmailService> _logger;
        private readonly HttpClient _httpClient;

        public ResendEmailService(IConfiguration config, ILogger<ResendEmailService> logger, HttpClient httpClient)
        {
            _config = config;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string plainTextContent, string htmlContent, string? segment = null)
        {
            var apiKey = _config["Resend:ApiKey"];
            
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("Resend API Key is not configured. Email simulated: [{Subject}] to [{Email}]", subject, toEmail);
                return;
            }

            // Select sender credentials based on tour segment
            string fromEmail, fromDisplayName;

            if (string.Equals(segment, "Ultra Luxury", StringComparison.OrdinalIgnoreCase))
            {
                // fromEmail = _config["Smtp:Concierge:Email"] ?? "concierge@outreachtours.com";
                fromEmail = "concierge@markopilot.com"; // TEMPORARY: Using verified domain
                fromDisplayName = "Outreach Tours Concierge";
            }
            else
            {
                // fromEmail = _config["Smtp:Journeys:Email"] ?? "journeys@outreachtours.com";
                fromEmail = "journeys@markopilot.com"; // TEMPORARY: Using verified domain
                fromDisplayName = "Outreach Tours Journeys";
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                var payload = new
                {
                    from = $"{fromDisplayName} <{fromEmail}>",
                    to = new[] { toEmail },
                    subject = subject,
                    html = htmlContent,
                    text = plainTextContent
                };

                request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email sent successfully via Resend: [{Subject}] to [{Email}] from [{From}]", subject, toEmail, fromEmail);
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to send email via Resend. Status: {Status}, Details: {Details}", response.StatusCode, errorDetails);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while sending email [{Subject}] to [{Email}] via Resend", subject, toEmail);
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
