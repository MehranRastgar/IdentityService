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
using MailEvents;

namespace Mail.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MailController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IRedisService _redisService;
        private readonly IPublishEndpoint _publishEndpoint;

        private readonly MailPanel _mailPanel;

        public MailController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService, IRedisService redisService, IPublishEndpoint publishEndpoint, MailPanel mailPanel)
        {
            _userManager = userManager;
            _publishEndpoint = publishEndpoint;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _redisService = redisService;
            _mailPanel = mailPanel;
        }
        
        [HttpPost("request-mail")]
        public async Task<IActionResult> RequestMail([FromBody] RequestMailDto requestMailDto)
        {
            var user = await _userManager.FindByNameAsync(requestMailDto.UserName);
            if (user == null)
            {
                return Unauthorized();
            }

            var Mail = user.Email;
            var userName = user.UserName;

            // Generate a random 6-digit OTP code
            var mailCode = GenerateMailCode();
            
            // Store OTP in Redis with 2-minute expiration
            var redisKey = $"{userName}:{Mail}";
            await _redisService.SetValueAsync(redisKey, mailCode);
            await _redisService.SetExpirationAsync(redisKey, TimeSpan.FromMinutes(2));

            // Produce OTP info to RMQ (RabbitMQ) - add your RabbitMQ publishing logic here
            var messageInfo = new MailMessage
            {
                Receiver = Mail,
                Title = "Mail Verification",
                data = mailCode,
                Timestamp = DateTime.UtcNow,
                CorrelationId = Guid.NewGuid().ToString()
            };
            var result = await _mailPanel.ProduceMail(messageInfo, _publishEndpoint);
            return Ok(new { message = "Mail code sent successfully" });
        }

        [HttpPost("login-mail")]
        public async Task<IActionResult> LoginMail([FromBody] LoginMailDto loginMailDto)
        {
            var user = await _userManager.FindByNameAsync(loginMailDto.UserName);
            if (user == null)
            {
                return Unauthorized();
            }

            var phoneNumber = user.PhoneNumber;
            var userName = user.UserName;
            var code = loginMailDto.Code;

            // Fetch OTP info from Redis for the passed-in username
            var redisKey = $"{userName}:{phoneNumber}";
            var mailCode = await _redisService.GetValueAsync(redisKey);

            if (string.IsNullOrEmpty(mailCode))
            {
                return BadRequest(new { message = "OTP code expired or invalid" });
            }

            if (mailCode != code)
            {
                return BadRequest(new { message = "Invalid OTP code" });
            }

            var token = await _tokenService.GenerateTokenAsync(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(1),
                SameSite = SameSiteMode.Strict
            };


            Response.Cookies.Append("jwt", token, cookieOptions);

            return Ok(new { message = "Logged in successfully" });
        }

        private string GenerateMailCode()
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
