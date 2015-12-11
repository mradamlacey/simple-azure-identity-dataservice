using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace DataServices.SimpleAzureIdentityDataService.Models
{
    /// <summary>
    /// Base class for resources from the API, includes basic Hypermedia attributes link links , etc...
    /// </summary>
    public abstract class HypermediaResource
    {
        private readonly List<HypermediaLink> links = new List<HypermediaLink>();

        [JsonProperty(Order = 100)]
        public List<HypermediaLink> Links { get { return links; } }

        public void AddLink(HypermediaLink link)
        {
            if(link == null)
            {
                throw new ArgumentNullException("link");
            }
            links.Add(link);
        }

        public void AddLinks(params HypermediaLink[] links)
        {
            foreach (var link in links)
            {
                AddLink(link);
            }
        }
    }
}