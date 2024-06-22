using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FmsProtos.Grpc;
using System.IO;
using System.Threading.Tasks;

namespace IdentityService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class OrganizaitonsController : ControllerBase
  {
    private readonly IConfiguration _configuration;

    public OrganizaitonsController(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrganizations(int pageNumber = 1, int pageSize = 10, string? q = "", string? asc = "true", string? sortBy = "")
    {
      var userOrganizationIdClaim = User.FindFirstValue("OrganizationId");
      if (userOrganizationIdClaim == null)
      {
        return Unauthorized("Organization ID not found in token.");
      }

      if (!int.TryParse(userOrganizationIdClaim, out int userOrganizationId))
      {
        return BadRequest("Invalid Organization ID format.");
      }


      var grpcChannel = GrpcChannel.ForAddress(_configuration["GrpcSettings:AssetManagerUrl"], new GrpcChannelOptions
      {
        HttpHandler = new HttpClientHandler
        {
          ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        }
      });

      var grpcClient = new OrganizationService.OrganizationServiceClient(grpcChannel);

      var grpcRequest = new GetOrganizationsRequest
      {
        UserOrganizationId = userOrganizationId,
        PageNumber = pageNumber,
        PageSize = pageSize,
        Query = q ?? string.Empty,
        Asc = asc == "true",
        SortBy = sortBy ?? string.Empty
      };

      var grpcResponse = await grpcClient.GetOrganizationsAsync(grpcRequest);

      return Ok(new
      {
        data = grpcResponse.Organizations,
        totalItems = grpcResponse.TotalItems,
        totalPages = grpcResponse.TotalPages,
        currentPage = grpcResponse.CurrentPage,
        pageSize = grpcResponse.PageSize
      });
    }
  }
}