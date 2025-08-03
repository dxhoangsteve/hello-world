using System;
using System.Collections.Generic;

namespace BaseSource.ViewModels.Shared.Account
{
    public class UserInfoResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<string> Roles { get; set; }
        public DateTime JoinedDate { get; set; }

        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public bool TwoFactor { get; set; }

        public string ApiKey { get; set; }
    }
}
