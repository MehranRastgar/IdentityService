using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityService.Models;
using IdentityService.Services.Interfaces;

namespace IdentityService.Services
{
  public class PermissionManager : IPermissionManager
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public PermissionManager(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
    }

    public async Task<bool> HasPermissionAsync(string userId, string permission)
    {
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null) return false;

      var userClaims = await _userManager.GetClaimsAsync(user);
      return userClaims.Any(c => c.Type == "Permission" && c.Value == permission);
    }

    public async Task AddPermissionToUserAsync(string userId, string permission)
    {
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null) throw new ArgumentException("User not found", nameof(userId));

      var claim = new Claim("Permission", permission);
      await _userManager.AddClaimAsync(user, claim);
    }

    public async Task RemovePermissionFromUserAsync(string userId, string permission)
    {
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null) throw new ArgumentException("User not found", nameof(userId));

      var claim = new Claim("Permission", permission);
      await _userManager.RemoveClaimAsync(user, claim);
    }

    public async Task AddPermissionToRoleAsync(string roleId, string permission)
    {
      var role = await _roleManager.FindByIdAsync(roleId);
      if (role == null) throw new ArgumentException("Role not found", nameof(roleId));

      var roleClaims = await _roleManager.GetClaimsAsync(role);
      if (roleClaims.Any(c => c.Type == "Permission" && c.Value == permission)) return;

      var claim = new Claim("Permission", permission);
      await _roleManager.AddClaimAsync(role, claim);
    }

    public async Task RemovePermissionFromRoleAsync(string roleId, string permission)
    {
      var role = await _roleManager.FindByIdAsync(roleId);
      if (role == null) throw new ArgumentException("Role not found", nameof(roleId));

      var claim = new Claim("Permission", permission);
      await _roleManager.RemoveClaimAsync(role, claim);
    }

    public async Task<IList<string>> GetPermissionsForUserAsync(string userId)
    {
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null) throw new ArgumentException("User not found", nameof(userId));

      var userClaims = await _userManager.GetClaimsAsync(user);
      return userClaims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();
    }

    public async Task<IList<string>> GetPermissionsForRoleAsync(string roleId)
    {
      var role = await _roleManager.FindByIdAsync(roleId);
      if (role == null) throw new ArgumentException("Role not found", nameof(roleId));

      var roleClaims = await _roleManager.GetClaimsAsync(role);
      return roleClaims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();
    }
  }
}
