using Microsoft.AspNetCore.Http;

namespace IdentityService.Models.Dtos
{
  public class UserImageDto
  {
    public IFormFile Image { get; set; }
  }
}
