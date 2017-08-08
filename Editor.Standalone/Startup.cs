using System;
using System.Web.Http;
using Autofac.Extras.CommonServiceLocator;
using Autofac.Integration.WebApi;
using Editor.Front;
using Editor.Front.WebSockets;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Owin;
using Owin.WebSocket.Extensions;

namespace Editor.Standalone
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();
            var container = ContainerConfig.Build();

            WebApiConfig.Register(config, container);

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            appBuilder.UseAutofacMiddleware(container);
            appBuilder.UseAutofacWebApi(config);
            appBuilder.UseWebApi(config);

            var fileSystem = new EmbeddedResourceFileSystem(typeof(Global).Assembly, "Editor.Front");
            var options = new FileServerOptions
                          {
                              FileSystem = fileSystem
                          };
            options.StaticFileOptions.FileSystem = fileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = true;

            appBuilder.UseFileServer(options);
            appBuilder.MapWebSocketPattern<EditorWebSocket>("/ws/editor/(?<documentId>[0-9A-Fa-f\\-]{36})/(?<clientId>[0-9A-Fa-f\\-]{36})",
                                                            new AutofacServiceLocator(container));
        }

        private static void Main(string[] args)
        {
            var baseAddress = args.Length > 0 && !string.IsNullOrEmpty(args[0]) ? args[0] : "http://localhost:9000/";

            using (WebApp.Start<Startup>(baseAddress))
            {
                Console.WriteLine(baseAddress);
                Console.ReadLine();
            }
        }
    }
}