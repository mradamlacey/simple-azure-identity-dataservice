using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataServices.SimpleAzureIdentityDataService.Models
{
    /// <summary>
    /// Identifies a name for the property along with a concatenated address
    /// </summary>
    public class PropertyAddress
    {
        /// <summary>
        /// Friendly, common or alias name for the address
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Address of the property
        /// </summary>
        public String Address { get; set; }
    }
}