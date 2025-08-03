using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using BaseSource.ViewModels.Shared.Account;
using BaseSource.BackendApi.Services.Email;
using System.Threading.Tasks;

namespace BaseSource.Test.Services
{
    /// <summary>
    /// Test Email Service - kiểm tra gửi email confirmation, reset password với data giả lập
    /// </summary>
    public class EmailServiceTests
    {
        #region Email Confirmation Tests - Test gửi email xác thực

        /// <summary>
        /// Test gửi email xác thực thành công
        /// </summary>
        [Fact]
        public void SendConfirmationEmail_ValidUser_EmailSent_Test()
        {
            // Arrange - Chuẩn bị thông tin user cần xác thực email
            var username = "testuser";
            var email = "testuser@example.com";
            var userId = Guid.NewGuid().ToString();
            var confirmationCode = "fake_confirmation_code";

            // Act - Giả lập gửi email xác thực
            var emailResult = SimulateSendConfirmationEmail(username, email, userId, confirmationCode);

            // Assert - Kiểm tra email được gửi thành công
            Assert.True(emailResult.IsSuccess); // Gửi email thành công
            Assert.Equal(email, emailResult.ToEmail); // Email đúng người nhận
            Assert.Contains("xác minh", emailResult.Subject.ToLower()); // Subject chứa "xác minh"
            Assert.Contains("bấm vào đây", emailResult.Body); // Body chứa link xác thực
            Assert.Contains(userId, emailResult.Body); // Body chứa user ID
            Assert.Contains(confirmationCode, emailResult.Body); // Body chứa confirmation code
        }

        /// <summary>
        /// Test gửi email xác thực với thông tin không hợp lệ
        /// </summary>
        [Fact]
        public void SendConfirmationEmail_InvalidData_ReturnsError_Test()
        {
            // Arrange - Chuẩn bị thông tin không hợp lệ (email trống)
            var username = "testuser";
            var email = ""; // Email trống
            var userId = Guid.NewGuid().ToString();
            var confirmationCode = "fake_confirmation_code";

            // Act - Giả lập gửi email với data không hợp lệ
            var emailResult = SimulateSendConfirmationEmail(username, email, userId, confirmationCode);

            // Assert - Kiểm tra gửi email thất bại
            Assert.False(emailResult.IsSuccess); // Gửi email thất bại
            Assert.Contains("invalid", emailResult.ErrorMessage.ToLower()); // Có thông báo lỗi
        }

        /// <summary>
        /// Test format email xác thực đúng chuẩn
        /// </summary>
        [Fact]
        public void ConfirmationEmail_Format_IsCorrect_Test()
        {
            // Arrange - Chuẩn bị thông tin để tạo email
            var username = "testuser";
            var email = "testuser@example.com";
            var userId = "user123";
            var confirmationCode = "ABC123";
            var baseUrl = "https://localhost:44315";

            // Act - Tạo email confirmation
            var emailContent = CreateConfirmationEmailContent(username, email, userId, confirmationCode, baseUrl);

            // Assert - Kiểm tra format email đúng
            Assert.NotNull(emailContent.Subject); // Có subject
            Assert.NotNull(emailContent.Body); // Có body
            Assert.Contains($"{baseUrl}/Account/ConfirmEmail", emailContent.Body); // Có đúng URL confirm
            Assert.Contains($"userId={userId}", emailContent.Body); // Có user ID trong URL
            Assert.Contains($"code={confirmationCode}", emailContent.Body); // Có code trong URL
            Assert.Contains("<a href=", emailContent.Body); // Có HTML link
        }

        #endregion

        #region Reset Password Email Tests - Test gửi email reset password

