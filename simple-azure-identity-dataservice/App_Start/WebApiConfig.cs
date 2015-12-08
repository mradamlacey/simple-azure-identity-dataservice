using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

using DataServices.SimpleAzureIdentityDataService.Common;

namespace simple_azure_identity_dataservice
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configure and start Log4Net
            log4net.Config.XmlConfigurator.Configure();
            
            // Web API configuration and services
            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
            config.Services.Add(typeof(IExceptionLogger), new Log4NetExceptionLogger());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
