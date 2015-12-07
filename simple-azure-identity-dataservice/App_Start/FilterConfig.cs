using System.Web;
using System.Web.Mvc;

namespace simple_azure_identity_dataservice
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
