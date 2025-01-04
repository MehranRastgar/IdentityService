using IdentityService.Models;
using IdentityService.Models.Common;
using IdentityService.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityService.Services.Interfaces
{
  public interface IUserService
  {
    Task<PagedResult<ApplicationUser>> GetAllUsersAsync(int pageNumber, int pageSize, string q, bool asc, string sortBy);
    Task<ApplicationUser> GetUserByIdAsync(string userId);
    Task<IdentityResult> CreateUserAsync(string email, string password, string userName);
    Task<IdentityResult> CreateUserAsync(CreateUserByPhoneDto createUserObject);
    Task<IdentityResult> UpdateUserAsync(string userId, string newEmail, string newName);
    Task<IdentityResult> DeleteUserAsync(string userId);
    Task<IdentityResult> AddUserToRoleAsync(string userId, string role);
  }
}
