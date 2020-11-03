using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using ClangReader.Utilities;

namespace ClangReader.Pipelined
{
    public class FileToLinesProcessor : Processorbase
    {
        private readonly ChannelWriter<BatchLineReadResult> _lineResultWriter;
        private readonly FastLineReader _lineReader;

        public FileToLinesProcessor(FastLineReader lineReader, ChannelWriter<BatchLineReadResult> lineResultWriter) : base(1)
        {
            _lineResultWriter = lineResultWriter;
            _lineReader = lineReader;
        }

        private protected override async Task RunAsync(CancellationToken cancellation)
        {
            foreach (var batch in EnumerateBatches())
            {
                await _lineResultWriter.WriteAsync(batch, cancellation);
            }
        }

        private IEnumerable<BatchLineReadResult> EnumerateBatches()
        {
            IMemoryOwner<char> memoryOwner = null;
            Memory<char> memory = default;
            var availableMemory = Memory<char>.Empty;
            List<BatchLineItem> currentResults = default;

            foreach (var rawLine in _lineReader.ReadLine())
            {
                if (availableMemory.Length < rawLine.Count)
                {
                    if (currentResults?.Count > 0)
                    {
                        yield return new BatchLineReadResult
                        {
                            Items = currentResults,
                            Disposable = memoryOwner
                        };
                    }

                    InitializeNewBatch(rawLine.Count + 1, out memoryOwner, out memory, out availableMemory, out currentResults);
                }

                rawLine.CopyTo(availableMemory.Span);
                var lineCopy = availableMemory[..rawLine.Count];
                availableMemory = availableMemory[rawLine.Count..];

                // ReSharper disable once PossibleNullReferenceException
                currentResults.Add(new BatchLineItem(lineCopy));
            }

            if (currentResults?.Count > 0)
            {
                yield return new BatchLineReadResult
                {
                    Items = currentResults,
                    Disposable = memoryOwner
                };
            }
        }

        private static void InitializeNewBatch(int minMemory, out IMemoryOwner<char> owner, out Memory<char> newMemory, out Memory<char> availableMemory, out List<BatchLineItem> currentResults)
        {
            //2 * 10 * 1024 = 20.480 byte
            var memorySize = Math.Max(10 * 1024, minMemory);
            owner = MemoryPool<char>.Shared.Rent(memorySize);
            newMemory = owner.Memory;
            availableMemory = newMemory;
            currentResults = new List<BatchLineItem>(newMemory.Length / 124);
        }
    }
}