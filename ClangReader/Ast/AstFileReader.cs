using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

using ClangReader.Utilities;

namespace ClangReader.Ast
{
    public class AstFileReader
    {
        private readonly string _filePath;

        public AstFileReader(string filePath)
        {
            _filePath = filePath;
        }

        private char[] _spaceChar = new[] { ' ' };

        public void Parse()
        {
            var fastReader = new FastLineReader(_filePath);

            foreach (var rawLine in fastReader.ReadLine())
            {
                var line = rawLine.Span;

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

                tokenEnd = line.IndexOf(' ');

                ReadOnlySpan<char> token;
                ReadOnlySpan<char> declaration = ReadOnlySpan<char>.Empty;

                if (tokenEnd == -1)
                {
                    token = line[tokenStart..];
                }
                else
                {
                    token = line[tokenStart..tokenEnd];
                    declaration = line.Slice(tokenEnd + 1);
                }


            }
        }
    }
}