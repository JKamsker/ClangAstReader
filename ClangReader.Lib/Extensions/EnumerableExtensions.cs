using System;
using System.Collections.Generic;

namespace ClangReader.Lib.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> TrySwallow<T>(this IEnumerable<T> input)
        {
            using var enumerator = input.GetEnumerator();
            while (TryMoveNext(enumerator))
            {
                yield return enumerator.Current;
            }
        }

        private static bool TryMoveNext<T>(IEnumerator<T> enumerator)
        {
            try
            {
                return enumerator.MoveNext();
            }
            catch (Exception e)
            {
                return TryMoveNext(enumerator);
            }
        }

        public static IEnumerable<T> TakeWhile<T>( IEnumerable<T> input, Func<T, bool> predicate)
        {
            foreach (var value in input)
            {
                if (!predicate(value))
                {
                    yield break;
                }

                yield return value;
            }
        }

        public static IEnumerable<List<T>> ChunkIn<T>(this IEnumerable<T> input, int items)
        {
            var list = new List<T>(items);
            foreach (var value in input)
            {
                if (list.Count >= items)
                {
                    yield return list;
                    list = new List<T>(items);
                }
            }

            if (list.Count != 0)
            {
                yield return list;
            }
        }
    }
}