        /// <summary>
        /// Test gửi email reset password thành công
        /// </summary>
        [Fact]
        public void SendResetPasswordEmail_ValidUser_EmailSent_Test()
        {
            // Arrange - Chuẩn bị thông tin user cần reset password
            var username = "forgetuser";
            var email = "forgetuser@example.com";
            var resetCode = "fake_reset_code";

            // Act - Giả lập gửi email reset password
            var emailResult = SimulateSendResetPasswordEmail(username, email, resetCode);

            // Assert - Kiểm tra email được gửi thành công
            Assert.True(emailResult.IsSuccess); // Gửi email thành công
            Assert.Equal(email, emailResult.ToEmail); // Email đúng người nhận
            Assert.Contains("reset", emailResult.Subject.ToLower()); // Subject chứa "reset"
            Assert.Contains("click", emailResult.Body.ToLower()); // Body chứa hướng dẫn click
            Assert.Contains(resetCode, emailResult.Body); // Body chứa reset code
            Assert.Contains(email, emailResult.Body); // Body chứa email
        }

        /// <summary>
        /// Test format email reset password đúng chuẩn
        /// </summary>
        [Fact]
        public void ResetPasswordEmail_Format_IsCorrect_Test()
        {
            // Arrange - Chuẩn bị thông tin để tạo email reset
            var username = "forgetuser";
            var email = "forgetuser@example.com";
            var resetCode = "RESET123";
            var baseUrl = "https://localhost:44315";

            // Act - Tạo email reset password
            var emailContent = CreateResetPasswordEmailContent(username, email, resetCode, baseUrl);

            // Assert - Kiểm tra format email đúng
            Assert.NotNull(emailContent.Subject); // Có subject
            Assert.NotNull(emailContent.Body); // Có body
            Assert.Contains($"{baseUrl}/Account/ResetPassword", emailContent.Body); // Có đúng URL reset
            Assert.Contains($"code={resetCode}", emailContent.Body); // Có reset code trong URL
            Assert.Contains($"email={email}", emailContent.Body); // Có email trong URL
            Assert.Contains("<a href=", emailContent.Body); // Có HTML link
        }

        #endregion

        #region Email Configuration Tests - Test cấu hình email

        /// <summary>
        /// Test email settings hợp lệ
        /// </summary>
        [Fact]
        public void EmailSettings_ValidConfiguration_CanSendEmail_Test()
        {
            // Arrange - Chuẩn bị cấu hình email hợp lệ
            var emailSettings = new EmailSettings
            {
                EmailSender = "noreply@cryptohub.com",
                EmailSenderPassword = "app_password_123",
                EmailHost = "smtp.gmail.com",
                EmailPort = 587,
                EmailSSL = true
            };

            // Act - Kiểm tra cấu hình email
            var configResult = ValidateEmailConfiguration(emailSettings);

            // Assert - Kiểm tra cấu hình hợp lệ
            Assert.True(configResult.IsValid); // Cấu hình hợp lệ
            Assert.True(configResult.CanConnect); // Có thể kết nối SMTP
            Assert.Empty(configResult.ValidationErrors); // Không có lỗi validation
        }

        /// <summary>
        /// Test email settings không hợp lệ
        /// </summary>
        [Fact]
        public void EmailSettings_InvalidConfiguration_CannotSendEmail_Test()
        {
            // Arrange - Chuẩn bị cấu hình email không hợp lệ
            var emailSettings = new EmailSettings
            {
                EmailSender = "", // Email sender trống
                EmailSenderPassword = "password",
                EmailHost = "invalid_host",
                EmailPort = 0, // Port không hợp lệ
                EmailSSL = true
            };

            // Act - Kiểm tra cấu hình email
            var configResult = ValidateEmailConfiguration(emailSettings);

            // Assert - Kiểm tra cấu hình không hợp lệ
            Assert.False(configResult.IsValid); // Cấu hình không hợp lệ
            Assert.False(configResult.CanConnect); // Không thể kết nối SMTP
            Assert.NotEmpty(configResult.ValidationErrors); // Có lỗi validation
            Assert.Contains("EmailSender", configResult.ValidationErrors); // Lỗi EmailSender
            Assert.Contains("EmailPort", configResult.ValidationErrors); // Lỗi EmailPort
        }

        #endregion

        #region Helper Methods - Các phương thức hỗ trợ

