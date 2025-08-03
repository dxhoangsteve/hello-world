using BaseSource.Shared.Constants;
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
using BaseSource.ViewModels.Shared;

namespace BaseSource.ApiIntegration.SharedApi.Bank
{
    public interface IBankApiClient
    {
        Task<ApiResult<List<BankInfoVm>>> GetAll();
    }

    public class BankApiClient : IBankApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BankApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResult<List<BankInfoVm>>> GetAll()
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.BackendApiClient);
            return await client.GetAsync<ApiResult<List<BankInfoVm>>>("/api/Bank/GetAll");
        }
    }
}
