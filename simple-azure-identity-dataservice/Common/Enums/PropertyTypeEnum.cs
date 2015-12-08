using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DataServices.SimpleAzureIdentityDataService.Common.Enums
{
    public enum PropertyTypes
    {
        [Description("OFC")]
        Office = 1,

        [Description("IND")]
        Industrial = 2,

        [Description("RET")]
        Retail = 3
    }
}