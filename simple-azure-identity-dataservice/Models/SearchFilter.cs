using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

using DataServices.SimpleAzureIdentityDataService.Common.Converters;

namespace DataServices.SimpleAzureIdentityDataService.Models
{
    [TypeConverter(typeof(SearchFiltersConverter))]
    public class SearchFilters
    {
        public List<SearchFilter> Filters { get; set; }

        public SearchFilters()
        {
            Filters = new List<SearchFilter>();
        }
    }

    /// <summary>
    /// A particular filter for a search request
    /// </summary>
    public class SearchFilter
    {
        public String FieldName { get; set; }

        public List<SearchFilterComparison> Predicates { get; set; }

        public SearchFilter()
        {
            Predicates = new List<SearchFilterComparison>();
        }

    }

    public class SearchFilterComparison
    {
        public String Value { get; set; }

        public String Comparison { get; set; }
    }
}