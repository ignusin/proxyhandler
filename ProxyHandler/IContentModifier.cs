using System.Net.Http;
using System.Threading.Tasks;

namespace ProxyHandler
{
    public interface IContentModifier
    {
        Task ModifyContent(HttpResponseMessage httpResponseMessage);
    }
}
