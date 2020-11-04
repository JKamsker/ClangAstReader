using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ClangReader.Lib.Collections;
using ClangReader.Lib.Extensions;

namespace ClangReader.Pipelined
{
    public class LineBatchToMinimalInfoProcessor : Processorbase
    {
        private readonly ChannelReader<BatchLineReadResult> _input;

        public LineBatchToMinimalInfoProcessor(ChannelReader<BatchLineReadResult> input) : base(1)
        {
            _input = input;
        }

        private protected override async Task RunAsync(CancellationToken cancellation)
        {
            IMemoryOwner<char> memoryOwner = null;
            Memory<char> memory = default;
            var availableMemory = Memory<char>.Empty;
            List<BatchLineItem> currentResults = default;

            while (true)
            {
                using var batchItem = await _input.ReadAsync(cancellation);
                foreach (var item in batchItem)
                {
                    GetEssentialPart(item.LineCopy.AsReadOnlyArraySegment(), out var lineDepth, out var essential);
                    ParseTokenAndDescription(essential, out var token, out var description);
                    var descriptionSegments = GetSegments(description);

                    var length = GetLength(token, descriptionSegments);
                }
            }
        }

        private static int GetLength(ReadOnlyArraySegment<char> token,
            List<ReadOnlyArraySegment<char>> descriptionSegments)
        {
            var length = token.Count;
            foreach (var segment in descriptionSegments)
            {
                length += segment.Count;
            }

            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static void GetEssentialPart(ReadOnlyArraySegment<char> line, out int lineDepth,
            out ReadOnlyArraySegment<char> essential)
        {
            lineDepth = 0;
            var tokenStart = -1;
            var tokenEnd = -1;

            for (var i = 0; i < line.Count; i += 2)
            {
                if (line[i] == '|' || line[i] == '`' || line[i] == ' ' || line[i] == '-')
                {
                    lineDepth++;
                    continue;
                }

                tokenStart = i;
                break;
            }

            essential = line[tokenStart..];
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static void ParseTokenAndDescription(in ReadOnlyArraySegment<char> line,
            out ReadOnlyArraySegment<char> token, out ReadOnlyArraySegment<char> declaration)
        {
            var tokenEnd = line.IndexOf(' ');
            if (tokenEnd == -1)
            {
                token = line;
                declaration = ReadOnlyArraySegment<char>.Empty;
            }
            else
            {
                token = line[..tokenEnd];
                declaration = line.Slice(tokenEnd + 1);
            }
        }

        private static readonly char[] _separators = new []{'<','>','\'', '\"' };

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static List<ReadOnlyArraySegment<char>> GetSegments(ReadOnlyArraySegment<char> source)
        {
            var sourceCopy = source.Slice(0);
            // var sourceCopy = source.AsSpan();
            // Avg: 15,..
            // To 16 cause its a natural cap
            var parts = new List<ReadOnlyArraySegment<char>>(16);

            var depth = 0;
            var quotedSingle = false;
            var quotedDouble = false;
            while (sourceCopy.Count > 0)
            {
                var tryAgain = false;

                for (var i = 0; i < sourceCopy.Count; i++)
                {
                    var current = sourceCopy[i];
                    if (current == '<' && !quotedDouble && !quotedSingle)
                    {
                        depth++;
                    }
                    else if (current == '>' && !quotedDouble && !quotedSingle && depth > 0)
                    {
                        depth--;
                    }
                    else if (current == '\'' && !quotedDouble)
                    {
                        if (quotedSingle)
                        {
                            depth--;
                        }
                        else
                        {
                            depth++;
                        }

                        quotedSingle = !quotedSingle;
                    }
                    else if (current == '\"' && !quotedSingle)
                    {
                        if (quotedDouble)
                        {
                            depth--;
                        }
                        else
                        {
                            depth++;
                        }

                        quotedDouble = !quotedDouble;
                    }
                    else if (current == ' ' && depth == 0)
                    {
                        var cutTighter = sourceCopy[0] == '\'' && sourceCopy[i - 1] == '\'';

                        var subvalue = cutTighter ?
                            sourceCopy.Slice(1, i - 2) :
                            sourceCopy.Slice(0, i);

                        parts.Add(subvalue);
                        //yield return subvalue;

                        sourceCopy = sourceCopy.Slice(i + 1);
                        tryAgain = true;
                        break;
                    }
                }

                if (tryAgain) continue;

                parts.Add(sourceCopy);
                return parts;
                //yield break;
            }

            //maxsize = Math.Max(parts.Count, maxsize);
            //sizes.Add(parts.Count);
            return parts;
        }
    }
}