using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Toolkit.HighPerformance.Extensions;

namespace ClangReader.Utilities
{
    public class FastLineReader
    {
        private readonly string _filePath;

        public FastLineReader(string filePath)
        {
            _filePath = filePath;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public IEnumerable<ReadOnlyListEx<char>> ReadLine()
        {

            var list = new ListEx<char>();

            using var memoryOwner = MemoryPool<char>.Shared.Rent(10 * 1024);
            using var fileStream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var sr = new StreamReader(fileStream);

            var memory = memoryOwner.Memory;

            var tmpMemory = ArraySegment<char>.Empty;
            var parseResult = new ParseLineResult(ParseLineStatus.Success, 0);

            while (true)
            {
                tmpMemory = ReadSpan(sr, memory.AsArraySegment());
                if (!parseResult.Status.HasFlag(ParseLineStatus.EndlRequired))
                {
                    parseResult = new ParseLineResult(ParseLineStatus.Success, 0);
                }

                if (tmpMemory.Count == 0)
                {
                    if (list.Count != 0)
                    {
                        yield return list.AsReadonlyEx();
                        list.Clear();
                    }
                    yield break;
                }

                while (parseResult.Status.HasFlag(ParseLineStatus.Success) || (parseResult.Status.HasFlag(ParseLineStatus.EndlRequired) && tmpMemory.Count > 0))
                {
                    parseResult = ParseLine(tmpMemory, list, parseResult.Status);
                    tmpMemory = tmpMemory.Slice(parseResult.Index);

                    // Faster hasflags...i guess
                    if ((parseResult.Status & ParseLineStatus.Success) != 0)
                    {
                        yield return list.AsReadonlyEx();
                        list.Clear();
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        private static ArraySegment<char> ReadSpan(StreamReader sr, ArraySegment<char> memory)
        {
            var read = sr.Read(memory);
            if (read == 0)
            {
                return ArraySegment<char>.Empty;
            }

            return memory.Slice(0, read);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="input"></param>
        /// <param name="builder"></param>
        /// <returns>True means new line was found</returns>
        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        private ParseLineResult ParseLine(ReadOnlySpan<char> input, ListEx<char> builder, in ParseLineStatus previousStatus = ParseLineStatus.None)
        {
            if (input.Length == 0)
            {
                return new ParseLineResult(ParseLineStatus.BufferRequired, 0);
            }

            if (previousStatus != ParseLineStatus.Success)
            {
                // Faster hasflags...i guess
                if ((previousStatus & ParseLineStatus.EndlRequired) != 0)
                {
                    if (input[0] == '\n')
                    {
                        return new ParseLineResult(ParseLineStatus.Success, 1);
                    }

                    return new ParseLineResult(ParseLineStatus.Success, 0);
                }
            }

            var index = input.IndexOfAny('\r', '\n');
            if (index == -1)
            {
                builder.AddRange(input);
                return new ParseLineResult(ParseLineStatus.BufferRequired, input.Length);
            }

            var newSpan = input[..index];
            builder.AddRange(newSpan);

            var rest = input[index..];
            if (rest.Length > 1)
            {
                // Cool, we know for sure, if there is an \n
                if (rest[1] == '\n')
                {
                    return new ParseLineResult(ParseLineStatus.Success, index + 2);
                }

                return new ParseLineResult(ParseLineStatus.Success, index + 1);
            }
            else
            {
                return new ParseLineResult(ParseLineStatus.BufferRequired | ParseLineStatus.EndlRequired, index + 1);
            }
        }

        //Turned out its not that fast after all...
        //public async IAsyncEnumerable<AstLineResult> ReadHierarchyAsync(CancellationToken token)
        //{
        //    var list = new ListEx<char>();

        //    var pool = MemoryPool<char>.Shared;
        //    using var memoryOwner = pool.Rent(10 * 1024 * 1024);
        //    using var fileStream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        //    using var sr = new StreamReader(fileStream);

        //    var memory = memoryOwner.Memory;

        //    //var stringBuilder = new StringBuilder();
        //    ReadOnlyMemory<char> readMemory = ReadOnlyMemory<char>.Empty;

        //    var lines = 0;

        //    while (true)
        //    {
        //        readMemory = await ReadMemoryAsync(sr, memory, token);
        //        var parseResult = new ParseLineResult(ParseLineStatus.Success, 0);

        //        if (readMemory.Length == 0)
        //        {
        //            if (list.Count != 0)
        //            {
        //                yield return new AstLineResult(list.AsReadonlyEx());
        //                list.Clear();
        //            }
        //            yield break;
        //        }

        //        while ((parseResult.Status & ParseLineStatus.Success) != 0)
        //        {
        //            parseResult = ParseLine(readMemory.Span, list, parseResult.Status);
        //            readMemory = readMemory.Slice(parseResult.Index);

        //            // Faster hasflags...i guess
        //            if ((parseResult.Status & ParseLineStatus.Success) != 0)
        //            {
        //                yield return new AstLineResult(list);
        //                list.Clear();
        //            }
        //        }

        //        // Console.WriteLine($"{sr.BaseStream.Position} / {sr.BaseStream.Length}");
        //    }
        //}

        //private static async Task<ReadOnlyMemory<char>> ReadMemoryAsync(StreamReader sr, Memory<char> memory, CancellationToken token)
        //{
        //    var read = await sr.ReadAsync(memory, token);
        //    if (read == 0)
        //    {
        //        return Memory<char>.Empty;
        //    }

        //    return memory.Slice(0, read);
        //}

        private readonly struct ParseLineResult
        {
            public ParseLineStatus Status { get; }
            public int Index { get; }

            public ParseLineResult(ParseLineStatus status, int index)
            {
                Status = status;
                Index = index;
            }
        }

        [Flags]
        private enum ParseLineStatus : byte
        {
            None = 0,
            Success = 1 << 0,         // Cool, we found a line
            BufferRequired = 1 << 1,
            EndlRequired = 1 << 2,    // Eh, super rare edge case, where we have the \r but \n or whatever comes after it, is missing
        }
    }
}