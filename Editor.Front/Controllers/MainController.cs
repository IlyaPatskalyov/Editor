using System;
using System.Web.Mvc;
using Editor.Front.DocumentSessions;
using Editor.Front.Sessions;
using Editor.Storage;

namespace Editor.Front.Controllers
{
    public class MainController : Controller
    {
        private readonly IDocumentsRepository documentsRepository;
        private readonly IDocumentSessionsRepository documentSessionsRepository;

        public MainController(IDocumentsRepository documentsRepository, IDocumentSessionsRepository documentSessionsRepository)
        {
            this.documentsRepository = documentsRepository;
            this.documentSessionsRepository = documentSessionsRepository;
        }

        public ActionResult Index(Session session)
        {
            ViewBag.Documents = documentsRepository.GetByUserId(session.UserId);
            return View();
        }

        [HttpPost]
        public ActionResult Create(Session session)
        {
            var document = documentsRepository.Create(session.UserId);
            return Redirect($"Editor/{document.Id}");
        }

        public ActionResult Editor(Guid id)
        {
            var document = documentsRepository.Get(id);
            if (document == null)
                return Redirect("/");
            ViewBag.DocumentId = id;
            return View();
        }

        [HttpPost]
        public ActionResult SaveEditor(Guid id)
        {
            var documentSession = documentSessionsRepository.Get(id);
            documentSession?.Save(content => documentsRepository.UpdateContent(id, content));
            return Redirect("/");
        }
    }
}