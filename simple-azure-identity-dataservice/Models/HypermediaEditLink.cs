using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataServices.SimpleAzureIdentityDataService.Models
{
    public class HypermediaEditLink : HypermediaLink
    {
        public const string Relation = "edit";

        public HypermediaEditLink(string href, string title = null) : base(Relation, href, title)
        {

        }
    }
}