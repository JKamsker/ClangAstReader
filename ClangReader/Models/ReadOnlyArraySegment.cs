﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;

namespace ClangReader.Models
{

    public static class ReadOnlyArraySegmentExtensions
    {
        public static unsafe ReadOnlyArraySegment<char> AsReadOnlyArraySegment(this string input)
        {

            //input.ToCharArray();

            var data = input.GetPinnableReference();
            fixed (char* p = &input.GetPinnableReference())
            {
                for (int i = 0; i < input.Length; i++)
                {
                    Console.WriteLine(*(p + i));

                }
            }

            return default;

        }
    }
    public readonly struct ReadOnlyArraySegment<T> : IReadOnlyList<T> // IList<T>,
    {
        // Do not replace the array allocation with Array.Empty. We don't want to have the overhead of
        // instantiating another generic type in addition to ReadOnlyArraySegment<T> for new type parameters.
        public static ReadOnlyArraySegment<T> Empty { get; } = new ReadOnlyArraySegment<T>(new T[0]);

        private readonly T[]? _array; // Do not rename (binary serialization)
        private readonly int _offset; // Do not rename (binary serialization)
        private readonly int _count; // Do not rename (binary serialization)

        public ReadOnlyArraySegment(T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            _array = array;
            _offset = 0;
            _count = array.Length;
        }

        public ReadOnlyArraySegment(T[] array, int offset, int count)
        {
            // Validate arguments, check is minimal instructions with reduced branching for inlinable fast-path
            // Negative values discovered though conversion to high values when converted to unsigned
            // Failure should be rare and location determination and message is delegated to failure functions
            if (array == null || (uint)offset > (uint)array.Length || (uint)count > (uint)(array.Length - offset))
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (offset < 0)
                    throw new ArgumentOutOfRangeException(nameof(offset), "ArgumentOutOfRange_NeedNonNegNum");
                if (count < 0)
                    throw new ArgumentOutOfRangeException(nameof(count), "ArgumentOutOfRange_NeedNonNegNum");

                //ThrowHelper.ThrowArraySegmentCtorValidationFailedExceptions(array, offset, count);
            }

            _array = array;
            _offset = offset;
            _count = count;
        }

        public T[]? Array => _array;

        public int Offset => _offset;

