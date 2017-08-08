using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Autofac;
using Editor.Front.WebApiExtensions;
using Newtonsoft.Json;

namespace Editor.Front
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, IContainer container)
        {
            ConfigureFormatters(config);
            ConfigureRoutes(config);
            ConfigureLogs(config, container);
        }
        
        private static void ConfigureLogs(HttpConfiguration config, IContainer container)
        {
            config.Services.Add(typeof(IExceptionLogger), container.Resolve<GlobalExceptionLogger>());
            config.MessageHandlers.Add(container.Resolve<LogMessageHandler>());
        }


        private static void ConfigureRoutes(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("DefaultApi",
                                       "api/{controller}/{id}",
                                       new
                                       {
                                           id = RouteParameter.Optional
                                       }
            );
        }

        private static void ConfigureFormatters(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
                                                                 {
                                                                     Formatting = Formatting.Indented
                                                                 };
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}