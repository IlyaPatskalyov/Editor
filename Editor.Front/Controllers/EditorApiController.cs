using System;
using System.Web.Http;
using Editor.Front.DocumentSessions;
using Editor.Model;

namespace Editor.Front.Controllers
{
    public class EditorApiController : ApiController
    {
        private readonly IDocumentSessionsRepository documentSessionsRepository;

        public EditorApiController(IDocumentSessionsRepository documentSessionsRepository)
        {
            this.documentSessionsRepository = documentSessionsRepository;
        }

        [HttpGet]
        public object Get(Guid documentId, int? revision = null)
        {
            return documentSessionsRepository.GetOrLoad(documentId).GetState(revision);
        }

        [HttpPost]
        public object Post([FromUri] Guid documentId, [FromUri] Guid clientId, [FromBody] Operation[] operations,
                           int? revision = null, [FromUri] int position = 0)
        {
            var documentSession = documentSessionsRepository.GetOrLoad(documentId);
            documentSession.AddOrUpdateAuthor(clientId, position);

            if (operations != null && operations.Length > 0)
                return documentSession.ChangeState(revision, operations);
            return
                documentSession.GetState(revision);
        }
    }
}