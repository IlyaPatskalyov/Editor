using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Model;
using NUnit.Framework;

namespace Editor.Tests
{
    public class EnumerableExtensionsTest
    {
        [Test]
        public void TestFastToArray()
        {
            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.FastToArray<object>(null, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerableExtensions.FastToArray(new object[0], -1));


            var list = new List<int> {5, 1, 0};
            CollectionAssert.AreEquivalent(list, list.FastToArray(list.Count));

            list = new List<int>();
            CollectionAssert.AreEquivalent(list, list.FastToArray(list.Count));
        }


        [Test]
        public void TestBatch()
        {
            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.Batch<object>(null, 1).ToArray());
            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerableExtensions.Batch(new object[0], -1).ToArray());


            var list = new List<int> {1, 2, 3, 4, 5, 6, 7};
            CollectionAssert.AreEquivalent(new[]
                                           {
                                               new[] {1, 2, 3},
                                               new[] {4, 5, 6},
                                               new[] {7}
                                           }, list.Batch(3));
        }


        [Test]
        public void TestSearchForList()
        {
            var comparer = Comparer<int>.Default;
            Assert.Throws<ArgumentNullException>(() => ((List<object>) null).Search(null, Comparer<object>.Default));
            Assert.AreEqual(0, new List<int> {0}.Search(0, comparer));
            Assert.AreEqual(0, new List<int> { }.Search(0, comparer));
            Assert.AreEqual(1, new List<int> {0, 1, 4}.Search(2, comparer));
            Assert.AreEqual(1, new List<int> {0, 1, 4}.Search(1, comparer));
            Assert.AreEqual(2, new List<int> {0, 1, 2}.Search(2, comparer));
        }

        [Test]
        public void TestSearchForArray()
        {
            var comparer = Comparer<int>.Default;
            Assert.Throws<ArgumentNullException>(() => ((object[]) null).Search(null, Comparer<object>.Default));
            Assert.AreEqual(0, new[] {0}.Search(0, comparer));
            Assert.AreEqual(0, new int[0].Search(0, comparer));
            Assert.AreEqual(1, new[] {0, 1, 4}.Search(2, comparer));
            Assert.AreEqual(1, new[] {0, 1, 4}.Search(1, comparer));
            Assert.AreEqual(2, new[] {0, 1, 2}.Search(2, comparer));
        }
    }
}