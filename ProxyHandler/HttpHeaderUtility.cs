using System.Linq;

namespace ProxyHandler
{
    public class HttpHeaderUtility
    {
        private static readonly string[] _contentHeaderNames = new[]
        {
            "Allow",
            "Content-Disposition",
            "Content-Encoding",
            "Content-Language",
            "Content-Length",
            "Content-Location",
            "Content-MD5",
            "Content-Range",
            "Content-Type",
            "Expires",
            "Last-Modified"
        };

        public static bool IsContentHeader(string headerName)
        {
            return _contentHeaderNames.Any(x => x == headerName);
        }
    }
}
