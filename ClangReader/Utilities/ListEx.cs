﻿using System;
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
        static ListEx()
        {
        }

        private static FieldInfo GetField(string name)
        {
            return typeof(List<T>).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public int Version
        {
            get => MethodStorage.VersionGetter(this);
            set => MethodStorage.VersionSetter(this, value);
        }

        public int Count
        {
            get => base.Count;
            set
            {
                MethodStorage.SizeSetter(this, value);
                Version++;
            }
        }

        public Span<T> Span => GetUnderlyingBuffer().AsSpan(0, base.Count);
        public Memory<T> Memory => GetUnderlyingBuffer().AsMemory(0, base.Count);

        public ListEx()
        {
        }

        public ListEx(int capacity) : base(capacity)
        {
        }

        public ListEx(IEnumerable<T> collection) : base(collection)
        {
        }

        public void AddRange(ReadOnlySpan<T> input)
        {
            var _size = base.Count;

            var count = input.Length;
            var index = input.Length;

            if (count <= 0)
            {
                return;
            }

            EnsureCapacity(_size + count);
            var _items = GetUnderlyingBuffer();
            //var index = Index;

            if (index < _size)
            {
                Array.Copy(_items, index, _items, index + count, _size - index);
            }

            input.CopyTo(_items.AsSpan(_size));

            Count = _size + count;
            Version++;
        }

        public ReadOnlyCollectionEx<T> AsReadonlyEx() => new ReadOnlyCollectionEx<T>(this, GetUnderlyingBuffer);

        public static implicit operator ReadOnlyCollectionEx<T>(ListEx<T> listEx) => listEx.AsReadonlyEx();

        private T[] GetUnderlyingBuffer() => MethodStorage.BufferRetreiver(this);

        private void EnsureCapacity(int min) => MethodStorage.CapacitySetter(this, min);

        private static class MethodStorage
        {
            public static readonly Func<List<T>, T[]> BufferRetreiver;
            public static readonly Action<List<T>, int> CapacitySetter;

            public static readonly Action<List<T>, int> SizeSetter;
            public static readonly Action<List<T>, int> VersionSetter;
            public static readonly Func<List<T>, int> VersionGetter;

            static MethodStorage()
            {
                BufferRetreiver = GenerateBufferRetreiver();
                CapacitySetter = GetCapacitySetter();

                SizeSetter = FastReflection.CreateFieldSetter<List<T>, int>(GetField("_size"));

                VersionSetter = FastReflection.CreateFieldSetter<List<T>, int>(GetField("_version"));
                VersionGetter = FastReflection.CreateFieldGetter<List<T>, int>(GetField("_version"));
            }

            private static Action<List<T>, int> GetCapacitySetter()
            {
                return (Action<List<T>, int>)typeof(List<T>)
                    .GetMethod("EnsureCapacity", BindingFlags.Instance | BindingFlags.NonPublic)
                    .CreateDelegate(typeof(Action<List<T>, int>));
            }

            private static Func<List<T>, T[]> GenerateBufferRetreiver()
            {
                var field = typeof(List<T>)
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Single(x => x.FieldType.IsArray);

                var param = Expression.Parameter(typeof(List<T>), "instance");
                var exp = Expression.Field(param, field);
                var lambda = Expression.Lambda<Func<List<T>, T[]>>(exp, param);
                return lambda.Compile(false);
            }
        }
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

        public static implicit operator ReadOnlyCollectionEx<T>(List<T> listEx) => new ListEx<T>(listEx).AsReadonlyEx();
    }
}