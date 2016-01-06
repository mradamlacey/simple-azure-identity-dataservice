using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataServices.SimpleAzureIdentityDataService.Models
{
    /// <summary>
    /// Information about an availability within a property, includes amount of space (in square feet)
    /// and identifying information for the floor/suite number
    /// </summary>
    public class Space : HypermediaResource
    {
        public String Id { get; set; }

        public String AvailabilityType { get; set; }

        public String Suite { get; set; }

        public String Floor { get; set; }

        public int FloorSequence { get; set; }

        public DateTime ListedOn { get; set; }

        public DateTime AvailableOn { get; set; }

        public String ListingSource { get; set; }

        public String ListingType { get; set; }

        public String Status { get; set; }

        public double TotalAreaSquareFeet { get; set; }
        public double AvailableSquareFeet { get; set; }
        public double AskingRentMonthly { get; set; }
        public double AskingRentYearly { get; set; }
        public double AskingPrice { get; set; }

        public AuditInfo Audit { get; set; }

        public Property Property { get; set; }

        public Space()
        {
            Audit = new AuditInfo();
            Property = new Property();
        }

    }
}