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

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

    }
}
