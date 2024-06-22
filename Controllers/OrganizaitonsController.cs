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
    private readonly OrganizationGrpcClient _organizationGrpcClient;

    public OrganizaitonsController(IConfiguration configuration, OrganizationGrpcClient organizationGrpcClient)
    {
      _configuration = configuration;
      _organizationGrpcClient = organizationGrpcClient;

    }

    // [HttpGet]
    // public async Task<IActionResult> GetOrganizations(int pageNumber = 1, int pageSize = 10, string? q = "", string? asc = "true", string? sortBy = "")
    // {
    //  return 
    // }
  }
}