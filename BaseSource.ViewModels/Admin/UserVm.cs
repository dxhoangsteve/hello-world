using BaseSource.Shared.Enums;
using BaseSource.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BaseSource.ViewModels.Admin
{
    public class GetUserPagingRequest_Admin : PageQuery
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool? Locked { get; set; }
        public string RoleId { get; set; }
    }

    public class UserVm
    {
        public string Id { get; set; }
        public string CustomId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public IList<string> Roles { get; set; }

        public DateTime JoinedDate { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public DateTime? LockoutEndDateUtc
        {
            get
            {
                return LockoutEnd != null ? LockoutEnd.Value.UtcDateTime : null;
            }
        }

        public string Phone { get; set; }
        public bool EmailConfirmed { get; set; }
    }
    public class UserAdminInfoDto
    {
        public string Id { get; set; }
        public string CustomId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }

        public DateTime JoinedDate { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }

        public string Phone { get; set; }
        public string PasswordText { get; set; }
    }

    public class RoleAssignVm
    {
        [Required]
        public string Id { get; set; }
        public List<SelectItem> Roles { get; set; } = new List<SelectItem>();
    }

    public class EditBalanceVm
    {
        [Required]
        public string UserId { get; set; }

        [Display(Name = "Số tiền (+/-)")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public long? Amount { get; set; }

        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
    }

    public class UserVerifyDto
    {
        [Required]
        public string UserId { get; set; }
    }

    public class CreateUserAdminVm
    {
        [Display(Name = "Mã NV")]
        public string CustomId { get; set; }

        [Display(Name = "Tên")]
        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        public string FirstName { get; set; }

        [Display(Name = "Họ")]
        public string LastName { get; set; }

        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        //[RegularExpression("^[A-z0-9]+$", ErrorMessage = "UserName không được chứa ký tự đặc biệt")]
        public string UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        public string Password { get; set; }

        [Display(Name = "SĐT")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class EditUserAdminVm : CreateUserAdminVm
    {
        public string Id { get; set; }
    }


    public class GetAllUserRequest_Admin
    {
    }
}