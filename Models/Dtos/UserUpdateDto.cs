namespace IdentityService.Models.Dtos
{
  public class UserUpdateDto
  {
    public string Name { get; set; }
    public string Family { get; set; }
    public string NationalNumber { get; set; }
    public string MobileNumber { get; set; }
    public AddressModel? Address { get; set; }
    public int? OrganizationId { get; set; }
  }
}
