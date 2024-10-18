using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RF1.Models;
using RF1.Services.EmailService;
using RF1.Services.UserAccessorService;

namespace RF1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserAccessorService _userAccessorService;
        private readonly UserManager<User> _userManager;
        private readonly EmailService _emailService;
        public UsersController(IUserAccessorService userAccessorService, UserManager<User> userManager, EmailService emailService)
        {
            _userAccessorService = userAccessorService;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: api/Users/GetUserId
        [Authorize]
        [HttpGet("{UserId}")]
        public async Task<ActionResult<string>> GetUserId()
        {
            var userId = _userAccessorService.GetUserId();

            return Ok(userId);
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Email is required.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User not found.");

            string resetToken = GenerateResetToken(email);
            user.ResetToken = resetToken; 
            user.ResetTokenExpiration = DateTime.UtcNow.AddHours(1);

            await _userManager.UpdateAsync(user); 

            string resetLink = $"http://localhost:5173/?token={resetToken}";

            string subject = "Password Reset Request";
            string body = $@"
            <p>Hello,</p>
            <p>We received a request to reset your password. Click the link below to reset it:</p>
            <a href='{resetLink}'>Reset Password</a>
            <p>If you did not request a password reset, please ignore this email.</p>";

            _emailService.SendEmail(email, subject, body);

            return Ok(new { Message = "Password reset link has been sent to your email." });
        }

        private string GenerateResetToken(string email)
        {
            return Guid.NewGuid().ToString();
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] string resetToken, [FromForm] string newPassword)
        {
            if (string.IsNullOrEmpty(resetToken) || string.IsNullOrEmpty(newPassword))
            {
                return BadRequest("Token and new password are required.");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.ResetToken == resetToken);

            if (user == null)
            {
                return NotFound("Invalid token.");
            }

            if (user.ResetTokenExpiration < DateTime.UtcNow)
            {
                return BadRequest("Token has expired. Please request a new password reset.");
            }

            var result = await _userManager.RemovePasswordAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("Error removing old password.");
            }

            result = await _userManager.AddPasswordAsync(user, newPassword);
            if (!result.Succeeded)
            {
                return BadRequest("Error setting new password.");
            }

            user.ResetToken = null;
            user.ResetTokenExpiration = null;
            await _userManager.UpdateAsync(user);

            return Ok(new { Message = "Password has been reset successfully." });
        }

    }
}
