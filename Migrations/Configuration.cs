using System.Data.Entity.Migrations;
using HelpDesk.Web.Data;

namespace HelpDesk.Web.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<HelpDeskContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "HelpDesk.Web.Data.HelpDeskContext";
        }

        protected override void Seed(HelpDeskContext context)
        {
            SeedData.Initialize(context);
        }
    }
}
