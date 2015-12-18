using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

using Swashbuckle.Swagger.Annotations;

using DataServices.SimpleAzureIdentityDataService.Common.Security;
using DataServices.SimpleAzureIdentityDataService.Repositories;
using DataServices.SimpleAzureIdentityDataService.Models;

namespace DataServices.SimpleAzureIdentityDataService.Controllers
{
    [RoutePrefix("api/spaces")]
    [SwaggerResponse(statusCode: HttpStatusCode.Unauthorized, Description = "Not authorized to make this request")]
    [SwaggerResponse(statusCode: HttpStatusCode.Forbidden, Description = "Authentication data provided is forbidden.  Please re-request or refresh the access token or ensure correct format for authentication data")]
    [ConfigurableAuthorizeAttribute]
    public class SpaceController : ApiController
    {
        private ElasticsearchRepository elasticsearchRepository;

        public SpaceController()
        {
            this.elasticsearchRepository = new ElasticsearchRepository();
        }

        /// <summary>
        /// Returns a list of properties filtered by the specified filters
        /// </summary>
        /// <param name="FullTextQuery">Text to use to match as a full text search across various attributes of a property</param>
        /// <param name="Filters">Set of comparisons and predicates to filter the search results by</param>
        /// <param name="Limit">Total number or results to return, maximum of 1000</param>
        /// <param name="Offset">Starting record number to return, to be used to page across entire result set</param>
        /// <returns>Matching property search results</returns>
        [Route("", Name = "GetSpaces")]
        [HttpGet]
        [SwaggerResponse(statusCode: HttpStatusCode.BadRequest, Description = "Invalid search or filter criteria submitted")]
        public async Task<SpaceList> GetCollection(String FullTextQuery = null, SearchFilters Filters = null, int Offset = 0, int Limit = 10)
        {
            if (Limit > 1000)
            {
                throw new ArgumentException("Limit", "Limit should be greater than 0 and less than or equal to 1000");
            }

            SpaceList resp = new SpaceList();
            var result = await elasticsearchRepository.QuerySpaces(FullTextQuery, Filters, Offset, Limit);

            resp.Items = result.Items;
            resp.Total = result.Total;
            resp.Offset = Offset;
            resp.Limit = Limit;

            return resp;
        }

    }
}
