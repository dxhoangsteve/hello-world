using BaseSource.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BaseSource.Data.Entities
{
    public class UserProfile
    {
        public string UserId { get; set; }
        public string CustomId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }

        public DateTime JoinedDate { get; set; }
        public string ApiKey { get; set; }

        // object
        public AppUser AppUser { get; set; }
    }
}
