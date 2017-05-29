using System.Collections.Generic;
using Editor.Model;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Editor.Tests
{
    public class SerializationTest
    {
        [Test]
        public void Test()
        {
            var str = new EditorString();
            var operations = new List<Operation>();
            operations.AddRange(str.GenerateOperations("abc"));
            operations.AddRange(str.GenerateOperations(""));
            operations.AddRange(str.GenerateOperations("def"));

            var deserializedOperations = JsonConvert.DeserializeObject<Operation[]>(JsonConvert.SerializeObject(operations));
            var str2 = new EditorString();
            str2.ApplyOperations(deserializedOperations);

            Assert.AreEqual(str.ToString(), str2.ToString());
        }
    }
}