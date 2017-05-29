using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Editor.Model;

namespace Editor.Front.DocumentSessions
{
    public class DocumentSession : IDocumentSession
    {
        private readonly IDateTimeService dateTimeService;
        private readonly ConcurrentDictionary<Guid, Author> clients = new ConcurrentDictionary<Guid, Author>();
        private readonly ReaderWriterLockSlim operationsLocker = new ReaderWriterLockSlim();
        private readonly List<Operation> operationHistory;

        public DocumentSession(IDateTimeService dateTimeService, string content = null)
        {
            this.dateTimeService = dateTimeService;
            operationHistory = new List<Operation>();
            if (!string.IsNullOrEmpty(content))
                operationHistory.AddRange(new EditorString().GenerateOperations(content));
        }

        public DocumentState GetState(int? revision)
        {
            try
            {
                operationsLocker.EnterReadLock();
                return BuildDocumentState(GetOperationsByRevision(revision));
            }
            finally
            {
                operationsLocker.ExitReadLock();
            }
        }

        public DocumentState ChangeState(int? revision, Operation[] newOperations)
        {
            try
            {
                operationsLocker.EnterWriteLock();

                var toSend = GetOperationsByRevision(revision);
                operationHistory.AddRange(newOperations);

                return BuildDocumentState(toSend);
            }
            finally
            {
                operationsLocker.ExitWriteLock();
            }
        }

        public void AddOrUpdateAuthor(Guid clientId, int position)
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
            try
            {
                operationsLocker.EnterReadLock();
                var editorString = new EditorString();
                editorString.ApplyOperations(operationHistory);
                saveAction(editorString.ToString());
            }
            finally
            {
                operationsLocker.ExitReadLock();
            }
        }

        private DocumentState BuildDocumentState(Operation[] toSend)
        {
            return new DocumentState
                   {
                       Authors = clients.Where(t => t.Value.Position >= 0 && (dateTimeService.UtcNow - t.Value.LastUpdate).TotalSeconds < 10)
                                        .Select(a => a.Value)
                                        .ToArray(),
                       Revision = operationHistory.Count,
                       Operations = toSend
                   };
        }

        private Operation[] GetOperationsByRevision(int? revision)
        {
            return operationHistory.Skip(revision ?? 0).ToArray();
        }
    }
}