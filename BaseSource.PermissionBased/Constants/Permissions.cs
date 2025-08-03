using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.PermissionBased.Constants
{
    public static class Permissions
    {
        public static class User
        {
            public const string View = "Permissions.User.View";
            public const string Create = "Permissions.User.Create";
            public const string Edit = "Permissions.User.Edit";
            public const string Role = "Permissions.User.Role";
            public const string ResetPassword = "Permissions.User.ResetPassword";
            public const string Lock = "Permissions.User.Lock";
        }

        public static class AdminPage
        {
            public const string View = "Permissions.AdminPage.View";
        }

    }
}
