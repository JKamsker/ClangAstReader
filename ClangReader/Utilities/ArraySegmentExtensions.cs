using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using ClangReader.Models;
using ClangReader.Utilities;

namespace ClangReader
{
    public static class ArraySegmentExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<T> AsArraySegment<T>(this Memory<T> memory)
        {
            if (MemoryMarshal.TryGetArray(memory, out ArraySegment<T> as2))
            {
                return as2;
            }
            throw new ArgumentException("Cannot get ArraySegment", nameof(memory));
        }

        public static ReadOnlyArraySegment<T> AsReadOnlyArraySegment<T>(this Memory<T> memory)
        {
            return memory.AsArraySegment().AsReadOnly();
        }

        public static int IndexOf<T>(this ReadOnlyArraySegment<T> arraySegment, T value) where T : IEquatable<T>
        {
            return arraySegment.Span.IndexOf(value);
        }

        public static int IndexOf<T>(this ReadOnlyArraySegment<T> arraySegment, T value, int startIndex) where T : IEquatable<T>
        {
            var result = arraySegment.Span.Slice(startIndex).IndexOf(value);
            if (result == -1)
            {
                return result;
            }

            return result + startIndex;
        }

        public static int IndexOf<T>(this ReadOnlyArraySegment<T> arraySegment, ReadOnlySpan<T> value) where T : IEquatable<T>
        {
            return arraySegment.Span.IndexOf(value);
        }

        public static ReadOnlyArraySegment<T> AsReadOnly<T>(this ArraySegment<T> source) => new ReadOnlyArraySegment<T>(source);
    }
}