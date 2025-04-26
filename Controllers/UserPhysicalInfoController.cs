using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityService.Models;
using IdentityService.Models.Dtos;
using System.Security.Claims;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserPhysicalInfoController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserPhysicalInfoController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<UpdateUserPhysicalInfoDto>> GetPhysicalInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            var physicalInfo = new UpdateUserPhysicalInfoDto
            {
                Age = user.Age ?? 0,
                Gender = user.Gender ?? "male",
                Height = user.Height ?? 0,
                Weight = user.Weight ?? 0,
                ActivityLevel = user.ActivityLevel ?? "sedentary",
                Goal = user.Goal ?? "improve_fitness",
                BodyShapeId = user.BodyShapeId
            };

            return Ok(physicalInfo);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePhysicalInfo(UpdateUserPhysicalInfoDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            user.Age = model.Age;
            user.Gender = model.Gender;
            user.Height = model.Height;
            user.Weight = model.Weight;
            user.ActivityLevel = model.ActivityLevel;
            user.Goal = model.Goal;
            user.BodyShapeId = model.BodyShapeId;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "اطلاعات فیزیکی با موفقیت بروزرسانی شد" });
        }
    }
} 