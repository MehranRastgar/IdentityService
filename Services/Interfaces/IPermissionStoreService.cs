using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityService.Models;

namespace IdentityService.Services.Interfaces
{
  public interface IPermissionStoreService
  {
    Task<IEnumerable<Permission>> GetAllPermissionsAsync();
    Task<Permission> GetPermissionByNameAsync(string name);
    Task AddPermissionAsync(Permission permission);
    Task UpdatePermissionAsync(Permission permission);
    Task DeletePermissionAsync(string name);
  }
}
