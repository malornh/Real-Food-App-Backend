using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RF1.Services.UserAccessorService;

namespace RF1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserAccessorService _userAccessorService;
        public UsersController(IUserAccessorService userAccessorService)
        {
            _userAccessorService = userAccessorService;
        }

        // GET: api/Users/GetUserId
        [Authorize]
        [HttpGet("{UserId}")]
        public async Task<ActionResult<string>> GetUserId()
        {
            var userId = _userAccessorService.GetUserId();

            return Ok(userId);
        }
    }
}
