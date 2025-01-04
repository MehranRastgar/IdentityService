using System.Threading.Tasks;

namespace IdentityService.Services.Interfaces
{
  public interface IOTPService
  {
    Task<bool> GenerateOtpAsync(string mobileNumber);
    Task<bool> ValidateOtpAsync(string mobileNumber, string otp);
  }
}
