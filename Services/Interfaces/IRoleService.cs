using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityService.Models;

namespace IdentityService.Services.Interfaces
{
  public interface IRoleService
  {
    // Task<RoleModel> CreateRoleAsync(string roleName, List<string> permissionIds);
    Task<RoleModel> GetRoleByIdAsync(string roleId);
    Task<List<RoleModel>> GetAllRolesAsync();
    Task<bool> DeleteRoleAsync(string roleId);
  }
}
