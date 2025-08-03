using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BaseSource.PermissionBased.Constants.Permissions;

namespace BaseSource.PermissionBased.Constants
{
    public static class PermissionModules
    {
        public static List<string> GeneratePermissionsForModule(string module)
        {
            var result = new List<string>();
            switch (module)
            {
                case AdminPage:
                    result.AddRange(typeof(Permissions.AdminPage).GetFields().Select(x => x.GetValue(null).ToString()).ToList());
                    break;
                case User:
                    result.AddRange(typeof(Permissions.User).GetFields().Select(x => x.GetValue(null).ToString()).ToList());
                    break;
                default:
                    break;
            }

            return result;
        }

        public static List<string> GetAllPermissionsModules()
        {
            return new List<string>()
            {
                User,
                AdminPage,
            };
        }

        public const string User = "User";
        public const string AdminPage = "AdminPage";
    }
}
