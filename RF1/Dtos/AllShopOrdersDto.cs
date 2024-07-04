using RF1.Models;

public class AllShopOrdersDto
{
    public int Id { get; set; }
    public string Status { get; set; }
    public int Quantity { get; set; }
    public double? ShopPrice { get; set; }
    public DateOnly DateOrdered { get; set; }
    public ProductDto Product { get; set; }
    public FarmDto Farm { get; set; }
    public ShopDto Shop { get; set; }
}
