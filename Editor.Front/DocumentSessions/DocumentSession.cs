using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Editor.Front.WebSockets;
using Editor.Model;

namespace Editor.Front.DocumentSessions
{
    public class DocumentSession : IDocumentSession
    {
        private readonly ConcurrentDictionary<Guid, IEditorSocket> sockets = new ConcurrentDictionary<Guid, IEditorSocket>();
        private readonly ConcurrentDictionary<Guid, Author> clients = new ConcurrentDictionary<Guid, Author>();
        private readonly IDateTimeService dateTimeService;
        private readonly List<string> operationHistory;
        private readonly ReaderWriterLockSlim operationsLocker = new ReaderWriterLockSlim();

        public DocumentSession(IDateTimeService dateTimeService, string content = null)
        {
            this.dateTimeService = dateTimeService;
            operationHistory = new List<string>();
            if (!string.IsNullOrEmpty(content))
                operationHistory.AddRange(new EditorString().GenerateOperations(content));
        }

        public void Register(Guid clientId, IEditorSocket editorSocket)
        {
            sockets[clientId] = editorSocket;
        }

        public void Unregister(Guid clientId)
        {
            IEditorSocket editorSocket;
            sockets.TryRemove(clientId, out editorSocket);
        }

        public DocumentState GetState(Guid? clientId, int? revision)
        {
            operationsLocker.EnterReadLock();
            try
            {
                return BuildDocumentState(clientId, revision);
            }
            finally
            {
                operationsLocker.ExitReadLock();
            }
        }

        private void AddOperations(string[] newOperations)
        {
            if (newOperations != null && newOperations.Length > 0)
            {
                operationsLocker.EnterWriteLock();
                try
                {
                    operationHistory.AddRange(newOperations);
                }
                finally
                {
                    operationsLocker.ExitWriteLock();
                }
            }
        }

        public void Change(Guid clientId, DocumenChange change)
        {
            AddOrUpdateAuthor(clientId, change.Position);
            AddOperations(change.Operations.ToArray());
            foreach (var socket in sockets.Values)
                socket.FireChangeState();
        }

        private void AddOrUpdateAuthor(Guid clientId, int position)
        {
            clients[clientId] = new Author
                                {
                                    ClientId = clientId,
                                    LastUpdate = dateTimeService.UtcNow,
                                    Position = position
                                };
        }

        public void Save(Action<string> saveAction)
        {
            operationsLocker.EnterReadLock();
            try
            {
                var editorString = new EditorString();
                editorString.ApplyOperations(operationHistory);
                saveAction(editorString.ToString());
            }
            finally
            {
                operationsLocker.ExitReadLock();
            }
        }

        private DocumentState BuildDocumentState(Guid? clientId, int? revision)
        {
            var skip = revision ?? 0;
            var take = Math.Min(operationHistory.Count - skip, 10000);

            var operations = operationHistory.Skip(skip).Take(take);

            if (clientId.HasValue)
                operations = operations.Where(o => !EditorStringHelpers.AreEqualClientId(o, clientId.Value));

            return new DocumentState
                   {
                       Authors = clients.Where(t => t.Value.Position >= 0 && (dateTimeService.UtcNow - t.Value.LastUpdate).TotalSeconds < 10)
                                        .Select(a => a.Value)
                                        .ToArray(),
                       Revision = skip + take,
                       Operations = operations.ToArray()
                   };
        }
    }
}