using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Extensions.Primitives;

namespace ClangReader
{
    public class AstTokenContext
    {
        public string sourceFile;
        public int line;
        public int column;
    }

    public class AstToken
    {
        public string name;

        public string offset;
        public string relationOffset;
        public string fileContext;
        public string filePointer;

        public AstTokenContext context;
        public string[] properties = Array.Empty<string>();
        public string[] attributes = Array.Empty<string>();

        public List<AstToken> children = new List<AstToken>();
        public AstToken parent;

        public override string ToString()
        {
            return name;
        }

        public IEnumerable<AstToken> TraverseParents(bool includeThis = false)
        {
            if (includeThis)
            {
                yield return this;
            }

            if (parent == null)
            {
                yield break;
            }

            foreach (var parents in parent.TraverseParents(true))
            {
                yield return parents;
            }
        }

        public AstTokenDto AsTokenDto()
        {
            var thisCopy = new AstTokenDto
            {
                Name = name,
                Attributes = attributes,
                Properties = properties,
                Children = new List<AstTokenDto>(children.Count),
            };

            foreach (var child in children)
            {
                thisCopy.Children.Add(child.AsTokenDto());
            }

            return thisCopy;
        }
    }

    public class AstTokenDto
    {
        public string Name { get; set; }
        public string[] Properties { get; set; } = Array.Empty<string>();
        public string[] Attributes { get; set; } = Array.Empty<string>();

        public List<AstTokenDto> Children { get; set; }
    }

    public class AstTextFile
    {
        public string filename;
        public List<AstToken> rootTokens;

        public AstTextFile(string astDumpPath)
        {
            this.filename = astDumpPath;

            //  string[] lines = System.IO.File.ReadAllLines();

            var lines = File.ReadLines(astDumpPath);

            rootTokens = new List<AstToken>();
            var currentTokens = new List<AstToken>();

            var contextFilename = new AstTokenContext() { sourceFile = "<invalid sloc>", column = 0, line = 0 };
            foreach (var line in lines)
            {
                var lineDepth = 0;
                var tokenStart = -1;
                var tokenEnd = -1;

                for (var i = 0; i < line.Length; i += 2)
                {
                    if (line[i] == '|' || line[i] == '`' || line[i] == ' ' || line[i] == '-')
                    {
                        lineDepth++;
                        continue;
                    }

                    tokenStart = i;
                    break;
                }

                tokenEnd = line.IndexOf(' ', tokenStart);
                var token = "";
                var declaration = "";
                if (tokenEnd == -1)
                {
                    token = line.Substring(tokenStart, line.Length - tokenStart);
                }
                else
                {
                    token = line.Substring(tokenStart, tokenEnd - tokenStart);
                    declaration = line.Substring(tokenEnd + 1);
                }

                var astToken = ParseTokenDescription(token, declaration, contextFilename);
                if (lineDepth == 0)
                {
                    rootTokens.Add(astToken);
                    if (currentTokens.Count == 0)
                    {
                        currentTokens.Add(astToken);
                    }
                    else
                    {
                        currentTokens[0] = astToken;
                    }
                }
                else
                {
                    if (lineDepth >= currentTokens.Count)
                    {
                        currentTokens.Add(astToken);
                    }
                    else
                    {
                        currentTokens[lineDepth] = astToken;
                    }

                    currentTokens[lineDepth - 1].children.Add(astToken);
                    astToken.parent = currentTokens[lineDepth - 1];
                }
            }
        }

        protected static string GetSufix(string source)
        {
            if (source[source.Length - 1] == '>') return source;
            for (var i = source.Length - 1; i >= 0; i--)
            {
                if (char.IsUpper(source[i])) return source.Substring(i);
            }
            return source;
        }

        protected static string[] GetStringArray_o(string source)
        {
            var sourceCopy = source;
            var parts = new List<string>();

            var depth = 0;
            var quotedSingle = false;
            var quotedDouble = false;
            while (sourceCopy.Length > 0)
            {
                var tryAgain = false;
                for (var i = 0; i < sourceCopy.Length; i++)
                {
                    if (sourceCopy[i] == '<' && !quotedDouble && !quotedSingle)
                    {
                        depth++;
                    }
                    else if (sourceCopy[i] == '>' && !quotedDouble && !quotedSingle && depth > 0)
                    {
                        depth--;
                    }
                    else if (sourceCopy[i] == '\'' && !quotedDouble)
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
                    else if (sourceCopy[i] == '\"' && !quotedSingle)
                    {
                        if (quotedDouble) depth--;
                        else depth++;
                        quotedDouble = !quotedDouble;
                    }
                    else if (sourceCopy[i] == ' ' && depth == 0)
                    {
                        var cutTighter = sourceCopy[0] == '\'' && sourceCopy[i - 1] == '\'';

                        var subvalue = cutTighter ?
                            sourceCopy.Substring(1, i - 2) :
                            sourceCopy.Substring(0, i);

                        parts.Add(subvalue);

                        sourceCopy = sourceCopy.Substring(i + 1);
                        tryAgain = true;
                        break;
                    }
                }

                if (tryAgain) continue;

                parts.Add(sourceCopy);
                sourceCopy = "";
            }

            return parts.ToArray();
        }

