﻿using System.Web;
using System.Web.Http;
using Autofac.Integration.WebApi;

namespace Editor.Front
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            var container = ContainerConfig.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            LoggerConfig.Build();
            WebApiConfig.Register(GlobalConfiguration.Configuration, container);
            GlobalConfiguration.Configuration.EnsureInitialized();
        }
    }
}