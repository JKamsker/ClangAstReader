using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Toolkit.HighPerformance.Extensions;

namespace ClangReader.Lib.Collections
{
    public class StringToEnumMapper<TEnum> : IEnumerable
        where TEnum : Enum
    {
        private readonly Dictionary<int, TEnum> _enumMap = new Dictionary<int, TEnum>();

        public void Add(string value, TEnum @enum) => Add(value.AsSpan(), @enum);

        public void Add(ReadOnlySpan<char> value, TEnum @enum)
        {
            _enumMap[value.GetDjb2HashCode()] = @enum;
        }

        public bool TryGetMap(string value, out TEnum result) => TryGetMap(value.AsSpan(), out result);

        public bool TryGetMap(ReadOnlySpan<char> value, out TEnum result)
        {
            return _enumMap.TryGetValue(value.GetDjb2HashCode(), out result);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_enumMap).GetEnumerator();
        }
    }
}