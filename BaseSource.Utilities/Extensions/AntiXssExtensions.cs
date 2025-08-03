using Ganss.Xss;
using HtmlAgilityPack;

namespace BaseSource.Utilities.Extensions
{
    public static class AntiXssExtensions
    {
        public static string GetSafeHtml(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            var sanitizer = new HtmlSanitizer();
            var output = sanitizer.Sanitize(input);

            return output;
        }

        /// <summary>
        /// remove link keep text
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string RemoveLink(this string html)
        {
            var htmlContent = new HtmlDocument();
            htmlContent.LoadHtml(html);

            // Parse HTML content for links
            var links = htmlContent.DocumentNode.SelectNodes("//a");
            if (links == null)
            {
                return html;
            }

            foreach (var link in links)
            {
                link.ParentNode.RemoveChild(link, true);
            }

            return htmlContent.DocumentNode.OuterHtml;
        }

        /// <summary>
        /// set nofollow all link
        /// </summary>
        /// <param name="inputHtmlString"></param>
        /// <returns></returns>
        private static string SetLinkNofollow(string inputHtmlString)
        {
            var htmlContent = new HtmlDocument();
            htmlContent.LoadHtml(inputHtmlString);

            // Parse HTML content for links
            var links = htmlContent.DocumentNode.SelectNodes("//a");
            if (links == null)
            {
                return inputHtmlString;
            }

            foreach (var link in links)
            {
                link.SetAttributeValue("rel", "nofollow");
            }

            return htmlContent.DocumentNode.OuterHtml;
        }
    }
}
