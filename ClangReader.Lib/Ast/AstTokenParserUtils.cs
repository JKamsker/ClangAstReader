using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using ClangReader.Lib.Ast.Models;
using ClangReader.Lib.Collections;
using ClangReader.Lib.Extensions;

using Microsoft.Toolkit.HighPerformance.Extensions;

namespace ClangReader.Lib.Ast
{
    public class AstTokenParserUtils
    {

        static AstTokenParserUtils()
        {
           
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static void GetEssentialPart(ReadOnlyArraySegment<char> line, out int lineDepth, out ReadOnlyArraySegment<char> essential)
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
        public static void ParseTokenAndDescription(ReadOnlyArraySegment<char> line, out ReadOnlyArraySegment<char> token, out ReadOnlyArraySegment<char> description)
        {
            var tokenEnd = line.IndexOf(' ');
            if (tokenEnd == -1)
            {
                token = line;
                description = ReadOnlyArraySegment<char>.Empty;
            }
            else
            {
                token = line[..tokenEnd];
                description = line.Slice(tokenEnd + 1);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static void ParseTokenAndDescription(ReadOnlyArraySegment<char> line, int tokenStart, out ReadOnlyArraySegment<char> token, out ReadOnlyArraySegment<char> description)
        {
            var tokenEnd = line.IndexOf(' ', tokenStart);
            if (tokenEnd == -1)
            {
                token = line[tokenStart..];
                description = ReadOnlyArraySegment<char>.Empty;
            }
            else
            {
                token = line[tokenStart..tokenEnd];
                description = line.Slice(tokenEnd + 1);
            }
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

                        sourceCopy = sourceCopy.Slice(i + 1);
                        tryAgain = true;
                        break;
                    }
                }

                if (tryAgain) continue;

                {
                    var cutTighter1 = sourceCopy[0] == '\'' && sourceCopy[^1] == '\'';
                    if (cutTighter1)
                    {
                        parts.Add(sourceCopy[1..^1]);
                    }
                    else
                    {
                        parts.Add(sourceCopy);
                    }
                }

                return parts;
            }

            return parts;
        }

        public static ReadOnlySpan<char> GetSufixFromName(ReadOnlySpan<char> source)
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

        public static void ParseTokenDescription(AstToken token, ReadOnlyArraySegment<char> line)
        {
            AstTokenParserUtils.ParseTokenAndDescription(line, out var name, out var description);

            if (StringMappings.KnownTokenType.TryGetMap(name.Span, out var tokenType))
            {
                token.Type = tokenType;
            }
            else
            {
                token.unknownName = name.ToString();
                
            }


            var parameterStartIndex = 0;
            var parameters = AstTokenParserUtils.GetSegments(description).ToArray();

            var sufix = AstTokenParserUtils.GetSufixFromName(name.Span);
            if (StringMappings.TokenDescriptionActionMap.TryGetMap(sufix, out var action))
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
                        token.unknownName = string.Concat(name.Span, parameters[0]);
                        token.offset = parameters[1].ToString();
                        parameterStartIndex = 2;
                        break;

                    case TokenDescriptionAction.DoNothing:
                    default:
                        break;
                }
            }

            //token.properties = parameters.Where((value, index) => index >= parameterStartIndex).Select(x => x.ToString()).ToArray();
            if (parameterStartIndex > 0)
            {
                parameters = parameters[parameterStartIndex..];
            }
            ParseTokenContext(token);

