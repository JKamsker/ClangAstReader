using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace ClangReader
{
    public class StringArrayEnumerator //: IEnumerable<Span<string>>
    {
        public string Input { get; }

        public ReadOnlySpan<char> this[int index]
        {
            get
            {
                return string.Empty.AsSpan();
            }
        }

        public StringArrayEnumerator(string input)
        {
            Input = input;
        }

        public static IReadOnlyList<StringSegment> GetStringArray(string source)
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

        public static IReadOnlyList<StringSegment> GetStringArray_CachedLength(string source)
        {
            var sourceCopy = new StringSegment(source);
            var sclength = sourceCopy.Length;

            // var sourceCopy = source.AsSpan();
            // Avg: 15,..
            // To 16 cause its a natural cap
            var parts = new List<StringSegment>(16);

            var depth = 0;
            var quotedSingle = false;
            var quotedDouble = false;
            while (sclength > 0)
            {
                var tryAgain = false;
                for (var i = 0; i < sclength; i++)
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
                        sclength = sourceCopy.Length;
                        tryAgain = true;
                        break;
                    }
                }

                if (tryAgain) continue;

                parts.Add(sourceCopy);
                sourceCopy = StringSegment.Empty;
                sclength = 0;
            }

            //maxsize = Math.Max(parts.Count, maxsize);
            //sizes.Add(parts.Count);
            return parts.AsReadOnly();
        }

        //public static int maxsize = 0;
        //public static List<int> sizes = new List<int>();

        public static string[] GetStringArray_Old(string source)
        {
            //var seg = new ArraySegment<char>(source.ToCharArray());
            //seg[0] = 'd';

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

        public static string[] GetStringArray_reallyold(string source)
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
    }
}