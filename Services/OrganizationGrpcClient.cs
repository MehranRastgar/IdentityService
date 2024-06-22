using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Net.Client;
using FmsProtos.Grpc;

public class OrganizationGrpcClient
{
  private readonly OrganizationService.OrganizationServiceClient _client;

  public OrganizationGrpcClient(OrganizationService.OrganizationServiceClient client)
  {
    _client = client;
  }

  public async Task<List<int>> GetOrganizationAndSubOrganizationIdsAsync(int organizationId)
  {
    var request = new GetOrganizationIdsRequest { OrganizationId = organizationId };
    var response = await _client.GetOrganizationIdsAsync(request);
    return response.OrganizationIds.ToList();
  }
}
