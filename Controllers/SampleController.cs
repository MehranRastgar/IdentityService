using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace IdentityService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class SampleController : ControllerBase
  {
    // Public endpoint that anyone can access
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult Public()
    {
      return Ok(new { message = "This is a public endpoint." });
    }

    // Endpoint that requires the user to have the "ViewReports" permission
    [HttpGet("protected")]
    [CustomAuthorization("ViewReport")]
    public IActionResult Protected()
    {
      return Ok(new { message = "You have access to view reports." });
    }

    // Endpoint that requires the user to have the "EditData" permission
    [HttpPost("edit")]
    [CustomAuthorization("EditData")]
    public IActionResult EditData([FromBody] Dictionary<string, string> data)
    {
      return Ok(new { message = "Data has been edited successfully.", data });
    }
  }
}
