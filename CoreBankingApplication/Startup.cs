using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CoreBankingApplication.Startup))]
namespace CoreBankingApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
