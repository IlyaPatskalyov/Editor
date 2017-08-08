using System;
using System.Linq;
using Editor.Front.DocumentSessions;
using Editor.Model;
using NUnit.Framework;

namespace Editor.Tests
{
    public class DocumentSessionTest
    {
        private DocumentSession documentSession;
        private MockDateTimeService mockDateTimeService;

        [SetUp]
        protected void SetUp()
        {
            mockDateTimeService = new MockDateTimeService();
            documentSession = new DocumentSession(mockDateTimeService);
        }

        [Test]
        public void TestOperations()
        {
            var clientId = Guid.NewGuid();
            var clientId2 = Guid.NewGuid();
            var editorString = new EditorString(clientId);
            var operations1 = editorString.GenerateOperations("abc");

            
            documentSession.Change(clientId, new DocumenChange
                                             {
                                                 Operations = operations1
                                             });
            var state = documentSession.GetState(clientId, null);
            Assert.AreEqual(0, state.Operations.Length);
            Assert.AreEqual(3, state.Revision);

            var state2 = documentSession.GetState(clientId2, null);
            Assert.AreEqual(3, state2.Operations.Length);
            Assert.AreEqual(3, state2.Revision);

            var operations2 = editorString.GenerateOperations("adebc");

            documentSession.Change(clientId, new DocumenChange()
                                             {
                                                 Operations = operations2
                                             });
            var state3 = documentSession.GetState(clientId, null);
            Assert.AreEqual(0, state3.Operations.Length);
            Assert.AreEqual(5, state3.Revision);

            var state4 = documentSession.GetState(clientId2, 4);
            Assert.AreEqual(1, state4.Operations.Length);
            Assert.AreEqual(5, state4.Revision);
        }

        [Test]
        public void TestAuthors()
        {
            var clientId = Guid.NewGuid();
            mockDateTimeService.UtcNow = DateTime.UtcNow;

            var author1 = Guid.NewGuid();
            documentSession.Change(author1, new DocumenChange()
                                            {
                                                Position = 4,
                                                Operations = new string[0]
                                            });
            mockDateTimeService.AddSeconds(5);

            var author2 = Guid.NewGuid();
            documentSession.Change(author2, new DocumenChange()
                                            {
                                                Position = 0,
                                                Operations = new string[0]
                                            });

            var author3 = Guid.NewGuid();
            documentSession.Change(author3, new DocumenChange()
                                            {
                                                Position = -1,
                                                Operations = new string[0]
                                            });

            var state = documentSession.GetState(clientId, null);
            Assert.AreEqual(2, state.Authors.Length);
            Assert.AreEqual(4, state.Authors.First(a => a.ClientId == author1).Position);
            Assert.AreEqual(0, state.Authors.First(a => a.ClientId == author2).Position);

            mockDateTimeService.AddSeconds(5);
            var state2 = documentSession.GetState(clientId, null);
            Assert.AreEqual(1, state2.Authors.Length);

            mockDateTimeService.AddSeconds(5);
            var state3 = documentSession.GetState(clientId, null);
            Assert.AreEqual(0, state3.Authors.Length);
        }
    }
}