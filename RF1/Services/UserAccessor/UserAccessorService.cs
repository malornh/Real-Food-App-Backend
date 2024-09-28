using System.Security.Claims;

namespace RF1.Services.UserAccessorService
{
    public class UserAccessorService : IUserAccessorService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAccessorService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User not found.");
            }

            return user;
        }

        public string GetUserId()
        {
            var user = GetCurrentUser();

            if (!user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID is missing in the token.");
            }

            return userId;
        }
    }

}
