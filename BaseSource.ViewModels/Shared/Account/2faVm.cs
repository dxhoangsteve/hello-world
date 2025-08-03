using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BaseSource.ViewModels.Shared.Account
{
    public class GoogleAuthenticatorViewModel
    {
        public string SecretKey { get; set; }
        public string BarcodeUrl { get; set; }
    }

    public class EnableTwoFactorAuthenticationVm : GoogleAuthenticatorViewModel
    {
        [Required]
        [Display(Name = "Code")]
        [StringLength(6, MinimumLength = 6)]
        public string Code { get; set; }
    }

    public class VerifyTFAViewModel
    {
        [Required]
        [Display(Name = "2FA code")]
        [StringLength(6, MinimumLength = 6)]
        public string Code { get; set; }

        public string Token { get; set; }

        public string ReturnUrl { get; set; }
    }
}
