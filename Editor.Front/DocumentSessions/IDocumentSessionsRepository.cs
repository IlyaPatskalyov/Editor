using System;

namespace Editor.Front.DocumentSessions
{
    public interface IDocumentSessionsRepository
    {
        DocumentSession GetOrLoad(Guid documentId);

        DocumentSession Get(Guid documentId);
    }
}