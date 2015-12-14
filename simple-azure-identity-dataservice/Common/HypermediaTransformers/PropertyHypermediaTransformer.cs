using DataServices.SimpleAzureIdentityDataService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http.Routing;

namespace DataServices.SimpleAzureIdentityDataService.Common.HypermediaTransformers
{
    public class PropertyHypermediaTransformer : ObjectContentResponseEnricher<Property>
    {
        public override void Transform(Property property, HttpResponseMessage httpResponseMessage)
        {
            var helper = httpResponseMessage.RequestMessage.GetUrlHelper();

            property.AddLink(new HypermediaLink("self", helper.Link("Get", new { })));

        }
    }
}