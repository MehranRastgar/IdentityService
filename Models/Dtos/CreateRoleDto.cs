using System.Collections.Generic;

namespace IdentityService.Models.Dtos
{
  public class CreateRoleDto
  {
    public string Name { get; set; }
    public List<string> PermissionIds { get; set; }
  }
}
