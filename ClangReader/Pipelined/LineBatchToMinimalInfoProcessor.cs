using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ClangReader.Lib.Ast;
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
                    AstTokenParserUtils.GetEssentialPart(item.LineCopy.AsReadOnlyArraySegment(), out var lineDepth, out var essential);
                    AstTokenParserUtils.ParseTokenAndDescription(essential, out var token, out var description);
                    var descriptionSegments = AstTokenParserUtils.GetSegments(description);

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
    }
}