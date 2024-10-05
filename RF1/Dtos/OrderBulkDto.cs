using RF1.Models;

namespace RF1.Dtos
{
    public class OrderBulkDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }
        public decimal? ShopPrice { get; set; }
        public DateTime DateOrdered { get; set; }
        public ProductDto Product { get; set; }
        public FarmDto Farm { get; set; }
        public ShopDto Shop { get; set; }
    }
}
