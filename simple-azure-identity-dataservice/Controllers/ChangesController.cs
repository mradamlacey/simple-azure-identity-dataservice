using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

using Swashbuckle.Swagger.Annotations;

using DataServices.SimpleAzureIdentityDataService.Models;

namespace DataServices.SimpleAzureIdentityDataService.Controllers
{
    /// <summary>
    /// API to retrieve changes via a subscription mechanism
    /// </summary>
    [RoutePrefix("api/changes")]
    public class ChangesController : ApiController
    {
        /// <summary>
        /// Retrieves all the changes property that were made between the start and end timestamp
        /// </summary>
        /// <param name="start">Starting timestamp (inclusive) of changes to include</param>
        /// <param name="end">Ending timestamp (exclusive) of changes to include</param>
        /// <returns></returns>
        [Route("", Name = "GetBetweenDates")]
        [HttpGet]
        [SwaggerResponse(statusCode: HttpStatusCode.BadRequest, Description = "Invalid data submitted for the property")]
        public async Task<List<ChangeInfo>> GetBetweenDates(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new subscription for some entity
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        [Route("subscriptions", Name = "CreateSubscription")]
        [HttpPost]
        [SwaggerResponse(statusCode: HttpStatusCode.BadRequest, Description = "Invalid data submitted for the property")]
        public async Task<HttpResponseMessage> CreateNewSubscription(String Entity, String CallbackUrl)
        {
            Random seed = new Random();

            if (!Uri.IsWellFormedUriString(CallbackUrl, UriKind.Absolute))
            {
                throw new ArgumentException("CallbackUrl", "Invalid CallbackUrl provided: " + CallbackUrl);
            }

            // Fake async code...
            var taskSource = new TaskCompletionSource<Subscription>();
            taskSource.SetResult(new Subscription()
            {
                Id = String.Format("{0:d}", seed.Next(50000, 200000)),
                CreatedOn = DateTime.Now,
                NumberOfChanges = 0,
                Status = 0,
                CallbackUrl = CallbackUrl
            });

            var subscription = await taskSource.Task;

            HttpResponseMessage response = Request.CreateResponse<Subscription>(HttpStatusCode.Created, subscription);
            response.Headers.Add("Location", Url.Link("Get", new { propertyId = subscription.Id }));

            return response;
        }

        [Route("subscriptions", Name = "GetSubscriptions")]
        [HttpGet]
        public async Task<SubscriptionList> GetSubscriptions()
        {
            Random seed = new Random();

            // Fake async code...
            var taskSource = new TaskCompletionSource<Subscription>();
            taskSource.SetResult(new Subscription()
            {
                Id = String.Format("{0:d}", seed.Next(50000, 200000)),
                CreatedOn = DateTime.Now,
                NumberOfChanges = 0,
                Status = 0,
                CallbackUrl = "http://callback/url/here"
            });

            var subscription = await taskSource.Task;

            return new SubscriptionList()
            {
                Items = new List<Subscription>() { subscription }
            };
        }

        [SwaggerResponse(statusCode: HttpStatusCode.NotFound, Description = "Subscription with the specified ID does not exist")]
        [Route("subscriptions/{SubscriptionId}", Name = "GetSubscriptionById")]
        [HttpGet]
        public async Task<Subscription> GetSubscriptionById(String SubscriptionId)
        {
            

            // Fake async code...
            var taskSource = new TaskCompletionSource<Subscription>();
            taskSource.SetResult(new Subscription()
            {
                Id = SubscriptionId,
                CreatedOn = DateTime.Now,
                NumberOfChanges = 0,
                Status = 0,
                CallbackUrl = "http://callback/url/here"
            });

            var subscription = await taskSource.Task;

            return subscription;
        }

        [SwaggerResponse(statusCode: HttpStatusCode.NotFound, Description = "Subscription with the specified ID does not exist")]
        [Route("subscriptions/{SubscriptionId}", Name = "DeleteSubscription")]
        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteSubscription(String SubscriptionId)
        {
            // Fake async code...
            var taskSource = new TaskCompletionSource<Subscription>();
            taskSource.SetResult(new Subscription()
            {
                Id = SubscriptionId,
                CreatedOn = DateTime.Now,
                NumberOfChanges = 0,
                Status = 0,
                CallbackUrl = "http://callback/url/here"
            });

            var subscription = await taskSource.Task;

            var response = Request.CreateResponse(HttpStatusCode.NoContent);
            return response;
        }
    }
}
