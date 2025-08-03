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
using static BaseSource.Shared.Constants.RoleConstants;

namespace BaseSource.ApiIntegration.AdminApi
{
    public interface IDashboardAdminApiClient
    {
        Task<ApiResult<DashboardAdminVm>> GetStats();
    }

    public class DashboardAdminApiClient : IDashboardAdminApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DashboardAdminApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResult<DashboardAdminVm>> GetStats()
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.BackendApiClient);
            return await client.GetAsync<ApiResult<DashboardAdminVm>>("/api/admin/Dashboard/GetStats");
        }
    }
}
