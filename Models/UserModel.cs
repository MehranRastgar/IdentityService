// using Microsoft.EntityFrameworkCore;
// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;

// namespace IdentityService.Models
// {
//   public class UserModel
//   {
//     [Key]
//     [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//     public Guid Id { get; set; }

//     /*[Required]*/
//     public string? IdentityId { get; set; } // Foreign key to ASP.NET Identity User

//     [Required]
//     public string UserName { get; set; }

//     [Required]
//     public string Password { get; set; } // Consider storing a password hash, not the raw password

//     public string Name { get; set; }
//     public string Family { get; set; }
//     public string Email { get; set; }
//     public string MobileNumber { get; set; }
//     public string NationalNumber { get; set; }

//     // Many-to-Many relation with RoleModel
//     public List<RoleModel> Roles { get; } = new List<RoleModel>();

//     // One-to-One relation with AddressModel
//     public Guid? AddressId { get; set; }
//     public AddressModel Address { get; set; }
//   }
// }
