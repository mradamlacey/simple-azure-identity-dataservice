using DataServices.SimpleAzureIdentityDataService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http.Routing;

namespace DataServices.SimpleAzureIdentityDataService.Common.HypermediaTransformers
{
    public class PropertyListHypermediaTransformer : ObjectContentResponseEnricher<PropertyList>
    {
        public override void Transform(PropertyList properties, HttpResponseMessage httpResponseMessage)
        {
            var helper = httpResponseMessage.RequestMessage.GetUrlHelper();

            properties.AddLink(new HypermediaLink("self", helper.Link("GetCollection", new { })));

            properties.Items.ForEach(item =>
            {
                item.AddLink(new HypermediaLink("self", helper.Request.RequestUri + "/" + item.Id ));
            });
        }
    }
}