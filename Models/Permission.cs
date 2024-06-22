using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models
{
  public class Permission
  {
    [Key]
    public string Name { get; set; }
    public string Description { get; set; }
  }
}
