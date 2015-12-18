using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace DataServices.SimpleAzureIdentityDataService.Common.Security
{
    /// <summary>
    /// Custom Web API attribute to require authorization only if a specific web.config setting
    /// is set to true
    /// </summary>
    public class ConfigurableAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {

            var enableOAuth = Boolean.Parse(ConfigurationManager.AppSettings["EnableOAuth"].ToString());
            if (!enableOAuth)
            {
                return;
            }

            if (!actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                this.HandleUnauthorizedRequest(actionContext);
            }
            else
            {
                base.OnAuthorization(actionContext);
            }

        }

    }
}