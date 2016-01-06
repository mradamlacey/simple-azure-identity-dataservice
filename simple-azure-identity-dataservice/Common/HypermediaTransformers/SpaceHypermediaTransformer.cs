using DataServices.SimpleAzureIdentityDataService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http.Routing;

namespace DataServices.SimpleAzureIdentityDataService.Common.HypermediaTransformers
{
    public class SpaceHypermediaTransformer : ObjectContentResponseEnricher<Space>
    {
        public override void Transform(Space space, HttpResponseMessage httpResponseMessage)
        {
            var helper = httpResponseMessage.RequestMessage.GetUrlHelper();
            var baseUri = helper.Request.RequestUri.GetLeftPart(UriPartial.Path);
            var uri = helper.Request.RequestUri.GetLeftPart(UriPartial.Path);

            space.AddLink(new HypermediaLink("self", helper.Link("GetSpace", new { })));

            var propertiesUri = uri.ToString().Replace("spaces", "properties");

            var propResource = "properties/";
            propertiesUri = propertiesUri.Substring(0, propertiesUri.IndexOf(propResource) + propResource.Length - 1);

            space.AddLink(new HypermediaLink("property", space.Property != null ? propertiesUri + "/" + space.Property.Id : propertiesUri + "/---" ));
        }
    }
}