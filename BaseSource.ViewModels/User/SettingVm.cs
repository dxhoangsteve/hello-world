using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ViewModels.User
{
    public class SettingPublicVm
    {
        [Display(Name = "SĐT liên hệ")]
        public string ContactPhone { get; set; }

        [Display(Name = "Email liên hệ")]
        public string ContactEmail { get; set; }

        [Display(Name = "Support url")]
        public string SupportUrl { get; set; }
    }
}
