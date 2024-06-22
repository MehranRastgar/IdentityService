using System.Threading.Tasks;
using IdentityService.Models;

namespace IdentityService
{
  public interface ITokenService
  {
    Task<string> GenerateTokenAsync(ApplicationUser user);
  }
}
