using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Editor.Front.Sessions;
using Editor.Storage.EF;
using Editor.Storage.EF.SQLite;

namespace Editor.Front
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();
            var frontAssembly = typeof(Global).Assembly;

            builder.RegisterControllers(frontAssembly);
            builder.RegisterApiControllers(frontAssembly);
            builder.RegisterAssemblyTypes(frontAssembly).AsImplementedInterfaces();

            builder.RegisterType<SQLiteDbContext>().As<IDbContext>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(Loader.LoadFromBinDirectory("Editor.*.dll")).AsImplementedInterfaces();

            var container = builder.Build();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders[typeof(Session)] = new SessionBinder();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }


        public override void Init()
        {
            PostAuthenticateRequest += MvcApplication_PostAuthenticateRequest;
            base.Init();
        }

        void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }
    }
}