using System;
using System.Collections.Concurrent;
using Editor.Storage;

namespace Editor.Front.DocumentSessions
{
    public class DocumentSessionsRepository : IDocumentSessionsRepository
    {
        private readonly ConcurrentDictionary<Guid, IDocumentSession> documents = new ConcurrentDictionary<Guid, IDocumentSession>();
        private readonly IDocumentsRepository documentsRepository;
        private readonly Func<string, IDocumentSession> documentSessionFactory;

        public DocumentSessionsRepository(IDocumentsRepository documentsRepository,
                                          Func<string, IDocumentSession> documentSessionFactory)
        {
            this.documentsRepository = documentsRepository;
            this.documentSessionFactory = documentSessionFactory;
        }

        public IDocumentSession GetOrLoad(Guid documentId)
        {
            return documents.GetOrAdd(documentId, id => documentSessionFactory(documentsRepository.Get(id).Content));
        }

        public IDocumentSession Get(Guid documentId)
        {
            IDocumentSession documentSession;
            return documents.TryGetValue(documentId, out documentSession) ? documentSession : null;
        }
    }
}