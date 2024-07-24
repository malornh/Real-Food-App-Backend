using RF1.Models;

namespace RF1.Dtos
{
    public class CartDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ProductDto Product { get; set; }
    }
}
