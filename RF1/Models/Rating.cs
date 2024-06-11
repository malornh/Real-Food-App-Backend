using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RF1.Models
{
    public class Rating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name="Product")]
        public int? ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product {  get; set; }

        [Display(Name = "User")]
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public double RatingValue { get; set; }

        [Display(Name = "Date Rated")]
        public DateOnly DateRated { get; set; }

        public Rating()
        {
            DateRated = DateOnly.FromDateTime(DateTime.Now);
        }
    }
}
