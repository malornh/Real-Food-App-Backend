using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RF1.Models
{
    public class Farm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        [MaxLength(450)]
        public string? PhotoUrl { get; set; }

        [Display(Name = "User")]
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        [Required]
        public double Latitude {  get; set; }

        [Required]
        public double Longitude { get; set; }

        [Display(Name = "Default Delivery Radius")]
        public double DefaultDeliveryRadius { get; set; }

        public double Rating { get; set; }
    }
}
