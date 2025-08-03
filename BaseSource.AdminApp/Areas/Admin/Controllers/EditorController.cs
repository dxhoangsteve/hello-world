using BaseSource.AdminApp.Areas.Admin.Controllers;
using BaseSource.ApiIntegration.AdminApi.Article;
using BaseSource.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BaseSource.WebApp.Controllers
{
    public class EditorController : BaseAdminController
    {
        private readonly IEditorApiClient _editorApiClient;

        public EditorController(IEditorApiClient editorApiClient)
        {
            _editorApiClient = editorApiClient;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file, string id)
        {
            var result = await _editorApiClient.UploadImage(file, id);
            if (!result.IsSuccessed)
            {
                return BadRequest(result?.Message);
            }

            return Ok(new { location = result.ResultObj });
        }

        [HttpPost]
        public async Task<IActionResult> UploadVideo(IFormFile file, string id)
        {
            var result = await _editorApiClient.UploadVideo(file, id);
            if (!result.IsSuccessed)
            {
                return BadRequest(result?.Message);
            }

            return Ok(new { location = result.ResultObj });
        }
    }
}
