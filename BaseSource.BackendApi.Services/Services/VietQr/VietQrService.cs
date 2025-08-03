using BaseSource.Data.EF;
using BaseSource.ViewModels.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BaseSource.BackendApi.Services
{
    public interface IVietQrService
    {
        Task<ApiResult<List<GetBanksVietQrItem>>> GetBanks();
    }

    public class VietQrService : IVietQrService
    {
        private readonly BaseSourceDbContext _db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public VietQrService(BaseSourceDbContext db, IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<ApiResult<List<GetBanksVietQrItem>>> GetBanks()
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.vietqr.io/v2/banks");

            var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<GetBanksVietQr_ApiResponse>(body);
                if (result.code == "00")
                {
                    return new ApiSuccessResult<List<GetBanksVietQrItem>>(result.data);
                }
                else
                {
                    return new ApiErrorResult<List<GetBanksVietQrItem>>(body);
                }
            }

            return new ApiErrorResult<List<GetBanksVietQrItem>>(body);
        }

    }

    public class GetBanksVietQr_ApiResponse
    {
        public string code { get; set; }
        public string desc { get; set; }
        public List<GetBanksVietQrItem> data { get; set; }
    }

    public class GetBanksVietQrItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string bin { get; set; }
        public string shortName { get; set; }
        public string logo { get; set; }
        public int? transferSupported { get; set; }
        public int? lookupSupported { get; set; }
        public string swift_code { get; set; }
    }
}
