using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace ProxyHandler
{
    public class HttpContextHtmlUrlContentModifier : IContentModifier
    {
        public IConfiguration Configuration { get; set; }

        public HttpContextHtmlUrlContentModifier()
        {
            Configuration = DefaultConfiguration.Current;
        }

        public async Task ModifyContent(HttpResponseMessage httpResponseMessage)
        {
            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            var re = new Regex(
                "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled
            );

            content = re.Replace(content, match =>
            {
                var s = match.Value;
                var url = s.Substring(match.Groups[1].Index - match.Index, match.Groups[1].Length);

                if (url.StartsWith("/"))
                {
                    url = HttpRuntime.AppDomainAppVirtualPath + url;
                    s = s.Substring(0, match.Groups[1].Index - match.Index) + url + s.Substring(match.Groups[1].Index - match.Index + match.Groups[1].Length);
                }

                return s;
            });

            var modifiedContent = new StringContent(content);
            modifiedContent.Headers.Clear();

            foreach (var header in httpResponseMessage.Content.Headers)
            {
                modifiedContent.Headers.Add(header.Key, header.Value);
            }

            httpResponseMessage.Content = modifiedContent;
        }
    }
}
