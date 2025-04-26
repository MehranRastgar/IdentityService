using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models.Dtos
{
    public class UpdateUserPhysicalInfoDto
    {
        [Required]
        [Range(15, 100)]
        public int Age { get; set; }

        [Required]
        [RegularExpression("^(male|female)$")]
        public string Gender { get; set; } = null!;

        [Required]
        [Range(100, 250)]
        public decimal Height { get; set; }  // in centimeters

        [Required]
        [Range(30, 200)]
        public decimal Weight { get; set; }  // in kilograms

        [Required]
        [RegularExpression("^(sedentary|light|moderate|active|very_active)$")]
        public string ActivityLevel { get; set; } = null!;

        [Required]
        [RegularExpression("^(lose_weight|gain_muscle|maintain|improve_fitness)$")]
        public string Goal { get; set; } = null!;
    }
} 