using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace DataServices.SimpleAzureIdentityDataService.Common
{
    public abstract class ObjectContentResponseEnricher<T> : IMessageTransformer
    {
        public virtual bool CanTransform(Type contentType)
        {
            return contentType == typeof(T);
        }

        public abstract void Transform(T content, HttpResponseMessage httpResponseMessage);

        bool IMessageTransformer.CanTransform(HttpResponseMessage response)
        {
            var content = response.Content as ObjectContent;
            return (content != null && CanTransform(content.ObjectType));
        }

        HttpResponseMessage IMessageTransformer.Transform(HttpResponseMessage response)
        {
            T content;
            if (response.TryGetContentValue(out content))
            {
                Transform(content, response);
            }

            return response;
        }
    }
}