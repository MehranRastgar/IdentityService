using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityService.Models.Dtos
{
  //   public class UserCreateDto
  //   {
  //     [Required]
  //     public string UserName { get; set; }

  //     [Required]
  //     public string Password { get; set; } // Consider using a DTO that doesn't expose passwords directly
  //     public string Email { get; set; }
  //     public string Name { get; set; }
  //     public string Family { get; set; }
  //     public string NationalNumber { get; set; }
  //     public string MobileNumber { get; set; }
  //   }
  public class UserReadDto
  {
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Family { get; set; }
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public string NationalNumber { get; set; }
    public List<RoleReadDto> Roles { get; set; } // Only role names might be enough
    public AddressReadDto? Address { get; set; }
    public int? OrganizationId { get; set; }

  }
  //   public class UserUpdateDto
  //   {
  //     public string Name { get; set; }
  //     public string Family { get; set; }
  //     public string NationalNumber { get; set; }
  //     public string MobileNumber { get; set; }
  //   }


}
