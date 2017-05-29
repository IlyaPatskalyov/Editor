using System;
using System.Collections.Concurrent;
using Editor.Storage;

namespace Editor.Front.DocumentSessions
{
    public class DocumentSessionsRepository : IDocumentSessionsRepository
    {
        private readonly IDocumentsRepository documentsRepository;
        private static readonly ConcurrentDictionary<Guid, DocumentSession> documents = new ConcurrentDictionary<Guid, DocumentSession>();

        public DocumentSessionsRepository(IDocumentsRepository documentsRepository)
        {
            this.documentsRepository = documentsRepository;
        }

        public DocumentSession GetOrLoad(Guid documentId)
        {
            return documents.GetOrAdd(documentId, id => new DocumentSession(documentsRepository.Get(id).Content));
        }

        public DocumentSession Get(Guid documentId)
        {
            DocumentSession documentSession;
            return documents.TryGetValue(documentId, out documentSession) ? documentSession : null;
        }
    }
}