using BaseSource.Shared.Enums;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseSource.BackendApi.Services.Upload
{
    public interface IUploadService
    {
        Task DeleteOldFile(string oldFile, string newFile);
        Task<string> MoveTempFile(string image, string tempId, EUploadDirectoryType destType, string destId);
        Task DeleteOldFiles(List<string> oldFiles, List<string> newFiles);
        Task MoveTempFiles(List<string> images, string tempId, EUploadDirectoryType destType, string destId);
        Task<string> MoveTempFilesEditor(string html, string tempId, EUploadDirectoryType destType, string destId);

        List<string> CombineHostUrls(List<string> filePaths);
        List<string> CombineHostUrlsJson(string filePathsJson);
        string CombineHostUrl(string filePath);
        string RemoveHostUrl(string filePath);

        Task<ApiResult<string>> UploadFile(UploadFileVm model);
        Task<ApiResult<List<KeyValuePair<bool, string>>>> UploadFiles(UploadFilesVm model);
        Task<ApiResult<string>> DeleteFile(DeleteFileVm model);
        Task<ApiResult<List<KeyValuePair<bool, string>>>> DeleteFiles(DeleteFilesVm model);
        Task<ApiResult<string>> MoveFiles(MoveFilesVm model);
        Task<ApiResult<string>> DeleteFolder(DeleteFolderVm model);
    }
}
