
using System;
using System.Collections.Generic;
using System.Text;

namespace ClangReader.Utilities
{
    public readonly ref struct ModifyableString
    {

        public unsafe ModifyableString(int length)
        {
            var allocated = new string('\0', length);
            StringValue = allocated;

            fixed (char* destinationPtr = allocated)
            {
                SpanValue = new Span<char>(destinationPtr, length);
            }
        }

        public readonly string StringValue;
        public readonly Span<char> SpanValue;
    }
}
