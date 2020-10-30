using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClangReader
{
    public static class Extensions
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

        public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> input, Func<T, bool> predicate)
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
    }
}