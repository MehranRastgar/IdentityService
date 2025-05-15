using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Threading.Tasks;
using IdentityService.Models;
using IdentityService.Models.Dtos;
using IdentityService;
using System.Security.Cryptography;
using SmsEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using IdentityService.Data;

namespace Otp.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class OtpController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IRedisService _redisService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly OtpSms _otpSms;

    private readonly SmsPanel _smsPanel;

    private readonly ApplicationDbContext _dbContext;
    public OtpController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService, OtpSms otpSms, IRedisService redisService, IPublishEndpoint publishEndpoint, SmsPanel smsPanel, ApplicationDbContext dbContext)
    {
      _userManager = userManager;
      _dbContext = dbContext;
      _publishEndpoint = publishEndpoint;
      _signInManager = signInManager;
      _tokenService = tokenService;
      _redisService = redisService;
      _smsPanel = smsPanel;
      _otpSms = otpSms;

    }

    [HttpPost("register-otp")]
    public async Task<IActionResult> RegisterWithOtp([FromBody] RegisterWithOtpDto dto)
    {
        if (dto == null || string.IsNullOrEmpty(dto.MobileNumber) || string.IsNullOrEmpty(dto.OtpCode) || string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Family))
            return BadRequest(new { message = "Missing required fields" });

        // Check if user already exists
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.MobileNumber == dto.MobileNumber);
        if (existingUser != null)
            return BadRequest(new { message = "User with this mobile number already exists" });

        // Check OTP
        var redisKey = $"otp:{dto.MobileNumber}";
        var storedOtpCode = await _redisService.GetValueAsync(redisKey);
        if (string.IsNullOrEmpty(storedOtpCode) || storedOtpCode != dto.OtpCode)
            return BadRequest(new { message = "Invalid or expired OTP code" });

        // Create user (no password)
        var user = new ApplicationUser
        {
            UserName = dto.MobileNumber, // or generate a username
            Name = dto.Name,
            Family = dto.Family,
            MobileNumber = dto.MobileNumber,
            PhoneNumber = dto.MobileNumber,
            PhoneNumberConfirmed = true
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new { message = "Failed to create user", errors = result.Errors });

        // Remove OTP from Redis after successful registration
        // TODO: Remove OTP from Redis (no RemoveAsync method in IRedisService)
        // await _redisService.RemoveAsync(redisKey);

        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDto requestOtpDto)
    {
      ApplicationUser? user = null;
      if (requestOtpDto?.UserName != null)
        user = await _userManager.FindByNameAsync(requestOtpDto.UserName);
      if (requestOtpDto?.MobileNumber != null)
      {
        IQueryable<ApplicationUser> user2 = _dbContext.Users.Where(u => u.MobileNumber == requestOtpDto.MobileNumber);
        user = user2.FirstOrDefault();
      }

      // For registration, allow sending OTP even if user does not exist
      if (user == null && !string.IsNullOrEmpty(requestOtpDto?.MobileNumber))
      {
        var phoneNumber = requestOtpDto.MobileNumber;
        // Generate a random 6-digit OTP code
        var otpCode = GenerateOtpCode();
        await _otpSms.SendOtpAsync(phoneNumber, otpCode);
        // Store OTP in Redis with 2-minute expiration (keyed by mobile number)
        var redisKey = $"otp:{phoneNumber}";
        await _redisService.SetValueAsync(redisKey, otpCode);
        await _redisService.SetExpirationAsync(redisKey, TimeSpan.FromMinutes(2));
        // Produce OTP info to RMQ (RabbitMQ)
        var messageInfo = new SmsEvent
        {
          Message = otpCode,
          PhoneNumbers = new string[] { phoneNumber },
          Timestamp = DateTime.UtcNow,
          CorrelationId = Guid.NewGuid().ToString()
        };
        var result = await _smsPanel.ProduceSms(messageInfo, _publishEndpoint);
        return Ok(new { message = "OTP code sent successfully" });
      }

      if (user?.UserName == null)
      {
        return Unauthorized();
      }

      var phone = user.MobileNumber;
      var userName = user.UserName;
      // Generate a random 6-digit OTP code
      var code = GenerateOtpCode();
      await _otpSms.SendOtpAsync(phone, code);
      // Store OTP in Redis with 2-minute expiration (keyed by mobile number)
      var redisKey2 = $"otp:{phone}";
      await _redisService.SetValueAsync(redisKey2, code);
      await _redisService.SetExpirationAsync(redisKey2, TimeSpan.FromMinutes(2));
      // Produce OTP info to RMQ (RabbitMQ)
      var messageInfo2 = new SmsEvent
      {
        Message = code,
        PhoneNumbers = new string[] { phone },
        Timestamp = DateTime.UtcNow,
        CorrelationId = Guid.NewGuid().ToString()
      };
      var result2 = await _smsPanel.ProduceSms(messageInfo2, _publishEndpoint);
      return Ok(new { message = "OTP code sent successfully" });
    }

    [HttpPost("login-otp")]
    public async Task<IActionResult> LoginOtp([FromBody] LoginOtpDto loginOtpDto)
    {
      ApplicationUser? user = null;

      if (loginOtpDto?.UserName != null)
        user = await _userManager.FindByNameAsync(loginOtpDto.UserName);
      if (loginOtpDto?.MobileNumber != null)
      {
        IQueryable<ApplicationUser> user2 = _dbContext.Users.Where(u => u.MobileNumber == loginOtpDto.MobileNumber);
        user = user2.FirstOrDefault();
      }
      if (user == null)
      {
        return Unauthorized();
      }

      var phoneNumber = user.MobileNumber;
      var userName = user.UserName;
      var code = loginOtpDto.Code;

      // Fetch OTP info from Redis for the passed-in username
      var redisKey = $"{userName}:{phoneNumber}";
      var storedOtpCode = await _redisService.GetValueAsync(redisKey);

      if (string.IsNullOrEmpty(storedOtpCode))
      {
        return BadRequest(new { message = "OTP code expired or invalid" });
      }

      if (storedOtpCode != code)
      {
        return BadRequest(new { message = "Invalid OTP code" });
      }

      var token = await _tokenService.GenerateTokenAsync(user);

      var cookieOptions = new CookieOptions
      {
        HttpOnly = false, // Prevents client-side JavaScript from accessing the cookie
        SameSite = SameSiteMode.None, // Adjust based on your requirements
        Secure = false, // Ensures the cookie is only sent over HTTPS (recommended for production)
        IsEssential = false, // Marks the cookie as essential for the application
        Path = "/"
      };

      Response.Cookies.Append("jwt", token, cookieOptions);

      return Ok(new { message = "Logged in successfully", token });
    }

    private string GenerateOtpCode()
    {
      using (var rng = new RNGCryptoServiceProvider())
      {
        var byteArray = new byte[4];
        rng.GetBytes(byteArray);
        var random = BitConverter.ToUInt32(byteArray, 0) % 1000000;
        return random.ToString("D6");
      }
    }
  }
}
