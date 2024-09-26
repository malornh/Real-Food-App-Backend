using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using RF1.Models.Enums;

namespace RF1.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Type { get; set; }

        [Display (Name = "Farm")]
        [Required]
        public int FarmId { get; set; }

        [ForeignKey("FarmId")]
        public Farm Farm { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        [MaxLength(450)]
        public string? PhotoId { get; set; }

        [ForeignKey("PhotoId")]
        public PhotoLink Photo { get; set; }
        public string? Description { get; set; }

        [Display(Name = "Unit Of Measurement")]
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
        public int? Quantity { get; set; }

        [Required]
        public double PricePerUnit {  get; set; }
        public double? DeliveryRadius { get; set; }

        [Required]
        public int MinUnitOrder {  get; set; }

        [Display(Name = "Date Updated")]
        public DateOnly DateUpdated { get; set; }

        public Product()
        {
            DateUpdated = DateOnly.FromDateTime(DateTime.Now);
        }
    }
}
