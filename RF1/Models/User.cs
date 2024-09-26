using Microsoft.AspNetCore.Identity;

namespace RF1.Models
{
    public class User : IdentityUser
    {
        public List<Farm> Farms { get; set; }
        public List<Shop> Shops { get; set; }
        public List<PhotoLink> Photos { get; set; }
    }
}
