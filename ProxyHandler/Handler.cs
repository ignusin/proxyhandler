using System.Web;

namespace ProxyHandler
{
    public class Handler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var proxy = HttpContextProxy.CreateDefault(context);
            proxy.PerformCall();
        }
    }
}
