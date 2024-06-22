using IdentityService.Models;
using IdentityService.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services
{
  public class CustomUserService : ICustomUserService
  {
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomUserService(UserManager<ApplicationUser> userManager)
    {
      _userManager = userManager;
    }

    public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
    {
      return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
    {
      return await _userManager.UpdateAsync(user);
    }
  }
}