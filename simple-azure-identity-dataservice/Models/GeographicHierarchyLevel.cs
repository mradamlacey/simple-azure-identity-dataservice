using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataServices.SimpleAzureIdentityDataService.Models
{
    /// <summary>
    /// Represents a single level for a particular geographic hierarchy like Region or Submarket
    /// </summary>
    public class GeographicHierarchyLevel
    {
        /// <summary>
        /// Internal unique identifier for the level
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Name of the level
        /// </summary>
        public String Name { get; set; }
         
        /// <summary>
        /// Label or description of the level (like 'Region' or 'Sub market')
        /// </summary>
        public String Label { get; set; }

        /// <summary>
        /// Represents a numerical value for the value where 0 is the highest level, greater the number is lower in the hierarchy
        /// </summary>
        public int Level { get; set; }

    }
}