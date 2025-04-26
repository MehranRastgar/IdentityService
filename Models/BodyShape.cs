using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models
{
    public class BodyShape
    {
        [Key]
        public string Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string? Description { get; set; }
        
        public string? ImageUrl { get; set; }
        
        [Required]
        public string Category { get; set; }  // male or female
    }
} 