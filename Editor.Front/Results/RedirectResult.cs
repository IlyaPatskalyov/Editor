using System;
using System.Net;
using System.Net.Http;

namespace Editor.Front.Results
{
    public class RedirectResult : HttpResponseMessage
    {
        public RedirectResult(string location)
        {
            StatusCode = HttpStatusCode.Moved;
            Headers.Location = new Uri(location, UriKind.Relative);
        }
    }
}