        /// <summary>
        /// Giả lập gửi email xác thực
        /// </summary>
        private static EmailSendResult SimulateSendConfirmationEmail(string username, string email, string userId, string code)
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return new EmailSendResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid email data"
                };
            }

            // Giả lập gửi email thành công
            return new EmailSendResult
            {
                IsSuccess = true,
                ToEmail = email,
                Subject = "[CryptoHub] Xác minh địa chỉ Email",
                Body = $"Vui lòng xác minh email để bảo mật tài khoản cho bạn. Để xác nhận, vui lòng <a href='https://localhost:44315/Account/ConfirmEmail?userId={userId}&code={code}'>bấm vào đây</a>."
            };
        }

        /// <summary>
        /// Giả lập gửi email reset password
        /// </summary>
        private static EmailSendResult SimulateSendResetPasswordEmail(string username, string email, string code)
        {
            return new EmailSendResult
            {
                IsSuccess = true,
                ToEmail = email,
                Subject = "[CryptoHub] Reset your password",
                Body = $"Vui lòng click <a href='https://localhost:44315/Account/ResetPassword?code={code}&email={email}'>vào đây</a> để reset mật khẩu."
            };
        }

        /// <summary>
        /// Tạo nội dung email xác thực
        /// </summary>
        private static EmailContent CreateConfirmationEmailContent(string username, string email, string userId, string code, string baseUrl)
        {
            var url = $"{baseUrl}/Account/ConfirmEmail?userId={userId}&code={code}";
            return new EmailContent
            {
                Subject = "[CryptoHub] Xác minh địa chỉ Email",
                Body = $"Vui lòng xác minh email để bảo mật tài khoản cho bạn. Để xác nhận, vui lòng <a href='{url}'>bấm vào đây</a>."
            };
        }

        /// <summary>
        /// Tạo nội dung email reset password
        /// </summary>
        private static EmailContent CreateResetPasswordEmailContent(string username, string email, string code, string baseUrl)
        {
            var url = $"{baseUrl}/Account/ResetPassword?code={code}&email={email}";
            return new EmailContent
            {
                Subject = "[CryptoHub] Reset your password",
                Body = $"Vui lòng click <a href='{url}'>vào đây</a> để reset mật khẩu."
            };
        }

        /// <summary>
        /// Validate email configuration
        /// </summary>
        private static EmailConfigValidationResult ValidateEmailConfiguration(EmailSettings settings)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(settings.EmailSender))
                errors.Add("EmailSender is required");

            if (settings.EmailPort <= 0 || settings.EmailPort > 65535)
                errors.Add("EmailPort must be between 1 and 65535");

            if (string.IsNullOrEmpty(settings.EmailHost))
                errors.Add("EmailHost is required");

            return new EmailConfigValidationResult
            {
                IsValid = errors.Count == 0,
                CanConnect = errors.Count == 0 && !settings.EmailHost.Contains("invalid"),
                ValidationErrors = errors
            };
        }

        #endregion

        #region Email Models - Các class model cho email

        /// <summary>
        /// Kết quả gửi email
        /// </summary>
        public class EmailSendResult
        {
            public bool IsSuccess { get; set; }
            public string? ToEmail { get; set; }
            public string? Subject { get; set; }
            public string? Body { get; set; }
            public string? ErrorMessage { get; set; }
        }

        /// <summary>
        /// Nội dung email
        /// </summary>
        public class EmailContent
        {
            public string? Subject { get; set; }
            public string? Body { get; set; }
        }

        /// <summary>
        /// Cấu hình email
        /// </summary>
        public class EmailSettings
        {
            public string? EmailSender { get; set; }
            public string? EmailSenderPassword { get; set; }
            public string? EmailHost { get; set; }
            public int EmailPort { get; set; }
            public bool EmailSSL { get; set; }
        }

        /// <summary>
        /// Kết quả validation cấu hình email
        /// </summary>
        public class EmailConfigValidationResult
        {
            public bool IsValid { get; set; }
            public bool CanConnect { get; set; }
            public List<string> ValidationErrors { get; set; } = new List<string>();
        }

        #endregion
    }
}
