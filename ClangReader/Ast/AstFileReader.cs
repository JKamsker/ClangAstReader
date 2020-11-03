using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Serialization;

using ClangReader.Models;
using ClangReader.Utilities;

using Microsoft.Extensions.Primitives;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Enumerables;
using Microsoft.Toolkit.HighPerformance.Extensions;
using Microsoft.Toolkit.HighPerformance.Helpers;

namespace ClangReader.Ast
{
    internal enum ContextAction
    {
        InvalidSloc,
        BuildIn,
        ScratchSpace,
        Line,
        Col
    }

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
            _tokenDescriptionActionMap = new StringToEnumMapper<TokenDescriptionAction>
            {
                { "Decl", TokenDescriptionAction.DeclCase },

                { "Type", TokenDescriptionAction.OffsetFirst },
                { "Record", TokenDescriptionAction.OffsetFirst },
                { "Typedef", TokenDescriptionAction.OffsetFirst },
                { "Parm", TokenDescriptionAction.OffsetFirst },
                { "Specialization", TokenDescriptionAction.OffsetFirst },
                { "Function", TokenDescriptionAction.OffsetFirst },
                { "Enum", TokenDescriptionAction.OffsetFirst },
                { "Field", TokenDescriptionAction.OffsetFirst },
                { "Alias", TokenDescriptionAction.OffsetFirst },
                { "Comment", TokenDescriptionAction.OffsetFirst },
                { "Var", TokenDescriptionAction.OffsetFirst },

                { "Attr", TokenDescriptionAction.OffsetThenFileContext },
                { "Expr", TokenDescriptionAction.OffsetThenFileContext },
                { "Literal", TokenDescriptionAction.OffsetThenFileContext },
                { "Operator", TokenDescriptionAction.OffsetThenFileContext },
                { "Stmt", TokenDescriptionAction.OffsetThenFileContext },
                { "Cleanups", TokenDescriptionAction.OffsetThenFileContext },

                { "original", TokenDescriptionAction.NameThenOffset },

                { "Data", TokenDescriptionAction.DoNothing },
                { "Constructor", TokenDescriptionAction.DoNothing },
                { "Assignment", TokenDescriptionAction.DoNothing },
                { "Destructor", TokenDescriptionAction.DoNothing },
                { "Argument", TokenDescriptionAction.DoNothing },
                { "Initializer", TokenDescriptionAction.DoNothing },
                { "public", TokenDescriptionAction.DoNothing },
                { "private", TokenDescriptionAction.DoNothing },
                { "protected", TokenDescriptionAction.DoNothing },
                { "virtual", TokenDescriptionAction.DoNothing },
                { "<<<NULL>>>", TokenDescriptionAction.DoNothing },
                { "Overrides:", TokenDescriptionAction.DoNothing },
                { "...", TokenDescriptionAction.DoNothing },
                { "array", TokenDescriptionAction.DoNothing },
                { "value:", TokenDescriptionAction.DoNothing },
                { "Guid", TokenDescriptionAction.DoNothing },
                { "inherited", TokenDescriptionAction.DoNothing },
            };

