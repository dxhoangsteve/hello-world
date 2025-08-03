
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.Shared.Account;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ApiIntegration.WebApi
{
    public interface IUserApiClient
    {
        Task<ApiResult<string>> Register(RegisterRequestVm model);
        Task<ApiResult<LoginResponseVm>> Authenticate(LoginRequestVm model);
        Task<ApiResult<string>> ConfirmEmail(ConfirmEmailVm model);
        Task<ApiResult<string>> ForgotPassword(ForgotPasswordVm model);
        Task<ApiResult<string>> ResetPassword(ResetPasswordVm model);
        Task<ApiResult<UserInfoResponse>> GetUserInfo();
        Task<ApiResult<string>> EditProfile(EditProfileVm model);
        Task<ApiResult<string>> ChangePassword(ChangePasswordVm model);
        Task<ApiResult<string>> AuthenticateExternalAsync(UserClaimRequest model);

        Task<ApiResult<GoogleAuthenticatorViewModel>> EnableTwoFactorAuthentication();
        Task<ApiResult<string>> EnableTwoFactorAuthentication(string inputCode);
        Task<ApiResult<string>> DisableTwoFactorAuthentication();
        Task<ApiResult<string>> VerifyTwoFactorAuthentication(VerifyTFAViewModel model);

        Task<ApiResult<string>> ResetApiKey();
    }
}
