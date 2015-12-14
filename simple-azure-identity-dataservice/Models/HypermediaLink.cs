using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataServices.SimpleAzureIdentityDataService.Models
{
    public class HypermediaLink
    {
        public string Rel { get; private set; }
        public string Href { get; private set; }
        public string Title { get; private set; }
        public string Method { get; private set; }

        public HypermediaLink(string relation, string href, string title = null, string method = "GET")
        {
            if(relation == null)
            {
                throw new ArgumentNullException("relation");
            }
            if (href == null)
            {
                throw new ArgumentNullException("href");
            }

            Rel = relation;
            Href = href;
            Title = title;
            Method = method;
        }
    }
}