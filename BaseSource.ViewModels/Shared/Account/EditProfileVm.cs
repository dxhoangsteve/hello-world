using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ViewModels.Shared.Account
{
    public class EditProfileVm
    {
        [Display(Name = "Tên")]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Họ")]
        public string LastName { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
    }
}
