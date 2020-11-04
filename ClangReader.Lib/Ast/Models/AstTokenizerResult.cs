using ClangReader.Models;

namespace ClangReader.Utilities
{
    public struct AstTokenizerResult
    {
        public int LineDepth { get; }
        public ReadOnlyArraySegment<char> Token { get; }
        public ReadOnlyArraySegment<char> Declaration { get; }

        public AstTokenizerResult(int lineDepth, in ReadOnlyArraySegment<char> token, in ReadOnlyArraySegment<char> declaration)
        {
            LineDepth = lineDepth;
            Token = token;
            Declaration = declaration;
        }

        public void Deconstruct(out int lineDepth, out ReadOnlyArraySegment<char> token, out ReadOnlyArraySegment<char> declaration)
        {
            token = Token;
            declaration = Declaration;
            lineDepth = LineDepth;
        }
    }
}