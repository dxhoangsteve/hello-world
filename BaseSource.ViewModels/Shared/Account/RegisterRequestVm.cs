using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ViewModels.Shared.Account
{
    public class RegisterRequestVm
    {

        //[Display(Name = "Tên đăng nhập")]
        //[Required]
        //public string UserName { get; set; }

        [Display(Name = "Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required]
        [StringLength(50, ErrorMessage = "Password minimum 6 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Nhập lại mật khẩu")]
        [Required]
        [StringLength(50, ErrorMessage = "Password minimum 6 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
