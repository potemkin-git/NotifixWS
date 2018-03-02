using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Notifix
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

 
            routes.MapRoute(
                name: "Default",
                url: "notifix/api/start",
                defaults: new { controller = "Notifix", action = "CheckService" }
            );
        }
    }
}
