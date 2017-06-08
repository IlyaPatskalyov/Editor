using Autofac;
using Autofac.Integration.WebApi;
using Editor.Front.DocumentSessions;
using Editor.Front.Sessions;
using Editor.Storage.EF;
using Editor.Storage.EF.SQLite;

namespace Editor.Front
{
    public class ContainerConfig
    {
        public static IContainer Build()
        {
            var builder = new ContainerBuilder();
            var frontAssembly = typeof(Global).Assembly;
            
            

            builder.RegisterAssemblyTypes(frontAssembly).AsImplementedInterfaces().SingleInstance();
            
            var assemblies = Loader.LoadFromBinDirectory("Editor.*.dll");
            builder.RegisterAssemblyTypes(assemblies).AsSelf().AsImplementedInterfaces().SingleInstance();
            
            builder.RegisterType<SQLiteDbContext>().As<IDbContext>().InstancePerLifetimeScope();
            
            builder.RegisterApiControllers(frontAssembly);

            builder.RegisterWebApiModelBinderProvider();
            builder.RegisterType<SessionBinder>().AsModelBinderForTypes(typeof(Session));
            builder.RegisterType<DocumentSession>().As<IDocumentSession>().InstancePerDependency();
            return builder.Build();
        }
    }
}