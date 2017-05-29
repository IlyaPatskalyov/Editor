using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Editor.Model;

namespace Editor.Front.DocumentSessions
{
    public class DocumentSession
    {
        private ReaderWriterLockSlim operationsLocker = new ReaderWriterLockSlim();

        private readonly IEditorString editorString;
        private List<Operation> operationHistory;

        private ConcurrentDictionary<Guid, Author> clients;

        public DocumentSession(string content)
        {
            editorString = new EditorString();
            operationHistory = new List<Operation>();
            operationHistory.AddRange(editorString.GenerateOperations(content ?? ""));
            clients = new ConcurrentDictionary<Guid, Author>();
        }


        public DocumentState GetState(int? revision)
        {
            try
            {
                operationsLocker.EnterReadLock();
                return new DocumentState
                       {
                           Authors = AliveAuthors,
                           Revision = operationHistory.Count,
                           Operations = GetOperationsByRevision(revision)
                       };
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
                editorString.ApplyOperations(newOperations);
                operationHistory.AddRange(newOperations);

                return new DocumentState
                       {
                           Authors = AliveAuthors,
                           Revision = operationHistory.Count,
                           Operations = toSend
                       };
            }
            finally
            {
                operationsLocker.ExitWriteLock();
            }
        }

        public void Save(Action<string> saveAction)
        {
            try
            {
                operationsLocker.EnterReadLock();
                saveAction(editorString.ToString());
            }
            finally
            {
                operationsLocker.ExitReadLock();
            }
        }


        public void AddOrUpdateAuthor(Guid clientId, int position)
        {
            clients[clientId] = new Author
                                {
                                    ClientId = clientId,
                                    LastUpdate = DateTime.UtcNow,
                                    Position = position
                                };
        }

        private Operation[] GetOperationsByRevision(int? revision)
        {
            return operationHistory.Skip(revision ?? 0).ToArray();
        }

        public Author[] AliveAuthors => clients.Where(t => t.Value.Position >= 0 && (DateTime.UtcNow - t.Value.LastUpdate).TotalSeconds < 10)
                                               .Select(a => a.Value)
                                               .ToArray();
    }
}