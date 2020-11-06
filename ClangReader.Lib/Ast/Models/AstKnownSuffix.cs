﻿namespace ClangReader.Lib.Ast.Models
{
    public enum AstKnownSuffix
    {
        //Cursors
        NOT_PARSED,

        Cursor,
        Attr,
        InheritableAttr,
        InheritableParamAttr,
        ParameterABIAttr,
        StmtAttr,
        TypeAttr,
        AccessSpecDecl,
        BindingDecl,
        BlockDecl,
        BuiltinTemplateDecl,
        CapturedDecl,
        ClassScopeFunctionSpecializationDecl,
        ClassTemplateDecl,
        ClassTemplatePartialSpecializationDecl,
        ClassTemplateSpecializationDecl,
        ConceptDecl,
        ConstructorUsingShadowDecl,
        CXXConstructorDecl,
        CXXConversionDecl,
        CXXDeductionGuideDecl,
        CXXDestructorDecl,
        CXXMethodDecl,
        CXXRecordDecl,
        Decl,
        DeclaratorDecl,
        DecompositionDecl,
        EmptyDecl,
        EnumConstantDecl,
        EnumDecl,
        ExportDecl,
        ExternCContextDecl,
        FieldDecl,
        FileScopeAsmDecl,
        FriendDecl,
        FriendTemplateDecl,
        FunctionDecl,
        FunctionTemplateDecl,
        ImplicitParamDecl,
        ImportDecl,
        IndirectFieldDecl,
        LabelDecl,
        LifetimeExtendedTemporaryDecl,
        LinkageSpecDecl,
        MSGuidDecl,
        MSPropertyDecl,
        NamedDecl,
        NamespaceAliasDecl,
        NamespaceDecl,
        NonTypeTemplateParmDecl,
        ObjCAtDefsFieldDecl,
        ObjCCategoryDecl,
        ObjCCategoryImplDecl,
        ObjCCompatibleAliasDecl,
        ObjCContainerDecl,
        ObjCImplDecl,
        ObjCImplementationDecl,
        ObjCInterfaceDecl,
        ObjCIvarDecl,
        ObjCMethodDecl,
        ObjCPropertyDecl,
        ObjCPropertyImplDecl,
        ObjCProtocolDecl,
        ObjCTypeParamDecl,
        OMPAllocateDecl,
        OMPCapturedExprDecl,
        OMPDeclareMapperDecl,
        OMPDeclareReductionDecl,
        OMPRequiresDecl,
        OMPThreadPrivateDecl,
        ParmVarDecl,
        PragmaCommentDecl,
        PragmaDetectMismatchDecl,
        RecordDecl,
        RedeclarableTemplateDecl,
        RequiresExprBodyDecl,
        StaticAssertDecl,
        TagDecl,
        TemplateDecl,
        TemplateTemplateParmDecl,
        TemplateTypeParmDecl,
        TranslationUnitDecl,
        TypeAliasDecl,
        TypeAliasTemplateDecl,
        TypeDecl,
        TypedefDecl,
        TypedefNameDecl,
        UnresolvedUsingTypenameDecl,
        UnresolvedUsingValueDecl,
        UsingDecl,
        UsingDirectiveDecl,
        UsingPackDecl,
        UsingShadowDecl,
        ValueDecl,
        VarDecl,
        VarTemplateDecl,
        VarTemplatePartialSpecializationDecl,
        VarTemplateSpecializationDecl,
        AbstractConditionalOperator,
        AddrLabelExpr,
        ArrayInitIndexExpr,
        ArrayInitLoopExpr,
        ArraySubscriptExpr,
        ArrayTypeTraitExpr,
        AsTypeExpr,
        AtomicExpr,
        BinaryConditionalOperator,
        BinaryOperator,
        BlockExpr,
        BuiltinBitCastExpr,
        CallExpr,
        CastExpr,
        CharacterLiteral,
        ChooseExpr,
        CoawaitExpr,
        CompoundAssignOperator,
        CompoundLiteralExpr,
        ConceptSpecializationExpr,
        ConditionalOperator,
        ConstantExpr,
        ConvertVectorExpr,
        CoroutineSuspendExpr,
        CoyieldExpr,
        CStyleCastExpr,
        CUDAKernelCallExpr,
        CXXAddrspaceCastExpr,
        CXXBindTemporaryExpr,
        CXXBoolLiteralExpr,
        CXXConstCastExpr,
        CXXConstructExpr,
        CXXDefaultArgExpr,
        CXXDefaultInitExpr,
        CXXDeleteExpr,
        CXXDependentScopeMemberExpr,
        CXXDynamicCastExpr,
        CXXFoldExpr,
        CXXFunctionalCastExpr,
        CXXInheritedCtorInitExpr,
        CXXMemberCallExpr,
        CXXNamedCastExpr,
        CXXNewExpr,
        CXXNoexceptExpr,
        CXXNullPtrLiteralExpr,
        CXXOperatorCallExpr,
        CXXPseudoDestructorExpr,
        CXXReinterpretCastExpr,
        CXXRewrittenBinaryOperator,
        CXXScalarValueInitExpr,
        CXXStaticCastExpr,
        CXXStdInitializerListExpr,
        CXXTemporaryObjectExpr,
        CXXThisExpr,
        CXXThrowExpr,
        CXXTypeidExpr,
        CXXUnresolvedConstructExpr,
        CXXUuidofExpr,
        DeclRefExpr,
        DependentCoawaitExpr,
        DependentScopeDeclRefExpr,
        DesignatedInitExpr,
        DesignatedInitUpdateExpr,
        ExplicitCastExpr,
        Expr,
        ExpressionTraitExpr,
        ExprWithCleanups,
        ExtVectorElementExpr,
        FixedPointLiteral,
        FloatingLiteral,
        FullExpr,
        FunctionParmPackExpr,
        GenericSelectionExpr,
        GNUNullExpr,
        ImaginaryLiteral,
        ImplicitCastExpr,
        ImplicitValueInitExpr,
        InitListExpr,
        IntegerLiteral,
        LambdaExpr,
        MaterializeTemporaryExpr,
        MatrixSubscriptExpr,
        MemberExpr,
        MSPropertyRefExpr,
        MSPropertySubscriptExpr,
        NoInitExpr,
        ObjCArrayLiteral,
        ObjCAvailabilityCheckExpr,
        ObjCBoolLiteralExpr,
        ObjCBoxedExpr,
        ObjCBridgedCastExpr,
        ObjCDictionaryLiteral,
        ObjCEncodeExpr,
        ObjCIndirectCopyRestoreExpr,
        ObjCIsaExpr,
        ObjCIvarRefExpr,
        ObjCMessageExpr,
        ObjCPropertyRefExpr,
        ObjCProtocolExpr,
        ObjCSelectorExpr,
        ObjCStringLiteral,
        ObjCSubscriptRefExpr,
        OffsetOfExpr,
        OMPArraySectionExpr,
        OMPArrayShapingExpr,
        OMPIteratorExpr,
        OpaqueValueExpr,
        OverloadExpr,
        PackExpansionExpr,
        ParenExpr,
        ParenListExpr,
        PredefinedExpr,
        PseudoObjectExpr,
        RecoveryExpr,
        RequiresExpr,
        ShuffleVectorExpr,
        SizeOfPackExpr,
        SourceLocExpr,
        StmtExpr,
        StringLiteral,
        SubstNonTypeTemplateParmExpr,
        SubstNonTypeTemplateParmPackExpr,
        TypeTraitExpr,
        TypoExpr,
        UnaryExprOrTypeTraitExpr,
        UnaryOperator,
        UnresolvedLookupExpr,
        UnresolvedMemberExpr,
        UserDefinedLiteral,
        VAArgExpr,
        InclusionDirective,
        MacroDefinitionRecord,
        MacroExpansion,
        PreprocessedEntity,
        PreprocessingDirective,
        CXXBaseSpecifier,
        Ref,
        AsmStmt,
        AttributedStmt,
        BreakStmt,
        CapturedStmt,
        CaseStmt,
        CompoundStmt,
        ContinueStmt,
        CoreturnStmt,
        CoroutineBodyStmt,
        CXXCatchStmt,
        CXXForRangeStmt,
        CXXTryStmt,
        DeclStmt,
        DefaultStmt,
        DoStmt,
        ForStmt,
        GCCAsmStmt,
        GotoStmt,
        IfStmt,
        IndirectGotoStmt,
        LabelStmt,
        MSAsmStmt,
        MSDependentExistsStmt,
        NullStmt,
        ObjCAtCatchStmt,
        ObjCAtFinallyStmt,
        ObjCAtSynchronizedStmt,
        ObjCAtThrowStmt,
        ObjCAtTryStmt,
        ObjCAutoreleasePoolStmt,
        ObjCForCollectionStmt,
        OMPAtomicDirective,
        OMPBarrierDirective,
        OMPCancelDirective,
        OMPCancellationPointDirective,
        OMPCriticalDirective,
        OMPDepobjDirective,
        OMPDistributeDirective,
        OMPDistributeParallelForDirective,
        OMPDistributeParallelForSimdDirective,
        OMPDistributeSimdDirective,
        OMPExecutableDirective,
        OMPFlushDirective,
        OMPForDirective,
        OMPForSimdDirective,
        OMPLoopDirective,
        OMPMasterDirective,
        OMPMasterTaskLoopDirective,
        OMPMasterTaskLoopSimdDirective,
        OMPOrderedDirective,
        OMPParallelDirective,
        OMPParallelForDirective,
        OMPParallelForSimdDirective,
        OMPParallelMasterDirective,
        OMPParallelMasterTaskLoopDirective,
        OMPParallelMasterTaskLoopSimdDirective,
        OMPParallelSectionsDirective,
        OMPScanDirective,
        OMPSectionDirective,
        OMPSectionsDirective,
        OMPSimdDirective,
        OMPSingleDirective,
        OMPTargetDataDirective,
        OMPTargetDirective,
        OMPTargetEnterDataDirective,
        OMPTargetExitDataDirective,
        OMPTargetParallelDirective,
        OMPTargetParallelForDirective,
        OMPTargetParallelForSimdDirective,
        OMPTargetSimdDirective,
        OMPTargetTeamsDirective,
        OMPTargetTeamsDistributeDirective,
        OMPTargetTeamsDistributeParallelForDirective,
        OMPTargetTeamsDistributeParallelForSimdDirective,
        OMPTargetTeamsDistributeSimdDirective,
        OMPTargetUpdateDirective,
        OMPTaskDirective,
        OMPTaskgroupDirective,
        OMPTaskLoopDirective,
        OMPTaskLoopSimdDirective,
        OMPTaskwaitDirective,
        OMPTaskyieldDirective,
        OMPTeamsDirective,
        OMPTeamsDistributeDirective,
        OMPTeamsDistributeParallelForDirective,
        OMPTeamsDistributeParallelForSimdDirective,
        OMPTeamsDistributeSimdDirective,
        ReturnStmt,
        SEHExceptStmt,
        SEHFinallyStmt,
        SEHLeaveStmt,
        SEHTryStmt,
        Stmt,
        SwitchCase,
        SwitchStmt,
        ValueStmt,
        WhileStmt,

        //Types
        AdjustedType,

        ArrayType,
        AtomicType,
        AttributedType,
        AutoType,
        BlockPointerType,
        BuiltinType,
        ComplexType,
        ConstantArrayType,
        ConstantMatrixType,
        DecayedType,
        DecltypeType,
        DeducedTemplateSpecializationType,
        DeducedType,
        DependentAddressSpaceType,
        DependentExtIntType,
        DependentNameType,
        DependentSizedArrayType,
        DependentSizedExtVectorType,
        DependentSizedMatrixType,
        DependentTemplateSpecializationType,
        DependentVectorType,
        ElaboratedType,
        EnumType,
        ExtIntType,
        ExtVectorType,
        FunctionNoProtoType,
        FunctionProtoType,
        FunctionType,
        IncompleteArrayType,
        InjectedClassNameType,
        LValueReferenceType,
        MacroQualifiedType,
        MatrixType,
        MemberPointerType,
        ObjCInterfaceType,
        ObjCObjectPointerType,
        ObjCObjectType,
        ObjCTypeParamType,
        PackExpansionType,
        ParenType,
        PipeType,
        PointerType,
        RecordType,
        ReferenceType,
        RValueReferenceType,
        SubstTemplateTypeParmPackType,
        SubstTemplateTypeParmType,
        TagType,
        TemplateSpecializationType,
        TemplateTypeParmType,
        Type,
        TypedefType,
        TypeOfExprType,
        TypeOfType,
        TypeWithKeyword,
        UnaryTransformType,
        UnresolvedUsingType,
        VariableArrayType,
        VectorType,

        TypeVisibilityAttr,

        //Generated by source

        CXXRecord,
        BuiltinAttr,
        NoThrowAttr,
        DefinitionData,
        DefaultConstructor,
        CopyConstructor,
        MoveConstructor,
        CopyAssignment,
        MoveAssignment,
        Destructor,
        MaxFieldAlignmentAttr,
        TemplateArgument,
        QualType,
        Value,
        TemplateTypeParm,
        Typedef,
        CXXCtorInitializer,
        Original,
        VisibilityAttr,
        ReturnsNonNullAttr,
        AllocSizeAttr,
        WarnUnusedResultAttr,
        MSAllocatorAttr,
        AllocAlignAttr,
        Enum,
        DeprecatedAttr,
        PureAttr,
        AlwaysInlineAttr,
        Field,
        ClassTemplateSpecialization,
        AlignedAttr,
        ConstAttr,
        DLLImportAttr,
        Public,
        NoInlineAttr,
        FormatAttr,
        RestrictAttr,
        MSNoVTableAttr,
        Overrides,
        NoAliasAttr,
        UuidAttr,
        Function,
        MSInheritanceAttr,
        SelectAnyAttr,
        NULL,
        TypeAlias,
        PointerAttr,
        MSGuid,
        Var,
        ReturnsTwiceAttr,
        Inherited,
        Private,
        SectionAttr,
        OwnerAttr,
        CXX11NoReturnAttr,
        FinalAttr,
        ClassTemplatePartialSpecialization,
        Array_filler,
        Element,
        Elements,
        Target,
        Nominated,
        Constructed,
        OverrideAttr,
        Filler,
        UnresolvedUsingTypename,
        Virtual,
        MSVtorDispAttr,
        FullComment,
        ParagraphComment,
        TextComment,
        VerbatimLineComment,
        BlockCommandComment,
        ParamCommandComment
    }
}