using BaseSource.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaseSource.Utilities.Helper
{
    public class UIHelper
    {
        public static string GetAvatar(string avatar)
        {
            if (string.IsNullOrEmpty(avatar))
            {
                avatar = "/images/avatar.png";
            }
            return avatar;
        }

        public static string GetImage(string img)
        {
            if (string.IsNullOrEmpty(img))
            {
                img = "/images/noimage.jpg";
            }
            return img;
        }
    }
}