        protected static string[] GetStringArray(string source)
        {
            var sourceCopy = source.AsSpan();
            var parts = new List<string>();

            var depth = 0;
            var quotedSingle = false;
            var quotedDouble = false;
            while (sourceCopy.Length > 0)
            {
                var tryAgain = false;
                for (var i = 0; i < sourceCopy.Length; i++)
                {
                    if (sourceCopy[i] == '<' && !quotedDouble && !quotedSingle)
                    {
                        depth++;
                    }
                    else if (sourceCopy[i] == '>' && !quotedDouble && !quotedSingle && depth > 0)
                    {
                        depth--;
                    }
                    else if (sourceCopy[i] == '\'' && !quotedDouble)
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
                    else if (sourceCopy[i] == '\"' && !quotedSingle)
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
                    else if (sourceCopy[i] == ' ' && depth == 0)
                    {
                        var cutTighter = sourceCopy[0] == '\'' && sourceCopy[i - 1] == '\'';

                        var subvalue = cutTighter ?
                            sourceCopy.Slice(1, i - 2) :
                            sourceCopy.Slice(0, i);

                        parts.Add(subvalue.ToString());

                        sourceCopy = sourceCopy.Slice(i + 1);
                        tryAgain = true;
                        break;
                    }
                }

                if (tryAgain) continue;

                parts.Add(sourceCopy.ToString());
                sourceCopy = Span<char>.Empty;
            }

            return parts.ToArray();
        }

        public static IReadOnlyList<StringSegment> GetStringSegments(string source)
        {
            var sourceCopy = new StringSegment(source);

            // var sourceCopy = source.AsSpan();
            // Avg: 15,..
            // To 16 cause its a natural cap
            var parts = new List<StringSegment>(16);

            var depth = 0;
            var quotedSingle = false;
            var quotedDouble = false;
            while (sourceCopy.Length > 0)
            {
                var tryAgain = false;
                for (var i = 0; i < sourceCopy.Length; i++)
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
                            sourceCopy.Subsegment(1, i - 2) :
                            sourceCopy.Subsegment(0, i);

                        parts.Add(subvalue);

                        sourceCopy = sourceCopy.Subsegment(i + 1);
                        tryAgain = true;
                        break;
                    }
                }

                if (tryAgain) continue;

                parts.Add(sourceCopy);
                sourceCopy = StringSegment.Empty;
            }

