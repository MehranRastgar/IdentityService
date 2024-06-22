using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityService.Data;
using IdentityService.Models;
using IdentityService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services
{
  public class RoleService : IRoleService
  {
    private readonly ApplicationDbContext _context;

    public RoleService(ApplicationDbContext context)
    {
      _context = context;
    }

    // public async Task<RoleModel> CreateRoleAsync(string roleName, List<string> permissionIds)
    // {
    //   var permissions = await _context.Permissions.Where(p => permissionIds.Contains(p.Id)).ToListAsync();
    //   var role = new RoleModel
    //   {
    //     Id = Guid.NewGuid().ToString(),
    //     Name = roleName,
    //     Permissions = permissions
    //   };

    //   _context.Roles.Add(role);
    //   await _context.SaveChangesAsync();

    //   return role;
    // }

    public async Task<RoleModel> GetRoleByIdAsync(string roleId)
    {
      return await _context.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Id == roleId);
    }

    public async Task<List<RoleModel>> GetAllRolesAsync()
    {
      return await _context.Roles.Include(r => r.Permissions).ToListAsync();
    }

    public async Task<bool> DeleteRoleAsync(string roleId)
    {
      var role = await _context.Roles.FindAsync(roleId);
      if (role == null) return false;

      _context.Roles.Remove(role);
      await _context.SaveChangesAsync();
      return true;
    }
  }
}
