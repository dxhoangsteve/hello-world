using BaseSource.Shared.Constants;
using Resources.Names;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Shared.Enums
{
    public enum EUploadFileType : int
    {
        All = 1,
        Image,
        Video,
        Doc
    }

    public enum EUploadDirectoryType : int
    {
        Editor = 1,
    }

    public enum EGender : byte
    {
        [Display(Name = "Nam")]
        Male = 1,
        [Display(Name = "Nữ")]
        Female,
        [Display(Name = "Khác")]
        Other
    }

    public enum EChain : int
    {
        Ethereum = 1,
        BNBChain
    }
}
