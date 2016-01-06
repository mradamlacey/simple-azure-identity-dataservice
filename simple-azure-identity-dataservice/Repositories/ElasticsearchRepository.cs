using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Humanizer;

using DataServices.SimpleAzureIdentityDataService.Models;


namespace DataServices.SimpleAzureIdentityDataService.Repositories
{

    public class MappingInfo
    {
        public String DataType { get; set; }

        public String ElasticsearchFieldName { get; set; }

        public String ModelName { get; set; }

        public static MappingInfo Build(String ElasticsearchFieldName, String DataType)
        {
            MappingInfo mappingInfo = new MappingInfo();
            mappingInfo.ElasticsearchFieldName = ElasticsearchFieldName;
            mappingInfo.DataType = DataType;
            mappingInfo.ModelName = ElasticsearchFieldName.ToLower().Camelize();

            return mappingInfo;
        }

        public static MappingInfo Build(String ModelName, String ElasticsearchFieldName, String DataType)
        {
            MappingInfo mappingInfo = new MappingInfo();
            mappingInfo.ElasticsearchFieldName = ElasticsearchFieldName;
            mappingInfo.DataType = DataType;
            mappingInfo.ModelName = ModelName;

            return mappingInfo;
        }
    }

    public class ElasticsearchResult<T>
    {
        public List<T> Items { get; set; }
        public int Total { get; set; }

        public ElasticsearchResult(){
            Items = new List<T>();
        }
    }

    public class ElasticsearchRepository
    {
        private ILog log = LogManager.GetLogger(typeof(ElasticsearchRepository));

        private static List<MappingInfo> propertyMappingInfo = new List<MappingInfo>()
        {
            MappingInfo.Build("buildingName", "PROPERTY_NAME", "fulltext"),
            MappingInfo.Build("streetNumber", "STREET_NUMBER1", "fulltext"),
            MappingInfo.Build("streetNumber", "STREET_NUMBER2", "fulltext"),
            MappingInfo.Build("STREET_NAME", "fulltext"),
            MappingInfo.Build("STREET_TYPE", "fulltext"),
            MappingInfo.Build("CITY", "fulltext"),
            MappingInfo.Build("STATE", "fulltext"),
            MappingInfo.Build("COUNTY", "fulltext"),
            MappingInfo.Build("POSTAL_CODE", "fulltext"),
            MappingInfo.Build("COUNTRY", "fulltext"),
            MappingInfo.Build("alternateAddress.name", "alternate_address.name", "fulltext"),
            MappingInfo.Build("alternateAddress.address", "alternate_address.address", "fulltext"),
            MappingInfo.Build("CREATED_ON", "datetime"),
            MappingInfo.Build("CREATED_BY", "fulltext"),
            MappingInfo.Build("MODIFIED_ON", "datetime"),
            MappingInfo.Build("MODIFIED_BY", "fulltext"),
            MappingInfo.Build("IS_VERIFIED", "bool"),

            MappingInfo.Build("propertyType", "PROPERTY_TYPE_NAME", "match"),
            MappingInfo.Build("class", "CLASS_NAME", "match"),

            MappingInfo.Build("grossSquareFeet", "GROSS_SF", "numeric"),
            MappingInfo.Build("netRentableAreaSquareFeet", "NET_RENTABLE_AREA", "numeric"),
            MappingInfo.Build("availableSquareFeet", "AVAIL_SF_TOTAL", "numeric"),
            MappingInfo.Build("maxContiguousSquareFeet", "MAX_CONTIGUOUS_SF", "numeric"),

            MappingInfo.Build("yearBuilt", "YEAR_BUILT", "numeric"),
            MappingInfo.Build("monthBuilt", "MONTH_BUILT", "numeric"),

            MappingInfo.Build("geographicHierarchy.region.id", "REGION_ID", "match"),
            MappingInfo.Build("geographicHierarchy.region.name", "REGION_NAME", "fulltext"),
            MappingInfo.Build("geographicHierarchy.market.id", "MARKET_ID", "match"),
            MappingInfo.Build("geographicHierarchy.market.name", "MARKET_NAME", "fulltext"),
            MappingInfo.Build("geographicHierarchy.submarket.id", "SUB_MARKET_ID", "match"),
            MappingInfo.Build("geographicHierarchy.submarket.name", "SUB_MARKET_NAME", "fulltext"),
            MappingInfo.Build("geographicHierarchy.neighborhood.id", "NEIGHBORHOOD_ID", "match"),
            MappingInfo.Build("geographicHierarchy.neighborhood.name", "NEIGHBORHOOD_NAME", "fulltext"),
            MappingInfo.Build("geographicHierarchy.district.id", "DISTICT_ID", "match"),
            MappingInfo.Build("geographicHierarchy.district.name", "DISTRICT_NAME", "fulltext"),
        };

