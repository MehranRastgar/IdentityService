using AutoMapper;
using IdentityService.Models;
using IdentityService.Models.Common;
using IdentityService.Models.Dtos;
using IdentityService.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IMapper _mapper;


  public UserService(UserManager<ApplicationUser> userManager, IMapper mapper)
  {
    _userManager = userManager;
    _mapper = mapper;
  }

  /*  public async Task<PaginatedResponse<UserReadDto>> GetAllUsersAsync(int pageNumber, int pageSize, string? q = "", bool? asc = true)
    {
      var totalItems = _userManager.Users.Count();
      var users = _userManager.Users
                              .Skip((pageNumber - 1) * pageSize)
                              .Take(pageSize)
                              .ToList();

      return new PaginatedResponse<UserReadDto>
      {
        Data = _mapper.Map<List<UserReadDto>>(users),
        TotalItems = totalItems,
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
        CurrentPage = pageNumber,
        PageSize = pageSize
      };
    }*/
  public async Task<PagedResult<ApplicationUser>> GetAllUsersAsync(int pageNumber, int pageSize, string q, bool asc, string sortBy)
  {
    var query = _userManager.Users.AsQueryable();

    if (!string.IsNullOrEmpty(q))
    {
      query = query.Where(u => u.UserName.Contains(q) || u.Email.Contains(q) || u.Name.Contains(q) || u.Family.Contains(q));
    }

    switch (sortBy?.ToLower())
    {
      case "username":
        query = asc ? query.OrderBy(u => u.UserName) : query.OrderByDescending(u => u.UserName);
        break;
      case "email":
        query = asc ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email);
        break;
      case "name":
        query = asc ? query.OrderBy(u => u.Name) : query.OrderByDescending(u => u.Name);
        break;
      case "family":
        query = asc ? query.OrderBy(u => u.Family) : query.OrderByDescending(u => u.Family);
        break;
      default:
        query = asc ? query.OrderBy(u => u.UserName) : query.OrderByDescending(u => u.UserName);
        break;
    }

    var totalItems = await query.CountAsync();
    var users = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

    return new PagedResult<ApplicationUser>
    {
      Data = users,
      TotalItems = totalItems,
      PageSize = pageSize,
      CurrentPage = pageNumber,
      TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
    };
  }
  // public async Task<PaginatedResponse<UserReadDto>> GetAllUsersAsync(int pageNumber, int pageSize, string? q = "", bool asc = true, string? sortBy = "")
  // {
  //   // Search filter
  //   var query = _userManager.Users.AsQueryable();

  //   if (!string.IsNullOrWhiteSpace(q))
  //   {
  //     query = query.Where(user =>
  //         user.UserName.Contains(q) ||
  //         user.Email.Contains(q)
  //         ||
  //         user.Family.Contains(q) ||
  //         user.PhoneNumber.Contains(q) ||
  //         user.Name.Contains(q)
  //         ||
  //         user.NationalNumber.Contains(q)
  //     // Add more conditions if you have more fields to search by
  //     );
  //   }

  //   // Sorting
  //   if (!string.IsNullOrWhiteSpace(sortBy))
  //   {
  //     query = sortBy.ToLower() switch
  //     {
  //       "userName" => asc ? query.OrderBy(user => user.UserName) : query.OrderByDescending(user => user.UserName),
  //       "email" => asc ? query.OrderBy(user => user.Email) : query.OrderByDescending(user => user.Email),
  //       // Add more cases for other sortable fields
  //       _ => query // Default case if sortBy doesn't match any expected field
  //     };
  //   }

  //   var totalItems = await query.CountAsync();

  //   var users = await query.Skip((pageNumber - 1) * pageSize)
  //                          .Take(pageSize)
  //                          .ToListAsync();

  //   return new PaginatedResponse<UserReadDto>
  //   {
  //     Data = _mapper.Map<List<UserReadDto>>(users),
  //     TotalItems = totalItems,
  //     TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
  //     CurrentPage = pageNumber,
  //     PageSize = pageSize
  //   };
  // }


  // Method to get a single user by ID asynchronously
  public async Task<ApplicationUser> GetUserByIdAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      throw new KeyNotFoundException($"User with ID {userId} was not found.");
    }
    return user;
  }
  public async Task<IdentityResult> CreateUserAsync(string email, string password, string userName)
  {
    var user = new ApplicationUser
    {
      UserName = userName,
      Email = email
    };
    return await _userManager.CreateAsync(user, password);
  }

  public async Task<IdentityResult> UpdateUserAsync(string userId, string newEmail, string newName)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return IdentityResult.Failed(new IdentityError { Description = $"User with ID {userId} not found." });
    }

    user.Email = newEmail;
    user.UserName = newName;
    return await _userManager.UpdateAsync(user);
  }

  public async Task<IdentityResult> DeleteUserAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return IdentityResult.Failed(new IdentityError { Description = $"User with ID {userId} not found." });
    }

    return await _userManager.DeleteAsync(user);
  }

  public async Task<IdentityResult> AddUserToRoleAsync(string userId, string role)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return IdentityResult.Failed(new IdentityError { Description = $"User with ID {userId} not found." });
    }

    return await _userManager.AddToRoleAsync(user, role);
  }

}
