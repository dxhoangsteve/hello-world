using BaseSource.Shared.Constants;
using BaseSource.Shared.Enums;
using BaseSource.Utilities.Helper;
using BaseSource.ViewModels.Admin;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.User;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace BaseSource.ApiIntegration.AdminApi.Article
{
    public class EditorApiClient : IEditorApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EditorApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResult<string>> UploadImage(IFormFile file, string id)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.BackendApiClient);
            var multiContent = new MultipartFormDataContent();
            if (file != null)
            {
                var fileStreamContent = new StreamContent(file.OpenReadStream());
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                multiContent.Add(fileStreamContent, "ImageFile", file.FileName);
            }

            multiContent.Add(new StringContent(id ?? ""), "UploadId");

            return await client.PostFormAsync<ApiResult<string>>("/api/Editor/UploadImage", multiContent);
        }

        public async Task<ApiResult<string>> UploadVideo(IFormFile file, string id)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.BackendApiClient);
            var multiContent = new MultipartFormDataContent();
            if (file != null)
            {
                var fileStreamContent = new StreamContent(file.OpenReadStream());
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                multiContent.Add(fileStreamContent, "VideoFile", file.FileName);
            }

            multiContent.Add(new StringContent(id ?? ""), "UploadId");

            return await client.PostFormAsync<ApiResult<string>>("/api/Editor/UploadVideo", multiContent);
        }
    }
}
