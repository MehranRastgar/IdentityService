using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService
{
  public class TokenService : ITokenService
  {
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;

    public TokenService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<string> GenerateTokenAsync(ApplicationUser user)
    {
      if (user == null) throw new ArgumentNullException(nameof(user));

      var jwtSettings = _configuration.GetSection("Jwt").Get<JwtSettings>() ?? throw new ArgumentNullException(nameof(JwtSettings));

      var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwtSettings.Subject ?? throw new ArgumentNullException(nameof(jwtSettings.Subject))),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.NameIdentifier, user.Id ?? throw new ArgumentNullException(nameof(user.Id))), // Ensure UserId is included
                new Claim("UserId", user.Id ?? throw new ArgumentNullException(nameof(user.Id))),
                new Claim("UserName", user.UserName ?? throw new ArgumentNullException(nameof(user.UserName))),
                new Claim("IsSuperAdmin", user.IsSuperAdmin.ToString()), // if exist this feild
                new Claim("OrganizationId", user.OrganizationId?.ToString() ?? string.Empty) // if has organization
            };

      var userRoles = await _userManager.GetRolesAsync(user);
      foreach (var role in userRoles)
      {
        if (!string.IsNullOrWhiteSpace(role))
        {
          claims.Add(new Claim(ClaimTypes.Role, role));
        }
      }

      var userPermissions = await _userManager.GetClaimsAsync(user);
      foreach (var permission in userPermissions)
      {
        if (!string.IsNullOrWhiteSpace(permission.Type) && !string.IsNullOrWhiteSpace(permission.Value))
        {
          claims.Add(permission);
        }
      }

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key ?? throw new ArgumentNullException(nameof(jwtSettings.Key))));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          issuer: jwtSettings.Issuer ?? throw new ArgumentNullException(nameof(jwtSettings.Issuer)),
          audience: jwtSettings.Audience ?? throw new ArgumentNullException(nameof(jwtSettings.Audience)),
          claims: claims,
          expires: DateTime.UtcNow.AddDays(1),
          signingCredentials: creds);

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}
