using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace IdentityService
{
  public class CustomAuthorizationAttribute : Attribute, IAuthorizationFilter
  {
    private readonly string _permission;

    public CustomAuthorizationAttribute(string permission)
    {
      _permission = permission ?? throw new ArgumentNullException(nameof(permission));
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
      var user = context.HttpContext.User;

      if (!user.Identity.IsAuthenticated)
      {
        context.Result = new UnauthorizedResult();
        return;
      }

      var hasPermission = user.Claims.Any(c => c.Type == "Permission" && c.Value == _permission);
      if (!hasPermission)
      {
        context.Result = new ForbidResult();
      }
    }
  }
}
