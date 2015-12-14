using DataServices.SimpleAzureIdentityDataService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http.Routing;

namespace DataServices.SimpleAzureIdentityDataService.Common.HypermediaTransformers
{
    public class SubscriptionHypermediaTransformer : ObjectContentResponseEnricher<Subscription>
    {
        public override void Transform(Subscription sub, HttpResponseMessage httpResponseMessage)
        {
            var helper = httpResponseMessage.RequestMessage.GetUrlHelper();

            sub.AddLink(new HypermediaLink("self", helper.Link("GetSubscriptionById", new { }), null, "GET"));

            sub.AddLink(new HypermediaLink("delete", helper.Link("DeleteSubscription", new { }), null, "DELETE"));

        }
    }
}