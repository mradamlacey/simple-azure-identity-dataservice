using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataServices.SimpleAzureIdentityDataService.Models
{
    /// <summary>
    /// A particular filter for a search request
    /// </summary>
    public class SearchFilter
    {
        public String FieldName { get; set; }

        public String DataType { get; set; }

        public String Value { get; set; }

        public List<String> Values { get; set; }

    }
}