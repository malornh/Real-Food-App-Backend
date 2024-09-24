using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RF1.Models
{
    public class PhotoLink
    {
        [Key]
        [Column(TypeName = "nvarchar(450)")]
        [Required]
        [MaxLength(450)]
        public string Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
