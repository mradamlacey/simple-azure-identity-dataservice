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

using DataServices.SimpleAzureIdentityDataService.Models;


namespace DataServices.SimpleAzureIdentityDataService.Repositories
{
    public class ElasticsearchRepository
    {
        private ILog log = LogManager.GetLogger(typeof(ElasticsearchRepository));

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

                    var respOutput = new StringBuilder();
                    respOutput.Append("Elasticsearch response: " + url);
                    respOutput.Append(System.Environment.NewLine);
                    respOutput.Append(responseStr);

                    log.Debug(respOutput.ToString());

                    if (responseObj["hits"] != null && responseObj["hits"]["hits"] != null)
                    {
                        log.Debug("Success");
                    }
                    else
                    {
                        throw new ApplicationException("Invalid response from Elasticsearch");
                    }

                    return new List<Property>();
                }
                else
                {
                    throw new ApplicationException("Error from Elasticsearch");
                }
            }
        }


    }
}