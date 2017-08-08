using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Editor.Front.DocumentSessions;
using Editor.Front.Models;
using Editor.Front.Results;
using Editor.Front.Sessions;
using Editor.Storage;
using RazorEngine.Templating;

namespace Editor.Front.Controllers
{
    public class MainController : ApiController
    {
        private readonly IDocumentSessionsRepository documentSessionsRepository;
        private readonly IRazorEngineService razorEngineService;
        private readonly IDocumentsRepository documentsRepository;

        public MainController(IDocumentsRepository documentsRepository,
                              IDocumentSessionsRepository documentSessionsRepository,
                              IRazorEngineService razorEngineService)
        {
            this.documentsRepository = documentsRepository;
            this.documentSessionsRepository = documentSessionsRepository;
            this.razorEngineService = razorEngineService;
        }

        [HttpGet]
        [Route("~/")]
        public HttpResponseMessage Index([ModelBinder] Session session)
        {
            var model = new IndexModel
                        {
                            Documents = documentsRepository.GetByUserId(session.UserId),
                            UserId = session.UserId
                        };
            return new RazorViewResult(razorEngineService, session, "Main.Index.cshtml", model);
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

            var model = new EditorModel
                        {
                            UserId = session.UserId,
                            DocumentId = id
                        };
            return new RazorViewResult(razorEngineService, session, "Main.Editor.cshtml", model);
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