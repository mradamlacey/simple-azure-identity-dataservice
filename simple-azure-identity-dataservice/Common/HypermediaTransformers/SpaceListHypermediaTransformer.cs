using DataServices.SimpleAzureIdentityDataService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http.Routing;

namespace DataServices.SimpleAzureIdentityDataService.Common.HypermediaTransformers
{
    public class SpaceListHypermediaTransformer : ObjectContentResponseEnricher<SpaceList>
    {
        public override void Transform(SpaceList spaces, HttpResponseMessage httpResponseMessage)
        {
            var helper = httpResponseMessage.RequestMessage.GetUrlHelper();
            var baseUri = helper.Request.RequestUri.GetLeftPart(UriPartial.Path);

            spaces.AddLink(new HypermediaLink("self", helper.Link("GetSpaces", new { })));

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Add paging links
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            spaces.AddLink(new HypermediaLink("first", baseUri + "/?offset=0&limit=" + spaces.Limit));

            int nextOffset = (spaces.Offset + spaces.Limit) <= spaces.Total ? spaces.Offset + spaces.Limit : -1;
            if(nextOffset >= 0)
            {
                spaces.AddLink(new HypermediaLink("next", baseUri + "/?offset=" + nextOffset + "&limit=" + spaces.Limit));
            }

            int prevOffset = (spaces.Offset - spaces.Limit) >= 0 ? spaces.Offset - spaces.Limit : -1;
            if (prevOffset >= 0)
            {
                spaces.AddLink(new HypermediaLink("previous", baseUri + "/?offset=" + prevOffset  + "&limit=" + spaces.Limit));
            }

            double div = spaces.Total / spaces.Limit;
            int lastOffset = (int) Math.Floor(div) * spaces.Limit;
            spaces.AddLink(new HypermediaLink("last", baseUri + "/?offset=" + lastOffset  + "&limit=" + spaces.Limit));            

            spaces.Items.ForEach(item =>
            {
                var uri = helper.Request.RequestUri.GetLeftPart(UriPartial.Path);

                item.AddLink(new HypermediaLink("self", uri + "/" + item.Id ));

                var propertiesUri = uri.ToString().Replace("spaces", "properties");
                item.AddLink(new HypermediaLink("property", propertiesUri + "/" + item.PropertyId ));
            });
        }
    }
}