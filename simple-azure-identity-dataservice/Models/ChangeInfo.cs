using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataServices.SimpleAzureIdentityDataService.Models
{
    /// <summary>
    /// Provides information about a particular change with all details including old and new values
    /// </summary>
    public class ChangeInfo
    {
        public DateTime ChangedOn { get; set; }

        public String ChangedBy { get; set; }

        public String Entity { get; set; }

        public String Id { get; set; }

        public Dictionary<String, String> OldValues { get; set; }

        public Dictionary<String, String> NewValues { get; set; }

        public ChangeInfo()
        {
            OldValues = new Dictionary<string, string>();
            NewValues = new Dictionary<string, string>();
        }
    }
}