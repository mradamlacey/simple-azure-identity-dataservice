using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DataServices.SimpleAzureIdentityDataService.Common.Enums;

namespace DataServices.SimpleAzureIdentityDataService.Models
{
    public class Property : HypermediaResource
    {
        /// <summary>
        /// Unique identifier for the a property
        /// </summary>
        public String Id { get; set; }
        /// <summary>
        /// Name of the building, if not provided will usually be the first portion of the address
        /// </summary>
        public String BuildingName { get; set; }
        /// <summary>
        /// Street number portion of the address
        /// </summary>
        public String StreetNumber { get; set; }
        /// <summary>
        /// Street name portion of the address
        /// </summary>
        public String StreetName { get; set; }
        /// <summary>
        /// Street type (dr, st, lane, etc...) portion of the address
        /// </summary>
        public String StreetType { get; set; }
        /// <summary>
        /// City portion of the address
        /// </summary>
        public String City { get; set; }
        /// <summary>
        /// State portion of the address
        /// </summary>
        public String State { get; set; }
        /// <summary>
        /// County portion of the address
        /// </summary>
        public String County { get; set; }
        /// <summary>
        /// Postal code (a.k.a. zip code) of the address
        /// </summary>
        public String PostalCode { get; set; }
        /// <summary>
        /// Country portion of the address
        /// </summary>
        public String Country { get; set; }

        public List<PropertyAddress> AlternateAddresses { get; set; }

        /// <summary>
        /// Latitude and Longitude of where the property is located
        /// </summary>
        public GeoLocation Location { get; set; }

        /// <summary>
        /// Primary type of the property, whether Industrial, Office, or Retail
        /// </summary>
        public String PropertyType { get; set; } 

        /// <summary>
        /// Audit information for the property, its verified status, and when it was last changed and created
        /// </summary>
        public AuditInfo Audit { get; set; }

        /// <summary>
        /// Indicator if the property has been verified and the associated information for the property can be trusted
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Geographic hierarchy (Region/Market/etc...) for the property, this will be a list of levels from highest (most general)
        /// to lowest (most specific)
        /// </summary>
        public List<GeographicHierarchyLevel> GeographicHierarchy { get; set; }
        
        /// <summary>
        /// Gross square feet of the property
        /// </summary>
        public double GrossSquareFeet { get; set; }

        /// <summary>
        /// Total amount of rentable area for the property in square feet
        /// </summary>
        public double NetRentableAreaSquareFeet { get; set; }

        /// <summary>
        /// Current of amount of available square feet across all availabilities for the property in square feet
        /// </summary>
        public double AvailableSquareFeet { get; set; }

        /// <summary>
        /// Maximum amount of square footage space that is contiguous across all availabilities for the property
        /// </summary>
        public double MaxContiguousSquareFeet { get; set; }

        /// <summary>
        /// Year that the property was built on
        /// </summary>
        public int YearBuilt { get; set; }

        /// <summary>
        /// Month that the property was built on
        /// </summary>
        public int MonthBuilt { get; set; }

        /// <summary>
        /// Class of the property (e.g. A, B, C, D...)
        /// </summary>
        public String Class { get; set; }

        /// <summary>
        /// Full address of the property
        /// </summary>
        public String Address
        {
            get
            {
                return this.StreetNumber + " " + this.StreetName + " " + this.StreetType + " " + this.City + " " + this.State + " " + this.PostalCode;
            }
        }

        public Property()
        {
            Audit = new AuditInfo();
            Location = new GeoLocation();
            AlternateAddresses = new List<PropertyAddress>();
            GeographicHierarchy = new List<GeographicHierarchyLevel>();
        }
    }
}