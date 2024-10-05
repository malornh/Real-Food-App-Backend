using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RF1.Models;
using RF1.Services.UserAccessorService;

namespace RF1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserAccessorService _userAccessorService;
        private readonly UserManager<User> _userManager;
        public UsersController(IUserAccessorService userAccessorService, UserManager<User> userManager)
        {
            _userAccessorService = userAccessorService;
            _userManager = userManager;
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

            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, newPassword);

            return Ok(new { Message = "Password has been changed successfully." });
        }
    }
}
