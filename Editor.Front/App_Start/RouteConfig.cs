using System.Web.Mvc;
using System.Web.Routing;

namespace Editor.Front
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Main", action = "Index", id = UrlParameter.Optional}
            );
        }
    }
}