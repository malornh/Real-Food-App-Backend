using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RF1.Models
{
    public class OrderDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Shop")]
        [Required]
        public int ShopId { get; set; }

        [Display(Name = "Product")]
        [Required]
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double? ShopPrice { get; set; }
        public string Status { get; set; }

        [Display(Name = "Date Ordered")]
        public DateOnly DateOrdered { get; set; }
        public bool SoldOut { get; set; }

        public OrderDto()
        {
            SoldOut = false;
            DateOrdered = DateOnly.FromDateTime(DateTime.Now);
        }
    }
}
