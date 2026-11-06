using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(HelpDesk.Web.Startup))]

namespace HelpDesk.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
