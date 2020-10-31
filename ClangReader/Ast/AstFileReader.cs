using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClangReader.Ast
{
    public struct AstLineResult
    {
        public static readonly AstLineResult Default = new AstLineResult();

        public StringBuilder StringBuilder;

        public AstLineResult(StringBuilder stringBuilder)
        {
            StringBuilder = stringBuilder;
        }
    }

    public class AstFileReader
    {
        private readonly string _filePath;

        public AstFileReader(string filePath)
        {
            _filePath = filePath;
        }

        public async IAsyncEnumerable<AstLineResult> ReadHierarchyAsync(CancellationToken token)
        {


            var pool = MemoryPool<char>.Shared;
            using var memoryOwner = pool.Rent(10 * 1024 * 1024);
            using var fileStream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var sr = new StreamReader(fileStream);

            var memory = memoryOwner.Memory;

            var stringBuilder = new StringBuilder();
            ReadOnlyMemory<char> readMemory = ReadOnlyMemory<char>.Empty;

            var lines = 0;

            while (true)
            {
                readMemory = await ReadMemoryAsync(sr, memory, token);
                var parseResult = new ParseLineResult(ParseLineStatus.Success, 0);

                if (readMemory.Length == 0)
                {
                    if (stringBuilder.Length != 0)
                    {
                        yield return new AstLineResult(stringBuilder);
                        stringBuilder = stringBuilder.Clear();
                    }
                    yield break;
                }

                while ((parseResult.Status & ParseLineStatus.Success) != 0)
                {
                    parseResult = ParseLine(readMemory.Span, stringBuilder, parseResult.Status);
                    readMemory = readMemory.Slice(parseResult.Index);

                    // Faster hasflags...i guess
                    if ((parseResult.Status & ParseLineStatus.Success) != 0)
                    {
                        yield return new AstLineResult(stringBuilder);
                        stringBuilder = stringBuilder.Clear();
                    }
                }

                // Console.WriteLine($"{sr.BaseStream.Position} / {sr.BaseStream.Length}");
            }
        }

        private static async Task<ReadOnlyMemory<char>> ReadMemoryAsync(StreamReader sr, Memory<char> memory, CancellationToken token)
        {
            var read = await sr.ReadAsync(memory, token);
            if (read == 0)
            {
                //memory.Span.Clear();

                //var read1 = await sr.ReadAsync(memory, token);
                //215632982
                //yield break;
                return Memory<char>.Empty;
            }

            return memory.Slice(0, read);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="input"></param>
        /// <param name="builder"></param>
        /// <returns>True means new line was found</returns>
        private ParseLineResult ParseLine(ReadOnlySpan<char> input, StringBuilder builder, in ParseLineStatus previousStatus = ParseLineStatus.None)
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
                builder.Append(input);
                return new ParseLineResult(ParseLineStatus.BufferRequired, input.Length);
            }

            var newSpan = input[..index];
            builder.Append(newSpan);

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