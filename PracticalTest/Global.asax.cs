using NLog;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PracticalTest
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            LogManager.Setup().LoadConfigurationFromFile("NLog.config");
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
