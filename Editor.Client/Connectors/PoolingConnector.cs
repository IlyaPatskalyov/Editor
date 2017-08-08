using System;
using Bridge.Html5;
using Bridge.jQuery2;
using Editor.Front.DocumentSessions;

namespace Editor.Client.Connectors
{
    public class PoolingConnector : ConnectorBase
    {
        private readonly Guid clientId;
        private readonly string documentId;
        private bool lockObject;

        public PoolingConnector(string documentId, Guid clientId, Action<DocumentState> onUpdateState) : base(onUpdateState)
        {
            this.documentId = documentId;
            this.clientId = clientId;
        }

        protected override void Ping()
        {
            if (lockObject)
                return;
            lockObject = true;
            var change = BuildChanges();
            jQuery
                .Ajax(new AjaxOptions
                      {
                          Url = $"/api/editorapi?documentId={documentId}&clientId={clientId}",
                          ContentType = "application/json",
                          Type = "POST",
                          Data = JSON.Stringify(change)
                      })
                .Done(delegate(object data, string str, jqXHR jqXHR)
                      {
#pragma warning disable 618
                          var state = JSON.Parse<DocumentState>(jqXHR.ResponseText);
                          ProcessNewState(state);

#pragma warning restore 618
                      })
                .Fail(() => CancelChanges(change))
                .Always(() => { lockObject = false; });
        }
    }
}