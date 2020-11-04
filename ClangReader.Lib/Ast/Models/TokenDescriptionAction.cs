namespace ClangReader.Lib.Ast.Models
{
    internal enum TokenDescriptionAction
    {
        DoNothing,
        DeclCase,

        OffsetFirst,
        OffsetThenFileContext,
        NameThenOffset
    }
}