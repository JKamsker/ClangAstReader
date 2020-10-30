using System.Collections.Generic;
using ClangReader.Types;

namespace ClangReader.LanguageTranslation
{
    internal class TranslationFile
    {
        public Dictionary<string, TypedefDeclaration> typedef = new Dictionary<string, TypedefDeclaration>();
        public Dictionary<string, VariableDeclaration> variables = new Dictionary<string, VariableDeclaration>();
        public Dictionary<string, FunctionDeclaration> functions = new Dictionary<string, FunctionDeclaration>();
        public Dictionary<string, StructureDeclaration> structures = new Dictionary<string, StructureDeclaration>();
        public Dictionary<string, EnumDeclaration> enums = new Dictionary<string, EnumDeclaration>();
    }
}