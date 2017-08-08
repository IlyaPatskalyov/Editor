using System;
using Editor.Front.WebSockets;

namespace Editor.Front.DocumentSessions
{
    public interface IDocumentSession
    {
        void Register(Guid clientId, IEditorSocket editorSocket);
        void Unregister(Guid clientId);

        DocumentState GetState(Guid? clientId, int? revision);

        void Change(Guid clientId, DocumenChange change);

        void Save(Action<string> saveAction);
    }
}