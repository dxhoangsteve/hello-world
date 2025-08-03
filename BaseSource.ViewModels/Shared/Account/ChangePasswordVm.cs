using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ViewModels.Shared.Account
{
    public class ChangePasswordVm
    {
        [Display(Name = "Mật khẩu hiện tại")]
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [Required]
        [StringLength(50, ErrorMessage = "Password minimum 6 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Xác nhận mật khẩu")]
        [Required]
        [StringLength(50, ErrorMessage = "Password minimum 6 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
