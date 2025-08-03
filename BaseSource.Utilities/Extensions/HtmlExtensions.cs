using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace BaseSource.Utilities.Extensions
{
    public class HtmlExtensions
    {
        public static List<string> Nl2List(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new List<string>();
            else
            {
                var lines = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();//text.Split('\n').ToList();
                lines = lines.Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct().ToList();
                return lines;
            }
        }

        public static string List2StringNl(List<string> list)
        {
            if (list == null || list.Count == 0)
                return string.Empty;
            else
            {
                list = list.Where(x => !string.IsNullOrEmpty(x.Trim())).ToList();
                var rs = string.Join(Environment.NewLine, list);
                //for (int i = 0; i < list.Count; i++)
                //{
                //    if (i > 0)
                //        rs += ("\r\n");
                //    rs += list[i];
                //}
                return rs;
            }
        }

        // Hàm trích xuất URL ảnh từ nội dung bài viết
        public static List<string> ExtractImageUrls(string html)
        {
            var urls = new List<string>();
            if (!string.IsNullOrEmpty(html))
            {
                var regex = new Regex(@"<img[^>]+src=['""](?<url>[^'""]+)['""]", RegexOptions.IgnoreCase);
                foreach (Match match in regex.Matches(html))
                {
                    urls.Add(match.Groups["url"].Value);
                }
            }
            return urls;
        }
    }
}
