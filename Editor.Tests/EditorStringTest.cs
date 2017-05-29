using System.Collections.Generic;
using Editor.Model;
using NUnit.Framework;

namespace Editor.Tests
{
    public class EditorStringTests
    {
        [Test]
        public void TestSimple()
        {
            var str = new EditorString();
            var remote = new EditorString();

            CheckChange(str, remote, "abcdef");
            CheckChange(str, remote, "abc123def");
            CheckChange(str, remote, "abcdef");
        }

        [Test]
        public void TestReplace()
        {
            var str = new EditorString();
            var remote = new EditorString();
            CheckChange(str, remote, "abc");
            CheckChange(str, remote, "123");
        }

        [Test]
        public void TestMuptipleApply()
        {
            var str = new EditorString();
            var operations = str.GenerateOperations("abc");
            str.ApplyOperations(operations);
            Assert.AreEqual("aabbcc", str.ToString());
        }

        [Test]
        public void TestConcurrent()
        {
            var str = new EditorString();
            var gen1 = str.GenerateOperations("abc");
            var gen2a = str.GenerateOperations("adec");

            var str2 = new EditorString();
            str2.ApplyOperations(gen1);
            var gen2b = str2.GenerateOperations("aghc");

            str.ApplyOperations(gen2b);
            str2.ApplyOperations(gen2a);
            Assert.AreEqual("adgehbc", str.ToString());
            Assert.AreEqual("agdhebc", str2.ToString());
        }


        [Test]
        public void TestCombine()
        {
            var str = new EditorString();
            var operations = new List<Operation>();
            operations.AddRange(str.GenerateOperations("abc"));
            operations.AddRange(str.GenerateOperations(""));
            operations.AddRange(str.GenerateOperations("def"));

            var str2 = new EditorString();
            str2.ApplyOperations(operations.ToArray());
            Assert.AreEqual("def", str2.ToString());
        }

        [Test]
        public void TestOneSymbol()
        {
            var str = new EditorString();
            var remote = new EditorString();
            CheckChange(str, remote, "q");
            CheckChange(str, remote, "qq");
            CheckChange(str, remote, "qqq");
            CheckChange(str, remote, "qq");
            CheckChange(str, remote, "q");
            CheckChange(str, remote, "");
            CheckChange(str, remote, "1");
        }

        private static void CheckChange(EditorString str, EditorString remote, string change)
        {
            var operations = str.GenerateOperations(change);
            remote.ApplyOperations(operations);
            Assert.AreEqual(change, str.ToString());
            Assert.AreEqual(change, remote.ToString());
        }
    }
}