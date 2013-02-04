using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace FileManager.Web
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
            config.Routes.MapHttpRoute(
                name: "ControllerAndAction",
                routeTemplate: "api/{controller}/{action}"
            );

			config.Routes.MapHttpRoute(
					name: "DefaultApi",
					routeTemplate: "api/{controller}/{id}",
					defaults: new
					{
						id = RouteParameter.Optional
					}
			);


			var json = config.Formatters.JsonFormatter;
			json.SerializerSettings.PreserveReferencesHandling =
					Newtonsoft.Json.PreserveReferencesHandling.Objects;

			config.Formatters.Remove(config.Formatters.XmlFormatter);
		}
	}
}
