using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClangReader.Utilities
{
    public class EnclosureOptions
    {
        public char StartChar { get; }
        public char EndChar { get; }

        public EnclosureOptions(char startAndEndChar) : this(startAndEndChar, startAndEndChar)
        {
        }

        public EnclosureOptions(char startChar, char endChar)
        {
            StartChar = startChar;
            EndChar = endChar;
        }
    }

    public class StringSplitEngine
    {
        private readonly EnclosureOptions[] _options;
        private char[] _startChars;

        public StringSplitEngine(params EnclosureOptions[] options)
        {
            _options = options;
            _startChars = options.Select(x => x.StartChar).Append(' ').ToArray();
        }

        public string[] Split(string input)
        {
            var temp = input;
            var results = new List<string>();
            var currentOptions = _options.ToList();

            while (!string.IsNullOrEmpty(temp))
            {
                var startIndex = temp.IndexOfAny(_startChars);
                if (startIndex == -1)
                {
                    results.AddRange(temp.Split(' '));
                    break;
                }

                var character = temp[startIndex];
                if (character == ' ')
                {
                    if (startIndex != 0)
                    {
                        results.Add(temp.Substring(0, startIndex));
                    }

                    temp = temp.Substring(startIndex + 1, temp.Length - startIndex - 1);
                    continue;
                }

                var options = currentOptions.First(x => x.StartChar == character);
                if (options.StartChar == options.EndChar)
                {
                    var next = temp.IndexOf(options.StartChar, startIndex + 1);
                    if (next == -1)
                    {
                        currentOptions.Remove(options);
                        continue;
                    }

                    if (startIndex != 0)
                    {
                        results.Add(temp.Substring(0, startIndex));
                    }
                    results.Add(temp.Substring(startIndex + 1, next - startIndex - 1));
                    temp = temp.Substring(next + 1, temp.Length - next - 1);
                }
                else
                {
                    var next = temp.IndexOf(options.EndChar, startIndex + 1);
                    if (next == -1)
                    {
                        currentOptions.Remove(options);
                        continue;
                    }

                    if (startIndex != 0)
                    {
                        results.Add(temp.Substring(0, startIndex));
                    }

                    var depth = 0;
                    for (int i = startIndex; i < temp.Length; i++)
                    {
                        if (temp[i] == options.StartChar)
                        {
                            depth++;
                            continue;
                        }

                        if (temp[i] == options.EndChar)
                        {
                            depth--;
                        }

                        if (depth == 0)
                        {
                            results.Add(temp.Substring(startIndex, i - startIndex));
                            temp = temp.Substring(startIndex - i, temp.Length - (startIndex + i));
                            break;
                        }
                    }
                    Debugger.Break();
                }
            }

            return results.ToArray();
        }

        private class OptionsHelper
        {
            public EnclosureOptions Options { get; }

            public int CurrentLevel { get; set; }

            public OptionsHelper(EnclosureOptions options)
            {
                Options = options;
            }
        }
    }
}