using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using ClangReader.Lib.Collections;

namespace ClangReader.Lib.Ast
{
    public class AstFileReaderContext : IDisposable, IEnumerable<ReadOnlyArraySegment<char>>
    {
        private readonly IEnumerable<ReadOnlyListEx<char>> _reader;
        private IEnumerator<ReadOnlyListEx<char>> _enumerator;
        private bool _shouldMoveNext = true;

        public bool HasEnded { get; private set; }
        public int CurrentLine { get; set; }

        public AstFileReaderContext(IEnumerable<ReadOnlyListEx<char>> reader)
        {
            _reader = reader;
        }

        public ReadOnlyListEx<char> GetNexLine()
        {
            if (TryGetNextLine(out var result))
            {
                return result;
            }

            return null;

            /* if (HasEnded)
                {
                    return null;
                }

                if (_enumerator == null)
                {
                    _enumerator = _reader.GetEnumerator();
                }

                if (_shouldMoveNext)
                {
                    if (_enumerator.MoveNext())
                    {
                        CurrentLine++;
                        return _enumerator.Current;
                    }
                }
                else
                {
                    _shouldMoveNext = true;

                    return _enumerator.Current;
                }

                HasEnded = true;
                return null;*/
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public bool TryGetNextLine(out ReadOnlyListEx<char> line)
        {
            line = null;
            if (HasEnded)
            {
                return false;
            }

            if (_enumerator == null)
            {
                _enumerator = _reader.GetEnumerator();
            }

            if (_shouldMoveNext)
            {
                if (_enumerator.MoveNext())
                {
                    CurrentLine++;
                    line = _enumerator.Current;
                    return true;
                }
            }
            else
            {
                _shouldMoveNext = true;

                line = _enumerator.Current;
                return true;
            }

            HasEnded = true;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public void SkipSubTree()
        {
            var enumerator = _enumerator;

            var origLineDepth = AstTokenParserUtils.GetLineDepth(enumerator.Current.Span);
            var tempDepth = origLineDepth + 1;

            while (origLineDepth < tempDepth && enumerator.MoveNext())
            {
                CurrentLine++;
                tempDepth = AstTokenParserUtils.GetLineDepth(enumerator.Current.Span);
            }

            _shouldMoveNext = false;
        }

        public void Dispose()
        {
            _enumerator?.Dispose();
            _enumerator = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private IEnumerable<ReadOnlyArraySegment<char>> ReadLinesInternal()
        {
            HasEnded = false;
            while (!HasEnded)
            {
                var line = GetNexLine();
                if (line == null)
                {
                    yield break;
                }
                yield return line.ArraySegment;
            }
        }

        public IEnumerator<ReadOnlyArraySegment<char>> GetEnumerator()
        {
            return ReadLinesInternal().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}