        public int Count => _count;

        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_count)
                {
                    throw new ArgumentOutOfRangeException("ArgumentOutOfRange_Index");
                }

                return _array![_offset + index];
            }
            //private set
            //{
            //    if ((uint)index >= (uint)_count)
            //    {
            //        throw new ArgumentOutOfRangeException("ArgumentOutOfRange_Index");
            //    }

            //    _array![_offset + index] = value;
            //}
        }

        public Enumerator GetEnumerator()
        {
            ThrowInvalidOperationIfDefault();
            return new Enumerator(this);
        }

        public override int GetHashCode()
        {
            if (_array == null)
            {
                return 0;
            }

            int hash = HashCode.Combine(5381, _offset, _count);

            // The array hash is expected to be an evenly-distributed mixture of bits,
            // so rather than adding the cost of another rotation we just xor it.
            hash ^= _array.GetHashCode();
            return hash;
        }

        public void CopyTo(T[] destination) => CopyTo(destination, 0);

        public void CopyTo(T[] destination, int destinationIndex)
        {
            ThrowInvalidOperationIfDefault();
            System.Array.Copy(_array!, _offset, destination, destinationIndex, _count);
        }

        public void CopyTo(ReadOnlyArraySegment<T> destination)
        {
            ThrowInvalidOperationIfDefault();
            destination.ThrowInvalidOperationIfDefault();

            if (_count > destination._count)
            {
                throw new ArgumentException("Argument_DestinationTooShort", "destination");
            }

            System.Array.Copy(_array!, _offset, destination._array!, destination._offset, _count);
        }

        public override bool Equals(object? obj)
        {
            if (obj is ReadOnlyArraySegment<T>)
                return Equals((ReadOnlyArraySegment<T>)obj);
            else
                return false;
        }

        public bool Equals(ReadOnlyArraySegment<T> obj)
        {
            return obj._array == _array && obj._offset == _offset && obj._count == _count;
        }

        public ReadOnlyArraySegment<T> Slice(int index)
        {
            ThrowInvalidOperationIfDefault();

            if ((uint)index > (uint)_count)
            {
                throw new ArgumentOutOfRangeException("ArgumentOutOfRange_Index");
            }

            return new ReadOnlyArraySegment<T>(_array!, _offset + index, _count - index);
        }

        public ReadOnlyArraySegment<T> Slice(int index, int count)
        {
            ThrowInvalidOperationIfDefault();

            if ((uint)index > (uint)_count || (uint)count > (uint)(_count - index))
            {
                throw new ArgumentOutOfRangeException("ArgumentOutOfRange_Index");
            }

            return new ReadOnlyArraySegment<T>(_array!, _offset + index, count);
        }

        public T[] ToArray()
        {
            ThrowInvalidOperationIfDefault();

            if (_count == 0)
            {
                return Empty._array!;
            }

            var array = new T[_count];
            System.Array.Copy(_array!, _offset, array, 0, _count);
            return array;
        }

        public static bool operator ==(ReadOnlyArraySegment<T> a, ReadOnlyArraySegment<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ReadOnlyArraySegment<T> a, ReadOnlyArraySegment<T> b)
        {
            return !(a == b);
        }

        public static implicit operator ReadOnlyArraySegment<T>(T[] array) => array != null ? new ReadOnlyArraySegment<T>(array) : default;

        #region IList<T>

        //T IList<T>.this[int index]
        //{
        //    get
        //    {
        //        ThrowInvalidOperationIfDefault();
        //        if (index < 0 || index >= _count)
        //            throw new ArgumentOutOfRangeException("ArgumentOutOfRange_Index");

        //        return _array![_offset + index];
        //    }

        //    set
        //    {
        //        ThrowInvalidOperationIfDefault();
        //        if (index < 0 || index >= _count)
        //            throw new ArgumentOutOfRangeException("ArgumentOutOfRange_Index");

        //        _array![_offset + index] = value;
        //    }
        //}

        //int IList<T>.IndexOf(T item)
        //{
        //    ThrowInvalidOperationIfDefault();

        //    int index = System.Array.IndexOf<T>(_array!, item, _offset, _count);

        //    Debug.Assert(index == -1 ||
        //                    (index >= _offset && index < _offset + _count));

        //    return index >= 0 ? index - _offset : -1;
        //}

        //void IList<T>.Insert(int index, T item)
        //{
        //    throw new NotSupportedException();
        //}

        //void IList<T>.RemoveAt(int index)
        //{
        //    throw new NotSupportedException();
        //}

        #endregion IList<T>

        #region IReadOnlyList<T>

        T IReadOnlyList<T>.this[int index]
        {
            get
            {
                ThrowInvalidOperationIfDefault();
                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException("ArgumentOutOfRange_Index");

                return _array![_offset + index];
            }
        }

        #endregion IReadOnlyList<T>

        #region ICollection<T>

        //bool ICollection<T>.IsReadOnly
        //{
        //    get
        //    {
        //        // the indexer setter does not throw an exception although IsReadOnly is true.
        //        // This is to match the behavior of arrays.
        //        return true;
        //    }
        //}

        //void ICollection<T>.Add(T item)
        //{
        //    throw new NotSupportedException();
        //}

        //void ICollection<T>.Clear()
        //{
        //    throw new NotSupportedException();
        //}

        //bool ICollection<T>.Contains(T item)
        //{
        //    ThrowInvalidOperationIfDefault();

        //    int index = System.Array.IndexOf<T>(_array!, item, _offset, _count);

        //    Debug.Assert(index == -1 ||
        //                    (index >= _offset && index < _offset + _count));

        //    return index >= 0;
        //}

        //bool ICollection<T>.Remove(T item)
        //{
        //    throw new NotSupportedException();
        //    return default;
        //}

        #endregion ICollection<T>

        #region IEnumerable<T>

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        #endregion IEnumerable<T>

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion IEnumerable

        private void ThrowInvalidOperationIfDefault()
        {
            if (_array == null)
            {
                throw new InvalidOperationException("InvalidOperation_NullArray");
            }
        }

        public struct Enumerator : IEnumerator<T>
        {
            private readonly T[]? _array;
            private readonly int _start;
            private readonly int _end; // cache Offset + Count, since it's a little slow
            private int _current;

            internal Enumerator(ReadOnlyArraySegment<T> arraySegment)
            {
                Debug.Assert(arraySegment.Array != null);
                Debug.Assert(arraySegment.Offset >= 0);
                Debug.Assert(arraySegment.Count >= 0);
                Debug.Assert(arraySegment.Offset + arraySegment.Count <= arraySegment.Array.Length);

                _array = arraySegment.Array;
                _start = arraySegment.Offset;
                _end = arraySegment.Offset + arraySegment.Count;
                _current = arraySegment.Offset - 1;
            }

            public bool MoveNext()
            {
                if (_current < _end)
                {
                    _current++;
                    return (_current < _end);
                }
                return false;
            }

            public T Current
            {
                get
                {
                    if (_current < _start)
                        throw new InvalidOperationException("InvalidOperation_EnumNotStarted");
                    if (_current >= _end)
                        throw new InvalidOperationException("InvalidOperation_EnumEnded");
                    return _array![_current];
                }
            }

            object? IEnumerator.Current => Current;

            void IEnumerator.Reset()
            {
                _current = _start - 1;
            }

            public void Dispose()
            {
            }
        }
    }
}