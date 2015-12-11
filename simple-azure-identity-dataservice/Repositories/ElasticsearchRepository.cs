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
    public class ElasticsearchRepository
    {
        private ILog log = LogManager.GetLogger(typeof(ElasticsearchRepository));

        public async Task<Property> GetPropertyById(String Id){
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ElasticsearchBaseUrl"]);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var url = String.Format("{0}/{1}//property/{2}", ConfigurationManager.AppSettings["ElasticsearchBaseUrl"], 
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

        public async Task<List<Property>> QueryProperties(String fulltextQuery)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ElasticsearchBaseUrl"]);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var requestJObj = new JObject();
                requestJObj.Add("query", new JObject());

                var query = requestJObj["query"] as JObject;
                query.Add("match_all", new JObject());

                var filter = new JObject();

                requestJObj.Add("filter", filter);

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

                        return properties;
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

            return property;
        }

    }
}