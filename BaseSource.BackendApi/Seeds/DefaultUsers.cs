using BaseSource.Data.Entities;
using BaseSource.PermissionBased.Constants;
using BaseSource.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using BaseSource.PermissionBased.Helpers;
using BaseSource.PermissionBased.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Data;

namespace BaseSource.BackendApi.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedDataAsync(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            await roleManager.SeedClaimsForSuperAdmin();
        }

        private async static Task SeedClaimsForSuperAdmin(this RoleManager<AppRole> roleManager)
        {
            //Check if Role Exists
            var adminRoleInDb = await roleManager.FindByNameAsync(RoleConstants.Admin);
            if (adminRoleInDb == null)
            {
                await roleManager.CreateAsync(new AppRole
                {
                    Id = RoleConstants.Id.Admin,
                    Name = RoleConstants.Admin,
                    Description = "Administrator role"
                });
            }

            var allPermissions = ClaimsHelper.GetAllPermissions();
            var claims = await roleManager.GetClaimsAsync(adminRoleInDb);
            foreach (var claim in claims)
            {
                if (!allPermissions.Any(x => x.Value == claim.Value))
                {
                    await roleManager.RemoveClaimAsync(adminRoleInDb, claim);
                }
            }

            foreach (var permissionModule in PermissionModules.GetAllPermissionsModules())
            {
                await roleManager.AddPermissionClaimByModule(adminRoleInDb, permissionModule);
            }
        }
    }
}
