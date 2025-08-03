using BaseSource.PermissionBased.Constants;
using BaseSource.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Utilities.Extensions
{
    public static class PrincipalExtensions
    {
        public static bool IsInAllRoles(this IPrincipal principal, params string[] roles)
        {
            return roles.All(r1 => r1.Split(',').All(r2 => principal.IsInRole(r2.Trim())));
        }

        public static bool IsInAnyRoles(this IPrincipal principal, params string[] roles)
        {
            return roles.Any(r1 => r1.Split(',').Any(r2 => principal.IsInRole(r2.Trim())));
        }

        public static bool IsInPermission(this IPrincipal principal, string permission)
        {
            var pers = ((ClaimsIdentity)principal.Identity).Claims
                                .Where(c => c.Type.Equals(ApplicationClaimTypes.Permission))
                                .Select(c => c.Value).ToList();

            return pers.Any(x => x.Equals(permission.Trim()));
        }

        public static bool IsInAnyPermissions(this IPrincipal principal, params string[] permissions)
        {
            var pers = ((ClaimsIdentity)principal.Identity).Claims
                                .Where(c => c.Type.Equals(ApplicationClaimTypes.Permission))
                                .Select(c => c.Value).ToList();

            return permissions.Any(x => pers.Any(y => y.Equals(x.Trim())));
        }
    }
}
