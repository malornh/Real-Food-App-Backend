using System.Security.Claims;

namespace RF1.Services.UserAccessorService
{
    public interface IUserAccessorService
    {
        ClaimsPrincipal GetCurrentUser();
        string GetUserId();
    }
}
