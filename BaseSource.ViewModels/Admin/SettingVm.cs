using BaseSource.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ViewModels.Admin
{
    public class ConfigViewModel
    {
        #region Mail
        [Display(Name = "Sender Email")]
        [Required]
        public string EmailSender { get; set; }

        [Display(Name = "Password Sender Email")]
        [Required]
        public string EmailSenderPassword { get; set; }

        [Display(Name = "Email Host")]
        [Required]
        public string EmailHost { get; set; }

        [Display(Name = "Email Port")]
        [Required]
        public string EmailPort { get; set; }

        [Display(Name = "Email SSL")]
        [Required]
        public string EmailSSL { get; set; }
        #endregion

        [Display(Name = "SĐT liên hệ")]
        public string ContactPhone { get; set; }

        [Display(Name = "Email liên hệ")]
        public string ContactEmail { get; set; }

        [Display(Name = "Support url")]
        public string SupportUrl { get; set; }
    }
}
