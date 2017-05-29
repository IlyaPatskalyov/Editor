using System;
using Editor.Model;

namespace Editor.Front.DocumentSessions
{
    public interface IDocumentSession
    {
        DocumentState GetState(int? revision);
        DocumentState ChangeState(int? revision, Operation[] newOperations);
        void AddOrUpdateAuthor(Guid clientId, int position);
        void Save(Action<string> saveAction);
    }
}