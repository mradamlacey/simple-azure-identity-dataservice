using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;

using DataServices.SimpleAzureIdentityDataService.Models;
using DataServices.SimpleAzureIdentityDataService.Repositories;
using Swashbuckle.Swagger.Annotations;

namespace DataServices.SimpleAzureIdentityDataService.Controllers
{
    
    [RoutePrefix("api/properties")]
    [SwaggerResponse(statusCode: HttpStatusCode.Unauthorized, Description = "Not authorized to make this request")]
    [SwaggerResponse(statusCode: HttpStatusCode.Forbidden, Description = "Authentication data provided is forbidden.  Please re-request or refresh the access token or ensure correct format for authentication data")]
    public class PropertyController : ApiController
    {

        private ElasticsearchRepository elasticsearchRepository;

        /// <summary>
        /// Public default constructor
        /// </summary>
        public PropertyController()
        {
            this.elasticsearchRepository = new ElasticsearchRepository();
        }

        // GET: api/Property
        /// <summary>
        /// Returns a list of properties filtered by the specified filters
        /// </summary>
        /// <returns></returns>
        [Route("", Name = "GetCollection")]
        [HttpGet]
        [SwaggerResponse(statusCode: HttpStatusCode.BadRequest, Description = "Invalid search or filter criteria submitted")]
        public async Task<List<Property>> Get()
        {
            return await elasticsearchRepository.QueryProperties(null);
        }

        // GET: api/Property/5
        /// <summary>
        /// Retrieve a single property by unique ID
        /// </summary>
        /// <param name="id">ID of the property</param>
        /// <returns>Property information</returns>
        [SwaggerResponse(statusCode:HttpStatusCode.NotFound, Description = "Property with the specified ID does not exist")]
        [Route("{propertyId}", Name = "Get")]
        [HttpGet]
        public async Task<Property> Get(String propertyId)
        {
            return await elasticsearchRepository.GetPropertyById(propertyId);
        }

        // POST: api/Property
        [Route("", Name ="Create")]
        [HttpPost]
        [SwaggerResponse(statusCode: HttpStatusCode.BadRequest, Description = "Invalid data submitted for the property")]
        public async void Post([FromBody]Property property)
        {
        }

        // PUT: api/Property/5
        [Route("{propertyId}", Name ="Update")]
        [HttpPut]
        [SwaggerResponse(statusCode: HttpStatusCode.NotFound, Description = "Property with the specified ID does not exist")]
        [SwaggerResponse(statusCode: HttpStatusCode.BadRequest, Description = "Invalid data submitted for the property")]
        public async void Put(string propertyId, [FromBody]Property property)
        {
        }

        // DELETE: api/Property/5
        [Route("{propertyId}", Name = "Delete")]
        [HttpDelete]
        [SwaggerResponse(statusCode: HttpStatusCode.NotFound, Description = "Property with the specified ID does not exist")]
        public async void Delete(string propertyId)
        {
        }
    }
}
