using System;
using Editor.Model;
using Newtonsoft.Json;
using NUnit.Framework;
using Char = Editor.Model.Char;

namespace Editor.Tests
{
    public class OperationSerializerTest
    {
        [Test]
        public void Test()
        {
            var clientId = Guid.NewGuid();
            var expected = new Operation(OperationType.Delete, new Char(new CharId(clientId, 0), "1", Char.Begin.Id, Char.End.Id));
            var actual = OperationSerializer.Deserialize(OperationSerializer.Serialize(expected));

            Console.WriteLine(clientId);
            Console.WriteLine(OperationSerializer.Serialize(expected));
            Assert.AreEqual(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
        }
    }
}