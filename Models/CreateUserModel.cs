namespace IdentityService.Models
{
  public class CreateUserModel
  {
    public string UserName { get; set; }
    public string Password { get; set; }
    public string MobileNumber { get; set; }
    public string Otp { get; set; }
  }
}
