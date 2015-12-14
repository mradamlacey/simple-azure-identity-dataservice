using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataServices.SimpleAzureIdentityDataService.Models
{
    public class SubscriptionList : HypermediaResource
    {
        /// <summary>
        /// List of subscriptions currently active
        /// </summary>
        public List<Subscription> Items { get; set; }

        /// <summary>
        /// Total number of subscriptions in the entire set for the client
        /// </summary>
        public int Total { get; set; }

        public SubscriptionList()
        {
            Items = new List<Subscription>();
        }
    }

}