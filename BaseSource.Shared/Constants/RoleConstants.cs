using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Shared.Constants
{
    public class RoleConstants
    {
        public const string Admin = "Admin";

        public static readonly ReadOnlyCollection<String> NotAllowedModify =
            new ReadOnlyCollection<string>(new List<String> { Admin });

        public class Id
        {
            public const string Admin = "3423d55f-e6e9-41dc-8e3e-a9efb36e3a69";
        }
    }
}
