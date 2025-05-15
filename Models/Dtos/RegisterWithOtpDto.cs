namespace IdentityService.Models.Dtos
{
    public class RegisterWithOtpDto
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public string MobileNumber { get; set; }
        public string OtpCode { get; set; }
    }
} 