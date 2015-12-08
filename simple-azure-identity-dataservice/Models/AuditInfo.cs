using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataServices.SimpleAzureIdentityDataService.Models
{
    public class AuditInfo
    {
        public bool IsVerified { get; set; }
        public String LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public String CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}