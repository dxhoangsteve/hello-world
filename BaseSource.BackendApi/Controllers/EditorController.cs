using BaseSource.BackendApi.Services.Upload;
using BaseSource.Data.EF;
using BaseSource.Shared.Enums;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BaseSource.BackendApi.Controllers
{
    public class EditorController : BaseApiController
    {
        private readonly IUploadService _uploadService;
        private readonly BaseSourceDbContext _db;
        public EditorController(
            IUploadService uploadService,
            BaseSourceDbContext db)
        {
            _uploadService = uploadService;
            _db = db;
        }

        [HttpPost("UploadImage")]
        [Produces("application/json", Type = typeof(ApiResult<string>))]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageEditorVm model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            var result = await _uploadService.UploadFile(new UploadFileVm
            {
                File = model.ImageFile,
                DirType = EUploadDirectoryType.Editor,
                FileType = EUploadFileType.Image,
                FolderName = $"temp/{model.UploadId}"
            });
            if (!result.IsSuccessed)
            {
                return Ok(result);
            }
            return Ok(new ApiSuccessResult<string>(_uploadService.CombineHostUrl(result.ResultObj)));
        }

        [HttpPost("UploadVideo")]
        [Produces("application/json", Type = typeof(ApiResult<string>))]
        public async Task<IActionResult> UploadVideo([FromForm] UploadVideoEditorVm model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            var result = await _uploadService.UploadFile(new UploadFileVm
            {
                File = model.VideoFile,
                DirType = EUploadDirectoryType.Editor,
                FileType = EUploadFileType.Video,
                FolderName = $"temp/{model.UploadId}"
            });
            if (!result.IsSuccessed)
            {
                return Ok(result);
            }
            return Ok(new ApiSuccessResult<string>(_uploadService.CombineHostUrl(result.ResultObj)));
        }
    }
}
