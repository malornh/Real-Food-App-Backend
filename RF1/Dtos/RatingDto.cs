using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RF1.Models
{
    public class RatingDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Product")]
        [Required]
        public int ProductId { get; set; }

        [Display(Name = "User")]
        [Required]
        public string UserId { get; set; }

        [Required]
        public int RatingValue { get; set; }

        [Display(Name = "Date Rated")]
        public DateOnly DateRated { get; set; }

        public RatingDto()
        {
            DateRated = DateOnly.FromDateTime(DateTime.Now);
        }
    }
}
