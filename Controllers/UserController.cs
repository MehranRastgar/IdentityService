using Microsoft.AspNetCore.Mvc;
using IdentityService.Models;
using IdentityService.Models.Dtos;
using IdentityService.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using IdentityService.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using IdentityService.Services;

namespace IdentityService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UserController : ControllerBase
  {

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserService _userService;
    private readonly ICustomUserService _customUserService;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;
    private readonly IRoleService _roleService;
    private readonly IOTPService _otpService;

    public UserController(UserManager<ApplicationUser> userManager, ICustomUserService customUserService, IUserService userService, IMapper mapper, ApplicationDbContext context, IRoleService roleService, IOTPService otpService)
    {

      _userManager = userManager;
      _customUserService = customUserService;
      _userService = userService;
      _mapper = mapper;
      _context = context;
      _roleService = roleService;
      _otpService = otpService;
    }


    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (userId == null)
      {
        return Unauthorized("User ID not found in token.");
      }

      var user = await _userService.GetUserByIdAsync(userId);
      if (user == null)
      {
        return NotFound("User not found.");
      }

      return Ok(user);
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserCreateDto userCreate)
    {
      var user = _mapper.Map<ApplicationUser>(userCreate);
      var result = await _customUserService.CreateUserAsync(user, userCreate.Password);

      if (result.Succeeded)
      {
        return Ok(new { message = "User registered successfully.", userId = user.Id });
      }
      else
      {
        var errors = result.Errors.Select(e => e.Description);
        return BadRequest(new { errors });
      }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
      var user = await _userService.GetUserByIdAsync(id);
      if (user == null)
      {
        return NotFound();
      }
      return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto userUpdate)
    {
      var user = await _userService.GetUserByIdAsync(id);
      if (user == null)
      {
        return NotFound($"User with ID {id} not found.");
      }

      _mapper.Map(userUpdate, user);
      var result = await _customUserService.UpdateUserAsync(user);
      if (result.Succeeded)
      {
        return Ok(user);
      }
      else
      {
        return BadRequest(result.Errors);
      }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
      var user = await _userManager.FindByIdAsync(id);
      if (user == null)
      {
        return NotFound($"User with ID {id} not found.");
      }

      var result = await _userManager.DeleteAsync(user);
      if (result.Succeeded)
      {
        return Ok($"User with ID {id} has been deleted.");
      }
      else
      {
        return BadRequest(result.Errors);
      }
    }

    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadImage(string id, [FromForm] UserImageDto userImage)
    {
      var user = await _userManager.FindByIdAsync(id);
      if (user == null)
      {
        return NotFound($"User with ID {id} not found.");
      }

      if (userImage.Image != null && userImage.Image.Length > 0)
      {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
        if (!Directory.Exists(uploadsFolder))
        {
          Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{user.UserName}_{Path.GetRandomFileName()}{Path.GetExtension(userImage.Image.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
          await userImage.Image.CopyToAsync(stream);
        }

        user.ImageUrl = $"/images/{uniqueFileName}";
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
          return Ok(new { imageUrl = user.ImageUrl });
        }
        else
        {
          return BadRequest(result.Errors);
        }
      }
      return BadRequest("Invalid image.");
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers(int pageNumber = 1, int pageSize = 10, string? q = "", string? asc = "true", string? sortBy = "")
    {
      var response = await _userService.GetAllUsersAsync(pageNumber, pageSize, q, asc == "true", sortBy);
      return Ok(new
      {
        data = response.Data,
        totalItems = response.TotalItems,
        totalPages = response.TotalPages,
        currentPage = response.CurrentPage,
        pageSize = response.PageSize
      });
    }

    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtp([FromBody] OtpRequestModel model)
    {
      var result = await _otpService.GenerateOtpAsync(model.MobileNumber);
      if (result)
      {
        return Ok(new { Message = "OTP sent successfully." });
      }
      return BadRequest(new { Message = "Failed to send OTP." });
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model)
    {
      var isValidOtp = await _otpService.ValidateOtpAsync(model.MobileNumber, model.Otp);
      if (!isValidOtp)
      {
        return BadRequest(new { Message = "Invalid OTP." });
      }

      var result = await _userService.CreateUserAsync(model.MobileNumber, model.Password, model.UserName);
      if (result.Succeeded)
      {
        return Ok(new { Message = "User created successfully." });
      }
      return BadRequest(result.Errors);
    }

    // [HttpPost("{id}/permissions")]
    // public async Task<IActionResult> AddPermissionsToUser(string id, [FromBody] List<string> permissionIds)
    // {
    //   var user = await _userManager.FindByIdAsync(id);
    //   if (user == null)
    //   {
    //     return NotFound($"User with ID {id} not found.");
    //   }

    //   var permissions = await _context.Permissions.Where(p => permissionIds.Contains(p.Id)).ToListAsync();
    //   user.Permissions.AddRange(permissions);
    //   var result = await _userManager.UpdateAsync(user);

    //   if (result.Succeeded)
    //   {
    //     return Ok(user);
    //   }
    //   else
    //   {
    //     return BadRequest(result.Errors);
    //   }
    // }

    // [HttpPost("{id}/roles")]
    // public async Task<IActionResult> AddRoleToUser(string id, [FromBody] string roleId)
    // {
    //   var user = await _userManager.FindByIdAsync(id);
    //   if (user == null)
    //   {
    //     return NotFound($"User with ID {id} not found.");
    //   }

    //   var role = await _roleService.GetRoleByIdAsync(roleId);
    //   if (role == null)
    //   {
    //     return NotFound($"Role with ID {roleId} not found.");
    //   }

    //   user.Permissions.AddRange(role.Permissions);
    //   var result = await _userManager.UpdateAsync(user);

    //   if (result.Succeeded)
    //   {
    //     return Ok(user);
    //   }
    //   else
    //   {
    //     return BadRequest(result.Errors);
    //   }
    // }
  }
}
