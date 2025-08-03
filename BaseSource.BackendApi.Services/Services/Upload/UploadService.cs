using BaseSource.BackendApi.Services.Helper;
using BaseSource.Shared.Constants;
using BaseSource.Shared.Enums;
using BaseSource.ViewModels.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BaseSource.Utilities.Helper;
using BaseSource.ViewModels.Shared;
using BaseSource.Utilities.Extensions;
using static System.Net.Mime.MediaTypeNames;

namespace BaseSource.BackendApi.Services.Upload
{
    public class UploadService : IUploadService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public UploadService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task DeleteOldFile(string oldFile, string newFile)
        {
            if (!string.IsNullOrEmpty(oldFile) && oldFile != newFile)
            {
                await DeleteFile(new DeleteFileVm() { FilePath = RemoveHostUrl(oldFile) });
            }
        }

        public async Task<string> MoveTempFile(string image, string tempId, EUploadDirectoryType destType, string destId)
        {
            if (string.IsNullOrEmpty(image))
            {
                return null;
            }

            var tempDir = $"upload/editor/temp/{tempId}";
            var destDir = $"upload/{destType.ToString()}/{destId}";
            var moveFiles = new List<string>();

            if (image.Contains(tempDir))
            {
                // move file
                await MoveFiles(new MoveFilesVm()
                {
                    TempFolderName = tempId,
                    TempFilePaths = new List<string>() { image },
                    DestDirType = destType,
                    DestFolderName = destId
                });

                image = image.Replace(tempDir, destDir);
            }

            return RemoveHostUrl(image);
        }

        public async Task DeleteOldFiles(List<string> oldFiles, List<string> newFiles)
        {
            if (newFiles != null)
            {
                for (int i = 0; i < newFiles.Count; i++)
                {
                    newFiles[i] = RemoveHostUrl(newFiles[i]);
                }
            }

            if (oldFiles?.Count() > 0)
            {
                var deletedFiles = new List<string>();
                foreach (var item in oldFiles)
                {
                    if (newFiles == null || !newFiles.Any(x => x == item))
                    {
                        deletedFiles.Add(RemoveHostUrl(item));
                    }
                }

                if (deletedFiles.Count > 0)
                {
                    var deleted = await DeleteFiles(new DeleteFilesVm() { FilePaths = deletedFiles });
                }
            }
        }

        public async Task MoveTempFiles(List<string> images, string tempId, EUploadDirectoryType destType, string destId)
        {
            if (images?.Count() > 0)
            {
                var tempDir = $"upload/editor/temp/{tempId}";
                var destDir = $"upload/{destType.ToString()}/{destId}";
                var moveFiles = new List<string>();

                for (int i = 0; i < images.Count; i++)
                {
                    if (images[i].Contains(tempDir))
                    {
                        moveFiles.Add(images[i]);
                        images[i] = RemoveHostUrl(images[i].Replace(tempDir, destDir));
                    }
                }
                if (moveFiles.Count > 0)
                {
                    // move file
                    await MoveFiles(new MoveFilesVm()
                    {
                        TempFolderName = tempId,
                        TempFilePaths = moveFiles,
                        DestDirType = destType,
                        DestFolderName = destId
                    });
                }
            }
        }

        public async Task<string> MoveTempFilesEditor(string html, string tempId, EUploadDirectoryType destType, string destId)
        {
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }

            var tempDir = $"upload/editor/temp/{tempId}";
            var destDir = $"upload/{destType.ToString()}/{destId}";
            var moveFiles = new List<string>();

            var imageUrls = HtmlExtensions.ExtractImageUrls(html); // Lấy danh sách ảnh trong nội dung
            imageUrls = imageUrls.Where(x => x.Contains(tempDir)).ToList();

            if (imageUrls.Count > 0)
            {
                html = html.Replace(tempDir, destDir); // Cập nhật URL trong nội dung
                for (int i = 0; i < imageUrls.Count; i++)
                {
                    moveFiles.Add(imageUrls[i]);
                    imageUrls[i] = imageUrls[i].Replace(tempDir, destDir);
                }
            }

