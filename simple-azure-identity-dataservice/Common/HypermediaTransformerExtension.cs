using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DataServices.SimpleAzureIdentityDataService.Common
{
    public static class HypermediaTransformerExtension
    {
        public static void AddResponseTransformer(this HttpConfiguration config, params IMessageTransformer[] transformers)
        {
            foreach (var transformer in transformers)
            {
                config.GetResponseTransformers().Add(transformer);
            }
        }

        public static Collection<IMessageTransformer> GetResponseTransformers(this HttpConfiguration config)
        {
            return (Collection<IMessageTransformer>)config.Properties.GetOrAdd(
                    typeof(Collection<IMessageTransformer>),
                    k => new Collection<IMessageTransformer>()
                );
        }
    }
}