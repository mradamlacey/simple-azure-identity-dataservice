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

        public String PropertyId { get; set; }

        public String Description { get; set; }

        public String Suite { get; set; }

        public String FloorName { get; set; }

        public int FloorSequence { get; set; }

        public DateTime ListedOn { get; set; }

        public DateTime AvailableOn { get; set; }

        public double RentalRateMinimum { get; set; }

        public double RentalRateMaximum { get; set; }

        public AuditInfo Audit { get; set; }

        public Space()
        {
            Audit = new AuditInfo();
        }

    }
}