        private static List<MappingInfo> spaceMappingInfo = new List<MappingInfo>()
        {
             MappingInfo.Build("property.id", "property.id", "match"),
        MappingInfo.Build("property.streetNumber1", "property.street_number1", "fulltext"),
        MappingInfo.Build("property.streetNumber2", "property.street_number2", "fulltext"),
        MappingInfo.Build("property.streetName", "property.street_name", "fulltext"),
        MappingInfo.Build("property.streetType", "property.street_type", "fulltext"),
         MappingInfo.Build("property.city", "property.city", "fulltext"),
       MappingInfo.Build("property.state", "property.state", "fulltext"),
       MappingInfo.Build("property.county", "property.county", "fulltext"),
        MappingInfo.Build("property.postalCode", "property.postal_code", "fulltext"),
        MappingInfo.Build("property.country", "property.country", "fulltext"),
        MappingInfo.Build("property.alternateAddress.name", "alternate_address.name", "fulltext"),
        MappingInfo.Build("property.alternateAddress.address", "alternate_address.address", "fulltext"),
        MappingInfo.Build("audit.createdOn", "CREATED_ON", "datetime"),
        MappingInfo.Build("audit.cratedBy", "CREATED_BY", "fulltext"),
       MappingInfo.Build("audit.modifiedOn", "MODIFIED_ON", "datetime"),
        MappingInfo.Build("audit.modifiedBy", "MODIFIED_BY", "fulltext"),
        MappingInfo.Build("audit.isVerified", "IS_VERIFIED", "boolean"),

        MappingInfo.Build("property.propertyType", "property.property_type_name", "match"),
        MappingInfo.Build("property.class", "property.class_name", "match"),

        MappingInfo.Build("property.grossSquareFeet", "property.gross_sf", "numeric"),
        MappingInfo.Build("property.netRentableAreaSquareFeet", "property.net_rentable_area", "numeric"),
        // TODO: Need to index this field
        MappingInfo.Build("property.availableSquareFeet", "property.AVAIL_SF_TOTAL", "numeric"),
        // TODO: Need to index this field
        MappingInfo.Build("property.maxContiguousSquareFeet", "property.MAX_CONTIGUOUS_SF", "numeric"),

       MappingInfo.Build("property.yearBuilt", "property.year_built", "numeric"),
        // TODO: Need to index this field
        MappingInfo.Build("property.monthBuilt", "property.month_built", "numeric"),

      MappingInfo.Build("property.geographicHierarchy.region.id", "property.region_id", "match"),
        MappingInfo.Build("property.geographicHierarchy.region.name", "property.region_name", "fulltext"),
     MappingInfo.Build("property.geographicHierarchy.market.id", "property.market_id", "match"),
     MappingInfo.Build("property.geographicHierarchy.market.name", "property.market_name", "fulltext"),
   MappingInfo.Build("property.geographicHierarchy.submarket.id", "property.sub_market_id", "match"),
    MappingInfo.Build("property.geographicHierarchy.submarket.name", "property.sub_market_name", "fulltext"),
   MappingInfo.Build("property.geographicHierarchy.neighborhood.id", "property.neighborhood_id", "match"),
       MappingInfo.Build("property.geographicHierarchy.neighborhood.name", "property.neighborhood_name", "fulltext"),
        MappingInfo.Build("property.geographicHierarchy.district.id", "property.district_id", "match"),
       MappingInfo.Build("property.geographicHierarchy.district.name", "property.district_name", "fulltext"),

        MappingInfo.Build("AVAILABILITY_TYPE", "match"),
        MappingInfo.Build("totalAreaSquareFeet", "TOTAL_AREA_SF", "numeric"),
        MappingInfo.Build("availableSquareFeet", "SQ_FT_AVAILABLE", "numeric"),
      MappingInfo.Build("askingRentMonthly", "ASKING_RENTAL_RATE_MONTHLY", "numeric"),
       MappingInfo.Build("askingRentYearly", "ASKING_RENTAL_RATE_YEARLY", "numeric"),
       MappingInfo.Build("askingPrice", "ASKING_PRICE", "numeric"),
      MappingInfo.Build("FLOOR", "fulltext"),
   MappingInfo.Build("SUITE", "fulltext"),

       MappingInfo.Build("LISTING_SOURCE", "fulltext"),
       MappingInfo.Build("LISTING_TYPE", "match"),
       MappingInfo.Build("STATUS", "match"),

     MappingInfo.Build("DATE_ON_MARKET", "datetime"),
        MappingInfo.Build("DATE_AVAILABLE", "datetime")
        };

        private List<MappingInfo> _GetMappingInfoByModelName(string EntityName, string ModelName)
        {
            List<MappingInfo> mappingInfo = null;
            if(EntityName == "property")
            {
                mappingInfo = propertyMappingInfo;
            }
            if(EntityName == "space")
            {
                mappingInfo = spaceMappingInfo;
            }
            var matching = mappingInfo.Where(m => m.ModelName == ModelName).ToList<MappingInfo>();

            return matching;
        }

        private List<MappingInfo> _GetMappingInfoByElasticsearchFieldName(string FieldName)
        {
            var matching = propertyMappingInfo.Where(m => m.ElasticsearchFieldName == FieldName).ToList<MappingInfo>();

            return matching;
        }

        public async Task<Property> GetPropertyById(String Id){
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ElasticsearchBaseUrl"]);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var url = String.Format("{0}/{1}/property/{2}", ConfigurationManager.AppSettings["ElasticsearchBaseUrl"], 
                    ConfigurationManager.AppSettings["ElasticsearchIndexName"],
                    Id);

