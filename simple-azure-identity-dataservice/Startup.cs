using Owin;

namespace DataServices.SimpleAzureIdentityDataService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}