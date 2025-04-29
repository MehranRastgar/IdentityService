using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using IdentityService.Models;
using IdentityService.Models.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace IdentityService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
      var user = await _userManager.FindByNameAsync(loginDto.UserName);
      if (user == null || !(await _userManager.CheckPasswordAsync(user, loginDto.Password)))
      {
        return Unauthorized();
      }

      var token = await _tokenService.GenerateTokenAsync(user);

      var cookieOptions = new CookieOptions
      {
        HttpOnly = true,
        Expires = DateTime.UtcNow.AddDays(30),
        SameSite = SameSiteMode.None,
        Secure = true
      };

      Response.Cookies.Append("jwt", token, cookieOptions);

      return Ok(new { message = "Logged in successfully" });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
      Response.Cookies.Delete("jwt");
      return Ok(new { message = "Logged out successfully" });
    }

    [Authorize]
    [HttpPost("auth-check")]
    public async Task<IActionResult> AuthCheck()
    {
      var userName = GetUserNameFromToken();
      var user = await _userManager.FindByNameAsync(userName);
      if (user == null)
      {
        return Unauthorized();
      }

      var userData = new
      {
        user.Id,
        user.UserName,
        user.Email,
        user.MobileNumber,
        user.AddressId,
      };

      return Ok(new { message = "Authenticated", user = userData });
    }
    private string GetUserNameFromToken()
    {
      return User?.Claims?.FirstOrDefault(c => c.Type == "UserName")?.Value;
    }

  }
}
