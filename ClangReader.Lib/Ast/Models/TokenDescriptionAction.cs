namespace ClangReader.Lib.Ast.Models
{
    public enum TokenDescriptionAction
    {
        DoNothing,
        DeclCase,

        OffsetFirst,
        OffsetThenFileContext,
        NameThenOffset
    }
}