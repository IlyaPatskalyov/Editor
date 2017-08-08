using System;
using System.Collections.Generic;
using System.Linq;
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
            Assert.AreEqual("aabcbc", str.ToString());
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
            Assert.AreEqual("aghdebc", str.ToString());
            Assert.AreEqual("adeghbc", str2.ToString());
        }

        [Test]
        public void TestConcurrent2()
        {
            var str = new EditorString();
            var str2 = new EditorString();
            var str3 = new EditorString();

            var gen1 = str.GenerateOperations("12");
            str2.ApplyOperations(gen1);

            var gen2 = str2.GenerateOperations("1a2");
            str.ApplyOperations(gen2);

            str3.ApplyOperations(gen1.Concat(gen2).ToArray());

            Assert.AreEqual(str.ToString(), str2.ToString());
            Assert.AreEqual(str.ToString(), str3.ToString());
        }


        [Test]
        public void TestCombine()
        {
            var str = new EditorString();
            var operations = new List<string>();
            operations.AddRange(str.GenerateOperations("abc"));
            operations.AddRange(str.GenerateOperations(""));
            operations.AddRange(str.GenerateOperations("def"));

            var str2 = new EditorString();
            str2.ApplyOperations(operations.ToArray());
            Assert.AreEqual("def", str2.ToString());
        }

        [Test]
        public void TestSplitApply()
        {
            var str = new EditorString();
            var operations = new List<string>();
            operations.AddRange(str.GenerateOperations("1234567890"));

            var str2 = new EditorString();
            str2.ApplyOperations(operations.GetRange(0, operations.Count / 2));
            str2.ApplyOperations(operations.GetRange(operations.Count / 2, operations.Count / 2));
            Assert.AreEqual(str.ToString(), str2.ToString());
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

        [Test]
        public void TestPerfomance()
        {
            var str = new EditorString();
            var bigstring = new string('a', 2 * 1024 * 1024);
            Console.WriteLine(bigstring.Length);
            str.GenerateOperations(bigstring);

            bigstring = bigstring.Insert(bigstring.Length / 2, new string('b', bigstring.Length));
            Console.WriteLine(bigstring.Length);
            str.GenerateOperations(bigstring);

            bigstring = bigstring.Remove(bigstring.Length / 4, bigstring.Length / 2);
            Console.WriteLine(bigstring.Length);
            str.GenerateOperations(bigstring);

            bigstring = string.Concat(Enumerable.Repeat("ab", bigstring.Length / 2));
            Console.WriteLine(bigstring.Length);
            str.GenerateOperations(bigstring);

            Console.WriteLine(bigstring.Length);
            Assert.AreEqual(str.ToString(), bigstring);
        }
    }
}