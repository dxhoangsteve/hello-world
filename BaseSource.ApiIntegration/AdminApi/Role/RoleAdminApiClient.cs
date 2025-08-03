using BaseSource.Shared.Constants;
using BaseSource.ViewModels.Admin;
using BaseSource.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BaseSource.Utilities.Helper;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using BaseSource.PermissionBased.ViewModels;

namespace BaseSource.ApiIntegration.AdminApi
{
    public interface IRoleAdminApiClient
    {
        Task<ApiResult<List<RoleVm>>> GetAlls();
        Task<ApiResult<string>> Create(CreateRoleVm model);
        Task<ApiResult<RoleVm>> GetById(string id);
        Task<ApiResult<string>> Edit(EditRoleVm model);
        Task<ApiResult<string>> Delete(string id);
        Task<ApiResult<PermissionVm>> GetPermissionsByRoleId(string roleId);
        Task<ApiResult<string>> UpdatePermission(PermissionVm model);
    }

    public class RoleAdminApiClient : IRoleAdminApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RoleAdminApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResult<List<RoleVm>>> GetAlls()
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.BackendApiClient);
            return await client.GetAsync<ApiResult<List<RoleVm>>>("/api/admin/Role/GetAlls");
        }

        public async Task<ApiResult<RoleVm>> GetById(string id)
        {
            var obj = new
            {
                id = id
            };
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.BackendApiClient);
            return await client.GetAsync<ApiResult<RoleVm>>("/api/admin/Role/GetById", obj);
        }

        public async Task<ApiResult<string>> Create(CreateRoleVm model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.BackendApiClient);
            return await client.PostAsync<ApiResult<string>>("/api/admin/Role/Create", model);
        }

        public async Task<ApiResult<string>> Edit(EditRoleVm model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.BackendApiClient);
            return await client.PostAsync<ApiResult<string>>("/api/admin/Role/Edit", model);
        }

        public async Task<ApiResult<string>> Delete(string id)
        {
            var dic = new Dictionary<string, string>()
            {
                { "id", id }
            };

            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.BackendApiClient);
            return await client.PostAsyncFormUrl<ApiResult<string>>("/api/admin/Role/Delete", dic);
        }

        public async Task<ApiResult<PermissionVm>> GetPermissionsByRoleId(string roleId)
        {
            var obj = new
            {
                roleId = roleId
            };
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.BackendApiClient);
            return await client.GetAsync<ApiResult<PermissionVm>>("/api/admin/Role/GetPermissionsByRoleId", obj);
        }

        public async Task<ApiResult<string>> UpdatePermission(PermissionVm model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.BackendApiClient);
            return await client.PostAsync<ApiResult<string>>("/api/admin/Role/UpdatePermission", model);
        }
    }
}
