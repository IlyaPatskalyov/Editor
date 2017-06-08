using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Editor.Front.Sessions;
using RazorEngine;
using Encoding = System.Text.Encoding;

namespace Editor.Front.Results
{
    public class RazorViewResult : HttpResponseMessage
    {

        public RazorViewResult(Session session, string viewName, object model)
        {
            Content = new StringContent(GetContent(viewName, model), Encoding.UTF8, "text/html");
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

        private string GetContent(string viewName, object model)
        {
            var stream = typeof(Global).Assembly.GetManifestResourceStream($"Editor.Front.Views.{viewName}");
            using (var reader = new StreamReader(stream))
            {
                var template = reader.ReadToEnd();
                return Razor.Parse(template, model);
            }
        }
    }
}