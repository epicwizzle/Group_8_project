using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SSD_Lab1.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Username")]
        override public string Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Display(Name = "City")]
        [StringLength(100)]
        public string? City { get; set; }
    }
}
