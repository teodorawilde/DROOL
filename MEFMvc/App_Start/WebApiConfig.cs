using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MEFMvc
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{exchangeName}",
                defaults: new { exchangeName = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ConversionApi",
                routeTemplate: "api/{controller}/{exchangeName}/{value}/{from}/{to}/"                
            );

        }
    }
}