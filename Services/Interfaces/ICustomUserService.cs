using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
namespace IdentityService.Services.Interfaces
{
  public interface ICustomUserService
  {
    Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
    Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
  }
}