using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DataServices.SimpleAzureIdentityDataService.Common.Enums;

namespace DataServices.SimpleAzureIdentityDataService.Models
{
    public class Property
    {
        public int Id { get; set; }
        public String BuildingName { get; set; }
        public String StreetNumber { get; set; }
        public String StreetName { get; set; }
        public String StreetType { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public String County { get; set; }
        public String PostalCode { get; set; }
        public String Country { get; set; }

        public GeoLocation Location { get; set; }

        public PropertyTypes PropertyType { get; set; } 

        public AuditInfo Audit { get; set; }

        public String Address
        {
            get
            {
                return this.StreetNumber + " " + this.StreetName;
            }
        }

        public Property()
        {
            Audit = new AuditInfo();
        }
    }
}