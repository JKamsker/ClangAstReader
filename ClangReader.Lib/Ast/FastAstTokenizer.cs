﻿using System;
using System.Buffers;
using System.Collections.Generic;

using ClangReader.Lib.Ast.Models;
using ClangReader.Lib.Collections;
using ClangReader.Lib.Extensions;
using ClangReader.Lib.IO;

namespace ClangReader.Lib.Ast
{
    public class FastAstTokenizer
    {
        private readonly FastLineReader _lineReader;

        public FastAstTokenizer(FastLineReader lineReader)
        {
            _lineReader = lineReader;
        }

        public IEnumerable<AstTokenizerResult> TokenizeLines()
        {
            //ResizeBuffer(10 * 1024, null, out var bufferOwner, out var buffer);

            foreach (var rawLine in _lineReader.ReadLine())
            {
                var line = rawLine.ArraySegment;

                var lineDepth = 0;
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

                //var required = line.Span[tokenStart..];
                //if (required.Length > buffer.Length)
                //{
                //    ResizeBuffer(required.Length, null, out bufferOwner, out buffer);
                //}

                //required.CopyTo(buffer.Span);

                // This could be done later at any other thread tho...
                AstTokenParserUtils.ParseTokenAndDescription(line, tokenStart, out var token, out var declaration);
                yield return new AstTokenizerResult(lineDepth, token, declaration);

                //yield return new AstTokenizerResult(lineDepth, ReadOnlyArraySegment<char>.Empty, ReadOnlyArraySegment<char>.Empty);
            }
        }

        public IEnumerable<AstTokenizerBatchResult> AstTokenizerBatchResults()
        {
            InitializeNewBatch
            (
                minMemory: 0,
                out IMemoryOwner<char> owner,
                out Memory<char> newMemory,
                out Memory<char> availableMemory,
                out List<AstTokenizerBatchItem> currentResults
            );

            foreach (var rawLine in _lineReader.ReadLine())
            {
                var line = rawLine.ArraySegment;

                AstTokenParserUtils.GetEssentialPart(line, out var lineDepth, out var essentialPart);

                if (availableMemory.Length < essentialPart.Count)
                {
                    if (currentResults.Count > 0)
                    {
                        yield return new AstTokenizerBatchResult(currentResults, owner);
                    }

                    InitializeNewBatch(essentialPart.Count + 1, out owner, out newMemory, out availableMemory, out currentResults);
                }

                essentialPart.CopyTo(availableMemory.AsArraySegment());
                var essentialCopy = availableMemory[..essentialPart.Count].AsReadOnlyArraySegment();
                availableMemory = availableMemory[essentialPart.Count..];

                currentResults.Add(new AstTokenizerBatchItem(lineDepth, essentialCopy));
            }

            if (currentResults.Count > 0)
            {
                yield return new AstTokenizerBatchResult(currentResults, owner);
            }
        }

        private static void InitializeNewBatch(int minMemory, out IMemoryOwner<char> owner, out Memory<char> newMemory, out Memory<char> availableMemory, out List<AstTokenizerBatchItem> currentResults)
        {
            var memorySize = Math.Max(10 * 1024, minMemory);
            owner = MemoryPool<char>.Shared.Rent(memorySize);
            newMemory = owner.Memory;
            availableMemory = newMemory;
            currentResults = new List<AstTokenizerBatchItem>(newMemory.Length / 124);
        }

        private static void ResizeBuffer(int minimumSize, IMemoryOwner<char> current, out IMemoryOwner<char> newOwner, out Memory<char> newMemory)
        {
            current?.Dispose();

            newOwner = MemoryPool<char>.Shared.Rent(minimumSize);
            newMemory = newOwner.Memory;
        }
    }
}