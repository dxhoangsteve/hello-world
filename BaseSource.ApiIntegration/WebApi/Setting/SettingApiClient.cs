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
using BaseSource.ViewModels.User;

namespace BaseSource.ApiIntegration.WebApi
{
    public class SettingApiClient : ISettingApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SettingApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResult<SettingPublicVm>> GetAll()
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.BackendApiClient);
            return await client.GetAsync<ApiResult<SettingPublicVm>>("/api/Setting/GetAll");
        }
    }
}
