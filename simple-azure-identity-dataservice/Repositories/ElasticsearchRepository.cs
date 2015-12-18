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

        private List<MappingInfo> _GetMappingInfoByModelName(string ModelName)
        {
            var matching = propertyMappingInfo.Where(m => m.ModelName == ModelName).ToList<MappingInfo>();

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
                        var mappingInfos = _GetMappingInfoByModelName(f.FieldName);
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
                                term[_GetElasticsearchFieldNameForComparison(f.FieldName, p)] = p.Value;

                                var newFilter = new JObject();
                                newFilter["term"] = term;
                                topLevelAndFilterClause.Add(newFilter);
                            });
                        }

                        if(mappingInfo.DataType == "datetime")
                        {
                            // Dont support yet
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

                var url = String.Format("{0}/{1}//property/_search", ConfigurationManager.AppSettings["ElasticsearchBaseUrl"], ConfigurationManager.AppSettings["ElasticsearchIndexName"]);

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
                        var mappingInfos = _GetMappingInfoByModelName(f.FieldName);
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
                                term[_GetElasticsearchFieldNameForComparison(f.FieldName, p)] = p.Value;

                                var newFilter = new JObject();
                                newFilter["term"] = term;
                                topLevelAndFilterClause.Add(newFilter);
                            });
                        }

                        if (mappingInfo.DataType == "datetime")
                        {
                            // Dont support yet
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

        private String _GetElasticsearchFieldNameForComparison(String FieldName, SearchFilterComparison Comparison)
        {
            var mappingInfos = _GetMappingInfoByModelName(FieldName);
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

        private String _GetPascalName(String name)
        {
            name = name.ToLower();
            return name.Pascalize();
        }

        private GeographicHierarchyLevel _ParseGeoHierarchy(String elasticsearchFieldPrefix, String levelLabel, int levelOrder, JToken source)
        {
            if (source[elasticsearchFieldPrefix + "_ID"] == null) {
                return null;
            }
        
            GeographicHierarchyLevel level = new GeographicHierarchyLevel()
            {
                Label = levelLabel,
                Level = levelOrder,
                Id = source[elasticsearchFieldPrefix + "_ID"].ToString(),
                Name = source[elasticsearchFieldPrefix + "_NAME"] != null ? source[elasticsearchFieldPrefix + "_NAME"].ToString() : null
            };      

            return level;
        }

        private Property _ParsePropertySource(String id, JToken source)
        {
            Property property = new Property();

            property.Id = id;
            property.BuildingName = source["PROPERTY_NAME"].ToString();
            if (source["STREET_NUMBER2"] != null)
            {
                property.StreetNumber = source["STREET_NUMBER1"].ToString() + "-" + source["STREET_NUMBER2"].ToString();
            }
            else
            {
                property.StreetNumber = source["STREET_NUMBER1"].ToString();
            }
            property.StreetName = source["STREET_NAME"].ToString();
            property.StreetType = source["STREET_TYPE"].ToString();
            property.City = source["CITY"].ToString();
            property.State = source["STATE"].ToString();
            property.County = source["COUNTY"].ToString();
            property.PostalCode = source["POSTAL_CODE"].ToString();
            property.Country = source["COUNTRY"].ToString();

            var geoParts = source["location"] != null ? source["location"].ToString().Split(',').ToList<String>() : new List<String>();
            if (geoParts.Count != 2)
            {
                throw new ArgumentException("Invalid geo location from property id: " + property.Id + ": " + source["location"]);
            }
            property.Location.Latitude = geoParts.Count == 2 ? float.Parse(geoParts[0]) : -1.000F;
            property.Location.Longitude = geoParts.Count == 2 ? float.Parse(geoParts[1]) : -1.000F;

            var alternateAddresses = source["alternate_address"] as JArray;
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
                    property.IsVerified = tmpBool;
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

            property.PropertyType = source["PROPERTY_TYPE_NAME"].ToString();
            property.Class = source["CLASS_NAME"].ToString();

            double tmpDouble;

            property.GrossSquareFeet = !Double.TryParse(source["GROSS_SF"].ToString(), out tmpDouble) ? 0 : Double.Parse(source["GROSS_SF"].ToString());
            property.NetRentableAreaSquareFeet = !Double.TryParse(source["NET_RENTABLE_AREA"].ToString(), out tmpDouble) ? 0 : Double.Parse(source["NET_RENTABLE_AREA"].ToString());
            property.AvailableSquareFeet = !Double.TryParse(source["AVAIL_SF_TOTAL"].ToString(), out tmpDouble) ? 0 : Double.Parse(source["AVAIL_SF_TOTAL"].ToString());
            property.MaxContiguousSquareFeet = !Double.TryParse(source["MAX_CONTIGUOUS_SF"].ToString(), out tmpDouble) ? 0 : Double.Parse(source["MAX_CONTIGUOUS_SF"].ToString());

            int tmpInt;
            property.YearBuilt = !Int32.TryParse(source["YEAR_BUILT"].ToString(), out tmpInt) ? 0 : Int32.Parse(source["YEAR_BUILT"].ToString());
            property.MonthBuilt = !Int32.TryParse(source["MONTH_BUILT"].ToString(), out tmpInt) ? 0 : Int32.Parse(source["MONTH_BUILT"].ToString());

            return property;
        }

        private Space _ParseSpaceSource(String Id, JToken source)
        {
            Space space = new Space();
            space.Id = Id;

            var property = JToken.Parse(source["property"].ToString());
            space.PropertyId = property["id"].ToString(); ;

            return space;
        }
    }
}