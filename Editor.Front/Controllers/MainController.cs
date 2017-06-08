using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Editor.Front.DocumentSessions;
using Editor.Front.Results;
using Editor.Front.Sessions;
using Editor.Storage;

namespace Editor.Front.Controllers
{
    public class MainController : ApiController
    {
        private readonly IDocumentsRepository documentsRepository;
        private readonly IDocumentSessionsRepository documentSessionsRepository;

        public MainController(IDocumentsRepository documentsRepository, IDocumentSessionsRepository documentSessionsRepository)
        {
            this.documentsRepository = documentsRepository;
            this.documentSessionsRepository = documentSessionsRepository;
        }

        [HttpGet]
        [Route("~/")]
        public HttpResponseMessage Index([ModelBinder] Session session)
        {
            return new RazorViewResult(session, "Main.Index.cshtml", documentsRepository.GetByUserId(session.UserId));
        }

        [HttpPost]
        [Route("~/Create")]
        public HttpResponseMessage Create([ModelBinder] Session session)
        {
            var document = documentsRepository.Create(session.UserId);
            return new RedirectResult($"/Editor/{document.Id}");
        }

        [HttpGet]
        [Route("~/Editor/{id}")]
        public HttpResponseMessage Editor([ModelBinder] Session session, Guid id)
        {
            var document = documentsRepository.Get(id);
            if (document == null)
                return new RedirectResult("/");

            return new RazorViewResult(session, "Main.Editor.cshtml", id);
        }

        [HttpPost]
        [Route("~/Editor/{id}/Save")]
        public HttpResponseMessage SaveEditor(Guid id)
        {
            var documentSession = documentSessionsRepository.Get(id);
            documentSession?.Save(content => documentsRepository.UpdateContent(id, content));
            return new RedirectResult("/");
        }
    }
}