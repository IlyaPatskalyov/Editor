using System;
using Bridge.Html5;
using Editor.Front.DocumentSessions;

namespace Editor.Client.Connectors
{
    public class WebSocketConnector : ConnectorBase
    {
        private readonly Guid clientId;
        private readonly string documentId;
        private int oldPosition = -1;
        private WebSocket ws;

        public WebSocketConnector(string documentId, Guid clientId, Action<DocumentState> onUpdateState) : base(onUpdateState)
        {
            this.documentId = documentId;
            this.clientId = clientId;
        }

        public override void Start()
        {
            var location = Window.Location;
            var address = $"{(location.Protocol == "https:" ? "wss:" : "ws:")}//{location.Host}/ws/editor/{documentId}/{clientId}";
            ws = new WebSocket(address);
            ws.OnMessage = e =>
                           {
                               var state = JSON.Parse<DocumentState>((string) e.Data);
                               ProcessNewState(state);
                           };
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
            ws.Close();
        }

        protected override void Ping()
        {
            var changes = BuildChanges();
            if (changes.Operations.Length > 0 || oldPosition != changes.Position)
            {
                oldPosition = changes.Position;
                ws.Send(JSON.Stringify(changes));
            }
        }
    }
}