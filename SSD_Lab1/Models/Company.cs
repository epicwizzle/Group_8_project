using System.ComponentModel.DataAnnotations;

namespace SSD_Lab1.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Company Name")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Years in Business")]
        [Range(0, 200, ErrorMessage = "Years in business must be between 0 and 200")]
        public int YearsInBusiness { get; set; }

        [Required]
        [Display(Name = "Website")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(200)]
        public string Website { get; set; }

        [Display(Name = "Province")]
        [StringLength(50)]
        public string? Province { get; set; }
    }
}
