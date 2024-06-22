using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityService.Models;
using IdentityService.Services.Interfaces;

namespace IdentityService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class PermissionController : ControllerBase
  {
    private readonly IPermissionManager _permissionManager;
    private readonly IPermissionStoreService _permissionStoreService;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public PermissionController(IPermissionManager permissionManager, IPermissionStoreService permissionStoreService, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
      _permissionManager = permissionManager;
      _permissionStoreService = permissionStoreService;
      _roleManager = roleManager;
      _userManager = userManager;
    }

    [HttpPost("add-to-user")]
    public async Task<IActionResult> AddPermissionToUser(string userId, string permission)
    {
      await _permissionManager.AddPermissionToUserAsync(userId, permission);
      return Ok(new { message = "Permission added to user successfully." });
    }

    [HttpPost("remove-from-user")]
    public async Task<IActionResult> RemovePermissionFromUser(string userId, string permission)
    {
      await _permissionManager.RemovePermissionFromUserAsync(userId, permission);
      return Ok(new { message = "Permission removed from user successfully." });
    }

    [HttpPost("add-to-role")]
    public async Task<IActionResult> AddPermissionToRole(string roleId, string permission)
    {
      await _permissionManager.AddPermissionToRoleAsync(roleId, permission);
      return Ok(new { message = "Permission added to role successfully." });
    }

    [HttpPost("remove-from-role")]
    public async Task<IActionResult> RemovePermissionFromRole(string roleId, string permission)
    {
      await _permissionManager.RemovePermissionFromRoleAsync(roleId, permission);
      return Ok(new { message = "Permission removed from role successfully." });
    }

    [HttpGet("user-permissions")]
    public async Task<IActionResult> GetUserPermissions(string userId)
    {
      var permissions = await _permissionManager.GetPermissionsForUserAsync(userId);
      return Ok(permissions);
    }

    [HttpGet("role-permissions")]
    public async Task<IActionResult> GetRolePermissions(string roleId)
    {
      var permissions = await _permissionManager.GetPermissionsForRoleAsync(roleId);
      return Ok(permissions);
    }

    [HttpPost("create-role")]
    public async Task<IActionResult> CreateRoleWithPermissions(string roleName, List<string> permissions)
    {
      var role = new IdentityRole(roleName);
      var result = await _roleManager.CreateAsync(role);

      if (result.Succeeded)
      {
        foreach (var permission in permissions)
        {
          await _permissionManager.AddPermissionToRoleAsync(role.Id, permission);
        }

        return Ok(new { message = "Role created with permissions successfully." });
      }

      return BadRequest(result.Errors);
    }

    [HttpPost("update-user-permissions")]
    public async Task<IActionResult> UpdateUserPermissions(string userId, List<string> permissions)
    {
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        return NotFound(new { message = "User not found." });
      }

      var currentPermissions = await _permissionManager.GetPermissionsForUserAsync(userId);
      foreach (var permission in currentPermissions)
      {
        await _permissionManager.RemovePermissionFromUserAsync(userId, permission);
      }

      foreach (var permission in permissions)
      {
        await _permissionManager.AddPermissionToUserAsync(userId, permission);
      }

      return Ok(new { message = "User permissions updated successfully." });
    }

    [HttpGet("all-roles")]
    public IActionResult GetAllRoles()
    {
      var roles = _roleManager.Roles;
      return Ok(roles);
    }

    [HttpGet("user-me-permissions")]
    public async Task<IActionResult> GetCurrentUserPermissions()
    {
      var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
      if (string.IsNullOrEmpty(userId))
      {
        return Unauthorized(new { message = "User ID claim not found." });
      }

      var permissions = await _permissionManager.GetPermissionsForUserAsync(userId);
      return Ok(permissions);
    }

    // New endpoint to get all permissions
    [HttpGet("all-permissions")]
    public async Task<IActionResult> GetAllPermissions()
    {
      var permissions = await _permissionStoreService.GetAllPermissionsAsync();
      return Ok(permissions);
    }
  }
}
