// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Options;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.IdentityModel.Tokens;
// using System.IdentityModel.Tokens.Jwt;
// using System.Text;
// using Microsoft.Extensions.Logging;
// using IdentityService;

// public class JwtMiddleware
// {
//   private readonly RequestDelegate _next;
//   private readonly JwtSettings _jwtSettings;
//   private readonly ILogger<JwtMiddleware> _logger;

//   public JwtMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtSettings, ILogger<JwtMiddleware> logger)
//   {
//     _next = next;
//     _jwtSettings = jwtSettings.Value;
//     _logger = logger;
//   }

//   public async Task Invoke(HttpContext context)
//   {
//     var token = context.Request.Cookies["jwt"];

//     if (!string.IsNullOrEmpty(token))
//     {
//       AttachUserToContext(context, token);
//     }

//     await _next(context);
//   }

//   private void AttachUserToContext(HttpContext context, string token)
//   {
//     try
//     {
//       var tokenHandler = new JwtSecurityTokenHandler();
//       var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
//       tokenHandler.ValidateToken(token, new TokenValidationParameters
//       {
//         ValidateIssuerSigningKey = true,
//         IssuerSigningKey = new SymmetricSecurityKey(key),
//         ValidateIssuer = true,
//         ValidIssuer = _jwtSettings.Issuer,
//         ValidateAudience = true,
//         ValidAudience = _jwtSettings.Audience,
//         ValidateLifetime = true
//       }, out SecurityToken validatedToken);

//       var jwtToken = (JwtSecurityToken)validatedToken;
//       var userId = jwtToken.Claims.First(x => x.Type == "UserId").Value;

//       context.Items["User"] = userId;
//     }
//     catch (Exception ex)
//     {
//       _logger.LogError(ex, "JWT validation failed");
//       // Do nothing if JWT validation fails
//       // User is not attached to context so request won't have access to secure routes
//     }
//   }
// }


using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace IdentityService
{
  public class JwtMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
      _next = next;
      _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
      var token = context.Request.Cookies["jwt"]; // Extract JWT from cookie

      if (token != null)
        await AttachUserToContext(context, token);

      await _next(context);
    }

    private async Task AttachUserToContext(HttpContext context, string token)
    {
      try
      {
        var jwtSettings = _configuration.GetSection("Jwt").Get<JwtSettings>();
        // Console.WriteLine("JWT settings: {0}{1}{2}{3}", jwtSettings.Issuer, jwtSettings.Audience, jwtSettings.Key, token);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtSettings.Key);
        Console.WriteLine(new SymmetricSecurityKey(key));

        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = true,
          ValidIssuer = jwtSettings.Issuer,
          ValidateAudience = true,
          ValidAudience = jwtSettings.Audience,
          ValidateLifetime = true,
          ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        var claims = jwtToken.Claims.ToList();

        var identity = new ClaimsIdentity(claims, "jwt");
        var principal = new ClaimsPrincipal(identity);
        Console.WriteLine("JWT settings: {0}", claims[5]);

        context.User = principal;
      }
      catch (Exception ex)
      {
        // Log the exception or handle it
      }
    }
  }
}
