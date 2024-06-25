using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RF1.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display (Name = "Shop")]
        public int? ShopId { get; set; }

        [ForeignKey("ShopId")]
        public Shop Shop { get; set; }

        [Display(Name = "Product")]
        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int Quantity {  get; set; }
        public string Status { get; set; }
        public double? ShopPrice { get; set; }

        [Display(Name = "Date Ordered")]
        public DateOnly DateOrdered { get; set; }
        public bool SoldOut {  get; set; }

        public Order()
        {
            SoldOut = false;
            DateOrdered = DateOnly.FromDateTime(DateTime.Now);
        }
    }
}
