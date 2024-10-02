using RF1.Models;

namespace RF1.Dtos
{
    public class CartDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public ProductDto Product { get; set; }
        public int Quantity { get; set; }
        public ShopDto Shop { get; set; }
    }
}
