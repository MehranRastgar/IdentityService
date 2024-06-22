using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityService.Services.Interfaces
{
  public interface IPermissionManager
  {
    Task<bool> HasPermissionAsync(string userId, string permission);
    Task AddPermissionToUserAsync(string userId, string permission);
    Task RemovePermissionFromUserAsync(string userId, string permission);
    Task AddPermissionToRoleAsync(string roleId, string permission);
    Task RemovePermissionFromRoleAsync(string roleId, string permission);
    Task<IList<string>> GetPermissionsForUserAsync(string userId);
    Task<IList<string>> GetPermissionsForRoleAsync(string roleId);
  }
}
