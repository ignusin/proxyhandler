using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace ProxyHandler
{
    public class ContentModifierFactory : IContentModifierFactory
    {
        private readonly static Dictionary<string, IEnumerable<Type>> _contentModifiersIndex =
            new Dictionary<string, IEnumerable<Type>>()
            {
                { "TEXT/HTML", new[] { typeof(HtmlUrlContentModifier) } }
            };


        public IEnumerable<IContentModifier> GetContentModifiers(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.Content != null
                && httpResponseMessage.Content.Headers != null
                && httpResponseMessage.Content.Headers.ContentType != null)
            {
                var contentType = httpResponseMessage.Content.Headers.ContentType.MediaType.ToUpper();

                if (_contentModifiersIndex.ContainsKey(contentType))
                {
                    var contentModifiers = new List<IContentModifier>();
                    foreach (var contentModifierType in _contentModifiersIndex[contentType])
                    {
                        contentModifiers.Add((IContentModifier)Activator.CreateInstance(contentModifierType));
                    }

                    return contentModifiers;
                }
            }

            return null;
        }

        public bool HasContentModifiers(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.Content != null
                && httpResponseMessage.Content.Headers != null
                && httpResponseMessage.Content.Headers.ContentType != null)
            {
                var contentType = httpResponseMessage.Content.Headers.ContentType.MediaType.ToUpper();

                if (_contentModifiersIndex.ContainsKey(contentType))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
