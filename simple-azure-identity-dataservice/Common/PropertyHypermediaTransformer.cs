﻿using DataServices.SimpleAzureIdentityDataService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http.Routing;

namespace DataServices.SimpleAzureIdentityDataService.Common
{
    public class PropertyHypermediaTransformer : ObjectContentResponseEnricher<Property>
    {
        public override void Transform(Property property, HttpResponseMessage httpResponseMessage)
        {
            var helper = httpResponseMessage.RequestMessage.GetUrlHelper();

            // property.AddSelfAndEditLinks(GetUrlHelper().Link("SiteApi", new { controller = "Property", id = property.Id }));
            property.AddLink(new HypermediaLink("self", helper.Link("Get", new { controller = "Property", id = property.Id })));

            /*
            property.AddLink(new EditMediaLink(GetUrlHelper().Link("PostMedia", new { controller = "postmedia", siteId = post.SiteId, postId = post.Id })));

            property.Media.ForEach(mediaItem =>
                mediaItem.AddLink(new EditLink(GetUrlHelper().Link("PostMedia", new { controller = "postmedia", siteId = post.SiteId, postId = post.Id, id = mediaItem.Id })))
            );
            */
        }
    }
}