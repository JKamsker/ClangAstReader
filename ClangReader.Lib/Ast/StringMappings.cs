﻿using System;
using System.Collections.Generic;
using System.Text;
using ClangReader.Lib.Ast.Models;
using ClangReader.Lib.Collections;

namespace ClangReader.Lib.Ast
{
    internal static class StringMappings
    {
        internal static readonly StringToEnumMapper<TokenDescriptionAction> TokenDescriptionActionMap;
        internal static readonly StringToEnumMapper<ContextAction> ContextMap;
        internal static readonly StringToEnumMapper<AstType> KnownTokenType;


        static StringMappings()
        {
            TokenDescriptionActionMap = new StringToEnumMapper<TokenDescriptionAction>
            {
                { "Decl", TokenDescriptionAction.DeclCase },

                { "Type", TokenDescriptionAction.OffsetFirst },
                { "Record", TokenDescriptionAction.OffsetFirst },
                { "Typedef", TokenDescriptionAction.OffsetFirst },
                { "Parm", TokenDescriptionAction.OffsetFirst },
                { "Specialization", TokenDescriptionAction.OffsetFirst },
                { "Function", TokenDescriptionAction.OffsetFirst },
                { "Enum", TokenDescriptionAction.OffsetFirst },
                { "Field", TokenDescriptionAction.OffsetFirst },
                { "Alias", TokenDescriptionAction.OffsetFirst },
                { "Comment", TokenDescriptionAction.OffsetFirst },
                { "Var", TokenDescriptionAction.OffsetFirst },

                { "Attr", TokenDescriptionAction.OffsetThenFileContext },
                { "Expr", TokenDescriptionAction.OffsetThenFileContext },
                { "Literal", TokenDescriptionAction.OffsetThenFileContext },
                { "Operator", TokenDescriptionAction.OffsetThenFileContext },
                { "Stmt", TokenDescriptionAction.OffsetThenFileContext },
                { "Cleanups", TokenDescriptionAction.OffsetThenFileContext },

                { "original", TokenDescriptionAction.NameThenOffset },

                { "Data", TokenDescriptionAction.DoNothing },
                { "Constructor", TokenDescriptionAction.DoNothing },
                { "Assignment", TokenDescriptionAction.DoNothing },
                { "Destructor", TokenDescriptionAction.DoNothing },
                { "Argument", TokenDescriptionAction.DoNothing },
                { "Initializer", TokenDescriptionAction.DoNothing },
                { "public", TokenDescriptionAction.DoNothing },
                { "private", TokenDescriptionAction.DoNothing },
                { "protected", TokenDescriptionAction.DoNothing },
                { "virtual", TokenDescriptionAction.DoNothing },
                { "<<<NULL>>>", TokenDescriptionAction.DoNothing },
                { "Overrides:", TokenDescriptionAction.DoNothing },
                { "...", TokenDescriptionAction.DoNothing },
                { "array", TokenDescriptionAction.DoNothing },
                { "value:", TokenDescriptionAction.DoNothing },
                { "Guid", TokenDescriptionAction.DoNothing },
                { "inherited", TokenDescriptionAction.DoNothing },
            };

            ContextMap = new StringToEnumMapper<ContextAction>
            {
                { "<invalid sloc>", ContextAction.InvalidSloc },
                { "<built-in>", ContextAction.BuildIn },
                { "<scratch space>", ContextAction.ScratchSpace },
                { "line", ContextAction.Line },
                { "col", ContextAction.Col }
            };

            KnownTokenType = new StringToEnumMapper<AstType>
            {
                { "Cursor", AstType.Cursor },
                { "Attr", AstType.Attr },
                { "InheritableAttr", AstType.InheritableAttr },
                { "InheritableParamAttr", AstType.InheritableParamAttr },
                { "ParameterABIAttr", AstType.ParameterABIAttr },
                { "StmtAttr", AstType.StmtAttr },
                { "TypeAttr", AstType.TypeAttr },
                { "AccessSpecDecl", AstType.AccessSpecDecl },
                { "BindingDecl", AstType.BindingDecl },
                { "BlockDecl", AstType.BlockDecl },
                { "BuiltinTemplateDecl", AstType.BuiltinTemplateDecl },
                { "CapturedDecl", AstType.CapturedDecl },
                { "ClassScopeFunctionSpecializationDecl", AstType.ClassScopeFunctionSpecializationDecl },
                { "ClassTemplateDecl", AstType.ClassTemplateDecl },
                { "ClassTemplatePartialSpecializationDecl", AstType.ClassTemplatePartialSpecializationDecl },
                { "ClassTemplateSpecializationDecl", AstType.ClassTemplateSpecializationDecl },
                { "ConceptDecl", AstType.ConceptDecl },
                { "ConstructorUsingShadowDecl", AstType.ConstructorUsingShadowDecl },
                { "CXXConstructorDecl", AstType.CXXConstructorDecl },
                { "CXXConversionDecl", AstType.CXXConversionDecl },
                { "CXXDeductionGuideDecl", AstType.CXXDeductionGuideDecl },
                { "CXXDestructorDecl", AstType.CXXDestructorDecl },
                { "CXXMethodDecl", AstType.CXXMethodDecl },
                { "CXXRecordDecl", AstType.CXXRecordDecl },
                { "Decl", AstType.Decl },
                { "DeclaratorDecl", AstType.DeclaratorDecl },
                { "DecompositionDecl", AstType.DecompositionDecl },
                { "EmptyDecl", AstType.EmptyDecl },
                { "EnumConstantDecl", AstType.EnumConstantDecl },
                { "EnumDecl", AstType.EnumDecl },
                { "ExportDecl", AstType.ExportDecl },
                { "ExternCContextDecl", AstType.ExternCContextDecl },
                { "FieldDecl", AstType.FieldDecl },
                { "FileScopeAsmDecl", AstType.FileScopeAsmDecl },
                { "FriendDecl", AstType.FriendDecl },
                { "FriendTemplateDecl", AstType.FriendTemplateDecl },
                { "FunctionDecl", AstType.FunctionDecl },
                { "FunctionTemplateDecl", AstType.FunctionTemplateDecl },
                { "ImplicitParamDecl", AstType.ImplicitParamDecl },
                { "ImportDecl", AstType.ImportDecl },
                { "IndirectFieldDecl", AstType.IndirectFieldDecl },
                { "LabelDecl", AstType.LabelDecl },
                { "LifetimeExtendedTemporaryDecl", AstType.LifetimeExtendedTemporaryDecl },
                { "LinkageSpecDecl", AstType.LinkageSpecDecl },
                { "MSGuidDecl", AstType.MSGuidDecl },
                { "MSPropertyDecl", AstType.MSPropertyDecl },
                { "NamedDecl", AstType.NamedDecl },
                { "NamespaceAliasDecl", AstType.NamespaceAliasDecl },
                { "NamespaceDecl", AstType.NamespaceDecl },
                { "NonTypeTemplateParmDecl", AstType.NonTypeTemplateParmDecl },
                { "ObjCAtDefsFieldDecl", AstType.ObjCAtDefsFieldDecl },
                { "ObjCCategoryDecl", AstType.ObjCCategoryDecl },
                { "ObjCCategoryImplDecl", AstType.ObjCCategoryImplDecl },
                { "ObjCCompatibleAliasDecl", AstType.ObjCCompatibleAliasDecl },
                { "ObjCContainerDecl", AstType.ObjCContainerDecl },
                { "ObjCImplDecl", AstType.ObjCImplDecl },
                { "ObjCImplementationDecl", AstType.ObjCImplementationDecl },
                { "ObjCInterfaceDecl", AstType.ObjCInterfaceDecl },
                { "ObjCIvarDecl", AstType.ObjCIvarDecl },
                { "ObjCMethodDecl", AstType.ObjCMethodDecl },
                { "ObjCPropertyDecl", AstType.ObjCPropertyDecl },
                { "ObjCPropertyImplDecl", AstType.ObjCPropertyImplDecl },
                { "ObjCProtocolDecl", AstType.ObjCProtocolDecl },
                { "ObjCTypeParamDecl", AstType.ObjCTypeParamDecl },
                { "OMPAllocateDecl", AstType.OMPAllocateDecl },
                { "OMPCapturedExprDecl", AstType.OMPCapturedExprDecl },
                { "OMPDeclareMapperDecl", AstType.OMPDeclareMapperDecl },
                { "OMPDeclareReductionDecl", AstType.OMPDeclareReductionDecl },
                { "OMPRequiresDecl", AstType.OMPRequiresDecl },
                { "OMPThreadPrivateDecl", AstType.OMPThreadPrivateDecl },
                { "ParmVarDecl", AstType.ParmVarDecl },
                { "PragmaCommentDecl", AstType.PragmaCommentDecl },
                { "PragmaDetectMismatchDecl", AstType.PragmaDetectMismatchDecl },
                { "RecordDecl", AstType.RecordDecl },
                { "RedeclarableTemplateDecl", AstType.RedeclarableTemplateDecl },
                { "RequiresExprBodyDecl", AstType.RequiresExprBodyDecl },
                { "StaticAssertDecl", AstType.StaticAssertDecl },
                { "TagDecl", AstType.TagDecl },
                { "TemplateDecl", AstType.TemplateDecl },
                { "TemplateTemplateParmDecl", AstType.TemplateTemplateParmDecl },
                { "TemplateTypeParmDecl", AstType.TemplateTypeParmDecl },
                { "TranslationUnitDecl", AstType.TranslationUnitDecl },
                { "TypeAliasDecl", AstType.TypeAliasDecl },
                { "TypeAliasTemplateDecl", AstType.TypeAliasTemplateDecl },
                { "TypeDecl", AstType.TypeDecl },
                { "TypedefDecl", AstType.TypedefDecl },
                { "TypedefNameDecl", AstType.TypedefNameDecl },
                { "UnresolvedUsingTypenameDecl", AstType.UnresolvedUsingTypenameDecl },
                { "UnresolvedUsingValueDecl", AstType.UnresolvedUsingValueDecl },
                { "UsingDecl", AstType.UsingDecl },
                { "UsingDirectiveDecl", AstType.UsingDirectiveDecl },
                { "UsingPackDecl", AstType.UsingPackDecl },
                { "UsingShadowDecl", AstType.UsingShadowDecl },
                { "ValueDecl", AstType.ValueDecl },
                { "VarDecl", AstType.VarDecl },
                { "VarTemplateDecl", AstType.VarTemplateDecl },
                { "VarTemplatePartialSpecializationDecl", AstType.VarTemplatePartialSpecializationDecl },
                { "VarTemplateSpecializationDecl", AstType.VarTemplateSpecializationDecl },
                { "AbstractConditionalOperator", AstType.AbstractConditionalOperator },
                { "AddrLabelExpr", AstType.AddrLabelExpr },
                { "ArrayInitIndexExpr", AstType.ArrayInitIndexExpr },
                { "ArrayInitLoopExpr", AstType.ArrayInitLoopExpr },
                { "ArraySubscriptExpr", AstType.ArraySubscriptExpr },
                { "ArrayTypeTraitExpr", AstType.ArrayTypeTraitExpr },
                { "AsTypeExpr", AstType.AsTypeExpr },
                { "AtomicExpr", AstType.AtomicExpr },
                { "BinaryConditionalOperator", AstType.BinaryConditionalOperator },
                { "BinaryOperator", AstType.BinaryOperator },
                { "BlockExpr", AstType.BlockExpr },
                { "BuiltinBitCastExpr", AstType.BuiltinBitCastExpr },
                { "CallExpr", AstType.CallExpr },
                { "CastExpr", AstType.CastExpr },
                { "CharacterLiteral", AstType.CharacterLiteral },
                { "ChooseExpr", AstType.ChooseExpr },
                { "CoawaitExpr", AstType.CoawaitExpr },
                { "CompoundAssignOperator", AstType.CompoundAssignOperator },
                { "CompoundLiteralExpr", AstType.CompoundLiteralExpr },
                { "ConceptSpecializationExpr", AstType.ConceptSpecializationExpr },
                { "ConditionalOperator", AstType.ConditionalOperator },
                { "ConstantExpr", AstType.ConstantExpr },
                { "ConvertVectorExpr", AstType.ConvertVectorExpr },
                { "CoroutineSuspendExpr", AstType.CoroutineSuspendExpr },
                { "CoyieldExpr", AstType.CoyieldExpr },
                { "CStyleCastExpr", AstType.CStyleCastExpr },
                { "CUDAKernelCallExpr", AstType.CUDAKernelCallExpr },
                { "CXXAddrspaceCastExpr", AstType.CXXAddrspaceCastExpr },
                { "CXXBindTemporaryExpr", AstType.CXXBindTemporaryExpr },
                { "CXXBoolLiteralExpr", AstType.CXXBoolLiteralExpr },
                { "CXXConstCastExpr", AstType.CXXConstCastExpr },
                { "CXXConstructExpr", AstType.CXXConstructExpr },
                { "CXXDefaultArgExpr", AstType.CXXDefaultArgExpr },
                { "CXXDefaultInitExpr", AstType.CXXDefaultInitExpr },
                { "CXXDeleteExpr", AstType.CXXDeleteExpr },
                { "CXXDependentScopeMemberExpr", AstType.CXXDependentScopeMemberExpr },
                { "CXXDynamicCastExpr", AstType.CXXDynamicCastExpr },
                { "CXXFoldExpr", AstType.CXXFoldExpr },
                { "CXXFunctionalCastExpr", AstType.CXXFunctionalCastExpr },
                { "CXXInheritedCtorInitExpr", AstType.CXXInheritedCtorInitExpr },
                { "CXXMemberCallExpr", AstType.CXXMemberCallExpr },
                { "CXXNamedCastExpr", AstType.CXXNamedCastExpr },
                { "CXXNewExpr", AstType.CXXNewExpr },
                { "CXXNoexceptExpr", AstType.CXXNoexceptExpr },
                { "CXXNullPtrLiteralExpr", AstType.CXXNullPtrLiteralExpr },
                { "CXXOperatorCallExpr", AstType.CXXOperatorCallExpr },
                { "CXXPseudoDestructorExpr", AstType.CXXPseudoDestructorExpr },
                { "CXXReinterpretCastExpr", AstType.CXXReinterpretCastExpr },
                { "CXXRewrittenBinaryOperator", AstType.CXXRewrittenBinaryOperator },
                { "CXXScalarValueInitExpr", AstType.CXXScalarValueInitExpr },
                { "CXXStaticCastExpr", AstType.CXXStaticCastExpr },
                { "CXXStdInitializerListExpr", AstType.CXXStdInitializerListExpr },
                { "CXXTemporaryObjectExpr", AstType.CXXTemporaryObjectExpr },
                { "CXXThisExpr", AstType.CXXThisExpr },
                { "CXXThrowExpr", AstType.CXXThrowExpr },
                { "CXXTypeidExpr", AstType.CXXTypeidExpr },
                { "CXXUnresolvedConstructExpr", AstType.CXXUnresolvedConstructExpr },
                { "CXXUuidofExpr", AstType.CXXUuidofExpr },
                { "DeclRefExpr", AstType.DeclRefExpr },
                { "DependentCoawaitExpr", AstType.DependentCoawaitExpr },
                { "DependentScopeDeclRefExpr", AstType.DependentScopeDeclRefExpr },
                { "DesignatedInitExpr", AstType.DesignatedInitExpr },
                { "DesignatedInitUpdateExpr", AstType.DesignatedInitUpdateExpr },
                { "ExplicitCastExpr", AstType.ExplicitCastExpr },
                { "Expr", AstType.Expr },
                { "ExpressionTraitExpr", AstType.ExpressionTraitExpr },
                { "ExprWithCleanups", AstType.ExprWithCleanups },
                { "ExtVectorElementExpr", AstType.ExtVectorElementExpr },
                { "FixedPointLiteral", AstType.FixedPointLiteral },
                { "FloatingLiteral", AstType.FloatingLiteral },
                { "FullExpr", AstType.FullExpr },
                { "FunctionParmPackExpr", AstType.FunctionParmPackExpr },
                { "GenericSelectionExpr", AstType.GenericSelectionExpr },
                { "GNUNullExpr", AstType.GNUNullExpr },
                { "ImaginaryLiteral", AstType.ImaginaryLiteral },
                { "ImplicitCastExpr", AstType.ImplicitCastExpr },
                { "ImplicitValueInitExpr", AstType.ImplicitValueInitExpr },
                { "InitListExpr", AstType.InitListExpr },
                { "IntegerLiteral", AstType.IntegerLiteral },
                { "LambdaExpr", AstType.LambdaExpr },
                { "MaterializeTemporaryExpr", AstType.MaterializeTemporaryExpr },
                { "MatrixSubscriptExpr", AstType.MatrixSubscriptExpr },
                { "MemberExpr", AstType.MemberExpr },
                { "MSPropertyRefExpr", AstType.MSPropertyRefExpr },
                { "MSPropertySubscriptExpr", AstType.MSPropertySubscriptExpr },
                { "NoInitExpr", AstType.NoInitExpr },
                { "ObjCArrayLiteral", AstType.ObjCArrayLiteral },
                { "ObjCAvailabilityCheckExpr", AstType.ObjCAvailabilityCheckExpr },
                { "ObjCBoolLiteralExpr", AstType.ObjCBoolLiteralExpr },
                { "ObjCBoxedExpr", AstType.ObjCBoxedExpr },
                { "ObjCBridgedCastExpr", AstType.ObjCBridgedCastExpr },
                { "ObjCDictionaryLiteral", AstType.ObjCDictionaryLiteral },
                { "ObjCEncodeExpr", AstType.ObjCEncodeExpr },
                { "ObjCIndirectCopyRestoreExpr", AstType.ObjCIndirectCopyRestoreExpr },
                { "ObjCIsaExpr", AstType.ObjCIsaExpr },
                { "ObjCIvarRefExpr", AstType.ObjCIvarRefExpr },
                { "ObjCMessageExpr", AstType.ObjCMessageExpr },
                { "ObjCPropertyRefExpr", AstType.ObjCPropertyRefExpr },
                { "ObjCProtocolExpr", AstType.ObjCProtocolExpr },
                { "ObjCSelectorExpr", AstType.ObjCSelectorExpr },
                { "ObjCStringLiteral", AstType.ObjCStringLiteral },
                { "ObjCSubscriptRefExpr", AstType.ObjCSubscriptRefExpr },
                { "OffsetOfExpr", AstType.OffsetOfExpr },
                { "OMPArraySectionExpr", AstType.OMPArraySectionExpr },
                { "OMPArrayShapingExpr", AstType.OMPArrayShapingExpr },
                { "OMPIteratorExpr", AstType.OMPIteratorExpr },
                { "OpaqueValueExpr", AstType.OpaqueValueExpr },
                { "OverloadExpr", AstType.OverloadExpr },
                { "PackExpansionExpr", AstType.PackExpansionExpr },
                { "ParenExpr", AstType.ParenExpr },
                { "ParenListExpr", AstType.ParenListExpr },
                { "PredefinedExpr", AstType.PredefinedExpr },
                { "PseudoObjectExpr", AstType.PseudoObjectExpr },
                { "RecoveryExpr", AstType.RecoveryExpr },
                { "RequiresExpr", AstType.RequiresExpr },
                { "ShuffleVectorExpr", AstType.ShuffleVectorExpr },
                { "SizeOfPackExpr", AstType.SizeOfPackExpr },
                { "SourceLocExpr", AstType.SourceLocExpr },
                { "StmtExpr", AstType.StmtExpr },
                { "StringLiteral", AstType.StringLiteral },
                { "SubstNonTypeTemplateParmExpr", AstType.SubstNonTypeTemplateParmExpr },
                { "SubstNonTypeTemplateParmPackExpr", AstType.SubstNonTypeTemplateParmPackExpr },
                { "TypeTraitExpr", AstType.TypeTraitExpr },
                { "TypoExpr", AstType.TypoExpr },
                { "UnaryExprOrTypeTraitExpr", AstType.UnaryExprOrTypeTraitExpr },
                { "UnaryOperator", AstType.UnaryOperator },
                { "UnresolvedLookupExpr", AstType.UnresolvedLookupExpr },
                { "UnresolvedMemberExpr", AstType.UnresolvedMemberExpr },
                { "UserDefinedLiteral", AstType.UserDefinedLiteral },
                { "VAArgExpr", AstType.VAArgExpr },
                { "InclusionDirective", AstType.InclusionDirective },
                { "MacroDefinitionRecord", AstType.MacroDefinitionRecord },
                { "MacroExpansion", AstType.MacroExpansion },
                { "PreprocessedEntity", AstType.PreprocessedEntity },
                { "PreprocessingDirective", AstType.PreprocessingDirective },
                { "CXXBaseSpecifier", AstType.CXXBaseSpecifier },
                { "Ref", AstType.Ref },
                { "AsmStmt", AstType.AsmStmt },
                { "AttributedStmt", AstType.AttributedStmt },
                { "BreakStmt", AstType.BreakStmt },
                { "CapturedStmt", AstType.CapturedStmt },
                { "CaseStmt", AstType.CaseStmt },
                { "CompoundStmt", AstType.CompoundStmt },
                { "ContinueStmt", AstType.ContinueStmt },
                { "CoreturnStmt", AstType.CoreturnStmt },
                { "CoroutineBodyStmt", AstType.CoroutineBodyStmt },
                { "CXXCatchStmt", AstType.CXXCatchStmt },
                { "CXXForRangeStmt", AstType.CXXForRangeStmt },
                { "CXXTryStmt", AstType.CXXTryStmt },
                { "DeclStmt", AstType.DeclStmt },
                { "DefaultStmt", AstType.DefaultStmt },
                { "DoStmt", AstType.DoStmt },
                { "ForStmt", AstType.ForStmt },
                { "GCCAsmStmt", AstType.GCCAsmStmt },
                { "GotoStmt", AstType.GotoStmt },
                { "IfStmt", AstType.IfStmt },
                { "IndirectGotoStmt", AstType.IndirectGotoStmt },
                { "LabelStmt", AstType.LabelStmt },
                { "MSAsmStmt", AstType.MSAsmStmt },
                { "MSDependentExistsStmt", AstType.MSDependentExistsStmt },
                { "NullStmt", AstType.NullStmt },
                { "ObjCAtCatchStmt", AstType.ObjCAtCatchStmt },
                { "ObjCAtFinallyStmt", AstType.ObjCAtFinallyStmt },
                { "ObjCAtSynchronizedStmt", AstType.ObjCAtSynchronizedStmt },
                { "ObjCAtThrowStmt", AstType.ObjCAtThrowStmt },
                { "ObjCAtTryStmt", AstType.ObjCAtTryStmt },
                { "ObjCAutoreleasePoolStmt", AstType.ObjCAutoreleasePoolStmt },
                { "ObjCForCollectionStmt", AstType.ObjCForCollectionStmt },
                { "OMPAtomicDirective", AstType.OMPAtomicDirective },
                { "OMPBarrierDirective", AstType.OMPBarrierDirective },
                { "OMPCancelDirective", AstType.OMPCancelDirective },
                { "OMPCancellationPointDirective", AstType.OMPCancellationPointDirective },
                { "OMPCriticalDirective", AstType.OMPCriticalDirective },
                { "OMPDepobjDirective", AstType.OMPDepobjDirective },
                { "OMPDistributeDirective", AstType.OMPDistributeDirective },
                { "OMPDistributeParallelForDirective", AstType.OMPDistributeParallelForDirective },
                { "OMPDistributeParallelForSimdDirective", AstType.OMPDistributeParallelForSimdDirective },
                { "OMPDistributeSimdDirective", AstType.OMPDistributeSimdDirective },
                { "OMPExecutableDirective", AstType.OMPExecutableDirective },
                { "OMPFlushDirective", AstType.OMPFlushDirective },
                { "OMPForDirective", AstType.OMPForDirective },
                { "OMPForSimdDirective", AstType.OMPForSimdDirective },
                { "OMPLoopDirective", AstType.OMPLoopDirective },
                { "OMPMasterDirective", AstType.OMPMasterDirective },
                { "OMPMasterTaskLoopDirective", AstType.OMPMasterTaskLoopDirective },
                { "OMPMasterTaskLoopSimdDirective", AstType.OMPMasterTaskLoopSimdDirective },
                { "OMPOrderedDirective", AstType.OMPOrderedDirective },
                { "OMPParallelDirective", AstType.OMPParallelDirective },
                { "OMPParallelForDirective", AstType.OMPParallelForDirective },
                { "OMPParallelForSimdDirective", AstType.OMPParallelForSimdDirective },
                { "OMPParallelMasterDirective", AstType.OMPParallelMasterDirective },
                { "OMPParallelMasterTaskLoopDirective", AstType.OMPParallelMasterTaskLoopDirective },
                { "OMPParallelMasterTaskLoopSimdDirective", AstType.OMPParallelMasterTaskLoopSimdDirective },
                { "OMPParallelSectionsDirective", AstType.OMPParallelSectionsDirective },
                { "OMPScanDirective", AstType.OMPScanDirective },
                { "OMPSectionDirective", AstType.OMPSectionDirective },
                { "OMPSectionsDirective", AstType.OMPSectionsDirective },
                { "OMPSimdDirective", AstType.OMPSimdDirective },
                { "OMPSingleDirective", AstType.OMPSingleDirective },
                { "OMPTargetDataDirective", AstType.OMPTargetDataDirective },
                { "OMPTargetDirective", AstType.OMPTargetDirective },
                { "OMPTargetEnterDataDirective", AstType.OMPTargetEnterDataDirective },
                { "OMPTargetExitDataDirective", AstType.OMPTargetExitDataDirective },
                { "OMPTargetParallelDirective", AstType.OMPTargetParallelDirective },
                { "OMPTargetParallelForDirective", AstType.OMPTargetParallelForDirective },
                { "OMPTargetParallelForSimdDirective", AstType.OMPTargetParallelForSimdDirective },
                { "OMPTargetSimdDirective", AstType.OMPTargetSimdDirective },
                { "OMPTargetTeamsDirective", AstType.OMPTargetTeamsDirective },
                { "OMPTargetTeamsDistributeDirective", AstType.OMPTargetTeamsDistributeDirective },
                { "OMPTargetTeamsDistributeParallelForDirective", AstType.OMPTargetTeamsDistributeParallelForDirective },
                { "OMPTargetTeamsDistributeParallelForSimdDirective", AstType.OMPTargetTeamsDistributeParallelForSimdDirective },
                { "OMPTargetTeamsDistributeSimdDirective", AstType.OMPTargetTeamsDistributeSimdDirective },
                { "OMPTargetUpdateDirective", AstType.OMPTargetUpdateDirective },
                { "OMPTaskDirective", AstType.OMPTaskDirective },
                { "OMPTaskgroupDirective", AstType.OMPTaskgroupDirective },
                { "OMPTaskLoopDirective", AstType.OMPTaskLoopDirective },
                { "OMPTaskLoopSimdDirective", AstType.OMPTaskLoopSimdDirective },
                { "OMPTaskwaitDirective", AstType.OMPTaskwaitDirective },
                { "OMPTaskyieldDirective", AstType.OMPTaskyieldDirective },
                { "OMPTeamsDirective", AstType.OMPTeamsDirective },
                { "OMPTeamsDistributeDirective", AstType.OMPTeamsDistributeDirective },
                { "OMPTeamsDistributeParallelForDirective", AstType.OMPTeamsDistributeParallelForDirective },
                { "OMPTeamsDistributeParallelForSimdDirective", AstType.OMPTeamsDistributeParallelForSimdDirective },
                { "OMPTeamsDistributeSimdDirective", AstType.OMPTeamsDistributeSimdDirective },
                { "ReturnStmt", AstType.ReturnStmt },
                { "SEHExceptStmt", AstType.SEHExceptStmt },
                { "SEHFinallyStmt", AstType.SEHFinallyStmt },
                { "SEHLeaveStmt", AstType.SEHLeaveStmt },
                { "SEHTryStmt", AstType.SEHTryStmt },
                { "Stmt", AstType.Stmt },
                { "SwitchCase", AstType.SwitchCase },
                { "SwitchStmt", AstType.SwitchStmt },
                { "ValueStmt", AstType.ValueStmt },
                { "WhileStmt", AstType.WhileStmt },

                //Types
                { "AdjustedType", AstType.AdjustedType },
                { "ArrayType", AstType.ArrayType },
                { "AtomicType", AstType.AtomicType },
                { "AttributedType", AstType.AttributedType },
                { "AutoType", AstType.AutoType },
                { "BlockPointerType", AstType.BlockPointerType },
                { "BuiltinType", AstType.BuiltinType },
                { "ComplexType", AstType.ComplexType },
                { "ConstantArrayType", AstType.ConstantArrayType },
                { "ConstantMatrixType", AstType.ConstantMatrixType },
                { "DecayedType", AstType.DecayedType },
                { "DecltypeType", AstType.DecltypeType },
                { "DeducedTemplateSpecializationType", AstType.DeducedTemplateSpecializationType },
                { "DeducedType", AstType.DeducedType },
                { "DependentAddressSpaceType", AstType.DependentAddressSpaceType },
                { "DependentExtIntType", AstType.DependentExtIntType },
                { "DependentNameType", AstType.DependentNameType },
                { "DependentSizedArrayType", AstType.DependentSizedArrayType },
                { "DependentSizedExtVectorType", AstType.DependentSizedExtVectorType },
                { "DependentSizedMatrixType", AstType.DependentSizedMatrixType },
                { "DependentTemplateSpecializationType", AstType.DependentTemplateSpecializationType },
                { "DependentVectorType", AstType.DependentVectorType },
                { "ElaboratedType", AstType.ElaboratedType },
                { "EnumType", AstType.EnumType },
                { "ExtIntType", AstType.ExtIntType },
                { "ExtVectorType", AstType.ExtVectorType },
                { "FunctionNoProtoType", AstType.FunctionNoProtoType },
                { "FunctionProtoType", AstType.FunctionProtoType },
                { "FunctionType", AstType.FunctionType },
                { "IncompleteArrayType", AstType.IncompleteArrayType },
                { "InjectedClassNameType", AstType.InjectedClassNameType },
                { "LValueReferenceType", AstType.LValueReferenceType },
                { "MacroQualifiedType", AstType.MacroQualifiedType },
                { "MatrixType", AstType.MatrixType },
                { "MemberPointerType", AstType.MemberPointerType },
                { "ObjCInterfaceType", AstType.ObjCInterfaceType },
                { "ObjCObjectPointerType", AstType.ObjCObjectPointerType },
                { "ObjCObjectType", AstType.ObjCObjectType },
                { "ObjCTypeParamType", AstType.ObjCTypeParamType },
                { "PackExpansionType", AstType.PackExpansionType },
                { "ParenType", AstType.ParenType },
                { "PipeType", AstType.PipeType },
                { "PointerType", AstType.PointerType },
                { "RecordType", AstType.RecordType },
                { "ReferenceType", AstType.ReferenceType },
                { "RValueReferenceType", AstType.RValueReferenceType },
                { "SubstTemplateTypeParmPackType", AstType.SubstTemplateTypeParmPackType },
                { "SubstTemplateTypeParmType", AstType.SubstTemplateTypeParmType },
                { "TagType", AstType.TagType },
                { "TemplateSpecializationType", AstType.TemplateSpecializationType },
                { "TemplateTypeParmType", AstType.TemplateTypeParmType },
                { "Type", AstType.Type },
                { "TypedefType", AstType.TypedefType },
                { "TypeOfExprType", AstType.TypeOfExprType },
                { "TypeOfType", AstType.TypeOfType },
                { "TypeWithKeyword", AstType.TypeWithKeyword },
                { "UnaryTransformType", AstType.UnaryTransformType },
                { "UnresolvedUsingType", AstType.UnresolvedUsingType },
                { "VariableArrayType", AstType.VariableArrayType },
                { "VectorType", AstType.VectorType },



                //Own
                { "TypeVisibilityAttr", AstType.TypeVisibilityAttr },

                //own generated

                { "CXXRecord", AstType.CXXRecord },
                { "BuiltinAttr", AstType.BuiltinAttr },
                { "NoThrowAttr", AstType.NoThrowAttr },
                { "DefinitionData", AstType.DefinitionData },
                { "DefaultConstructor", AstType.DefaultConstructor },
                { "CopyConstructor", AstType.CopyConstructor },
                { "MoveConstructor", AstType.MoveConstructor },
                { "CopyAssignment", AstType.CopyAssignment },
                { "MoveAssignment", AstType.MoveAssignment },
                { "Destructor", AstType.Destructor },
                { "MaxFieldAlignmentAttr", AstType.MaxFieldAlignmentAttr },
                { "TemplateArgument", AstType.TemplateArgument },
                { "QualType", AstType.QualType },
                { "value:", AstType.Value },
                { "TemplateTypeParm", AstType.TemplateTypeParm },
                { "Typedef", AstType.Typedef },
                { "CXXCtorInitializer", AstType.CXXCtorInitializer },
                { "original", AstType.Original },
                { "VisibilityAttr", AstType.VisibilityAttr },
                { "ReturnsNonNullAttr", AstType.ReturnsNonNullAttr },
                { "AllocSizeAttr", AstType.AllocSizeAttr },
                { "WarnUnusedResultAttr", AstType.WarnUnusedResultAttr },
                { "MSAllocatorAttr", AstType.MSAllocatorAttr },
                { "AllocAlignAttr", AstType.AllocAlignAttr },
                { "Enum", AstType.Enum },
                { "DeprecatedAttr", AstType.DeprecatedAttr },
                { "PureAttr", AstType.PureAttr },
                { "AlwaysInlineAttr", AstType.AlwaysInlineAttr },
                { "Field", AstType.Field },
                { "ClassTemplateSpecialization", AstType.ClassTemplateSpecialization },
                { "AlignedAttr", AstType.AlignedAttr },
                { "ConstAttr", AstType.ConstAttr },
                { "DLLImportAttr", AstType.DLLImportAttr },
                { "public", AstType.Public },
                { "NoInlineAttr", AstType.NoInlineAttr },
                { "FormatAttr", AstType.FormatAttr },
                { "RestrictAttr", AstType.RestrictAttr },
                { "MSNoVTableAttr", AstType.MSNoVTableAttr },
                { "Overrides:", AstType.Overrides },
                { "NoAliasAttr", AstType.NoAliasAttr },
                { "UuidAttr", AstType.UuidAttr },
                { "Function", AstType.Function },
                { "MSInheritanceAttr", AstType.MSInheritanceAttr },
                { "SelectAnyAttr", AstType.SelectAnyAttr },
                { "<<<NULL>>>", AstType.NULL },
                { "TypeAlias", AstType.TypeAlias },
                { "PointerAttr", AstType.PointerAttr },
                { "MSGuid", AstType.MSGuid },
                { "Var", AstType.Var },
                { "ReturnsTwiceAttr", AstType.ReturnsTwiceAttr },
                { "inherited", AstType.Inherited },
                { "private", AstType.Private },
                { "SectionAttr", AstType.SectionAttr },
                { "OwnerAttr", AstType.OwnerAttr },
                { "CXX11NoReturnAttr", AstType.CXX11NoReturnAttr },
                { "FinalAttr", AstType.FinalAttr },
                { "ClassTemplatePartialSpecialization", AstType.ClassTemplatePartialSpecialization },
                { "array_filler:", AstType.Array_filler },
                { "element:", AstType.Element },
                { "elements:", AstType.Elements },
                { "target", AstType.Target },
                { "nominated", AstType.Nominated },
                { "constructed", AstType.Constructed },
                { "OverrideAttr", AstType.OverrideAttr },
                { "filler:", AstType.Filler },
                { "UnresolvedUsingTypename", AstType.UnresolvedUsingTypename },
                { "virtual", AstType.Virtual },
                { "MSVtorDispAttr", AstType.MSVtorDispAttr },
                { "FullComment", AstType.FullComment },
                { "ParagraphComment", AstType.ParagraphComment },
                { "TextComment", AstType.TextComment },
                { "VerbatimLineComment", AstType.VerbatimLineComment },
                { "BlockCommandComment", AstType.BlockCommandComment },
                { "ParamCommandComment", AstType.ParamCommandComment },

                { "CXXCastExpr", AstType.CxxCastExpr },




            };
        }
    }
}
