using System;
using System.Collections.Generic;
using System.Text;
using ClangReader.Lib.Ast.Models;

namespace ClangReader.Lib.Ast.Interception
{
    public abstract class AstParserInterceptor
    {
        public AstParserInterceptor()
        {
            
        }

        public abstract bool OnNodeParsed(AstFileReaderContext readerContext, AstToken token, int depth);
    }
}
