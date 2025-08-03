using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Data.Entities
{
    public class AppUser : IdentityUser<string>
    {
        public string PasswordText { get; set; }

        public virtual UserProfile UserProfile { get; set; }
    }

    public class AppRole : IdentityRole<string>
    {
        public string Description { get; set; }
    }

    public class AppUserLogin : IdentityUserLogin<string>
    {
    }

    //public class AppRoleClaim : IdentityRoleClaim<string>
    //{
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //}
}
