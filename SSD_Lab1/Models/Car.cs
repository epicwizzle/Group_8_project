using System.ComponentModel.DataAnnotations;

namespace SSD_Lab1.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Car Model")]
        [StringLength(100)]
        public string Model { get; set; }

        [Required]
        [Display(Name = "Model Year")]
        [Range(1885, 2135, ErrorMessage = "Model year must be between 1885 and 2135.")]
        public int ModelYear { get; set; }

        [Required]
        [Display(Name = "Cost")]
        public float Cost { get; set; }

    }
}