                var reqOutput = new StringBuilder();
                reqOutput.Append("Elasticsearch request: " + url);
                log.Debug(reqOutput.ToString());

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseStr = await response.Content.ReadAsStringAsync();
                    JObject responseObj = JObject.Parse(responseStr);

                    var respOutput = new StringBuilder();
                    respOutput.Append("Elasticsearch response: " + url);
                    respOutput.Append(System.Environment.NewLine);
                    respOutput.Append(responseStr);

                    log.Debug(respOutput.ToString());

                    if (responseObj["_source"] != null)
                    {
                        log.Debug("Success");
                        var source = responseObj["_source"];
                        return _ParsePropertySource(responseObj["_id"].ToString(), source);                      
                    }
                    else
                    {
                        throw new ApplicationException("Invalid response from Elasticsearch");
                    }
                }
                else
                {
                    throw new ApplicationException("Error from Elasticsearch");
                }
            }
        }

        public async Task<ElasticsearchResult<Property>> QueryProperties(String FulltextQuery, SearchFilters Filters, int Offset = 0, int Limit = 10)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ElasticsearchBaseUrl"]);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var requestJObj = new JObject();
                requestJObj.Add("query", new JObject());

                var filter = new JObject();
                if(Filters != null && Filters.Filters.Count > 0)
                {
                    var topLevelAndFilterClause = new JArray();
                    filter["and"] = topLevelAndFilterClause;

                    foreach(var f in Filters.Filters)
                    {
                        var mappingInfos = _GetMappingInfoByModelName("property", f.FieldName);
                        if(mappingInfos.Count > 1)
                        {
                            // Don't support filtering when a single model name is actually an aggregated version of multiple elasticsearch fields
                            continue;
                        }

                        var mappingInfo = mappingInfos.FirstOrDefault<MappingInfo>();
                        if(mappingInfo == null)
                        {
                            continue;
                        }

                        if (mappingInfo.DataType == "fulltext" || mappingInfo.DataType == "match")
                        {
                            f.Predicates.ForEach(p => {

                                var term = new JObject();
                                term[_GetElasticsearchFieldNameForComparison("property", f.FieldName, p)] = p.Value;

                                var newFilter = new JObject();
                                newFilter["term"] = term;
                                topLevelAndFilterClause.Add(newFilter);
                            });
                        }

                        if (mappingInfo.DataType == "numeric")
                        {
                            var newFilter = new JObject();
                            var fieldObj = new JObject();

                            newFilter["range"] = fieldObj;

                            var ranges = new JObject();

                            fieldObj[_GetElasticsearchFieldNameForComparison("property", f.FieldName)] = ranges;

                            foreach (var comparison in f.Predicates)
                            {
                                Double tmpDouble;
                                if (!Double.TryParse(comparison.Value, out tmpDouble))
                                {
                                    ranges[comparison.Comparison] = comparison.Value;
                                }
                                else
                                {
                                    ranges[comparison.Comparison] = tmpDouble;
                                }
                            }

                            topLevelAndFilterClause.Add(newFilter);
                        }

                        if (mappingInfo.DataType == "datetime")
                        {
                            var newFilter = new JObject();
                            var fieldObj = new JObject();

                            newFilter["range"] = fieldObj;

                            var ranges = new JObject();

                            fieldObj[_GetElasticsearchFieldNameForComparison("property", f.FieldName)] = ranges;

                            foreach (var comparison in f.Predicates)
                            {
                                Double tmpDouble;
                                if (!Double.TryParse(comparison.Value, out tmpDouble))
                                {
                                    ranges[comparison.Comparison] = comparison.Value;
                                }
                                else
                                {
                                    ranges[comparison.Comparison] = tmpDouble;
                                }
                            }

                            topLevelAndFilterClause.Add(newFilter);
                        }

                    }
                }

                requestJObj.Add("filter", filter);
                requestJObj.Add("size", Limit);
                requestJObj.Add("from", Offset);

                if (!String.IsNullOrEmpty(FulltextQuery))
                {
                    var multiMatchObj = new JObject();
                    multiMatchObj.Add("query", FulltextQuery);
                    multiMatchObj.Add("type", "cross_fields");

                    var fields = new JArray();
                    fields.Add("property_address^5");
                    fields.Add("ALTERNATE_ADDRESS");
                    fields.Add("REGION_NAME");
                    fields.Add("SUB_MARKET_NAME");
                    fields.Add("NEIGHBORHOOD_NAME");
                    fields.Add("STATE");
                    fields.Add("COUNTRY");
                    fields.Add("MARKET_NAME");
                    fields.Add("DISTRICT_NAME");
                    fields.Add("PROPERTY_NAME^5");

                    multiMatchObj.Add("fields", fields);
                    multiMatchObj.Add("operator", "and");

                    var query = requestJObj["query"] as JObject;
                    query.Add("multi_match", multiMatchObj);
                }
                else
                {
                    var query = requestJObj["query"] as JObject;
                    query.Add("match_all", new JObject());
                }

                HttpContent content = new StringContent(requestJObj.ToString());
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var url = String.Format("{0}/{1}/property/_search", ConfigurationManager.AppSettings["ElasticsearchBaseUrl"], ConfigurationManager.AppSettings["ElasticsearchIndexName"]);

                var reqOutput = new StringBuilder();
                reqOutput.Append("Elasticsearch request: " + url);
                reqOutput.Append(System.Environment.NewLine);
                reqOutput.Append(requestJObj.ToString());

                log.Debug(reqOutput.ToString());
                
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseStr = await response.Content.ReadAsStringAsync();
                    JObject responseObj = JObject.Parse(responseStr);
                    var properties = new List<Property>();

                    var respOutput = new StringBuilder();
                    respOutput.Append("Elasticsearch response: " + url);
                    respOutput.Append(System.Environment.NewLine);
                    respOutput.Append(responseStr);

                    log.Debug(respOutput.ToString());

                    if (responseObj["hits"] != null && responseObj["hits"]["hits"] != null)
                    {
                        log.Debug("Success");

                        JArray hits = responseObj["hits"]["hits"] as JArray;
                        foreach(var hit in hits)
                        {
                            var source = hit["_source"];                  
                            properties.Add(_ParsePropertySource(hit["_id"].ToString(), source));                       
                        }

                        ElasticsearchResult<Property> result = new ElasticsearchResult<Property>();
                        result.Items = properties;
                        result.Total = Int32.Parse(responseObj["hits"]["total"].ToString());

                        return result;
                    }
                    else
                    {
                        throw new ApplicationException("Invalid response from Elasticsearch");
                    }
                }
                else
                {
                    throw new ApplicationException("Error from Elasticsearch");
                }
            }
        }

        public async Task<Space> GetSpaceById(String Id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ElasticsearchBaseUrl"]);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var url = String.Format("{0}/{1}/lease-space-availability/{2}", ConfigurationManager.AppSettings["ElasticsearchBaseUrl"],
                    ConfigurationManager.AppSettings["ElasticsearchIndexName"],
                    Id);

                var reqOutput = new StringBuilder();
                reqOutput.Append("Elasticsearch request: " + url);
                log.Debug(reqOutput.ToString());

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseStr = await response.Content.ReadAsStringAsync();
                    JObject responseObj = JObject.Parse(responseStr);

                    var respOutput = new StringBuilder();
                    respOutput.Append("Elasticsearch response: " + url);
                    respOutput.Append(System.Environment.NewLine);
                    respOutput.Append(responseStr);

                    log.Debug(respOutput.ToString());

                    if (responseObj["_source"] != null)
                    {
                        log.Debug("Success");
                        var source = responseObj["_source"];
                        return _ParseSpaceSource(responseObj["_id"].ToString(), source);
                    }
                    else
                    {
                        throw new ApplicationException("Invalid response from Elasticsearch");
                    }
                }
                else
                {
                    throw new ApplicationException("Error from Elasticsearch");
                }
            }
        }

        public async Task<ElasticsearchResult<Space>> QuerySpaces(String FulltextQuery, SearchFilters Filters, int Offset = 0, int Limit = 10)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ElasticsearchBaseUrl"]);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var requestJObj = new JObject();
                requestJObj.Add("query", new JObject());

                var filter = new JObject();
                if (Filters != null && Filters.Filters.Count > 0)
                {
                    var topLevelAndFilterClause = new JArray();
                    filter["and"] = topLevelAndFilterClause;

                    foreach (var f in Filters.Filters)
                    {
                        var mappingInfos = _GetMappingInfoByModelName("space", f.FieldName);
                        if (mappingInfos.Count > 1)
                        {
                            // Don't support filtering when a single model name is actually an aggregated version of multiple elasticsearch fields
                            continue;
                        }

                        var mappingInfo = mappingInfos.FirstOrDefault<MappingInfo>();
                        if (mappingInfo == null)
                        {
                            continue;
                        }

                        if (mappingInfo.DataType == "fulltext" || mappingInfo.DataType == "match")
                        {
                            f.Predicates.ForEach(p => {

                                var term = new JObject();
                                term[_GetElasticsearchFieldNameForComparison("space", f.FieldName, p)] = p.Value;

                                var newFilter = new JObject();
                                newFilter["term"] = term;
                                topLevelAndFilterClause.Add(newFilter);
                            });
                        }

                        if (mappingInfo.DataType == "numeric")
                        {
                            var newFilter = new JObject();
                            var fieldObj = new JObject();

                            newFilter["range"] = fieldObj;

                            var ranges = new JObject();

                            fieldObj[_GetElasticsearchFieldNameForComparison("space", f.FieldName)] = ranges;

                            foreach (var comparison in f.Predicates)
                            {
                                Double tmpDouble;
                                if (!Double.TryParse(comparison.Value, out tmpDouble))
                                {
                                    ranges[comparison.Comparison] = comparison.Value;
                                }
                                else
                                {
                                    ranges[comparison.Comparison] = tmpDouble;
                                }                                
                            }

                            topLevelAndFilterClause.Add(newFilter);
                        }

                        if (mappingInfo.DataType == "datetime")
                        {
                            var newFilter = new JObject();
                            var fieldObj = new JObject();

                            newFilter["range"] = fieldObj;

                            var ranges = new JObject();

                            fieldObj[_GetElasticsearchFieldNameForComparison("space", f.FieldName)] = ranges;

                            foreach (var comparison in f.Predicates)
                            {
                                Double tmpDouble;
                                if (!Double.TryParse(comparison.Value, out tmpDouble))
                                {
                                    ranges[comparison.Comparison] = comparison.Value;
                                }
                                else
                                {
                                    ranges[comparison.Comparison] = tmpDouble;
                                }
                            }

                            topLevelAndFilterClause.Add(newFilter);
                        }

                    }
                }

                requestJObj.Add("filter", filter);
                requestJObj.Add("size", Limit);
                requestJObj.Add("from", Offset);

                if (!String.IsNullOrEmpty(FulltextQuery))
                {
                    var multiMatchObj = new JObject();
                    multiMatchObj.Add("query", FulltextQuery);
                    multiMatchObj.Add("type", "cross_fields");

                    var fields = new JArray();
                    fields.Add("lease_availability_id^5");
                    fields.Add("SUITE^5");
                    fields.Add("FLOOR");
                    fields.Add("property.address^5");
                    fields.Add("property.region_name");
                    fields.Add("property.sub_market_name");
                    fields.Add("property.neighborhood_name");
                    fields.Add("property.state");
                    fields.Add("property.country");
                    fields.Add("property.market_name");
                    fields.Add("property.district_name");

                    multiMatchObj.Add("fields", fields);
                    multiMatchObj.Add("operator", "and");

                    var query = requestJObj["query"] as JObject;
                    query.Add("multi_match", multiMatchObj);
                }
                else
                {
                    var query = requestJObj["query"] as JObject;
                    query.Add("match_all", new JObject());
                }

                HttpContent content = new StringContent(requestJObj.ToString());
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var url = String.Format("{0}/{1}/lease-space-availability/_search", ConfigurationManager.AppSettings["ElasticsearchBaseUrl"], ConfigurationManager.AppSettings["ElasticsearchIndexName"]);

                var reqOutput = new StringBuilder();
                reqOutput.Append("Elasticsearch request: " + url);
                reqOutput.Append(System.Environment.NewLine);
                reqOutput.Append(requestJObj.ToString());

                log.Debug(reqOutput.ToString());

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseStr = await response.Content.ReadAsStringAsync();
                    JObject responseObj = JObject.Parse(responseStr);
                    var spaces = new List<Space>();

                    var respOutput = new StringBuilder();
                    respOutput.Append("Elasticsearch response: " + url);
                    respOutput.Append(System.Environment.NewLine);
                    respOutput.Append(responseStr);

                    log.Debug(respOutput.ToString());

                    if (responseObj["hits"] != null && responseObj["hits"]["hits"] != null)
                    {
                        log.Debug("Success");

                        JArray hits = responseObj["hits"]["hits"] as JArray;
                        foreach (var hit in hits)
                        {
                            var source = hit["_source"];
                            spaces.Add(_ParseSpaceSource(hit["_id"].ToString(), source));
                        }

                        ElasticsearchResult<Space> result = new ElasticsearchResult<Space>();
                        result.Items = spaces;
                        result.Total = Int32.Parse(responseObj["hits"]["total"].ToString());

                        return result;
                    }
                    else
                    {
                        throw new ApplicationException("Invalid response from Elasticsearch");
                    }
                }
                else
                {
                    throw new ApplicationException("Error from Elasticsearch");
                }
            }
        }

        private String _GetElasticsearchFieldNameForComparison(String EntityName, String FieldName, SearchFilterComparison Comparison)
        {
            var mappingInfos = _GetMappingInfoByModelName(EntityName, FieldName);
            var mappingInfo = mappingInfos.FirstOrDefault<MappingInfo>();
            if (mappingInfo == null)
            {
                throw new ArgumentException("FieldName", "Unable to find " + FieldName + " in lookup table for known fields");
            }

            if(mappingInfo.DataType == "match")
            {
                if (Comparison.Comparison == "eq")
                {
                    return mappingInfo.ElasticsearchFieldName;
                }
                else
                {
                    throw new ArgumentException("Only 'eq' is a valid comparison operator for field of type 'match'");
                }
            }
            if(mappingInfo.DataType == "fulltext")
            {
                if (Comparison.Comparison == "eq")
                {
                    return mappingInfo.ElasticsearchFieldName + ".raw";
                }
                else if (Comparison.Comparison == "like")
                {
                    return mappingInfo.ElasticsearchFieldName;
                }
                else
                {
                    throw new NotImplementedException("Only 'eq' and 'like' are valid comparison operators");
                }
            }

            var msg = String.Format("Unable to find FieldName for {0}, data type: {1}, comparison: {2}", 
                FieldName, mappingInfo.DataType, Comparison.Comparison);
            throw new NotImplementedException(msg);
        }

        private String _GetElasticsearchFieldNameForComparison(String EntityName, String FieldName)
        {
            var mappingInfos = _GetMappingInfoByModelName(EntityName, FieldName);
            var mappingInfo = mappingInfos.FirstOrDefault<MappingInfo>();
            if (mappingInfo == null)
            {
                throw new ArgumentException("FieldName", "Unable to find " + FieldName + " in lookup table for known fields");
            }

            if (mappingInfo.DataType == "numeric")
            {
                return mappingInfo.ElasticsearchFieldName;
            }

            if (mappingInfo.DataType == "date")
            {
                return mappingInfo.ElasticsearchFieldName;
            }

            var msg = String.Format("Unable to find FieldName for {0}, data type: {1}",
                FieldName, mappingInfo.DataType);
            throw new NotImplementedException(msg);
        }

        private String _GetPascalName(String name)
        {
            name = name.ToLower();
            return name.Pascalize();
        }

        private GeographicHierarchyLevel _ParseGeoHierarchy(String elasticsearchFieldPrefix, String levelLabel, int levelOrder, JToken source, bool useLowerCaseElasticsearchFields = false)
        {
            var idSuffix = useLowerCaseElasticsearchFields ? "_id" : "_ID";
            var nameSuffix = useLowerCaseElasticsearchFields ? "_name" : "_NAME";

            elasticsearchFieldPrefix = useLowerCaseElasticsearchFields ? elasticsearchFieldPrefix.ToLower() : elasticsearchFieldPrefix;

            if (source[elasticsearchFieldPrefix + idSuffix] == null) {
                return null;
            }
        
            GeographicHierarchyLevel level = new GeographicHierarchyLevel()
            {
                Label = levelLabel,
                Level = levelOrder,
                Id = source[elasticsearchFieldPrefix + idSuffix].ToString(),
                Name = source[elasticsearchFieldPrefix + nameSuffix] != null ? source[elasticsearchFieldPrefix + nameSuffix].ToString() : null
            };      

            return level;
        }

        private Property _ParseNestedPropertySource(String id, JToken source)
        {
            Property property = new Property();

            property.Id = id;
            property.BuildingName = source["property_name"] != null ? source["property_name"].ToString() : null;
            if (source["street_number1"] != null)
            {
                property.StreetNumber = source["street_number1"].ToString() + "-" + source["street_number2"].ToString();
            }
            else
            {
                property.StreetNumber = source["street_number1"] != null ? source["street_number1"].ToString() : null;
            }
            property.StreetName = source["street_name"] != null ? source["street_name"].ToString() : null;
            property.StreetType = source["street_type"] != null ? source["street_type"].ToString() : null;
            property.City = source["city"] != null ? source["city"].ToString() : null;
            property.State = source["state"] != null ? source["state"].ToString() : null;
            property.County = source["county"] != null ? source["county"].ToString() : null;
            property.PostalCode = source["postal_code"] != null ? source["postal_code"].ToString() : null;
            property.Country = source["country"] != null ? source["country"].ToString() : null;

            var geoParts = source["location"] != null ? source["location"].ToString().Split(',').ToList<String>() : new List<String>();
            if (geoParts.Count != 2)
            {
                throw new ArgumentException("Invalid geo location from property id: " + property.Id + ": " + source["location"]);
            }
            property.Location.Latitude = geoParts.Count == 2 ? float.Parse(geoParts[0]) : -1.000F;
            property.Location.Longitude = geoParts.Count == 2 ? float.Parse(geoParts[1]) : -1.000F;

            var alternateAddresses = source["alternate_address"] != null && source["alternate_address"] as JArray != null ? 
                source["alternate_address"] as JArray : 
                new JArray();

            foreach (var item in alternateAddresses)
            {
                if (item["name"] != null && !String.IsNullOrEmpty(item["name"].ToString()))
                {
                    var address = new PropertyAddress()
                    {
                        Name = item["name"].ToString(),
                        Address = item["address"] != null ? item["address"].ToString() : null
                    };
                    property.AlternateAddresses.Add(address);
                }
            }
            
            var region = _ParseGeoHierarchy("REGION", "Region", 0, source, true);
            var market = _ParseGeoHierarchy("MARKET", "Market", 1, source, true);
            var subMarket = _ParseGeoHierarchy("SUB_MARKET", "Sub Market", 2, source, true);
            var district = _ParseGeoHierarchy("DISTRICT", "District", 3, source, true);
            var neighborhood = _ParseGeoHierarchy("NEIGHBORHOOD", "Neighborhood", 4, source, true);

            if (region != null)
            {
                property.GeographicHierarchy.Add(region);
            }
            if (market != null)
            {
                property.GeographicHierarchy.Add(market);
            }
            if (subMarket != null)
            {
                property.GeographicHierarchy.Add(subMarket);
            }
            if (district != null)
            {
                property.GeographicHierarchy.Add(district);
            }
            if (neighborhood != null)
            {
                property.GeographicHierarchy.Add(neighborhood);
            }
            property.GeographicHierarchy = property.GeographicHierarchy.OrderBy(l => l.Level).ToList<GeographicHierarchyLevel>();

            property.PropertyType = source["property_type_name"] != null ? source["property_type_name"].ToString() : null;
            property.Class = source["class_name"] != null ? source["class_name"].ToString() : null;

            double tmpDouble;

            
            property.GrossSquareFeet = !Double.TryParse(source["gross_sf"] != null ? source["gross_sf"].ToString() : "", out tmpDouble) ? -1 : Double.Parse(source["gross_sf"].ToString());
            // TODO: Index the NET_RENTABLE_AREA_ field
            property.NetRentableAreaSquareFeet = !Double.TryParse(source["net_rentable_area"] != null ? source["net_rentable_area"].ToString() : "", out tmpDouble) ? 
                -1 : 
                Double.Parse(source["net_rentable_area"].ToString());
            property.AvailableSquareFeet = !Double.TryParse(source["avail_sf_total"] != null ? source["avail_sf_total"].ToString() : "", out tmpDouble) ? 
                -1 : 
                Double.Parse(source["avail_sf_total"].ToString());
            // TODO: Index the MAX_CONTIGUOUS_SF field
            property.MaxContiguousSquareFeet = !Double.TryParse(source["max_contiguous_sf"] != null ? source["max_contiguous_sf"].ToString() : "", out tmpDouble) ?
                -1 : 
                Double.Parse(source["max_contiguous_sf"].ToString());

            int tmpInt;
            property.YearBuilt = !Int32.TryParse(source["year_built"].ToString(), out tmpInt) ? -1 : Int32.Parse(source["year_built"].ToString());
            // TODO: Index the MONTH_BUILT field
            property.MonthBuilt = !Int32.TryParse(source["month_built"].ToString(), out tmpInt) ? -1 : Int32.Parse(source["month_built"].ToString());

            return property;
        }

        private Property _ParsePropertySource(String id, JToken source)
        {
            Property property = new Property();

            property.Id = id;
            property.BuildingName = source["PROPERTY_NAME"] != null ? source["PROPERTY_NAME"].ToString() : null;
            if (source["STREET_NUMBER2"] != null)
            {
                property.StreetNumber = source["STREET_NUMBER1"].ToString() + "-" + source["STREET_NUMBER2"].ToString();
            }
            else
            {
                property.StreetNumber = source["STREET_NUMBER1"] != null ? source["STREET_NUMBER1"].ToString() : null;
            }
            property.StreetName = source["STREET_NAME"] != null ? source["STREET_NAME"].ToString() : null;
            property.StreetType = source["STREET_TYPE"] != null ? source["STREET_TYPE"].ToString() : null;
            property.City = source["CITY"] != null ? source["CITY"].ToString() : null;
            property.State = source["STATE"] != null ? source["STATE"].ToString() : null;
            property.County = source["COUNTY"] != null ? source["COUNTY"].ToString() : null;
            property.PostalCode = source["POSTAL_CODE"] != null ? source["POSTAL_CODE"].ToString() : null;
            property.Country = source["COUNTRY"] != null ? source["COUNTRY"].ToString() : null;

            var geoParts = source["location"] != null ? source["location"].ToString().Split(',').ToList<String>() : new List<String>();
            if (geoParts.Count != 2)
            {
                throw new ArgumentException("Invalid geo location from property id: " + property.Id + ": " + source["location"]);
            }
            property.Location.Latitude = geoParts.Count == 2 ? float.Parse(geoParts[0]) : -1.000F;
            property.Location.Longitude = geoParts.Count == 2 ? float.Parse(geoParts[1]) : -1.000F;

            var alternateAddresses = source["alternate_address"] != null ? source["alternate_address"] as JArray : new JArray();
            foreach (var item in alternateAddresses)
            {
                if (item["name"] != null && !String.IsNullOrEmpty(item["name"].ToString()))
                {
                    var address = new PropertyAddress()
                    {
                        Name = item["name"].ToString(),
                        Address = item["address"] != null ? item["address"].ToString() : null
                    };
                    property.AlternateAddresses.Add(address);
                }
            }

            // Audit information
            DateTime tmp;
            if (source["CREATED_ON"] != null)
            {
                if (DateTime.TryParse(source["CREATED_ON"].ToString(), out tmp))
                {
                    property.Audit.CreatedOn = tmp;
                }
            }
            property.Audit.CreatedBy = source["CREATED_BY"] != null ? source["CREATED_BY"].ToString() : null;
            if (source["MODIFIED_ON"] != null)
            {
                if (DateTime.TryParse(source["MODIFIED_ON"].ToString(), out tmp))
                {
                    property.Audit.LastModifiedOn = tmp;
                }
            }
            property.Audit.LastModifiedBy = source["MODIFIED_BY"] != null ? source["MODIFIED_BY"].ToString() : null;

            if (source["IS_VERIFIED"] != null)
            {
                bool tmpBool;
                if (Boolean.TryParse(source["IS_VERIFIED"].ToString(), out tmpBool))
                {
                    property.Audit.IsVerified = tmpBool;
                }
            }

            var region = _ParseGeoHierarchy("REGION", "Region", 0, source);
            var market = _ParseGeoHierarchy("MARKET", "Market", 1, source);
            var subMarket = _ParseGeoHierarchy("SUB_MARKET", "Sub Market", 2, source);
            var district = _ParseGeoHierarchy("DISTRICT", "District", 3, source);
            var neighborhood = _ParseGeoHierarchy("NEIGHBORHOOD", "Neighborhood", 4, source);

            if (region != null)
            {
                property.GeographicHierarchy.Add(region);
            }
            if (market != null)
            {
                property.GeographicHierarchy.Add(market);
            }
            if (subMarket != null)
            {
                property.GeographicHierarchy.Add(subMarket);
            }
            if (district != null)
            {
                property.GeographicHierarchy.Add(district);
            }
            if (neighborhood != null)
            {
                property.GeographicHierarchy.Add(neighborhood);
            }
            property.GeographicHierarchy = property.GeographicHierarchy.OrderBy(l => l.Level).ToList<GeographicHierarchyLevel>();

            property.PropertyType = source["PROPERTY_TYPE_NAME"] != null ? source["PROPERTY_TYPE_NAME"].ToString() : null;
            property.Class = source["CLASS_NAME"] != null ? source["CLASS_NAME"].ToString() : null;

            double tmpDouble;

            property.GrossSquareFeet = !Double.TryParse(source["GROSS_SF"].ToString(), out tmpDouble) ? -1 : Double.Parse(source["GROSS_SF"].ToString());
            property.NetRentableAreaSquareFeet = !Double.TryParse(source["NET_RENTABLE_AREA"].ToString(), out tmpDouble) ? -1 : Double.Parse(source["NET_RENTABLE_AREA"].ToString());
            property.AvailableSquareFeet = !Double.TryParse(source["AVAIL_SF_TOTAL"].ToString(), out tmpDouble) ? -1 : Double.Parse(source["AVAIL_SF_TOTAL"].ToString());
            property.MaxContiguousSquareFeet = !Double.TryParse(source["MAX_CONTIGUOUS_SF"].ToString(), out tmpDouble) ? -1 : Double.Parse(source["MAX_CONTIGUOUS_SF"].ToString());

            int tmpInt;
            property.YearBuilt = !Int32.TryParse(source["YEAR_BUILT"].ToString(), out tmpInt) ? -1 : Int32.Parse(source["YEAR_BUILT"].ToString());
            property.MonthBuilt = !Int32.TryParse(source["MONTH_BUILT"].ToString(), out tmpInt) ? -1 : Int32.Parse(source["MONTH_BUILT"].ToString());

            return property;
        }

        private Space _ParseSpaceSource(String Id, JToken source)
        {
            Space space = new Space();
            space.Id = Id;

            var propertyToken = JToken.Parse(source["property"].ToString());

            space.Property = _ParseNestedPropertySource(propertyToken["id"].ToString(), propertyToken);

            Double tmpDouble;
            space.TotalAreaSquareFeet = !Double.TryParse(source["TOTAL_AREA_SF"].ToString(), out tmpDouble) ? 0 : Double.Parse(source["TOTAL_AREA_SF"].ToString());
            space.AvailableSquareFeet = !Double.TryParse(source["SQ_FT_AVAILABLE"].ToString(), out tmpDouble) ? 0 : Double.Parse(source["SQ_FT_AVAILABLE"].ToString());
            space.AskingRentMonthly = !Double.TryParse(source["ASKING_RENTAL_RATE_MONTHLY"].ToString(), out tmpDouble) ? 0 : Double.Parse(source["ASKING_RENTAL_RATE_MONTHLY"].ToString());
            space.AskingRentYearly = !Double.TryParse(source["ASKING_RENTAL_RATE_YEARLY"].ToString(), out tmpDouble) ? 0 : Double.Parse(source["ASKING_RENTAL_RATE_YEARLY"].ToString());
            space.AskingPrice = !Double.TryParse(source["ASKING_PRICE"].ToString(), out tmpDouble) ? 0 : Double.Parse(source["ASKING_PRICE"].ToString());

            // Audit information
            DateTime tmp;
            if (source["CREATED_ON"] != null)
            {
                if (DateTime.TryParse(source["CREATED_ON"].ToString(), out tmp))
                {
                    space.Audit.CreatedOn = tmp;
                }
            }
            space.Audit.CreatedBy = source["CREATED_BY"] != null ? source["CREATED_BY"].ToString() : null;
            if (source["MODIFIED_ON"] != null)
            {
                if (DateTime.TryParse(source["MODIFIED_ON"].ToString(), out tmp))
                {
                    space.Audit.LastModifiedOn = tmp;
                }
            }
            space.Audit.LastModifiedBy = source["MODIFIED_BY"] != null ? source["MODIFIED_BY"].ToString() : null;

            if (source["IS_VERIFIED"] != null)
            {
                bool tmpBool;
                if (Boolean.TryParse(source["IS_VERIFIED"].ToString(), out tmpBool))
                {
                    space.Audit.IsVerified = tmpBool;
                }
            }

            space.AvailabilityType = source["AVAILABILITY_TYPE"].ToString();
            space.Floor = source["FLOOR"].ToString();
            space.Suite = source["SUITE"].ToString();
            space.ListingSource = source["LISTING_SOURCE"].ToString();
            space.ListingType = source["LISTING_TYPE"].ToString();
            space.Status = source["STATUS"].ToString();

            return space;
        }
    }
}