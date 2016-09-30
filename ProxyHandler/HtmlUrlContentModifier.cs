using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProxyHandler
{
    public class HtmlUrlContentModifier : IContentModifier
    {
        private readonly static string[] _regexes = new[]
        {
            "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))",
            "src\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))",
            "action\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))"
        };

        public IConfiguration Configuration { get; set; }

        public HtmlUrlContentModifier()
        {
            Configuration = DefaultConfiguration.Current;
        }

        public async Task ModifyContent(HttpResponseMessage httpResponseMessage)
        {
            var contentHeaders = httpResponseMessage.Content.Headers;
            var hasGzipEncoding = contentHeaders.ContentEncoding
                .Any(x => string.Equals("gzip", x, StringComparison.InvariantCultureIgnoreCase));
            
            if (hasGzipEncoding)
            {
                return;
            }

            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            var anyChange = false;

            foreach (var regex in _regexes)
            {
                var re = new Regex(
                    regex,
                    RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled
                );

                content = re.Replace(content, match =>
                {
                    var s = match.Value;
                    var url = s.Substring(match.Groups[1].Index - match.Index, match.Groups[1].Length);

                    if (url.StartsWith("/"))
                    {
                        url = Configuration.IISApplicationVirtualPath + url;
                        s = s.Substring(0, match.Groups[1].Index - match.Index) + url + s.Substring(match.Groups[1].Index - match.Index + match.Groups[1].Length);

                        anyChange = true;
                    }

                    return s;
                });
            }

            if (!anyChange)
            {
                return;
            }

            var modifiedContent = new StringContent(content);

            foreach (var header in httpResponseMessage.Content.Headers)
            {
                if (string.Equals("Content-Length", header.Key, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (modifiedContent.Headers.Contains(header.Key))
                {
                    modifiedContent.Headers.Remove(header.Key);
                }

                modifiedContent.Headers.Add(header.Key, header.Value);
            }

            httpResponseMessage.Content = modifiedContent;
        }
    }
}