            if (moveFiles.Count > 0)
            {
                // move file
                await MoveFiles(new MoveFilesVm()
                {
                    TempFolderName = tempId,
                    TempFilePaths = moveFiles,
                    DestDirType = destType,
                    DestFolderName = destId
                });
            }
            return html;
        }

        public List<string> CombineHostUrls(List<string> filePaths)
        {
            if (filePaths == null)
            {
                return new List<string>();
            }

            return filePaths.Select(x => CombineHostUrl(x)).ToList();
        }

        public List<string> CombineHostUrlsJson(string filePathsJson)
        {
            var paths = string.IsNullOrEmpty(filePathsJson) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(filePathsJson);

            if (paths == null)
            {
                return new List<string>();
            }

            return paths.Select(x => CombineHostUrl(x)).ToList();
        }

        public string CombineHostUrl(string filePath)
        {
            string uploadHost = _configuration.GetValue<string>("UploadApiBaseAddress");
            return string.IsNullOrEmpty(filePath) ? null :
                (filePath.StartsWith("http") ? filePath : uploadHost + filePath);
        }

        public string RemoveHostUrl(string filePath)
        {
            string uploadHost = _configuration.GetValue<string>("UploadApiBaseAddress");
            return string.IsNullOrEmpty(filePath) ? null :
                (filePath.StartsWith(uploadHost) ? filePath.Remove(0, uploadHost.Length) : filePath);
        }

        public async Task<ApiResult<string>> UploadFile(UploadFileVm model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.UploadApiClient);
            var multiContent = new MultipartFormDataContent();
            if (model.File != null)
            {
                var fileStreamContent = new StreamContent(model.File.OpenReadStream());
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(model.File.ContentType);
                multiContent.Add(fileStreamContent, "File", model.File.FileName);
            }
            multiContent.Add(new StringContent(model.FileType.ToString()), "FileType");
            multiContent.Add(new StringContent(model.DirType.ToString()), "DirType");
            multiContent.Add(new StringContent(model.ReplaceExists.ToString()), "ReplaceExists");
            multiContent.Add(new StringContent(model.FolderName ?? ""), "FolderName");

            using (var response = await client.PostAsync("api/file/uploadfile", multiContent))
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResult<string>>(responseString);
                return result;
            }
        }

        public async Task<ApiResult<List<KeyValuePair<bool, string>>>> UploadFiles(UploadFilesVm model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.UploadApiClient);
            var multiContent = new MultipartFormDataContent();

            for (int i = 0; i < model.Files.Count; i++)
            {
                if (model.Files[i] != null)
                {
                    var fileStreamContent = new StreamContent(model.Files[i].OpenReadStream());
                    fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(model.Files[i].ContentType);
                    multiContent.Add(fileStreamContent, $"Files", model.Files[i].FileName);
                }
            }

            multiContent.Add(new StringContent(model.FileType.ToString()), "FileType");
            multiContent.Add(new StringContent(model.DirType.ToString()), "DirType");
            multiContent.Add(new StringContent(model.ReplaceExists.ToString()), "ReplaceExists");
            multiContent.Add(new StringContent(model.FolderName ?? ""), "FolderName");

            using (var response = await client.PostAsync("api/file/uploadfiles", multiContent))
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResult<List<KeyValuePair<bool, string>>>>(responseString);
                return result;
            }
        }

        public async Task<ApiResult<string>> DeleteFile(DeleteFileVm model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.UploadApiClient);
            return await client.PostAsync<ApiResult<string>>("/api/file/DeleteFile", model);
        }

        public async Task<ApiResult<List<KeyValuePair<bool, string>>>> DeleteFiles(DeleteFilesVm model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.UploadApiClient);
            return await client.PostAsync<ApiResult<List<KeyValuePair<bool, string>>>>("/api/file/DeleteFiles", model);
        }

        public async Task<ApiResult<string>> MoveFiles(MoveFilesVm model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.UploadApiClient);
            return await client.PostAsync<ApiResult<string>>("/api/file/MoveFiles", model);
        }

        public async Task<ApiResult<string>> DeleteFolder(DeleteFolderVm model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.AppSettings.UploadApiClient);
            return await client.PostAsync<ApiResult<string>>("/api/file/DeleteFolder", model);
        }
    }
}
