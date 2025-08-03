using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using BaseSource.ViewModels.Shared.Account;
using BaseSource.ViewModels.Common;
using BaseSource.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BaseSource.Shared.Constants;
using Newtonsoft.Json;
// using BaseSource.Utilities.Helpers;
using Google.Authenticator;

namespace BaseSource.Test.WebApp
{
    /// <summary>
    /// Test đăng nhập - kiểm tra các hàm thực tế được sử dụng trong login process
    /// </summary>
    public class LoginTests
    {
        #region LoginRequestVm Validation Tests - Test validation của LoginRequestVm

        /// <summary>
        /// Test LoginRequestVm với UserName trống - sử dụng DataAnnotations thực tế
        /// </summary>
        [Fact]
        public void LoginRequestVm_EmptyUserName_HasValidationError_Test()
        {
            // Arrange - Tạo LoginRequestVm với UserName trống
            var loginRequest = new LoginRequestVm
            {
                UserName = "", // Trống - vi phạm [Required]
                Password = "ValidPassword123!"
            };

            // Act - Sử dụng Validator.TryValidateObject() thực tế từ System.ComponentModel.DataAnnotations
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(loginRequest, null, null);
            var isValid = Validator.TryValidateObject(loginRequest, validationContext, validationResults, true);

            // Assert - Kiểm tra validation thất bại và có lỗi cho UserName
            Assert.False(isValid); // Model không hợp lệ
            Assert.Contains(validationResults, v => v.MemberNames.Contains("UserName")); // Có lỗi UserName
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("required") || v.ErrorMessage.Contains("Required")); // Thông báo required
        }

        /// <summary>
        /// Test LoginRequestVm với Password trống - sử dụng DataAnnotations thực tế
        /// </summary>
        [Fact]
        public void LoginRequestVm_EmptyPassword_HasValidationError_Test()
        {
            // Arrange - Tạo LoginRequestVm với Password trống
            var loginRequest = new LoginRequestVm
            {
                UserName = "testuser@example.com",
                Password = "" // Trống - vi phạm [Required]
            };

            // Act - Sử dụng Validator.TryValidateObject() thực tế
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(loginRequest, null, null);
            var isValid = Validator.TryValidateObject(loginRequest, validationContext, validationResults, true);

            // Assert - Kiểm tra validation thất bại và có lỗi cho Password
            Assert.False(isValid); // Model không hợp lệ
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Password")); // Có lỗi Password
        }

        /// <summary>
        /// Test LoginRequestVm với dữ liệu hợp lệ - sử dụng DataAnnotations thực tế
        /// </summary>
        [Fact]
        public void LoginRequestVm_ValidData_PassesValidation_Test()
        {
            // Arrange - Tạo LoginRequestVm với dữ liệu hợp lệ
            var loginRequest = new LoginRequestVm
            {
                UserName = "testuser@example.com",
                Password = "ValidPassword123!"
            };

            // Act - Sử dụng Validator.TryValidateObject() thực tế
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(loginRequest, null, null);
            var isValid = Validator.TryValidateObject(loginRequest, validationContext, validationResults, true);

            // Assert - Kiểm tra validation thành công
            Assert.True(isValid); // Model hợp lệ
            Assert.Empty(validationResults); // Không có lỗi validation
        }

        #endregion

        #region WebApp Result Models - Các class model cho kết quả WebApp

        /// <summary>
        /// Kết quả đăng nhập WebApp
        /// </summary>
        public class WebAppLoginResult
        {
            public bool IsSuccess { get; set; }
            public bool RequiresTwoFactor { get; set; }
            public string RedirectUrl { get; set; }
            public bool CookieSet { get; set; }
            public bool TempCookieSet { get; set; }
            public string ErrorMessage { get; set; }
        }

        /// <summary>
        /// Kết quả xác thực 2FA WebApp
        /// </summary>
        public class WebApp2FAResult
        {
            public bool IsSuccess { get; set; }
            public string RedirectUrl { get; set; }
            public bool FinalCookieSet { get; set; }
            public bool TempCookieDeleted { get; set; }
            public string ErrorMessage { get; set; }
        }

        /// <summary>
        /// Kết quả forgot password WebApp
        /// </summary>
        public class WebAppForgotPasswordResult
        {
            public bool IsSuccess { get; set; }
            public string SuccessMessage { get; set; }
            public string ErrorMessage { get; set; }
            public string ViewName { get; set; }
        }

        #endregion
    }
}


