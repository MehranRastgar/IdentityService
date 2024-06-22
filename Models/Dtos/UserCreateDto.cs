namespace IdentityService.Models.Dtos
{
  public class UserCreateDto
  {
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Family { get; set; }
    public string NationalNumber { get; set; }
    public string MobileNumber { get; set; }
    public AddressModel? Address { get; set; }
    public int? OrganizationId { get; set; }

  }
}
