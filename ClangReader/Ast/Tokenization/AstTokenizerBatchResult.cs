using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ClangReader.Models;

namespace ClangReader.Utilities
{
    public class AstTokenizerBatchResult : IDisposable, IEnumerable, IEnumerable<AstTokenizerBatchItem>
    {
        private object _syncRoot = new object();

        public AstTokenizerBatchResult(List<AstTokenizerBatchItem> tokenizerResults, IDisposable disposable)
        {
    
            TokenizerResults = tokenizerResults;
            Disposable = disposable;
            if (Disposable == null)
            {
                Debugger.Break();
            }
            foreach (var result in tokenizerResults)
            {
                result.ParentBatch = this;
            }
        }

        public IDisposable Disposable { get; private set; }
        public List<AstTokenizerBatchItem> TokenizerResults { get; }

        public volatile int __disposedChildren;

        public void Dispose()
        {
            Disposable?.Dispose();
            Disposable = null;
        }

        public IEnumerator<AstTokenizerBatchItem> GetEnumerator() => TokenizerResults.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)TokenizerResults).GetEnumerator();

        public void DisposeIfAllHaveBeenProcessed()
        {
            if (Disposable == null)
            {
                return;
            }

            var current = Interlocked.Increment(ref __disposedChildren);
            if (current < TokenizerResults.Count)
            {
                return;
            }

            lock (_syncRoot)
            {
                if (Disposable == null)
                {
                    return;
                }

                foreach (var tokenResult in TokenizerResults)
                {
                    if (!tokenResult.Processed)
                    {
                        return;
                    }
                }

                Disposable.Dispose();
                Disposable = null;
            }
        }
    }

    [DebuggerDisplay("[{" + nameof(LineDepth) + "}] {" + nameof(LineString) + "}")]
    public class AstTokenizerBatchItem
    {
        public int LineDepth { get; }
        public ReadOnlyArraySegment<char> Line { get; }

        public bool Processed { get; private set; }

        public string LineString => Line.Span.ToString();

        public AstTokenizerBatchResult ParentBatch { get; set; }

        public AstTokenizerBatchItem(int lineDepth, ReadOnlyArraySegment<char> line)
        {
            LineDepth = lineDepth;
            Line = line;
        }

        public void MarkAsProcessed()
        {
            Processed = true;
            ParentBatch.DisposeIfAllHaveBeenProcessed();
        }

        public void ParseTokenAndDeclraction(out ReadOnlyArraySegment<char> token, out ReadOnlyArraySegment<char> declaration)
        {
            var line = Line;

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
    }
}