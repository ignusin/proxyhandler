using System.Collections.Specialized;
using D = System.Diagnostics.Debug;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace ProxyHandler
{
    public static class Logger
    {
        private static void DebugOutputHeaders(NameValueCollection headers)
        {
            for (var i = 0; i < headers.Count; ++i)
            {
                D.WriteLine(headers.AllKeys[i] + ": " + headers[i]);
            }
        }

        private static void DebugOutputHeaders(HttpHeaders headers)
        {
            foreach (var header in headers)
            {
                foreach (var value in header.Value)
                {
                    D.WriteLine(header.Key + ": " + value);
                }
            }
        }

        private static void DebugOutputRequest(HttpRequest request)
        {
            D.WriteLine("HttpContext.Request (" + request.RawUrl + " " + request.HttpMethod + ")");
            D.WriteLine("Headers: ");
            DebugOutputHeaders(request.Headers);
        }

        private static void DebugOutputRequest(HttpRequestMessage httpRequestMessage)
        {
            D.WriteLine("HttpRequestMessage (" + httpRequestMessage.RequestUri + " " + httpRequestMessage.Method.Method + ")");
            D.WriteLine("Headers: ");
            DebugOutputHeaders(httpRequestMessage.Headers);

            if (httpRequestMessage.Content != null)
            {
                DebugOutputHeaders(httpRequestMessage.Content.Headers);
            }
        }

        public static void DebugOutputRequests(HttpRequest httpRequest, HttpRequestMessage httpRequestMessage)
        {
            D.WriteLine("********** REQUEST **********");
            DebugOutputRequest(httpRequest);
            D.WriteLine("");
            DebugOutputRequest(httpRequestMessage);
        }

        private static void DebugOutputResponse(HttpResponseMessage httpResponseMessage)
        {
            D.WriteLine("HttpResponseMessage (" + httpResponseMessage.RequestMessage.RequestUri.ToString() + "):");
            D.WriteLine("Status Code: " + (int)httpResponseMessage.StatusCode);
            D.WriteLine("Headers: ");
            DebugOutputHeaders(httpResponseMessage.Headers);
            DebugOutputHeaders(httpResponseMessage.Content.Headers);
        }

        private static void DebugOutputResponse(HttpResponse httpResponse)
        {
            D.WriteLine("HttpContext.Response:");
            D.WriteLine("Status Code: " + httpResponse.StatusCode);
            D.WriteLine("Headers: ");
            DebugOutputHeaders(httpResponse.Headers);
        }

        public static void DebugOutputResponses(HttpResponseMessage httpResponseMessage, HttpResponse httpResponse)
        {
            D.WriteLine("********** RESPONSE **********");
            DebugOutputResponse(httpResponseMessage);
            D.WriteLine("");
            DebugOutputResponse(httpResponse);
        }
    }
}
