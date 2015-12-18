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
            var baseUri = helper.Request.RequestUri.GetLeftPart(UriPartial.Path);

            properties.AddLink(new HypermediaLink("self", helper.Link("GetCollection", new { })));

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Add paging links
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            properties.AddLink(new HypermediaLink("first", baseUri + "/?offset=0&limit=" + properties.Limit));

            int nextOffset = (properties.Offset + properties.Limit) <= properties.Total ? properties.Offset + properties.Limit : -1;
            if(nextOffset >= 0)
            {
                properties.AddLink(new HypermediaLink("next", baseUri + "/?offset=" + nextOffset + "&limit=" + properties.Limit));
            }

            int prevOffset = (properties.Offset - properties.Limit) >= 0 ? properties.Offset - properties.Limit : -1;
            if (prevOffset >= 0)
            {
                properties.AddLink(new HypermediaLink("previous", baseUri + "/?offset=" + prevOffset  + "&limit=" + properties.Limit));
            }

            double div = properties.Total / properties.Limit;
            int lastOffset = (int) Math.Floor(div) * properties.Limit;
            properties.AddLink(new HypermediaLink("last", baseUri + "/?offset=" + lastOffset  + "&limit=" + properties.Limit));

            properties.Items.ForEach(item =>
            {
                var uri = helper.Request.RequestUri.GetLeftPart(UriPartial.Path);

                item.AddLink(new HypermediaLink("self", uri + "/" + item.Id ));                
            });
        }
    }
}