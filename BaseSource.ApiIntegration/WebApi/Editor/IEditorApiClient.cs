using BaseSource.Shared.Enums;
using BaseSource.ViewModels.Admin;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.User;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BaseSource.ApiIntegration.AdminApi.Article
{
    public interface IEditorApiClient
    {
        Task<ApiResult<string>> UploadImage(IFormFile file, string id);
        Task<ApiResult<string>> UploadVideo(IFormFile file, string id);
    }
}
