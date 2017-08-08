using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Editor.Front.DocumentSessions;
using Newtonsoft.Json;
using Owin.WebSocket;
using Serilog;

namespace Editor.Front.WebSockets
{
    public class EditorWebSocket : WebSocketConnection, IEditorSocket
    {
        private readonly ILogger logger;
        private readonly IDocumentSessionsRepository documentSessionsRepository;
        private Guid clientId;
        private Guid documentId;
        private IDocumentSession documentSession;
        private int? revision;

        public EditorWebSocket(ILogger logger, IDocumentSessionsRepository documentSessionsRepository)
            : base(10 * 1024 * 1024)
        {
            this.logger = logger;
            this.documentSessionsRepository = documentSessionsRepository;
        }

        public override Task OnMessageReceived(ArraySegment<byte> message, WebSocketMessageType type)
        {
            try
            {
                var request = Encoding.UTF8.GetString(message.Array, message.Offset, message.Count);
                var change = JsonConvert.DeserializeObject<DocumenChange>(request);
                documentSession.Change(clientId, change);
            }
            catch (Exception e)
            {
                logger.Warning("Error on receive websocket message", e);
                throw;
            }
            return Task.Delay(0);
        }

        public override async Task OnOpenAsync()
        {
            try
            {
                clientId = Guid.Parse(Arguments["clientId"]);
                documentId = Guid.Parse(Arguments["documentId"]);
                documentSession = documentSessionsRepository.GetOrLoad(documentId);

                DocumentState state;
                do
                {
                    state = documentSession.GetState(clientId, revision);
                    await SendText(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(state)), true);
                    revision = state.Revision;
                } while (state.Operations.Length > 0);

                documentSession.Register(clientId, this);
            }
            catch (Exception e)
            {
                logger.Warning("Error on open websocket", e);
                throw;
            }
        }

        public override void OnClose(WebSocketCloseStatus? closeStatus, string closeStatusDescription)
        {
            try
            {
                base.OnClose(closeStatus, closeStatusDescription);
                documentSession.Unregister(clientId);
            }
            catch (Exception e)
            {
                logger.Warning("Error on close websocket", e);
                throw;
            }
        }

        public override void OnReceiveError(Exception error)
        {
            base.OnReceiveError(error);
            logger.Warning("Receive websocket error", error);
        }

        private async Task PutState()
        {
            var state = documentSession.GetState(clientId, revision);
            await SendText(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(state)), true);
            revision = state.Revision;
        }

        public void FireChangeState()
        {
            Task.Factory.StartNew(PutState);
        }
    }
}