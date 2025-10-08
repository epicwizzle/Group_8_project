using System.ComponentModel.DataAnnotations;

namespace SSD_Lab1.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "First Name")]
        [StringLength(100)]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        [StringLength(100)]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Year of Birth")]
        [Range(1820, 2020, ErrorMessage = "Year of birth must be between 1820 and 2020.")]
        public int ModelYear { get; set; }
        [Required]
        [Display(Name = "Wage")]
        public float Wage { get; set; }

    }
}
