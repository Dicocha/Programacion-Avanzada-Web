using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using HelpDesk.Web.Data;
using HelpDesk.Web.Migrations;

namespace HelpDesk.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<HelpDeskContext, Configuration>());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
