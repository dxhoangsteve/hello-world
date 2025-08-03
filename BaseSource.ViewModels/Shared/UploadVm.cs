using BaseSource.Shared.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ViewModels.Shared
{
    public class UploadFileVm
    {
        [Required]
        public EUploadFileType FileType { get; set; }

        [Required]
        public EUploadDirectoryType DirType { get; set; }

        [Required]
        public IFormFile File { get; set; }

        public bool ReplaceExists { get; set; }

        public string FolderName { get; set; }
    }

    public class UploadFilesVm
    {
        [Required]
        public EUploadFileType FileType { get; set; }

        [Required]
        public EUploadDirectoryType DirType { get; set; }

        public List<IFormFile> Files { get; set; }

        public bool ReplaceExists { get; set; }

        public string FolderName { get; set; }
    }

    public class DeleteFileVm
    {
        [Required]
        public string FilePath { get; set; }
    }

    public class DeleteFilesVm
    {
        public List<string> FilePaths { get; set; }
    }

    public class MoveFilesVm
    {
        [Required]
        public string TempFolderName { get; set; }

        [Required]
        public List<string> TempFilePaths { get; set; }

        [Required]
        public EUploadDirectoryType DestDirType { get; set; }

        [Required]
        public string DestFolderName { get; set; }
    }

    public class DeleteFolderVm
    {
        [Required]
        public EUploadDirectoryType DirType { get; set; }

        [Required]
        public string FolderName { get; set; }
    }
}
