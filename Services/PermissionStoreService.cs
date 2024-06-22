using IdentityService.Data;
using IdentityService.Models;
using IdentityService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityService.Services
{
  public class PermissionStoreService : IPermissionStoreService
  {
    private readonly ApplicationDbContext _context;

    public PermissionStoreService(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<IEnumerable<Permission>> GetAllPermissionsAsync()
    {
      return await _context.Permissions.ToListAsync();
    }

    public async Task<Permission> GetPermissionByNameAsync(string name)
    {
      return await _context.Permissions.FindAsync(name);
    }

    public async Task AddPermissionAsync(Permission permission)
    {
      _context.Permissions.Add(permission);
      await _context.SaveChangesAsync();
    }

    public async Task UpdatePermissionAsync(Permission permission)
    {
      _context.Permissions.Update(permission);
      await _context.SaveChangesAsync();
    }

    public async Task DeletePermissionAsync(string name)
    {
      var permission = await _context.Permissions.FindAsync(name);
      if (permission != null)
      {
        _context.Permissions.Remove(permission);
        await _context.SaveChangesAsync();
      }
    }
  }
}
