using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using ClangReader.Models;

namespace ClangReader.Utilities
{
    public class AstTokenizerBatchResult : IDisposable, IEnumerable, IEnumerable<AstTokenizerBatchItem>
    {
        public AstTokenizerBatchResult(List<AstTokenizerBatchItem> tokenizerResults, IDisposable disposable)
        {
            TokenizerResults = tokenizerResults;
            Disposable = disposable;

            foreach (var result in tokenizerResults)
            {
                result.ParentBatch = this;
            }
        }

        public IDisposable Disposable { get; private set; }
        public List<AstTokenizerBatchItem> TokenizerResults { get; }

        public void Dispose()
        {
            Disposable?.Dispose();
            Disposable = null;
        }


        public IEnumerator<AstTokenizerBatchItem> GetEnumerator() => TokenizerResults.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) TokenizerResults).GetEnumerator();
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