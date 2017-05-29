using System;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using IModelBinder = System.Web.Http.ModelBinding.IModelBinder;
using ModelBindingContext = System.Web.Http.ModelBinding.ModelBindingContext;

namespace Editor.Front.Sessions
{
    public class SessionBinder : IModelBinder, System.Web.Mvc.IModelBinder
    {
        private const string CookieName = "UserId";

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(Session))
                return false;

            bindingContext.Model = Bind(GetHttpContextWrapper(actionContext.Request));
            return true;
        }

        public object BindModel(ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            return bindingContext.ModelType == typeof(Session) ? Bind(controllerContext.HttpContext) : null;
        }

        private static Session Bind(HttpContextBase context)
        {
            Guid userId;
            var userIdCookie = context.Request.Cookies[CookieName];
            if (userIdCookie == null || !Guid.TryParse(userIdCookie.Value, out userId))
            {
                userId = Guid.NewGuid();
                context.Response.Cookies.Add(new HttpCookie(CookieName, userId.ToString())
                                             {
                                                 HttpOnly = true,
                                                 Expires = DateTime.MaxValue
                                             });
            }
            return new Session
                   {
                       UserId = userId
                   };
        }

        private static HttpContextWrapper GetHttpContextWrapper(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
                return ((HttpContextWrapper) request.Properties["MS_HttpContext"]);
            if (HttpContext.Current != null)
                return new HttpContextWrapper(HttpContext.Current);
            return null;
        }
    }
}