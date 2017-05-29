using System;

namespace Editor.Storage
{
    public interface IDocumentsRepository
    {
        Document Create(Guid userId);

        Document Get(Guid id);

        Document[] GetByUserId(Guid userId);

        void Delete(Guid id, Guid userId);

        void UpdateContent(Guid id, string content);
    }
}