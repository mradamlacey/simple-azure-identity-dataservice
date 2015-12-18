using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace DataServices.SimpleAzureIdentityDataService.Common
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            
            JToken errorResponse = new JObject();
            errorResponse["message"] = "A server error has occurred.  Please retry or contact support";
            errorResponse["code"] = 500;

            HttpResponseMessage response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, errorResponse);
            response.Headers.Add("ContentType", "application/json");

            context.Result = new ResponseMessageResult(response);


            // TODO: Other exception handling and/or logging
            // base.Handle(context);
        }
    }
}