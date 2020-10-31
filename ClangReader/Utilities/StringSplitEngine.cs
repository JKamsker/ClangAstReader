using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace ClangReader.Utilities
{

    public class StringSegmentEx
    {
        //public static string Join(params StringSegment[] segments)
        //{
        //    int len = 0;
        //    foreach (var seg in segments)
        //    {
        //        len += seg.Length;
        //    }


        //    string.Create(len, segments, (x,y) =>
        //    {
                
        //    });
        //}
    }

    public class EnclosureOptions
    {
        public char StartChar { get; }
        public char EndChar { get; }

        public char[] StartAndEnd { get; }

        public EnclosureOptions(char startAndEndChar) : this(startAndEndChar, startAndEndChar)
        {
        }

        public EnclosureOptions(char startChar, char endChar)
        {
            StartChar = startChar;
            EndChar = endChar;
            if (startChar == endChar)
            {
                StartAndEnd = new[] { startChar };
                return;
            }

            StartAndEnd = new[] { startChar, endChar };
        }
    }

    public class StringSplitEngine
    {
        private readonly EnclosureOptions[] _options;
        private char[] _startChars;

        public StringSplitEngine(params EnclosureOptions[] options)
        {
            _options = options;
            RefreshStartChars();
        }

        private void RefreshStartChars()
        {
            _startChars = GetStartChars(_options);
        }

        private char[] GetStartChars(IEnumerable<EnclosureOptions> options)
        {
            return options.Select(x => x.StartChar).Append(' ').ToArray();
        }

        public string[] Split(string input)
        {
            var temp = input;
            var results = new List<string>();

            var currentOptions = _options.ToList();
            var currentStartChars = _startChars;

            while (!string.IsNullOrEmpty(temp))
            {
                var startIndex = temp.IndexOfAny(currentStartChars);
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
                        RefreshStartChars();
                        currentOptions.Remove(options);
                        currentStartChars = GetStartChars(currentOptions);
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
                    if (startIndex != 0)
                    {
                        results.Add(temp.Substring(0, startIndex));
                    }

                    var depth = 1;
                    var currentIndex = startIndex;
                    do
                    {
                        var foundAt = temp.IndexOfAny(options.StartAndEnd, currentIndex + 1);
                        if (foundAt == -1)
                        {
                            results.Add(temp);
                            temp = string.Empty;
                            break;
                        }
                        currentIndex = foundAt;

                        var foundChar = temp[foundAt];
                        if (foundChar == options.StartChar)
                        {
                            depth++;
                            continue;
                        }

                        if (foundChar == options.EndChar)
                        {
                            depth--;
                        }

                        if (depth <= 0)
                        {
                            var end = foundAt + 1;

                            var subs = temp[startIndex..end];
                            var newTemp = temp[end..];

                            results.Add(subs);
                            temp = newTemp;
                            break;
                        }
                    } while (depth > 0 && currentIndex < temp.Length);
                }
            }

            return results.ToArray();
        }


        private static char[] Space = new[] {' '};

        public ReadOnlyCollection<StringSegment> SplitSegmented(string input)
        {
            var temp = new StringSegment(input);
            var results = new List<StringSegment>(16);

            var currentOptions = _options.ToList();
            var currentStartChars = _startChars;

            while (temp.HasValue && temp != StringSegment.Empty)
            {
                var startIndex = temp.IndexOfAny(currentStartChars);
                if (startIndex == -1)
                {
                    var segments = temp.Split(Space);
                    foreach (var segment in segments)
                    {
                        results.Add(segment);
                    }
                    break;
                }

                var character = temp[startIndex];
                if (character == ' ')
                {
                    if (startIndex != 0)
                    {
                        results.Add(temp.Subsegment(0, startIndex));
                    }

                    temp = temp.Subsegment(startIndex + 1, temp.Length - startIndex - 1);
                    continue;
                }

                var options = currentOptions.First(x => x.StartChar == character);
                if (options.StartChar == options.EndChar)
                {
                    var next = temp.IndexOf(options.StartChar, startIndex + 1);
                    if (next == -1)
                    {
                        RefreshStartChars();
                        currentOptions.Remove(options);
                        currentStartChars = GetStartChars(currentOptions);
                        continue;
                    }

                    if (startIndex != 0)
                    {
                        results.Add(temp.Subsegment(0, startIndex));
                    }
                    results.Add(temp.Subsegment(startIndex + 1, next - startIndex - 1));
                    temp = temp.Subsegment(next + 1, temp.Length - next - 1);
                }
                else
                {
                    if (startIndex != 0)
                    {
                        results.Add(temp.Subsegment(0, startIndex));
                    }

                    var depth = 1;
                    var currentIndex = startIndex;
                    do
                    {
                        var foundAt = temp.IndexOfAny(options.StartAndEnd, currentIndex + 1);
                        if (foundAt == -1)
                        {
                            results.Add(temp);
                            temp = string.Empty;
                            break;
                        }
                        currentIndex = foundAt;

                        var foundChar = temp[foundAt];
                        if (foundChar == options.StartChar)
                        {
                            depth++;
                            continue;
                        }

                        if (foundChar == options.EndChar)
                        {
                            depth--;
                        }

                        if (depth <= 0)
                        {
                            var end = foundAt + 1;

                            //var sub1 = temp.Subsegment(startIndex, end - startIndex);
                            //var sub2 = temp.Subsegment(end);

                            //var subs = temp[startIndex..end];
                            //var newTemp = temp[end..];


                            var subs = temp.Subsegment(startIndex, end - startIndex);
                            var newTemp = temp.Subsegment(end);

                            results.Add(subs);
                            temp = newTemp;
                            break;
                        }
                    } while (depth > 0 && currentIndex < temp.Length);
                }
            }

            return results.AsReadOnly();
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