using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ViewModels.Shared.Account
{
    public class LoginRequestVm
    {
        [Display(Name = "Email")]
        [Required]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required]
        public string Password { get; set; }
    }

    public class LoginResponseVm
    {
        public string Token { get; set; }

        public bool TwoFactorEnabled { get; set; }
    }

    public class Login2FaVerifyObj
    {
        public string UserId { get; set; }

        public DateTime Exp { get; set; }
    }
}
