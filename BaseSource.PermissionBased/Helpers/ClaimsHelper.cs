using BaseSource.Data.Entities;
using BaseSource.PermissionBased.Constants;
using BaseSource.PermissionBased.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BaseSource.PermissionBased.Helpers
{
    public static class ClaimsHelper
    {
        public static List<RoleClaimsResponse> GetAllPermissions()
        {
            var allPermissions = new List<RoleClaimsResponse>();
            foreach (var permissionModule in PermissionModules.GetAllPermissionsModules())
            {
                foreach (var permission in PermissionModules.GeneratePermissionsForModule(permissionModule))
                {
                    allPermissions.Add(new RoleClaimsResponse { Value = permission, Type = ApplicationClaimTypes.Permission });
                }
            }

            return allPermissions;
        }

        //public static async Task AddPermissionClaim(this RoleManager<AppRole> roleManager, AppRole role, string permission)
        //{
        //    var allClaims = await roleManager.GetClaimsAsync(role);
        //    if (!allClaims.Any(a => a.Type == ApplicationClaimTypes.Permission && a.Value == permission))
        //    {
        //        await roleManager.AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, permission));
        //    }
        //}

        public static async Task AddPermissionClaimByModule(this RoleManager<AppRole> roleManager, AppRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = PermissionModules.GeneratePermissionsForModule(module);
            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == ApplicationClaimTypes.Permission && a.Value == permission))
                {
                    await roleManager.AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, permission));
                }
            }
        }
    }
}
