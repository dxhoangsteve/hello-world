using BaseSource.ViewModels.Admin;
using BaseSource.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ApiIntegration.AdminApi
{
    public interface IUserAdminApiClient
    {
        Task<ApiResult<PagedResult<UserVm>>> GetPagings(GetUserPagingRequest_Admin model);

        Task<ApiResult<List<SelectItem>>> GetAllRoles();
        Task<ApiResult<RoleAssignVm>> GetUserRoles(string id);
        Task<ApiResult<string>> RoleAssign(RoleAssignVm model);
        Task<ApiResult<string>> ResetPassword(string id);
        Task<ApiResult<string>> Lock(string id);
        Task<ApiResult<UserAdminInfoDto>> GetById(string userId);
        Task<ApiResult<string>> Edit(EditUserAdminVm model);
        Task<ApiResult<string>> VerifyEmailAsync(UserVerifyDto model);
        Task<ApiResult<string>> Create(CreateUserAdminVm model);

        Task<ApiResult<List<SelectItem>>> GetAllUsers(GetAllUserRequest_Admin request);
    }
}