            _contextMap = new StringToEnumMapper<ContextAction>
            {
                { "<invalid sloc>", ContextAction.InvalidSloc },
                { "<built-in>", ContextAction.BuildIn },
                { "<scratch space>", ContextAction.ScratchSpace },
                { "line", ContextAction.Line },
                { "col", ContextAction.Col }
            };
        }

        private readonly string _filePath;
        private Channel<AstTokenSurrogate> _workerInputChannel;
        private static StringToEnumMapper<TokenDescriptionAction> _tokenDescriptionActionMap;
        private static StringToEnumMapper<ContextAction> _contextMap;

        public AstFileReader(string filePath)
        {
            _filePath = filePath;

            //_workerInputChannel = Channel.CreateUnbounded<AstTokenSurrogate>(new UnboundedChannelOptions
            //{
            //    SingleWriter = true,
            //    AllowSynchronousContinuations = false,
            //});

            _workerInputChannel = Channel.CreateBounded<AstTokenSurrogate>(new BoundedChannelOptions(Environment.ProcessorCount * 2)
            {
                SingleWriter = true,
                AllowSynchronousContinuations = false,
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = false
            });
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
                        //TODO: Parse that shit
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
                            currentRoot = new AstToken(true) { name = "Unknown" };
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

                //astTokenizerBatchResult.Dispose();
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
                            currentBatch.Token.AddChild(itemtoken, batchItem.LineDepth - 2);
                        }

                        ParseTokenDescription(itemtoken, batchItem);
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

        private static void ParseTokenDescription(AstToken token, AstTokenizerBatchItem batchItem)
        {
            batchItem.ParseTokenAndDeclraction(out var name, out var description);
            token.name = name.ToString();

            var parameterStartIndex = 0;
            var parameters = GetSegments(description).ToArray();

            var sufix = GetSufix(name.Span);
            if (_tokenDescriptionActionMap.TryGetMap(sufix, out var action))
            {
                switch (action)
                {
                    case TokenDescriptionAction.DeclCase:
                        token.offset = parameters[0].ToString();

                        parameterStartIndex = 1;

                        var declFileContext = parameters[parameterStartIndex];

                        while (declFileContext[0] != '<')
                        {
                            if (token.relationOffset != null)
                            {
                                token.relationOffset = StringEx.Join(" ", token.relationOffset, parameters[parameterStartIndex], parameters[parameterStartIndex + 1]);
                            }
                            else
                            {
                                token.relationOffset = StringEx.Join(" ", parameters[parameterStartIndex], parameters[parameterStartIndex + 1]);
                            }

                            parameterStartIndex += 2;
                            declFileContext = parameters[parameterStartIndex];
                        }

                        token.fileContext = declFileContext.ToString();
                        token.filePointer = parameters[parameterStartIndex + 1].ToString();
                        parameterStartIndex += 2;
                        break;

                    case TokenDescriptionAction.OffsetFirst:
                        token.offset = parameters[0].ToString();
                        parameterStartIndex = 1;
                        break;

                    case TokenDescriptionAction.OffsetThenFileContext:
                        token.offset = parameters[0].ToString();
                        token.fileContext = parameters[1].ToString();
                        parameterStartIndex = 2;
                        break;

                    case TokenDescriptionAction.NameThenOffset:
                        token.name = string.Concat(name.Span, parameters[0]);
                        token.offset = parameters[1].ToString();
                        parameterStartIndex = 2;
                        break;

                    case TokenDescriptionAction.DoNothing:
                    default:
                        break;
                }
            }

            token.properties = parameters.Where((value, index) => index >= parameterStartIndex).Select(x => x.ToString()).ToArray();

            ParseTokenContext(token);

            ParseTokenPropertiesAndAttributes(token, parameterStartIndex, parameters);
        }

        private static void ParseTokenPropertiesAndAttributes(AstToken token, int parameterStartIndex, ReadOnlyArraySegment<char>[] parameters)
        {
            var tokenProperties = parameters.AsSpan(parameterStartIndex);
            var tokenAttributes = new List<string>();

            tokenProperties = PushStartToAttributesIfNecessary(tokenProperties, tokenAttributes, "implicit");
            tokenProperties = PushStartToAttributesIfNecessary(tokenProperties, tokenAttributes, "used");
            tokenProperties = PushStartToAttributesIfNecessary(tokenProperties, tokenAttributes, "referenced");

            tokenProperties = PushEndToAttributesIfNecessary(tokenProperties, tokenAttributes, "cinit");
            tokenProperties = PushEndToAttributesIfNecessary(tokenProperties, tokenAttributes, "extern");
            tokenProperties = PushEndToAttributesIfNecessary(tokenProperties, tokenAttributes, "callinit");
            tokenProperties = PushEndToAttributesIfNecessary(tokenProperties, tokenAttributes, "static");
            tokenProperties = PushEndToAttributesIfNecessary(tokenProperties, tokenAttributes, "definition");
            tokenProperties = PushEndToAttributesIfNecessary(tokenProperties, tokenAttributes, "nrvo");

            token.attributes = tokenAttributes.ToArray();
            token.properties = new string[tokenProperties.Length];
            for (var index = 0; index < tokenProperties.Length; index++)
            {
                token.properties[index] = tokenProperties[index].ToString();
            }
        }

        private static Span<ReadOnlyArraySegment<char>> PushEndToAttributesIfNecessary
        (
            Span<ReadOnlyArraySegment<char>> tokenProperties,
            List<string> tokenAttributes,
            ReadOnlySpan<char> token
        )
        {
            if (tokenProperties.Length <= 0 || tokenProperties[^1].Span != token)
            {
                return tokenProperties;
            }

            tokenAttributes.Add(tokenProperties[^1].ToString());
            tokenProperties = tokenProperties[..^1];

            return tokenProperties;
        }

        private static Span<ReadOnlyArraySegment<char>> PushStartToAttributesIfNecessary
        (
            Span<ReadOnlyArraySegment<char>> tokenProperties,
            List<string> tokenAttributes,
            ReadOnlySpan<char> token
        )
        {
            if (tokenProperties.Length <= 0 || tokenProperties[0].Span != token)
            {
                return tokenProperties;
            }

            tokenAttributes.Add(tokenProperties[0].ToString());
            tokenProperties = tokenProperties[1..];

            return tokenProperties;
        }

        private static void ParseTokenContext(AstToken token)
        {
            if (token.fileContext == null)
            {
                return;
            }

            var contextFilename = new AstTokenContext() { sourceFile = "<invalid sloc>", column = 0, line = 0 };
            var fileContext = token.fileContext.AsSpan(1, token.fileContext.Length - 2);
            var commaPosition = fileContext.IndexOf(',');

            if (commaPosition != -1)
            {
                fileContext = fileContext.Slice(0, commaPosition);
            }

            var tokenized = fileContext.Tokenize(':');
            tokenized.MoveNext();

            if (_contextMap.TryGetMap(tokenized.Current, out var result))
            {
                switch (result)
                {
                    case ContextAction.InvalidSloc:
                        contextFilename.sourceFile = "<invalid sloc>";
                        contextFilename.column = 0;
                        contextFilename.line = 0;
                        break;

                    case ContextAction.BuildIn:
                        contextFilename.sourceFile = "<built-in>";

                        if (!tokenized.MoveNext())
                            break;

                        contextFilename.line = int.Parse(tokenized.Current);

                        if (!tokenized.MoveNext())
                            break;

                        contextFilename.column = int.Parse(tokenized.Current);
                        break;

                    case ContextAction.ScratchSpace:
                        contextFilename.sourceFile = "<built-in>";

                        if (!tokenized.MoveNext())
                            break;
                        contextFilename.line = int.Parse(tokenized.Current);

                        if (!tokenized.MoveNext())
                            break;
                        contextFilename.column = int.Parse(tokenized.Current);
                        break;

                    case ContextAction.Line:

                        if (!tokenized.MoveNext())
                            break;
                        contextFilename.line = int.Parse(tokenized.Current);

                        if (!tokenized.MoveNext())
                            break;
                        contextFilename.column = int.Parse(tokenized.Current);
                        break;

                    case ContextAction.Col:

                        if (!tokenized.MoveNext())
                            break;

                        contextFilename.column = int.Parse(tokenized.Current);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                var partCount = fileContext.Count(':');
                if (partCount > 3)
                {
                    tokenized = fileContext.Tokenize(':');
                    tokenized.MoveNext();

                    var sb = new StringBuilder(fileContext.Length);
                    sb.Append(tokenized.Current);
                    partCount--;

                    while (partCount > 3)
                    {
                        tokenized.MoveNext();
                        sb.Append(':');
                        sb.Append(tokenized.Current);
                        partCount--;
                    }

                    contextFilename.sourceFile = sb.ToString();
                    if (tokenized.MoveNext()) contextFilename.line = int.Parse(tokenized.Current);
                    if (tokenized.MoveNext()) contextFilename.column = int.Parse(tokenized.Current);
                }
            }

            token.context = contextFilename;
        }

        private static ReadOnlySpan<char> GetSufix(ReadOnlySpan<char> source)
        {
            if (source[^1] == '>')
            {
                return source.ToString();
            }

            for (var i = source.Length - 1; i >= 0; i--)
            {
                if (char.IsUpper(source[i]))
                {
                    return source.Slice(i);
                }
            }
            return source;
        }

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