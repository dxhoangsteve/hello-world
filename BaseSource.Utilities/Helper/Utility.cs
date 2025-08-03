using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace BaseSource.Utilities.Helper
{
    public class Utility
    {
        public static string GetTextFromHtml(string html, int? length)
        {
            var descriptions = Regex.Replace(html, "<(.|\n)*?>", "").Trim();
            descriptions = Regex.Replace(descriptions, "&nbsp;|\n", " ");

            if (length.HasValue && length > 0)
            {
                if (descriptions.Length > length)
                {
                    descriptions = descriptions.Substring(0, length.Value);
                    if (descriptions.LastIndexOf(" ") != -1)
                    {
                        descriptions = descriptions.Substring(0, descriptions.LastIndexOf(" ")) + "...";
                    }
                }
            }

            return descriptions;
        }

        public static string FormatNumber(double? price)
        {
            string value = string.Empty;
            if (price.HasValue)
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("en-US");
                value = price.Value.ToString("#,###.####", cul.NumberFormat);
            }
            //return value;
            return value == string.Empty ? "0" : value;
        }

        public static string ConvertToUnSignString(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public static bool AreAllPropertiesNotNullOrEmpty(object obj)
        {
            if (obj == null) return false;

            var properties = obj.GetType().GetProperties();

            return properties.All(property =>
            {
                var value = property.GetValue(obj);
                if (value == null) return false;

                // Kiểm tra nếu là chuỗi rỗng
                if (value is string strValue && string.IsNullOrEmpty(strValue))
                {
                    return false;
                }

                return true;
            });
        }

        public static List<string> JsonToListString(string json)
        {
            return string.IsNullOrEmpty(json) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(json);
        }
    }
}
