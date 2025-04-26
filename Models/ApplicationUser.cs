using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

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

    // Physical Information
    public int? Age { get; set; }
    public string? Gender { get; set; }  // male, female
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal? Height { get; set; }  // in centimeters
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal? Weight { get; set; }  // in kilograms
    
    public string? ActivityLevel { get; set; }  // sedentary, light, moderate, active, very_active
    public string? Goal { get; set; }  // lose_weight, gain_muscle, maintain, improve_fitness
  }
  public class CreateUserByPhoneDto
  {
    public string phone { get; set; }
    public string password { get; set; }
    public string userName { get; set; }
  }
}
