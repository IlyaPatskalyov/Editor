using System;
using System.Web.Http;
using Editor.Front.DocumentSessions;

namespace Editor.Front.Controllers
{
    public class EditorApiController : ApiController
    {
        private readonly IDocumentSessionsRepository documentSessionsRepository;

        public EditorApiController(IDocumentSessionsRepository documentSessionsRepository)
        {
            this.documentSessionsRepository = documentSessionsRepository;
        }

        [HttpPost]
        public object Post([FromUri] Guid documentId, [FromUri] Guid clientId, [FromBody] DocumenChange change)
        {
            var documentSession = documentSessionsRepository.GetOrLoad(documentId);
            documentSession.Change(clientId, change);
            return documentSession.GetState(clientId, change.Revision);
        }
    }
}