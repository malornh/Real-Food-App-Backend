using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RF1.Models
{
    public class FarmDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public IFormFile PhotoFile { get; set; }

        public string? PhotoId { get; set; }
        public string Description { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Display(Name = "Default Delivery Radius")]
        public double DefaultDeliveryRadius { get; set; }

        public double? Rating { get; set; }
    }
}
