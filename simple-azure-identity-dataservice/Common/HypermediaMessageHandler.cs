using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DataServices.SimpleAzureIdentityDataService.Common
{
    public class HypermediaMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken)
                .ContinueWith(task =>
                {
                    var response = task.Result;
                    var transformers = request.GetConfiguration().GetResponseTransformers();

                    return transformers.Where(e => true)
                        .Aggregate(response, (resp, transformer) => transformer.Transform(response));
                });
        }
    }
}