using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Editor.Front.Sessions
{
    public class SessionBinder : IModelBinder
    {
        public const string CookieName = "UserId";

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(Session))
                return false;

            Guid userId;
            var userIdCookie = actionContext.Request.Headers.GetCookies(CookieName).FirstOrDefault();
            if (userIdCookie == null || !Guid.TryParse(userIdCookie[CookieName].Value, out userId))
                userId = Guid.NewGuid();
            bindingContext.Model = new Session
                                   {
                                       UserId = userId
                                   };
            return true;
        }
    }
}