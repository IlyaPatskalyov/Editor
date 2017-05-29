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
            var editorString = new EditorString();
            var operations1 = editorString.GenerateOperations("abc");

            var state = documentSession.ChangeState(null, operations1);
            Assert.AreEqual(0, state.Operations.Length);
            Assert.AreEqual(3, state.Revision);

            var state2 = documentSession.GetState(null);
            Assert.AreEqual(3, state2.Operations.Length);
            Assert.AreEqual(3, state2.Revision);

            var operations2 = editorString.GenerateOperations("adebc");

            var state3 = documentSession.ChangeState(3, operations2);
            Assert.AreEqual(0, state3.Operations.Length);
            Assert.AreEqual(5, state3.Revision);

            var state4 = documentSession.GetState(4);
            Assert.AreEqual(1, state4.Operations.Length);
            Assert.AreEqual(5, state4.Revision);
        }

        [Test]
        public void TestAuthors()
        {
            mockDateTimeService.UtcNow = DateTime.UtcNow;

            var author1 = Guid.NewGuid();
            documentSession.AddOrUpdateAuthor(author1, 4);

            mockDateTimeService.AddSeconds(5);

            var author2 = Guid.NewGuid();
            documentSession.AddOrUpdateAuthor(author2, 0);
            
            var author3 = Guid.NewGuid();
            documentSession.AddOrUpdateAuthor(author3, -1);

            var state = documentSession.GetState(null);
            Assert.AreEqual(2, state.Authors.Length);
            Assert.AreEqual(4, state.Authors.First(a => a.ClientId == author1).Position);
            Assert.AreEqual(0, state.Authors.First(a => a.ClientId == author2).Position);
            
            mockDateTimeService.AddSeconds(5);
            var state2 = documentSession.GetState(null);
            Assert.AreEqual(1, state2.Authors.Length);
            
            mockDateTimeService.AddSeconds(5);
            var state3 = documentSession.GetState(null);
            Assert.AreEqual(0, state3.Authors.Length);
        }
    }
}