            //maxsize = Math.Max(parts.Count, maxsize);
            //sizes.Add(parts.Count);
            return parts.AsReadOnly();
        }

        private static AstToken ParseTokenDescription(string name, string description, AstTokenContext contextFilename)
        {
            var token = new AstToken() { name = name };
            var parameters = GetStringSegments(description);
            var parameterStartIndex = 0;

            var sufix = GetSufix(name);
            switch (sufix)
            {
                case "Decl":
                    token.offset = parameters[0].ToString();

                    parameterStartIndex = 1;

                    var declFileContext = parameters[parameterStartIndex];

                    //token.fileContext = parameters[parameterStartIndex].ToString();
                    while (!declFileContext.StartsWith("<", StringComparison.Ordinal))
                    {
                        if (token.relationOffset != null)
                        {
                            token.relationOffset = token.relationOffset + " " + parameters[parameterStartIndex] + " " + parameters[parameterStartIndex + 1];
                        }
                        else
                        {
                            token.relationOffset = parameters[parameterStartIndex] + " " + parameters[parameterStartIndex + 1];
                        }

                        parameterStartIndex += 2;

                        declFileContext = parameters[parameterStartIndex];
                    }

                    token.fileContext = declFileContext.ToString();
                    token.filePointer = parameters[parameterStartIndex + 1].ToString();
                    parameterStartIndex += 2;
                    break;

                case "Type":
                case "Record":
                case "Typedef":
                case "Parm":
                case "Specialization":
                case "Function":
                case "Enum":
                case "Field":
                case "Alias":
                case "Comment":
                case "Var":

                    token.offset = parameters[0].ToString();
                    parameterStartIndex = 1;
                    break;

                case "Attr":
                case "Expr":
                case "Literal":
                case "Operator":
                case "Stmt":
                case "Cleanups":
                    token.offset = parameters[0].ToString();
                    token.fileContext = parameters[1].ToString();
                    parameterStartIndex = 2;
                    break;

                case "Data":
                case "Constructor":
                case "Assignment":
                case "Destructor":
                case "Argument":
                case "Initializer":
                case "public":
                case "private":
                case "protected":
                case "virtual":
                case "<<<NULL>>>":
                case "Overrides:":
                case "...":
                case "array":

                case "value:":
                case "Guid":
                case "inherited": //unsure

                    break;

                case "original":
                    token.name = name + parameters[0];
                    token.offset = parameters[1].ToString();
                    parameterStartIndex = 2;
                    break;

                default: break; //throw new NotImplementedException(name);
            }

            token.properties = parameters.Where((value, index) => index >= parameterStartIndex).Select(x => x.ToString()).ToArray();

            if (token.fileContext == null)
            {
                return token;
            }

            var fileContext = token.fileContext.Substring(1, token.fileContext.Length - 2);

            if (fileContext.Contains(",")) fileContext = fileContext.Substring(0, fileContext.IndexOf(","));

            var parts = fileContext.Split(':');

            while (parts.Length > 3)
            {
                var newPart = string.Join(":", parts[0], parts[1]);

                parts = Enumerable.Empty<string>()
                    .Concat(new[] { newPart })
                    .Concat(parts.Skip(2))
                    .ToArray();
            }

            if (parts[0] == "<invalid sloc>")
            {
                contextFilename.sourceFile = "<invalid sloc>";
                contextFilename.column = 0;
                contextFilename.line = 0;
            }
            else if (parts[0] == "<built-in>")
            {
                contextFilename.sourceFile = "<built-in>";
                contextFilename.line = int.Parse(parts[1]);
                contextFilename.column = int.Parse(parts[2]);
            }
            else if (parts[0] == "<scratch space>")
            {
                contextFilename.sourceFile = "<built-in>";
                contextFilename.line = int.Parse(parts[1]);
                contextFilename.column = int.Parse(parts[2]);
            }
            else if (parts[0] == "line")
            {
                contextFilename.line = int.Parse(parts[1]);
                contextFilename.column = int.Parse(parts[2]);
            }
            else if (parts[0] == "col")
            {
                contextFilename.column = int.Parse(parts[1]);
            }
            else
            {
                contextFilename.sourceFile = parts[0];
                contextFilename.line = int.Parse(parts[1]);
                contextFilename.column = int.Parse(parts[2]);
            }

            token.context = new AstTokenContext()
            {
                column = contextFilename.column,
                line = contextFilename.line,
                sourceFile = contextFilename.sourceFile
            };

            // remove service properties

            if (token.properties.Length > 0 && token.properties[0] == "implicit")
            {
                token.attributes = token.attributes.Append(token.properties[0]).ToArray();
                token.properties = token.properties.Where((value, index) => index >= 1).ToArray();
            }

            if (token.properties.Length > 0 && token.properties[0] == "used")
            {
                token.attributes = token.attributes.Append(token.properties[0]).ToArray();
                token.properties = token.properties.Where((value, index) => index >= 1).ToArray();
            }

            if (token.properties.Length > 0 && token.properties[0] == "referenced")
            {
                token.attributes = token.attributes.Append(token.properties[0]).ToArray();
                token.properties = token.properties.Where((value, index) => index >= 1).ToArray();
            }

            if (token.properties.Length > 0 && token.properties[^1] == "cinit")
            {
                token.attributes = token.attributes.Append(token.properties[^1]).ToArray();
                token.properties = token.properties.Where((value, index) => index < token.properties.Length - 1).ToArray();
            }

            if (token.properties.Length > 0 && token.properties[^1] == "extern")
            {
                token.attributes = token.attributes.Append(token.properties[^1]).ToArray();
                token.properties = token.properties.Where((value, index) => index < token.properties.Length - 1).ToArray();
            }

            if (token.properties.Length > 0 && token.properties[^1] == "callinit")
            {
                token.attributes = token.attributes.Append(token.properties[^1]).ToArray();
                token.properties = token.properties.Where((value, index) => index < token.properties.Length - 1).ToArray();
            }

            if (token.properties.Length > 0 && token.properties[^1] == "static")
            {
                token.attributes = token.attributes.Append(token.properties[^1]).ToArray();
                token.properties = token.properties.Where((value, index) => index < token.properties.Length - 1).ToArray();
            }

            if (token.properties.Length > 0 && token.properties[^1] == "definition")
            {
                token.attributes = token.attributes.Append(token.properties[^1]).ToArray();
                token.properties = token.properties.Where((value, index) => index < token.properties.Length - 1).ToArray();
            }

            if (token.properties.Length > 0 && token.properties[^1] == "nrvo")
            {
                token.attributes = token.attributes.Append(token.properties[^1]).ToArray();
                token.properties = token.properties.Where((value, index) => index < token.properties.Length - 1).ToArray();
            }

            return token;
        }
    }
}