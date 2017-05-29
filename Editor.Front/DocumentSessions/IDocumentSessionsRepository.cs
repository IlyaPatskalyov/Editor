using System;

namespace Editor.Front.DocumentSessions
{
    public interface IDocumentSessionsRepository
    {
        IDocumentSession GetOrLoad(Guid documentId);

        IDocumentSession Get(Guid documentId);
    }
}