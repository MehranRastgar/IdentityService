namespace IdentityService.Models.Dtos
{
  public class LoginDto
  {
    public string UserName { get; set; }
    public string Password { get; set; }
  }

  public class LoginOtpDto
  {
    public string UserName { get; set; }
    public string Code { get; set; }
  }

  public class RequestOtpDto
  {
    public string UserName { get; set; }
  }

  public class LoginMailDto
  {
    public string UserName { get; set; }
    public string Code { get; set; }
  }

  public class RequestMailDto
  {
    public string UserName { get; set; }
  }

}
