using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSD_Lab1.Models
{
    [Table("Messages")]
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Message")]
        public string MessageString { get; set; }

        [Display(Name = "Sent At")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm tt}")]
        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        public virtual ApplicationUser User { get; set; } = null!;

    }
}
