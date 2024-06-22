using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System;

namespace IdentityService.Models
{
  public class ApplicationUser : IdentityUser
  {
    public string? Name { get; set; }
    public string? Family { get; set; }
    public string? NationalNumber { get; set; }
    public string? MobileNumber { get; set; }
    public Guid? AddressId { get; set; }
    public AddressModel? Address { get; set; }
    public string? ImageUrl { get; set; }
    public List<Permission> Permissions { get; set; } = new List<Permission>();
    public int? OrganizationId { get; set; }
    public bool IsSuperAdmin { get; set; } = false;


  }
}
