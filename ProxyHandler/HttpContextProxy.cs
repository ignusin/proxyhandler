using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace ProxyHandler
{
    public class HttpContextProxy : IProxy
    {
        private readonly HttpContext _httpContext;

        public IConfiguration Configuration { get; set; }
        public IContentModifierFactory ContentModifierFactory { get; set; }

        public static HttpContextProxy CreateDefault(HttpContext httpContext)
        {
            return new HttpContextProxy(httpContext)
            {
                Configuration = DefaultConfiguration.Current,
                ContentModifierFactory = new ContentModifierFactory()
            };
        }

        public HttpContextProxy(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public void PerformCall()
        {
            Task.Run(() => PerformCallAsync()).Wait();
        }

        private async Task PerformCallAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var httpRequestMessage = CreateHttpRequestMessage();
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                await SendResponse(httpResponseMessage);
            }
        }

        private string GetRelativeUrl()
        {
            var appPath = HttpRuntime.AppDomainAppVirtualPath;
            var rawUrl = _httpContext.Request.RawUrl;

            if (rawUrl.StartsWith(appPath, StringComparison.InvariantCultureIgnoreCase))
            {
                rawUrl = rawUrl.Substring(appPath.Length);
            }

            return rawUrl;
        }

        private string CombineUrls(string baseUrl, string relativeUrl)
        {
            if (baseUrl.EndsWith("/") && relativeUrl.StartsWith("/"))
            {
                relativeUrl = relativeUrl.Substring(1);
            }
            else if (!baseUrl.EndsWith("/") && !relativeUrl.StartsWith("/"))
            {
                relativeUrl = "/" + relativeUrl;
            }

            return baseUrl + relativeUrl;
        }

        private HttpRequestMessage CreateHttpRequestMessage()
        {
            var request = _httpContext.Request;

            var method = new HttpMethod(request.HttpMethod);
            var url = CombineUrls(Configuration.TargetUrl, GetRelativeUrl());

            var httpRequestMessage = new HttpRequestMessage(method, url);

            foreach (var headerName in request.Headers.AllKeys)
            {
                if (string.Equals(headerName, "Host", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (!HttpHeaderUtility.IsContentHeader(headerName))
                {
                    foreach (var value in request.Headers.GetValues(headerName))
                    {
                        httpRequestMessage.Headers.Add(headerName, value);
                    }
                }
            }

            var targetUri = new Uri(Configuration.TargetUrl);
            httpRequestMessage.Headers.Host = targetUri.IsDefaultPort
                ? targetUri.Host
                : targetUri.Host + ":" + targetUri.Port.ToString();

            if (request.HttpMethod.ToUpper() != HttpMethod.Get.Method.ToUpper())
            {
                httpRequestMessage.Content = new StreamContent(request.InputStream);
                var contentHeaders = httpRequestMessage.Content.Headers;

                foreach (var headerName in request.Headers.AllKeys)
                {
                    if (HttpHeaderUtility.IsContentHeader(headerName))
                    {
                        foreach (var value in request.Headers.GetValues(headerName))
                        {
                            contentHeaders.Add(headerName, value);
                        }
                    }
                }
            }

#if _LOGGING
            Logger.DebugOutputRequests(request, httpRequestMessage);
#endif // _LOGGING

            return httpRequestMessage;
        }

        private async Task SendResponse(HttpResponseMessage httpResponseMessage)
        {
            var response = _httpContext.Response;
            response.ClearHeaders();
            response.StatusCode = (int)httpResponseMessage.StatusCode;

            foreach (var header in httpResponseMessage.Headers)
            {
                foreach (var value in header.Value)
                {
                    response.AddHeader(header.Key, value);
                }
            }

            foreach (var header in httpResponseMessage.Content.Headers)
            {
                foreach (var value in header.Value)
                {
                    response.AddHeader(header.Key, value);
                }
            }

            if (ContentModifierFactory.HasContentModifiers(httpResponseMessage))
            {
                var contentModifiers = ContentModifierFactory.GetContentModifiers(httpResponseMessage);

                foreach (var contentModifier in contentModifiers)
                {
                    await contentModifier.ModifyContent(httpResponseMessage);
                }
            }

            await httpResponseMessage.Content.CopyToAsync(response.OutputStream);

#if _LOGGING
            Logger.DebugOutputResponses(httpResponseMessage, response);
#endif // _LOGGING
        }
    }
}
