﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using ClangReader.Lib.Ast.Interception;
using ClangReader.Lib.Ast.Models;
using ClangReader.Lib.Collections;
using ClangReader.Lib.Extensions;
using ClangReader.Lib.IO;

using Microsoft.Toolkit.HighPerformance.Extensions;

namespace ClangReader.Lib.Ast
{
    public class AstTokenSurrogate
    {
        public AstToken Token { get; set; }

        // public bool IsParsed => !(Token is null) && Token.children;

        public List<AstTokenizerBatchItem> BatchItems { get; set; } = new List<AstTokenizerBatchItem>();

        public AstTokenSurrogate()
        {
        }
    }

    public class AstFileReader
    {
        static AstFileReader()
        {
        }

        private readonly string _filePath;
        private Channel<AstTokenSurrogate> _workerInputChannel;

        public AstFileReader(string filePath)
        {
            _filePath = filePath;

            _workerInputChannel = Channel.CreateBounded<AstTokenSurrogate>(new BoundedChannelOptions(Environment.ProcessorCount * 2)
            {
                SingleWriter = true,
                AllowSynchronousContinuations = false,
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = false
            });
        }

        public List<AstToken> Parse(AstParserInterceptor interceptor)
        {
            var fastReader = new FastLineReader(_filePath);
            var readerContext = new AstFileReaderContext(fastReader.ReadLine());

            AstToken currentRoot = null;
            var rootTokens = new List<AstToken>();

            foreach (var rawLine in readerContext)
            {
                AstTokenParserUtils.GetEssentialPart(rawLine, out var lineDepth, out var line);
                var token = new AstToken(true) { Line = readerContext.CurrentLine }; ;
                AstTokenParserUtils.ParseTokenDescription(token, line);

                if (lineDepth == 0)
                {
                    currentRoot = token;
                    rootTokens.Add(token);
                    continue;
                }

                if (currentRoot == null)
                {
                    currentRoot = new AstToken(true) { unknownName = "Unknown" };
                    rootTokens.Add(currentRoot);
                }

                if (interceptor.OnNodeParsed(readerContext, token, lineDepth))
                {
                    continue;
                }

                currentRoot.AddChild(token, lineDepth - 1);
            }

            return rootTokens;
        }

        private List<Task> _tasks = new List<Task>();

        public async Task ParseAsync()
        {
            //[0]	{[0, 1]}
            //[1]	{[1, 15045]}
            //[2]	{[2, 89154]}

            for (int i = 0; i < Environment.ProcessorCount * 1.2; i++) //Environment.ProcessorCount
            {
                _tasks.Add(Task.Run(async () => await RunProcessorAsync(default)));
            }

            var fastReader = new FastLineReader(_filePath);
            var tokenizer = new FastAstTokenizer(fastReader);

            var rootTokens = new List<AstToken>();
            AstToken currentRoot = null;

            //var statDict = new Dictionary<int, int>();
            var sw = Stopwatch.StartNew();
            var batchList = new List<AstTokenizerBatchItem>();
            foreach (var astTokenizerBatchResult in tokenizer.AstTokenizerBatchResults())
            {
                foreach (var item in astTokenizerBatchResult)
                {
                    var lineDepth = item.LineDepth;

                    if (lineDepth == 0)
                    {
                        currentRoot = new AstToken(true);
                        ParseTokenDescription(currentRoot, item);
                        rootTokens.Add(currentRoot);
                        item.MarkAsProcessed();
                        continue;
                    }

                    if (lineDepth == 1 && batchList.Count != 0)
                    {
                        if (currentRoot == null)
                        {
                            currentRoot = new AstToken(true) { unknownName = "Unknown" };
                            rootTokens.Add(currentRoot);
                        }

                        var surrogate = new AstTokenSurrogate
                        {
                            BatchItems = batchList,
                            Token = new AstToken(false)
                        };

                        currentRoot.AddChild(surrogate.Token);
                        await _workerInputChannel.Writer.WriteAsync(surrogate);

                        batchList = new List<AstTokenizerBatchItem>();
                    }

                    batchList.Add(item);
                }
            }

            _workerInputChannel.Writer.Complete();

            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            while (true)
            {
                await Task.Delay(1000);
            }

            Console.ReadLine();
        }

        public async Task RunProcessorAsync(CancellationToken cancellationTokentoken)
        {
            try
            {
                while (true)
                {
                    var currentBatch = await _workerInputChannel.Reader.ReadAsync(cancellationTokentoken);
                    foreach (var batchItem in currentBatch.BatchItems)
                    {
                        AstToken itemtoken;
                        if (batchItem.LineDepth == 1)
                        {
                            itemtoken = currentBatch.Token;
                            itemtoken.children ??= new List<AstToken>();
                        }
                        else
                        {
                            itemtoken = new AstToken();
                            currentBatch.Token.AddChild(itemtoken, batchItem.LineDepth - 1);
                        }

                        AstTokenParserUtils.ParseTokenDescription(itemtoken, batchItem.Line);
                        batchItem.MarkAsProcessed();
                    }
                }
            }
            catch (ChannelClosedException)
            {
                Console.WriteLine("Operation cancelled off task");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Task failed: {ex}");
            }
            finally
            {
                // Console.WriteLine("Finished off task");
            }
        }

        private static void ParseTokenDescription(AstToken token, AstTokenizerBatchItem batchItem) =>
            AstTokenParserUtils.ParseTokenDescription(token, batchItem.Line);
    }
}