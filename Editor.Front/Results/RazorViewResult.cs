using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Editor.Front.Sessions;
using RazorEngine.Templating;

namespace Editor.Front.Results
{
    public class RazorViewResult : HttpResponseMessage
    {
        public RazorViewResult(IRazorEngineService razorEngineService, Session session, string viewName, object model)
        {
            Content = new StringContent(GetContent(razorEngineService, viewName, model), Encoding.UTF8, "text/html");
            SetCookie(session);
        }

        private void SetCookie(Session session)
        {
            Headers.AddCookies(new[]
                               {
                                   new CookieHeaderValue(SessionBinder.CookieName, session.UserId.ToString())
                                   {
                                       HttpOnly = true,
                                       Path = "/",
                                       Expires = DateTimeOffset.MaxValue
                                   }
                               });
        }

        private string GetContent(IRazorEngineService razorEngineService, string viewName, object model)
        {
            return razorEngineService.RunCompile(viewName, model.GetType(), model);
        }
    }
}