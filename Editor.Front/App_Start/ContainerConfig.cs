using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Integration.WebApi;
using Editor.Front.DocumentSessions;
using Editor.Front.Sessions;
using Editor.Front.WebSockets;
using Editor.Storage.EF;
using Editor.Storage.EF.SQLite;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace Editor.Front
{
    public static class ContainerConfig
    {
        public static IContainer Build()
        {
            var builder = new ContainerBuilder();
            var frontAssembly = typeof(Global).Assembly;

            builder.RegisterInstance(LoggerConfig.Build()).AsImplementedInterfaces();

            var templateConfig = new TemplateServiceConfiguration
                                 {
                                     TemplateManager = new DelegateTemplateManager(GetView),
                                     DisableTempFileLocking = true,
                                     CachingProvider = new DefaultCachingProvider(t => { })
                                 };

            builder.RegisterInstance(RazorEngineService.Create(templateConfig)).AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(frontAssembly).AsImplementedInterfaces().SingleInstance();

            var assemblies = Loader.LoadFromBinDirectory("Editor.*.dll");
            builder.RegisterAssemblyTypes(assemblies).AsSelf().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<SQLiteDbContext>().As<IDbContext>().InstancePerLifetimeScope();

            builder.RegisterApiControllers(frontAssembly);

            builder.RegisterWebApiModelBinderProvider();
            builder.RegisterType<SessionBinder>().AsModelBinderForTypes(typeof(Session));
            builder.RegisterType<DocumentSession>().As<IDocumentSession>().InstancePerDependency();
            builder.RegisterType<EditorWebSocket>().InstancePerDependency();
            
            return builder.Build();
        }

        private static string GetView(string name)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Editor.Front.Views.{name}");
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}