﻿using System.Web.Mvc;
using System.Web.Routing;

namespace EECIP
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "EmergencyHoundWeb.Controllers" }
            );

            //routes.MapRoute(
            //    "topicUrls", // Route name
            //    string.Concat("thread", "/{slug}"), // URL with parameters
            //    new { controller = "Forum", action = "ShowTopic", slug = UrlParameter.Optional } // Parameter defaults
            //    );
        }
    }
}
