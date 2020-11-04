using System;
using System.Collections.Generic;

namespace ClangReader.Lib.Ast
{
    public class AstTokenDto
    {
        public string Name { get; set; }
        public string[] Properties { get; set; } = Array.Empty<string>();
        public string[] Attributes { get; set; } = Array.Empty<string>();

        public List<AstTokenDto> Children { get; set; }
    }
}