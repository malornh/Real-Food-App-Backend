using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("ChangePassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword([FromForm] string email, [FromForm] string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new ArgumentNullException("User not found.");

            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, newPassword);

            return Ok(new { Message = "Password has been changed successfully." });
        }

        [HttpPost("forgotPassword")]
        public IActionResult ForgotPassword([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Email is required.");

            var user = _userManager.FindByEmailAsync(email);
            if (user == null) throw new ArgumentNullException("User not found.");

            string resetToken = GenerateResetToken(email);

            string resetLink = $"https://yourfrontendapp.com/reset-password?token={resetToken}";

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
    }
}
