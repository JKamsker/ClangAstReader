namespace ClangReader
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