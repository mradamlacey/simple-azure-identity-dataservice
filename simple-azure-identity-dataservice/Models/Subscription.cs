using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataServices.SimpleAzureIdentityDataService.Models
{
    /// <summary>
    /// Details of a subscription to the Changes API
    /// </summary>
    public class Subscription : HypermediaResource
    {
        /// <summary>
        /// Id of the subscription
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Date that the subscription was created on
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Total number of changes published to this subscription
        /// </summary>
        public Int64 NumberOfChanges { get; set; }

        /// <summary>
        /// Status of the subscription, 0 means up to date
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// URL to invoke when a change message is published for this subscription
        /// </summary>
        public String CallbackUrl { get; set; }
    }
}