using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataServices.SimpleAzureIdentityDataService.Models
{
    public class PropertyList : HypermediaResource
    {
        /// <summary>
        /// List of properties, will only be the set of properties identified by the offset and limit parameters
        /// </summary>
        public List<Property> Items { get; set; }

        /// <summary>
        /// Total number of property in the entire set
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Current offset of the starting record of the result set
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Number of records turned in this result set (limit of 1000)
        /// </summary>
        public int Limit { get; set; }///

        public PropertyList()
        {
            Items = new List<Property>();
        }
    }

}