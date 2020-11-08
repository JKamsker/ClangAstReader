using ClangReader.Lib.Ast.Models;

namespace ClangReader.Lib.Ast.Interception
{
    public class TypeBasedFilterInterceptor : AstParserInterceptor
    {
        private readonly AstType _type;

        public TypeBasedFilterInterceptor(AstType allowedType)
        {
            _type = allowedType;
        }


        public override bool OnNodeParsed(AstFileReaderContext readerContext, AstToken token, int depth)
        {
            if (depth == 1 && token.Type != AstType.CXXMethodDecl)
            {
                readerContext.SkipSubTree();
                return true;
            }

            return false;
        }
    }
}