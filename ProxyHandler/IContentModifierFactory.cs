using System.Collections.Generic;
using System.Net.Http;

namespace ProxyHandler
{
    public interface IContentModifierFactory
    {
        bool HasContentModifiers(HttpResponseMessage httpResponseMessage);
        IEnumerable<IContentModifier> GetContentModifiers(HttpResponseMessage httpResponseMessage);
    }
}
