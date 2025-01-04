using IdentityService.Services.Interfaces;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace IdentityService.Services
{
  public class OTPService : IOTPService
  {
    private readonly IDatabase _redisDatabase;
    private readonly OtpSms _otpSms;

    public OTPService(IConnectionMultiplexer redisConnection, OtpSms otpSms)
    {
      _redisDatabase = redisConnection.GetDatabase();
      _otpSms = otpSms;
    }

    public async Task<bool> GenerateOtpAsync(string mobileNumber)
    {
      var otp = new Random().Next(10000, 99999).ToString();
      await _redisDatabase.StringSetAsync(mobileNumber, otp, TimeSpan.FromMinutes(5));

      await _otpSms.SendOtpAsync(mobileNumber);

      return true;
    }

    public async Task<bool> ValidateOtpAsync(string mobileNumber, string otp)
    {
      var storedOtp = await _redisDatabase.StringGetAsync(mobileNumber);
      return storedOtp == otp;
    }
  }
}



