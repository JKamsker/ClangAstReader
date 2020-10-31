using System;
using System.Reflection;

using ClangReader.Utilities;

namespace ClangReader
{
    public static class ArraySegmentExtensions
    {
        private class MStorage<T>
        {
            public static Lazy<Func<Memory<T>, object>> MemoryObjectGetter = new Lazy<Func<Memory<T>, object>>(() =>
            {
                var field = typeof(Memory<T>)
                    .GetField("_object", BindingFlags.Instance | BindingFlags.NonPublic);

                return FastReflection.CreateFieldGetter<Memory<T>, object>(field);
            });

            public static Lazy<Func<Memory<T>, int>> MemoryIndexGetter = new Lazy<Func<Memory<T>, int>>(() =>
            {
                var field = typeof(Memory<T>)
                    .GetField("_index", BindingFlags.Instance | BindingFlags.NonPublic);

                return FastReflection.CreateFieldGetter<Memory<T>, int>(field);
            });

            public static Lazy<Func<Memory<T>, int>> MemoryLengthGetter = new Lazy<Func<Memory<T>, int>>(() =>
            {
                var field = typeof(Memory<T>)
                    .GetField("_length", BindingFlags.Instance | BindingFlags.NonPublic);

                return FastReflection.CreateFieldGetter<Memory<T>, int>(field);
            });
        }

        public static ArraySegment<T> AsArraySegment<T>(this Memory<T> memory)
        {
            var memobject = MStorage<T>.MemoryObjectGetter.Value(memory);
            var memIndex = MStorage<T>.MemoryIndexGetter.Value(memory);
            var memLength = MStorage<T>.MemoryLengthGetter.Value(memory);

            return new ArraySegment<T>((T[])memobject, memIndex, memLength);
        }
    }
}