using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DataServices.SimpleAzureIdentityDataService.Models;
using System.ComponentModel;
using System.Globalization;

namespace DataServices.SimpleAzureIdentityDataService.Common.Converters
{

    class SearchFiltersConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                SearchFilters filters = new SearchFilters();

                string source = value as String;
                string[] stringSeparators = new string[] { "and", "or", "AND", "OR" };
                string[] result;
                result = source.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<String, List<String>> filterDict = new Dictionary<string, List<string>>();

                foreach (string s in result)
                {
                    if (String.IsNullOrWhiteSpace(s))
                    {
                        continue;
                    }

                    var predicateParts = s.Split(' ');
                    var predicateList = predicateParts.ToList<String>().Where<String>(part => { return !String.IsNullOrEmpty(part); });
                    if(predicateList.Count() != 3)
                    {
                        throw new ArgumentException("value", "Invalid format for predicate" + s);
                    }

                    var predicate = predicateList.ElementAt<string>(1) + " " + predicateList.ElementAt<string>(2);
                    if (filterDict.ContainsKey(predicateList.ElementAt<string>(0)))
                    {
                        filterDict[predicateList.ElementAt<string>(0)].Add(predicate);
                    }
                    else
                    {
                        filterDict.Add(predicateList.ElementAt<string>(0), new List<String>() { predicate });
                    }
                    
                }

                foreach(var keyValPair in filterDict)
                {
                    SearchFilter filter = new SearchFilter();
                    filter.FieldName = keyValPair.Key;

                    foreach(var predicate in keyValPair.Value)
                    {
                        var parts = predicate.Split(' ');
                        filter.Predicates.Add(new SearchFilterComparison() { Comparison = parts[0], Value = parts[1].Replace("'", "") });
                    }

                    filters.Filters.Add(filter);
                }

                return filters;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}