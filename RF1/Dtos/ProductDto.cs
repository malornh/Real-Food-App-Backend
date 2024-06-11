using RF1.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RF1.Models
{
    public class ProductDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Type { get; set; }

        [Display(Name = "Farm")]
        [Required]
        public int FarmId { get; set; }

        [Required]
        public string Image { get; set; }
        public string? Description { get; set; }

        [Display(Name = "Unit Of Measurement")]
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
        public int Quantity { get; set; }

        [Required]
        public double PricePerUnit { get; set; }
        public double DeliveryRadius { get; set; }

        [Required]
        public int MinUnitOrder { get; set; }

        [Display(Name = "Date Updated")]
        public DateOnly DateUpdated { get; set; }

        public double? Rating { get; set; }

        public ProductDto()
        {
            DateUpdated = DateOnly.FromDateTime(DateTime.Now);
        }
    }
}
