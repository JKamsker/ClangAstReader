using System;
using System.Collections.Generic;
using System.Text;

namespace ClangReader.Utilities
{
    public static class StringExtensions
    {
        public static string TrimStart
        (
            this string target, 
            string trimString, 
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase
        )
        {
            if (string.IsNullOrEmpty(trimString)) return target;

            string result = target;
            while (result.StartsWith(trimString, stringComparison))
            {
                result = result.Substring(trimString.Length);
            }

            return result;
        }
    }
}