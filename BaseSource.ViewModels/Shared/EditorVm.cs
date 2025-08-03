using BaseSource.Shared.Enums;
using BaseSource.ViewModels.Helper;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BaseSource.ViewModels.Shared
{
    public class UploadImageEditorVm : IValidatableObject
    {
        [Required]
        public IFormFile ImageFile { get; set; }

        [Required]
        public string UploadId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ImageFile.Length > (1024 * 1024 * 10))
            {
                yield return new ValidationResult(
              errorMessage: "Dung lượng ảnh tối đa 10MB",
              memberNames: new[] { "ImageFile" });
            }
        }
    }

    public class UploadImagesEditorVm : IValidatableObject
    {
        [Required]
        public List<IFormFile> ImageFiles { get; set; }

        [Required]
        public string UploadId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ImageFiles.Count == 0 || ImageFiles.Any(x => x.Length > (1024 * 1024 * 10)))
            {
                yield return new ValidationResult(
              errorMessage: "Dung lượng ảnh tối đa 10MB",
              memberNames: new[] { "ImageFiles" });
            }
        }
    }

    public class UploadVideoEditorVm : IValidatableObject
    {
        [Required]
        public IFormFile VideoFile { get; set; }

        [Required]
        public string UploadId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (VideoFile.Length > (1024 * 1024 * 100))
            {
                yield return new ValidationResult(
              errorMessage: "Dung lượng ảnh tối đa 100MB",
              memberNames: new[] { "ImageFile" });
            }
        }
    }

    public class UploadVideosEditorVm : IValidatableObject
    {
        [Required]
        public List<IFormFile> VideoFiles { get; set; }

        [Required]
        public string UploadId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (VideoFiles.Count == 0 || VideoFiles.Any(x => x.Length > (1024 * 1024 * 100)))
            {
                yield return new ValidationResult(
              errorMessage: "Dung lượng ảnh tối đa 100MB",
              memberNames: new[] { "ImageFiles" });
            }
        }
    }
}