            ParseTokenPropertiesAndAttributes(token, parameters);
        }

        private static void ParseTokenPropertiesAndAttributes(AstToken token, ReadOnlyArraySegment<char>[] parameters)
        {
            if (parameters.Length <= 0)
            {
                return;
            }

            var tokenProperties = parameters.AsSpan();
            var tokenAttributes = new List<string>();

            AstAttributes attributes = default;

            //tokenProperties = PushStartToAttributesIfNecessary(tokenProperties, tokenAttributes, "implicit");
            //tokenProperties = PushStartToAttributesIfNecessary(tokenProperties, tokenAttributes, "used");
            //tokenProperties = PushStartToAttributesIfNecessary(tokenProperties, tokenAttributes, "referenced");

            PushStartToAttributesIfNecessary(ref tokenProperties, ref attributes, "implicit", AstAttributes.Implicit);
            PushStartToAttributesIfNecessary(ref tokenProperties, ref attributes, "used", AstAttributes.Used);
            PushStartToAttributesIfNecessary(ref tokenProperties, ref attributes, "referenced", AstAttributes.Referenced);
            PushStartToAttributesIfNecessary(ref tokenProperties, ref attributes, "struct", AstAttributes.Struct);
            PushStartToAttributesIfNecessary(ref tokenProperties, ref attributes, "_GUID", AstAttributes.Guid);
            PushStartToAttributesIfNecessary(ref tokenProperties, ref attributes, "Default", AstAttributes.Default);

            PushEndToAttributesIfNecessary(ref tokenProperties, ref attributes, "__int128", AstAttributes.CInit);
            PushEndToAttributesIfNecessary(ref tokenProperties, ref attributes, "__int128_t", AstAttributes.CInit);

            PushEndToAttributesIfNecessary(ref tokenProperties, ref attributes, "cinit", AstAttributes.CInit);
            PushEndToAttributesIfNecessary(ref tokenProperties, ref attributes, "extern", AstAttributes.Extern);
            PushEndToAttributesIfNecessary(ref tokenProperties, ref attributes, "callinit", AstAttributes.Callinit);
            PushEndToAttributesIfNecessary(ref tokenProperties, ref attributes, "static", AstAttributes.Static);
            PushEndToAttributesIfNecessary(ref tokenProperties, ref attributes, "definition", AstAttributes.Definition);
            PushEndToAttributesIfNecessary(ref tokenProperties, ref attributes, "nrvo", AstAttributes.Nrvo);

            //tokenProperties = PushEndToAttributesIfNecessary(tokenProperties, tokenAttributes, "cinit");
            //tokenProperties = PushEndToAttributesIfNecessary(tokenProperties, tokenAttributes, "extern");
            //tokenProperties = PushEndToAttributesIfNecessary(tokenProperties, tokenAttributes, "callinit");
            //tokenProperties = PushEndToAttributesIfNecessary(tokenProperties, tokenAttributes, "static");
            //tokenProperties = PushEndToAttributesIfNecessary(tokenProperties, tokenAttributes, "definition");
            //tokenProperties = PushEndToAttributesIfNecessary(tokenProperties, tokenAttributes, "nrvo");

            token.Attributes = attributes;
            if (tokenAttributes.Count != 0)
            {
                token.additionalAttributes = tokenAttributes.ToArray();
            }

            if (tokenProperties.Length != 0)
            {
                token.properties = new string[tokenProperties.Length];
                for (var index = 0; index < tokenProperties.Length; index++)
                {
                    token.properties[index] = tokenProperties[index].ToString();
                }
            }
        }

        private static void PushStartToAttributesIfNecessary
        (
            ref Span<ReadOnlyArraySegment<char>> tokenProperties,
            ref AstAttributes attributes,
            ReadOnlySpan<char> token,
            AstAttributes sourceAttrib
        )
        {
            if (tokenProperties.Length <= 0 || !tokenProperties[0].Span.Equals(token, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            //tokenAttributes.Add(tokenProperties[0].ToString());
            attributes |= sourceAttrib;
            tokenProperties = tokenProperties[1..];

            return;
        }

        private static void PushEndToAttributesIfNecessary
        (
            ref Span<ReadOnlyArraySegment<char>> tokenProperties,
            ref AstAttributes attributes,
            ReadOnlySpan<char> token,
            AstAttributes sourceAttrib
        )
        {
            if (tokenProperties.Length <= 0 || !tokenProperties[^1].Span.Equals(token, StringComparison.OrdinalIgnoreCase))
            {
                //return tokenProperties;
                return;
            }

            //tokenAttributes.Add(tokenProperties[^1].ToString());
            attributes |= sourceAttrib;
            tokenProperties = tokenProperties[..^1];

            return;
        }

        private static Span<ReadOnlyArraySegment<char>> PushEndToAttributesIfNecessary
        (
            Span<ReadOnlyArraySegment<char>> tokenProperties,
            List<string> tokenAttributes,
            ReadOnlySpan<char> token
        )
        {
            if (tokenProperties.Length <= 0 || !tokenProperties[^1].Span.SequenceEqual(token))
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
            if (tokenProperties.Length <= 0 || !tokenProperties[0].Span.SequenceEqual(token))
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

            if (StringMappings.ContextMap.TryGetMap(tokenized.Current, out var result))
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
    }
}