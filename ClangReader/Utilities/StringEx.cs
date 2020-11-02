using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Extensions;

namespace ClangReader.Utilities
{
    public static class StringEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string Join(ReadOnlySpan<char> separator, ReadOnlySpan<char> str0, ReadOnlySpan<char> str1)
        {
            int length = checked(separator.Length + str0.Length + str1.Length);
            if (length == 0)
            {
                return string.Empty;
            }

            var result = new ModifyableString(length);
            var resultSpan = result.SpanValue;

            WriteValue(ref resultSpan, str0);

            SeperatorThenValue(ref resultSpan, separator, str1);
            return result.StringValue;
        }

     
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string Join(ReadOnlySpan<char> separator, ReadOnlySpan<char> str0, ReadOnlySpan<char> str1, ReadOnlySpan<char> str2)
        {
            int length = checked(separator.Length * 2 + str0.Length + str1.Length + str2.Length);
            if (length == 0)
            {
                return string.Empty;
            }

            var result = new ModifyableString(length);
            var resultSpan = result.SpanValue;

            WriteValue(ref resultSpan, str0);

            SeperatorThenValue(ref resultSpan, separator, str1);
            SeperatorThenValue(ref resultSpan, separator, str2);

            return result.StringValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string Join(ReadOnlySpan<char> separator, ReadOnlySpan<char> str0, ReadOnlySpan<char> str1, ReadOnlySpan<char> str2, ReadOnlySpan<char> str3)
        {
            int length = checked(separator.Length * 3 + str0.Length + str1.Length + str2.Length + str3.Length);
            if (length == 0)
            {
                return string.Empty;
            }

            var result = new ModifyableString(length);
            var resultSpan = result.SpanValue;

            WriteValue(ref resultSpan, str0);

            SeperatorThenValue(ref resultSpan, separator, str1);
            SeperatorThenValue(ref resultSpan, separator, str2);
            SeperatorThenValue(ref resultSpan, separator, str3);

            return result.StringValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string Join
        (
            ReadOnlySpan<char> separator, 
            ReadOnlySpan<char> str0, 
            ReadOnlySpan<char> str1, 
            ReadOnlySpan<char> str2, 
            ReadOnlySpan<char> str3,
            ReadOnlySpan<char> str4
        )
        {
            int length = checked(separator.Length * 4 + str0.Length + str1.Length + str2.Length + str3.Length + str4.Length);
            if (length == 0)
            {
                return string.Empty;
            }

            var result = new ModifyableString(length);
            var resultSpan = result.SpanValue;

            WriteValue(ref resultSpan, str0);

            SeperatorThenValue(ref resultSpan, separator, str1);
            SeperatorThenValue(ref resultSpan, separator, str2);
            SeperatorThenValue(ref resultSpan, separator, str3);
            SeperatorThenValue(ref resultSpan, separator, str4);

            return result.StringValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static void WriteValue(ref Span<char> resultSpan, in ReadOnlySpan<char> str0)
        {
            str0.CopyTo(resultSpan);
            resultSpan = resultSpan.Slice(str0.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static void SeperatorThenValue(ref Span<char> resultSpan, in ReadOnlySpan<char> separator, in ReadOnlySpan<char> str1)
        {
            separator.CopyTo(resultSpan);
            resultSpan = resultSpan.Slice(separator.Length);

            str1.CopyTo(resultSpan);
            resultSpan = resultSpan.Slice(str1.Length);
        }
    }
}