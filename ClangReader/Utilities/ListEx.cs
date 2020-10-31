using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ClangReader.Utilities
{

    public class ListEx<T> : List<T>
    {
        private static Func<List<T>, T[]> BufferRetreiver { get; set; }

        static ListEx()
        {
            var field = typeof(List<T>)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(x => x.FieldType.IsArray);

            var param = Expression.Parameter(typeof(List<T>), "instance");

            var exp = Expression.Field(param, field);

            var lambda = Expression.Lambda<Func<List<T>, T[]>>(exp, param);

            BufferRetreiver = lambda.Compile();
        }

        public ListEx()
        {
        }

        public ListEx(int capacity) : base(capacity)
        {

        }

        public ListEx(IEnumerable<T> collection) : base(collection)
        {

        }

        public ReadOnlyCollectionEx<T> AsReadonlyEx() => new ReadOnlyCollectionEx<T>(this, GetUnderlyingBuffer);

        public T[] GetUnderlyingBuffer() => BufferRetreiver(this);

        public Span<T> Span => GetUnderlyingBuffer().AsSpan(0, base.Count);
        public Memory<T> Memory => GetUnderlyingBuffer().AsMemory(0, base.Count);
    }

    public class ReadOnlyCollectionEx<T> : ReadOnlyCollection<T>
    {
        private readonly Func<T[]> _bufferRetreiver;

        public ReadOnlyCollectionEx(ListEx<T> list, Func<T[]> bufferRetreiver) : base(list)
        {
            _bufferRetreiver = bufferRetreiver;
        }

        public ReadOnlySpan<T> Span => _bufferRetreiver().AsSpan(0, base.Count);
        public ReadOnlyMemory<T> Memory => _bufferRetreiver().AsMemory(0, base.Count);
    }
}