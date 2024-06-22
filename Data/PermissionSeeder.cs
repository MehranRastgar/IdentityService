using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityService.Models;

namespace IdentityService.Data
{
  public static class PermissionSeeder
  {
    public static async Task SeedPermissionsAsync(IServiceProvider serviceProvider)
    {
      var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

      if (!dbContext.Permissions.Any())
      {
        var permissions = new List<Permission>
                {
                  //ocelot routes
                    new Permission { Name = "ManageGeofence", Description = "Manage Geofences" },
                    new Permission { Name = "ViewGeofence", Description = "View Geofences" },

                    new Permission { Name = "ViewReport", Description = "View reports" },
                    new Permission { Name = "ManageReport", Description = "Manage Reports" },

                    new Permission { Name = "ViewUsers", Description = "View Users" },
                    new Permission { Name = "ManageUsers", Description = "Manage Users" },

                    new Permission { Name = "ManagePermissions", Description = "Manage Permissions" },
                    new Permission { Name = "ViewPermissions", Description = "View Permissions" },

                    new Permission { Name = "ViewDevices", Description = "View Devices" },
                    new Permission { Name = "ManageDevices", Description = "Manage Devices" },

                    new Permission { Name = "ViewGroups", Description = "View Groups" },
                    new Permission { Name = "ManageGroups", Description = "Manage Groups" },

                    new Permission { Name = "ViewOrganizations", Description = "View Organizations" },
                    new Permission { Name = "ManageOrganizations", Description = "Manage Organizations" },

                    new Permission { Name = "ViewDeviceSettings", Description = "View Device Settings" },
                    new Permission { Name = "ManageDeviceSettings", Description = "Manage Device Settings" },

                    new Permission { Name = "ViewLiveReport", Description = "View Live Locations" },
                  //front routes
                    new Permission { Name = "ViewMap", Description = "View Map" },
                    new Permission { Name = "ViewMapMission", Description = "View Map without has mission" },

                    new Permission { Name = "ViewAllAsset", Description = "View All Asset On Organ" },
                    new Permission { Name = "ManageAllAsset", Description = "Manage All Asset On Organ" },

                    // new Permission { Name = "SuperAdmin", Description = "Can do Anything" },

                };

        dbContext.Permissions.AddRange(permissions);
        await dbContext.SaveChangesAsync();
      }
    }
  }
}


// var permissions = new List<Permission>
// [
//   "ManageGeofence",

// "ViewReport",
// "ManageReport"
// , "ViewUsers", "ManageUsers", "ManagePermissions", "ViewPermissions", "ViewDevices", "ManageDevices", "ViewGroups", "ManageGroups", "ViewOrganizations", "ManageOrganizations",
// "ViewDeviceSettings", "ManageDeviceSettings", "ViewLiveReport", "ViewMap",

// "ViewMapMission"

// ]