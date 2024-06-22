using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Threading.Tasks;

namespace IdentityService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ImageController : ControllerBase
  {
    [HttpGet("{imageName}")]
    public async Task<IActionResult> GetImage(string imageName, int width = 0, int height = 0, int quality = 75)
    {
      var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imageName);

      if (!System.IO.File.Exists(imagePath))
      {
        return NotFound("Image not found.");
      }

      var image = await Image.LoadAsync(imagePath);

      if (width > 0 || height > 0)
      {
        image.Mutate(x => x.Resize(new ResizeOptions
        {
          Size = new Size(width > 0 ? width : image.Width, height > 0 ? height : image.Height),
          Mode = ResizeMode.Max
        }));
      }

      var memoryStream = new MemoryStream();
      var encoder = new JpegEncoder { Quality = quality };
      await image.SaveAsync(memoryStream, encoder);
      memoryStream.Seek(0, SeekOrigin.Begin);

      return File(memoryStream, "image/jpeg", enableRangeProcessing: false);
    }
  }
}
