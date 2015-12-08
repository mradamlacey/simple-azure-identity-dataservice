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

namespace DataServices.SimpleAzureIdentityDataService.Controllers
{
    
    [RoutePrefix("api/properties")]
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
        [Route("")]
        [HttpGet]
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
        [Route("{propertyId}")]
        [HttpGet]
        public async Task<Property> Get(String propertyId)
        {
            return new Property();
        }

        // POST: api/Property
        [Route("")]
        [HttpPost]
        public async void Post([FromBody]Property property)
        {
        }

        // PUT: api/Property/5
        [Route("{propertyId}")]
        [HttpPut]
        public async void Put(string propertyId, [FromBody]Property property)
        {
        }

        // DELETE: api/Property/5
        [Route("{propertyId}")]
        [HttpDelete]
        public async void Delete(string propertyId)
        {
        }
    }
}
