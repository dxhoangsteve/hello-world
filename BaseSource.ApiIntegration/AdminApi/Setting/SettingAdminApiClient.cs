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

namespace BaseSource.ApiIntegration.AdminApi
{
    public class SettingAdminApiClient : ISettingAdminApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SettingAdminApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResult<ConfigViewModel>> GetAlls()
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.BackendApiClient);
            return await client.GetAsync<ApiResult<ConfigViewModel>>("/api/admin/Setting/GetAlls");
        }

        public async Task<ApiResult<string>> UpdateConfig(ConfigViewModel model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.BackendApiClient);
            return await client.PostAsync<ApiResult<string>>("/api/admin/Setting/UpdateConfig", model);
        }
    }
}
