using Microsoft.AspNetCore.Identity;

namespace RF1.Models
{
    public class User : IdentityUser
    {
        public List<PhotoLink> photos { get; set; }
    }
}
