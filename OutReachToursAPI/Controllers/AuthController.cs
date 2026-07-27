using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutReachToursAPI.Data;
using OutReachToursAPI.Models;
using System.Security.Cryptography;
using System.Text;

namespace OutReachToursAPI.Controllers
{
    public class ResetPasswordDto
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ForgotPasswordDto
    {
        public string Email { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly OutReachToursAPI.Services.IEmailService _emailService;

        public AuthController(AppDbContext context, IConfiguration config, OutReachToursAPI.Services.IEmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var hash = ComputeHash(loginDto.Password);
            if (user.PasswordHash != hash)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // Return user without password hash
            var safeUser = new User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RoleId = user.RoleId,
                Avatar = user.Avatar,
                ActiveLeads = user.ActiveLeads,
                ConversionRate = user.ConversionRate
            };

            return Ok(safeUser);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrEmpty(dto.NewPassword))
            {
                return BadRequest(new { message = "Token and new password are required." });
            }

            if (dto.NewPassword.Length < 6)
            {
                return BadRequest(new { message = "Password must be at least 6 characters long." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == dto.Token);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid or expired reset link." });
            }

            if (user.PasswordResetTokenExpiry.HasValue && user.PasswordResetTokenExpiry.Value < DateTime.UtcNow)
            {
                return BadRequest(new { message = "This reset link has expired. Please ask your administrator to send a new invite." });
            }

            user.PasswordHash = ComputeHash(dto.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password has been set successfully. You can now log in." });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest(new { message = "Email is required." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
            {
                // Always return Ok to prevent email enumeration attacks
                return Ok(new { message = "If an account with that email exists, a password reset link has been sent." });
            }

            var resetToken = Guid.NewGuid().ToString("N");
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(2);

            await _context.SaveChangesAsync();

            var frontendUrl = (_config["FRONTEND_URL"] ?? "https://outreach-admin-seven.vercel.app").TrimEnd('/');
            var resetUrl = $"{frontendUrl}/reset-password?token={resetToken}";

            var (plain, html) = OutReachToursAPI.Services.EmailTemplates.GetForgotPasswordEmail(user.Name, resetUrl);
            await _emailService.SendEmailAsync(user.Email, "Password Reset Request", plain, html);

            return Ok(new { message = "If an account with that email exists, a password reset link has been sent." });
        }

        public static string ComputeHash(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
