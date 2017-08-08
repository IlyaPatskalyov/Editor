using System;
using System.Collections.Generic;
using System.Linq;

namespace Editor.Model
{
    public static class EnumerableExtensions
    {
        public static T[] FastToArray<T>(this IEnumerable<T> enumerable, int size)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            var result = new T[size];
            var i = 0;
            foreach (var item in enumerable)
                result[i++] = item;
            return result;
        }

        public static IEnumerable<T[]> Batch<T>(this IEnumerable<T> source, int size)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            T[] batch = null;
            var i = 0;
            foreach (var item in source)
            {
                if (batch == null)
                    batch = new T[size];

                batch[i++] = item;

                if (i != size)
                    continue;

                yield return batch;

                batch = null;
                i = 0;
            }

            if (batch != null && i > 0)
                yield return batch.Take(i).ToArray();
        }

        public static int Search<T>(this List<T> collection, T value, IComparer<T> comparer)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (collection.Count == 0)
                return 0;
            var t = collection.BinarySearch(value, comparer);
            return t < 0 ? ~t - 1 : t;
        }

        public static int Search<T>(this T[] collection, T value, IComparer<T> comparer)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (collection.Length == 0)
                return 0;
            var t = Array.BinarySearch(collection, value, comparer);
            return t < 0 ? ~t - 1 : t;
        }
    }
}