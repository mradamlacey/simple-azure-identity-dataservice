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
using System.Web.Http.Description;

namespace DataServices.SimpleAzureIdentityDataService.Controllers
{
    
    [RoutePrefix("api/properties")]
    [SwaggerResponse(statusCode: HttpStatusCode.Unauthorized, Description = "Not authorized to make this request")]
    [SwaggerResponse(statusCode: HttpStatusCode.Forbidden, Description = "Authentication data provided is forbidden.  Please re-request or refresh the access token or ensure correct format for authentication data")]
    public class PropertyController : ApiController
    {

        private ElasticsearchRepository elasticsearchRepository;
        private EimDataRepository eimDataRepository;

        /// <summary>
        /// Public default constructor
        /// </summary>
        public PropertyController()
        {
            this.elasticsearchRepository = new ElasticsearchRepository();
            this.eimDataRepository = new EimDataRepository();
        }

        // GET: api/Property
        /// <summary>
        /// Returns a list of properties filtered by the specified filters
        /// </summary>
        /// <returns></returns>
        [Route("", Name = "GetCollection")]
        [HttpGet]
        [SwaggerResponse(statusCode: HttpStatusCode.BadRequest, Description = "Invalid search or filter criteria submitted")]
        public async Task<PropertyList> Get()
        {
            PropertyList resp = new PropertyList();
            resp.Items = await elasticsearchRepository.QueryProperties(null);

            return resp;
        }

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

        /// <summary>
        /// A new property is submitted to the platform for later asynchronous approval.  Clients
        /// are not allowe to directly create new propertys in the platform and so an approval/validation workflow will occur
        /// to process each submission.  Use the 'Changes' API to get notification
        /// when the property is actually created.
        /// </summary>
        /// <param name="property"></param>
        [Route("submitNew", Name = "SubmitNew")]
        [HttpPost]
        [SwaggerResponse(statusCode: HttpStatusCode.BadRequest, Description = "Invalid data submitted for the property")]
        public async Task<HttpResponseMessage> SubmitNew([FromBody]Property property)
        {
            SubmissionStatus status = new SubmissionStatus()
            {
                StatusCode = 0,
                Message = "New property submission was successfully received"
            };

            var result = await eimDataRepository.SubmitNewProperty(property);

            Random seed = new Random();
            var randomId = seed.Next(50000, 200000);
            HttpResponseMessage response = Request.CreateResponse<SubmissionStatus>(HttpStatusCode.Created, status);
            response.Headers.Add("Location", Url.Link("Get", new { propertyId = randomId}));

            return response;
        }

        /// <summary>
        /// An update to some attributes of a property is submitted to the platform for later asynchronous approval.  Clients
        /// are not allowed to directly update properties in the platform and so an approval/validation workflow will occur
        /// to process each submission.  Use the 'Changes' API to get notification when the property is actually created.
        /// </summary>
        /// <param name="property"></param>
        [Route("submitUpdate", Name = "SubmitUpdate")]
        [HttpPost]
        [SwaggerResponse(statusCode: HttpStatusCode.BadRequest, Description = "Invalid data submitted for the property")]
        public async Task<HttpResponseMessage> SubmitUpdate([FromBody]Property property)
        {
            SubmissionStatus status = new SubmissionStatus()
            {
                StatusCode = 0,
                Message = "Update to property submission was successfully received"
            };

            var result = await eimDataRepository.SubmitUpdateToProperty(property);

            Random seed = new Random();
            var randomId = seed.Next(50000, 200000);
            HttpResponseMessage response = Request.CreateResponse<SubmissionStatus>(HttpStatusCode.Created, status);
            response.Headers.Add("Location", Url.Link("Get", new { propertyId = randomId }));

            return response;
        }

        /// <summary>
        /// Creates a new property in the platform.  This differs from the normal approval workflow and directly
        /// commits a change into the datastore.
        /// </summary>
        /// <param name="property"></param>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("", Name ="Create")]
        [HttpPost]
        [SwaggerResponse(statusCode: HttpStatusCode.BadRequest, Description = "Invalid data submitted for the property")]
        public async void Post([FromBody]Property property)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
        }

        /// <summary>
        /// Updates an existing property in the platform.  This differs from the normal approval workflow and directly
        /// commits a change into the datastore.
        /// </summary>
        /// <param name="propertyId"></param>
        /// <param name="property"></param>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("{propertyId}", Name ="Update")]
        [HttpPut]
        [SwaggerResponse(statusCode: HttpStatusCode.NotFound, Description = "Property with the specified ID does not exist")]
        [SwaggerResponse(statusCode: HttpStatusCode.BadRequest, Description = "Invalid data submitted for the property")]
        public async void Put(string propertyId, [FromBody]Property property)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
        }

        /// <summary>
        /// Deletes a property from the platform.  This differs from the normal approval workflow and directly
        /// commits a change into the datastore.
        /// </summary>
        /// <param name="propertyId"></param>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("{propertyId}", Name = "Delete")]
        [HttpDelete]
        [SwaggerResponse(statusCode: HttpStatusCode.NotFound, Description = "Property with the specified ID does not exist")]
        public async void Delete(string propertyId)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
        }
    }
}
