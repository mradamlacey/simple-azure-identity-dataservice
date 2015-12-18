using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

using DataServices.SimpleAzureIdentityDataService.Common;
using DataServices.SimpleAzureIdentityDataService.Common.HypermediaTransformers;
using Newtonsoft.Json.Serialization;

namespace DataServices.SimpleAzureIdentityDataService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configure and start Log4Net
            log4net.Config.XmlConfigurator.Configure();

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Web API configuration and services
            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
            config.Services.Add(typeof(IExceptionLogger), new Log4NetExceptionLogger());

            config.MessageHandlers.Add(new HypermediaMessageHandler());

            config.AddResponseTransformer(
                new PropertyHypermediaTransformer(),
                new PropertyListHypermediaTransformer(),
                new SubscriptionHypermediaTransformer(),
                new SpaceListHypermediaTransformer()
            );

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Catch all conventions for routing
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
