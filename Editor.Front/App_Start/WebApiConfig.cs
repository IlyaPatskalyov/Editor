using System.Web.Http;
using Newtonsoft.Json;

namespace Editor.Front
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            ConfigureFormatters(config);
            ConfigureRoutes(config);
        }

        private static void ConfigureRoutes(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional}
            );
        }

        private static void ConfigureFormatters(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
                                                                 {
                                                                     Formatting = Formatting.Indented,
                                                                 };
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}