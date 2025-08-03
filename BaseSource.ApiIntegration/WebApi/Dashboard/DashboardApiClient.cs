using BaseSource.Shared.Constants;
using BaseSource.ViewModels.Common;
using System.Net.Http;
using System.Threading.Tasks;
using BaseSource.Utilities.Helper;
using BaseSource.ViewModels.User;

namespace BaseSource.ApiIntegration.WebApi
{
    public interface IDashboardApiClient
    {
        Task<ApiResult<DashboardVm>> GetStats();
    }

    public class DashboardApiClient : IDashboardApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DashboardApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResult<DashboardVm>> GetStats()
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.BackendApiClient);
            return await client.GetAsync<ApiResult<DashboardVm>>("/api/Dashboard/GetStats");
        }
    }
}
