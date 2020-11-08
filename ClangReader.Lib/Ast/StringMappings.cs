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
        internal static readonly StringToEnumMapper<AstKnownSuffix> KnownTokenType;


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

            KnownTokenType = new StringToEnumMapper<AstKnownSuffix>
            {
                { "Cursor", AstKnownSuffix.Cursor },
                { "Attr", AstKnownSuffix.Attr },
                { "InheritableAttr", AstKnownSuffix.InheritableAttr },
                { "InheritableParamAttr", AstKnownSuffix.InheritableParamAttr },
                { "ParameterABIAttr", AstKnownSuffix.ParameterABIAttr },
                { "StmtAttr", AstKnownSuffix.StmtAttr },
                { "TypeAttr", AstKnownSuffix.TypeAttr },
                { "AccessSpecDecl", AstKnownSuffix.AccessSpecDecl },
                { "BindingDecl", AstKnownSuffix.BindingDecl },
                { "BlockDecl", AstKnownSuffix.BlockDecl },
                { "BuiltinTemplateDecl", AstKnownSuffix.BuiltinTemplateDecl },
                { "CapturedDecl", AstKnownSuffix.CapturedDecl },
                { "ClassScopeFunctionSpecializationDecl", AstKnownSuffix.ClassScopeFunctionSpecializationDecl },
                { "ClassTemplateDecl", AstKnownSuffix.ClassTemplateDecl },
                { "ClassTemplatePartialSpecializationDecl", AstKnownSuffix.ClassTemplatePartialSpecializationDecl },
                { "ClassTemplateSpecializationDecl", AstKnownSuffix.ClassTemplateSpecializationDecl },
                { "ConceptDecl", AstKnownSuffix.ConceptDecl },
                { "ConstructorUsingShadowDecl", AstKnownSuffix.ConstructorUsingShadowDecl },
                { "CXXConstructorDecl", AstKnownSuffix.CXXConstructorDecl },
                { "CXXConversionDecl", AstKnownSuffix.CXXConversionDecl },
                { "CXXDeductionGuideDecl", AstKnownSuffix.CXXDeductionGuideDecl },
                { "CXXDestructorDecl", AstKnownSuffix.CXXDestructorDecl },
                { "CXXMethodDecl", AstKnownSuffix.CXXMethodDecl },
                { "CXXRecordDecl", AstKnownSuffix.CXXRecordDecl },
                { "Decl", AstKnownSuffix.Decl },
                { "DeclaratorDecl", AstKnownSuffix.DeclaratorDecl },
                { "DecompositionDecl", AstKnownSuffix.DecompositionDecl },
                { "EmptyDecl", AstKnownSuffix.EmptyDecl },
                { "EnumConstantDecl", AstKnownSuffix.EnumConstantDecl },
                { "EnumDecl", AstKnownSuffix.EnumDecl },
                { "ExportDecl", AstKnownSuffix.ExportDecl },
                { "ExternCContextDecl", AstKnownSuffix.ExternCContextDecl },
                { "FieldDecl", AstKnownSuffix.FieldDecl },
                { "FileScopeAsmDecl", AstKnownSuffix.FileScopeAsmDecl },
                { "FriendDecl", AstKnownSuffix.FriendDecl },
                { "FriendTemplateDecl", AstKnownSuffix.FriendTemplateDecl },
                { "FunctionDecl", AstKnownSuffix.FunctionDecl },
                { "FunctionTemplateDecl", AstKnownSuffix.FunctionTemplateDecl },
                { "ImplicitParamDecl", AstKnownSuffix.ImplicitParamDecl },
                { "ImportDecl", AstKnownSuffix.ImportDecl },
                { "IndirectFieldDecl", AstKnownSuffix.IndirectFieldDecl },
                { "LabelDecl", AstKnownSuffix.LabelDecl },
                { "LifetimeExtendedTemporaryDecl", AstKnownSuffix.LifetimeExtendedTemporaryDecl },
                { "LinkageSpecDecl", AstKnownSuffix.LinkageSpecDecl },
                { "MSGuidDecl", AstKnownSuffix.MSGuidDecl },
                { "MSPropertyDecl", AstKnownSuffix.MSPropertyDecl },
                { "NamedDecl", AstKnownSuffix.NamedDecl },
                { "NamespaceAliasDecl", AstKnownSuffix.NamespaceAliasDecl },
                { "NamespaceDecl", AstKnownSuffix.NamespaceDecl },
                { "NonTypeTemplateParmDecl", AstKnownSuffix.NonTypeTemplateParmDecl },
                { "ObjCAtDefsFieldDecl", AstKnownSuffix.ObjCAtDefsFieldDecl },
                { "ObjCCategoryDecl", AstKnownSuffix.ObjCCategoryDecl },
                { "ObjCCategoryImplDecl", AstKnownSuffix.ObjCCategoryImplDecl },
                { "ObjCCompatibleAliasDecl", AstKnownSuffix.ObjCCompatibleAliasDecl },
                { "ObjCContainerDecl", AstKnownSuffix.ObjCContainerDecl },
                { "ObjCImplDecl", AstKnownSuffix.ObjCImplDecl },
                { "ObjCImplementationDecl", AstKnownSuffix.ObjCImplementationDecl },
                { "ObjCInterfaceDecl", AstKnownSuffix.ObjCInterfaceDecl },
                { "ObjCIvarDecl", AstKnownSuffix.ObjCIvarDecl },
                { "ObjCMethodDecl", AstKnownSuffix.ObjCMethodDecl },
                { "ObjCPropertyDecl", AstKnownSuffix.ObjCPropertyDecl },
                { "ObjCPropertyImplDecl", AstKnownSuffix.ObjCPropertyImplDecl },
                { "ObjCProtocolDecl", AstKnownSuffix.ObjCProtocolDecl },
                { "ObjCTypeParamDecl", AstKnownSuffix.ObjCTypeParamDecl },
                { "OMPAllocateDecl", AstKnownSuffix.OMPAllocateDecl },
                { "OMPCapturedExprDecl", AstKnownSuffix.OMPCapturedExprDecl },
                { "OMPDeclareMapperDecl", AstKnownSuffix.OMPDeclareMapperDecl },
                { "OMPDeclareReductionDecl", AstKnownSuffix.OMPDeclareReductionDecl },
                { "OMPRequiresDecl", AstKnownSuffix.OMPRequiresDecl },
                { "OMPThreadPrivateDecl", AstKnownSuffix.OMPThreadPrivateDecl },
                { "ParmVarDecl", AstKnownSuffix.ParmVarDecl },
                { "PragmaCommentDecl", AstKnownSuffix.PragmaCommentDecl },
                { "PragmaDetectMismatchDecl", AstKnownSuffix.PragmaDetectMismatchDecl },
                { "RecordDecl", AstKnownSuffix.RecordDecl },
                { "RedeclarableTemplateDecl", AstKnownSuffix.RedeclarableTemplateDecl },
                { "RequiresExprBodyDecl", AstKnownSuffix.RequiresExprBodyDecl },
                { "StaticAssertDecl", AstKnownSuffix.StaticAssertDecl },
                { "TagDecl", AstKnownSuffix.TagDecl },
                { "TemplateDecl", AstKnownSuffix.TemplateDecl },
                { "TemplateTemplateParmDecl", AstKnownSuffix.TemplateTemplateParmDecl },
                { "TemplateTypeParmDecl", AstKnownSuffix.TemplateTypeParmDecl },
                { "TranslationUnitDecl", AstKnownSuffix.TranslationUnitDecl },
                { "TypeAliasDecl", AstKnownSuffix.TypeAliasDecl },
                { "TypeAliasTemplateDecl", AstKnownSuffix.TypeAliasTemplateDecl },
                { "TypeDecl", AstKnownSuffix.TypeDecl },
                { "TypedefDecl", AstKnownSuffix.TypedefDecl },
                { "TypedefNameDecl", AstKnownSuffix.TypedefNameDecl },
                { "UnresolvedUsingTypenameDecl", AstKnownSuffix.UnresolvedUsingTypenameDecl },
                { "UnresolvedUsingValueDecl", AstKnownSuffix.UnresolvedUsingValueDecl },
                { "UsingDecl", AstKnownSuffix.UsingDecl },
                { "UsingDirectiveDecl", AstKnownSuffix.UsingDirectiveDecl },
                { "UsingPackDecl", AstKnownSuffix.UsingPackDecl },
                { "UsingShadowDecl", AstKnownSuffix.UsingShadowDecl },
                { "ValueDecl", AstKnownSuffix.ValueDecl },
                { "VarDecl", AstKnownSuffix.VarDecl },
                { "VarTemplateDecl", AstKnownSuffix.VarTemplateDecl },
                { "VarTemplatePartialSpecializationDecl", AstKnownSuffix.VarTemplatePartialSpecializationDecl },
                { "VarTemplateSpecializationDecl", AstKnownSuffix.VarTemplateSpecializationDecl },
                { "AbstractConditionalOperator", AstKnownSuffix.AbstractConditionalOperator },
                { "AddrLabelExpr", AstKnownSuffix.AddrLabelExpr },
                { "ArrayInitIndexExpr", AstKnownSuffix.ArrayInitIndexExpr },
                { "ArrayInitLoopExpr", AstKnownSuffix.ArrayInitLoopExpr },
                { "ArraySubscriptExpr", AstKnownSuffix.ArraySubscriptExpr },
                { "ArrayTypeTraitExpr", AstKnownSuffix.ArrayTypeTraitExpr },
                { "AsTypeExpr", AstKnownSuffix.AsTypeExpr },
                { "AtomicExpr", AstKnownSuffix.AtomicExpr },
                { "BinaryConditionalOperator", AstKnownSuffix.BinaryConditionalOperator },
                { "BinaryOperator", AstKnownSuffix.BinaryOperator },
                { "BlockExpr", AstKnownSuffix.BlockExpr },
                { "BuiltinBitCastExpr", AstKnownSuffix.BuiltinBitCastExpr },
                { "CallExpr", AstKnownSuffix.CallExpr },
                { "CastExpr", AstKnownSuffix.CastExpr },
                { "CharacterLiteral", AstKnownSuffix.CharacterLiteral },
                { "ChooseExpr", AstKnownSuffix.ChooseExpr },
                { "CoawaitExpr", AstKnownSuffix.CoawaitExpr },
                { "CompoundAssignOperator", AstKnownSuffix.CompoundAssignOperator },
                { "CompoundLiteralExpr", AstKnownSuffix.CompoundLiteralExpr },
                { "ConceptSpecializationExpr", AstKnownSuffix.ConceptSpecializationExpr },
                { "ConditionalOperator", AstKnownSuffix.ConditionalOperator },
                { "ConstantExpr", AstKnownSuffix.ConstantExpr },
                { "ConvertVectorExpr", AstKnownSuffix.ConvertVectorExpr },
                { "CoroutineSuspendExpr", AstKnownSuffix.CoroutineSuspendExpr },
                { "CoyieldExpr", AstKnownSuffix.CoyieldExpr },
                { "CStyleCastExpr", AstKnownSuffix.CStyleCastExpr },
                { "CUDAKernelCallExpr", AstKnownSuffix.CUDAKernelCallExpr },
                { "CXXAddrspaceCastExpr", AstKnownSuffix.CXXAddrspaceCastExpr },
                { "CXXBindTemporaryExpr", AstKnownSuffix.CXXBindTemporaryExpr },
                { "CXXBoolLiteralExpr", AstKnownSuffix.CXXBoolLiteralExpr },
                { "CXXConstCastExpr", AstKnownSuffix.CXXConstCastExpr },
                { "CXXConstructExpr", AstKnownSuffix.CXXConstructExpr },
                { "CXXDefaultArgExpr", AstKnownSuffix.CXXDefaultArgExpr },
                { "CXXDefaultInitExpr", AstKnownSuffix.CXXDefaultInitExpr },
                { "CXXDeleteExpr", AstKnownSuffix.CXXDeleteExpr },
                { "CXXDependentScopeMemberExpr", AstKnownSuffix.CXXDependentScopeMemberExpr },
                { "CXXDynamicCastExpr", AstKnownSuffix.CXXDynamicCastExpr },
                { "CXXFoldExpr", AstKnownSuffix.CXXFoldExpr },
                { "CXXFunctionalCastExpr", AstKnownSuffix.CXXFunctionalCastExpr },
                { "CXXInheritedCtorInitExpr", AstKnownSuffix.CXXInheritedCtorInitExpr },
                { "CXXMemberCallExpr", AstKnownSuffix.CXXMemberCallExpr },
                { "CXXNamedCastExpr", AstKnownSuffix.CXXNamedCastExpr },
                { "CXXNewExpr", AstKnownSuffix.CXXNewExpr },
                { "CXXNoexceptExpr", AstKnownSuffix.CXXNoexceptExpr },
                { "CXXNullPtrLiteralExpr", AstKnownSuffix.CXXNullPtrLiteralExpr },
                { "CXXOperatorCallExpr", AstKnownSuffix.CXXOperatorCallExpr },
                { "CXXPseudoDestructorExpr", AstKnownSuffix.CXXPseudoDestructorExpr },
                { "CXXReinterpretCastExpr", AstKnownSuffix.CXXReinterpretCastExpr },
                { "CXXRewrittenBinaryOperator", AstKnownSuffix.CXXRewrittenBinaryOperator },
                { "CXXScalarValueInitExpr", AstKnownSuffix.CXXScalarValueInitExpr },
                { "CXXStaticCastExpr", AstKnownSuffix.CXXStaticCastExpr },
                { "CXXStdInitializerListExpr", AstKnownSuffix.CXXStdInitializerListExpr },
                { "CXXTemporaryObjectExpr", AstKnownSuffix.CXXTemporaryObjectExpr },
                { "CXXThisExpr", AstKnownSuffix.CXXThisExpr },
                { "CXXThrowExpr", AstKnownSuffix.CXXThrowExpr },
                { "CXXTypeidExpr", AstKnownSuffix.CXXTypeidExpr },
                { "CXXUnresolvedConstructExpr", AstKnownSuffix.CXXUnresolvedConstructExpr },
                { "CXXUuidofExpr", AstKnownSuffix.CXXUuidofExpr },
                { "DeclRefExpr", AstKnownSuffix.DeclRefExpr },
                { "DependentCoawaitExpr", AstKnownSuffix.DependentCoawaitExpr },
                { "DependentScopeDeclRefExpr", AstKnownSuffix.DependentScopeDeclRefExpr },
                { "DesignatedInitExpr", AstKnownSuffix.DesignatedInitExpr },
                { "DesignatedInitUpdateExpr", AstKnownSuffix.DesignatedInitUpdateExpr },
                { "ExplicitCastExpr", AstKnownSuffix.ExplicitCastExpr },
                { "Expr", AstKnownSuffix.Expr },
                { "ExpressionTraitExpr", AstKnownSuffix.ExpressionTraitExpr },
                { "ExprWithCleanups", AstKnownSuffix.ExprWithCleanups },
                { "ExtVectorElementExpr", AstKnownSuffix.ExtVectorElementExpr },
                { "FixedPointLiteral", AstKnownSuffix.FixedPointLiteral },
                { "FloatingLiteral", AstKnownSuffix.FloatingLiteral },
                { "FullExpr", AstKnownSuffix.FullExpr },
                { "FunctionParmPackExpr", AstKnownSuffix.FunctionParmPackExpr },
                { "GenericSelectionExpr", AstKnownSuffix.GenericSelectionExpr },
                { "GNUNullExpr", AstKnownSuffix.GNUNullExpr },
                { "ImaginaryLiteral", AstKnownSuffix.ImaginaryLiteral },
                { "ImplicitCastExpr", AstKnownSuffix.ImplicitCastExpr },
                { "ImplicitValueInitExpr", AstKnownSuffix.ImplicitValueInitExpr },
                { "InitListExpr", AstKnownSuffix.InitListExpr },
                { "IntegerLiteral", AstKnownSuffix.IntegerLiteral },
                { "LambdaExpr", AstKnownSuffix.LambdaExpr },
                { "MaterializeTemporaryExpr", AstKnownSuffix.MaterializeTemporaryExpr },
                { "MatrixSubscriptExpr", AstKnownSuffix.MatrixSubscriptExpr },
                { "MemberExpr", AstKnownSuffix.MemberExpr },
                { "MSPropertyRefExpr", AstKnownSuffix.MSPropertyRefExpr },
                { "MSPropertySubscriptExpr", AstKnownSuffix.MSPropertySubscriptExpr },
                { "NoInitExpr", AstKnownSuffix.NoInitExpr },
                { "ObjCArrayLiteral", AstKnownSuffix.ObjCArrayLiteral },
                { "ObjCAvailabilityCheckExpr", AstKnownSuffix.ObjCAvailabilityCheckExpr },
                { "ObjCBoolLiteralExpr", AstKnownSuffix.ObjCBoolLiteralExpr },
                { "ObjCBoxedExpr", AstKnownSuffix.ObjCBoxedExpr },
                { "ObjCBridgedCastExpr", AstKnownSuffix.ObjCBridgedCastExpr },
                { "ObjCDictionaryLiteral", AstKnownSuffix.ObjCDictionaryLiteral },
                { "ObjCEncodeExpr", AstKnownSuffix.ObjCEncodeExpr },
                { "ObjCIndirectCopyRestoreExpr", AstKnownSuffix.ObjCIndirectCopyRestoreExpr },
                { "ObjCIsaExpr", AstKnownSuffix.ObjCIsaExpr },
                { "ObjCIvarRefExpr", AstKnownSuffix.ObjCIvarRefExpr },
                { "ObjCMessageExpr", AstKnownSuffix.ObjCMessageExpr },
                { "ObjCPropertyRefExpr", AstKnownSuffix.ObjCPropertyRefExpr },
                { "ObjCProtocolExpr", AstKnownSuffix.ObjCProtocolExpr },
                { "ObjCSelectorExpr", AstKnownSuffix.ObjCSelectorExpr },
                { "ObjCStringLiteral", AstKnownSuffix.ObjCStringLiteral },
                { "ObjCSubscriptRefExpr", AstKnownSuffix.ObjCSubscriptRefExpr },
                { "OffsetOfExpr", AstKnownSuffix.OffsetOfExpr },
                { "OMPArraySectionExpr", AstKnownSuffix.OMPArraySectionExpr },
                { "OMPArrayShapingExpr", AstKnownSuffix.OMPArrayShapingExpr },
                { "OMPIteratorExpr", AstKnownSuffix.OMPIteratorExpr },
                { "OpaqueValueExpr", AstKnownSuffix.OpaqueValueExpr },
                { "OverloadExpr", AstKnownSuffix.OverloadExpr },
                { "PackExpansionExpr", AstKnownSuffix.PackExpansionExpr },
                { "ParenExpr", AstKnownSuffix.ParenExpr },
                { "ParenListExpr", AstKnownSuffix.ParenListExpr },
                { "PredefinedExpr", AstKnownSuffix.PredefinedExpr },
                { "PseudoObjectExpr", AstKnownSuffix.PseudoObjectExpr },
                { "RecoveryExpr", AstKnownSuffix.RecoveryExpr },
                { "RequiresExpr", AstKnownSuffix.RequiresExpr },
                { "ShuffleVectorExpr", AstKnownSuffix.ShuffleVectorExpr },
                { "SizeOfPackExpr", AstKnownSuffix.SizeOfPackExpr },
                { "SourceLocExpr", AstKnownSuffix.SourceLocExpr },
                { "StmtExpr", AstKnownSuffix.StmtExpr },
                { "StringLiteral", AstKnownSuffix.StringLiteral },
                { "SubstNonTypeTemplateParmExpr", AstKnownSuffix.SubstNonTypeTemplateParmExpr },
                { "SubstNonTypeTemplateParmPackExpr", AstKnownSuffix.SubstNonTypeTemplateParmPackExpr },
                { "TypeTraitExpr", AstKnownSuffix.TypeTraitExpr },
                { "TypoExpr", AstKnownSuffix.TypoExpr },
                { "UnaryExprOrTypeTraitExpr", AstKnownSuffix.UnaryExprOrTypeTraitExpr },
                { "UnaryOperator", AstKnownSuffix.UnaryOperator },
                { "UnresolvedLookupExpr", AstKnownSuffix.UnresolvedLookupExpr },
                { "UnresolvedMemberExpr", AstKnownSuffix.UnresolvedMemberExpr },
                { "UserDefinedLiteral", AstKnownSuffix.UserDefinedLiteral },
                { "VAArgExpr", AstKnownSuffix.VAArgExpr },
                { "InclusionDirective", AstKnownSuffix.InclusionDirective },
                { "MacroDefinitionRecord", AstKnownSuffix.MacroDefinitionRecord },
                { "MacroExpansion", AstKnownSuffix.MacroExpansion },
                { "PreprocessedEntity", AstKnownSuffix.PreprocessedEntity },
                { "PreprocessingDirective", AstKnownSuffix.PreprocessingDirective },
                { "CXXBaseSpecifier", AstKnownSuffix.CXXBaseSpecifier },
                { "Ref", AstKnownSuffix.Ref },
                { "AsmStmt", AstKnownSuffix.AsmStmt },
                { "AttributedStmt", AstKnownSuffix.AttributedStmt },
                { "BreakStmt", AstKnownSuffix.BreakStmt },
                { "CapturedStmt", AstKnownSuffix.CapturedStmt },
                { "CaseStmt", AstKnownSuffix.CaseStmt },
                { "CompoundStmt", AstKnownSuffix.CompoundStmt },
                { "ContinueStmt", AstKnownSuffix.ContinueStmt },
                { "CoreturnStmt", AstKnownSuffix.CoreturnStmt },
                { "CoroutineBodyStmt", AstKnownSuffix.CoroutineBodyStmt },
                { "CXXCatchStmt", AstKnownSuffix.CXXCatchStmt },
                { "CXXForRangeStmt", AstKnownSuffix.CXXForRangeStmt },
                { "CXXTryStmt", AstKnownSuffix.CXXTryStmt },
                { "DeclStmt", AstKnownSuffix.DeclStmt },
                { "DefaultStmt", AstKnownSuffix.DefaultStmt },
                { "DoStmt", AstKnownSuffix.DoStmt },
                { "ForStmt", AstKnownSuffix.ForStmt },
                { "GCCAsmStmt", AstKnownSuffix.GCCAsmStmt },
                { "GotoStmt", AstKnownSuffix.GotoStmt },
                { "IfStmt", AstKnownSuffix.IfStmt },
                { "IndirectGotoStmt", AstKnownSuffix.IndirectGotoStmt },
                { "LabelStmt", AstKnownSuffix.LabelStmt },
                { "MSAsmStmt", AstKnownSuffix.MSAsmStmt },
                { "MSDependentExistsStmt", AstKnownSuffix.MSDependentExistsStmt },
                { "NullStmt", AstKnownSuffix.NullStmt },
                { "ObjCAtCatchStmt", AstKnownSuffix.ObjCAtCatchStmt },
                { "ObjCAtFinallyStmt", AstKnownSuffix.ObjCAtFinallyStmt },
                { "ObjCAtSynchronizedStmt", AstKnownSuffix.ObjCAtSynchronizedStmt },
                { "ObjCAtThrowStmt", AstKnownSuffix.ObjCAtThrowStmt },
                { "ObjCAtTryStmt", AstKnownSuffix.ObjCAtTryStmt },
                { "ObjCAutoreleasePoolStmt", AstKnownSuffix.ObjCAutoreleasePoolStmt },
                { "ObjCForCollectionStmt", AstKnownSuffix.ObjCForCollectionStmt },
                { "OMPAtomicDirective", AstKnownSuffix.OMPAtomicDirective },
                { "OMPBarrierDirective", AstKnownSuffix.OMPBarrierDirective },
                { "OMPCancelDirective", AstKnownSuffix.OMPCancelDirective },
                { "OMPCancellationPointDirective", AstKnownSuffix.OMPCancellationPointDirective },
                { "OMPCriticalDirective", AstKnownSuffix.OMPCriticalDirective },
                { "OMPDepobjDirective", AstKnownSuffix.OMPDepobjDirective },
                { "OMPDistributeDirective", AstKnownSuffix.OMPDistributeDirective },
                { "OMPDistributeParallelForDirective", AstKnownSuffix.OMPDistributeParallelForDirective },
                { "OMPDistributeParallelForSimdDirective", AstKnownSuffix.OMPDistributeParallelForSimdDirective },
                { "OMPDistributeSimdDirective", AstKnownSuffix.OMPDistributeSimdDirective },
                { "OMPExecutableDirective", AstKnownSuffix.OMPExecutableDirective },
                { "OMPFlushDirective", AstKnownSuffix.OMPFlushDirective },
                { "OMPForDirective", AstKnownSuffix.OMPForDirective },
                { "OMPForSimdDirective", AstKnownSuffix.OMPForSimdDirective },
                { "OMPLoopDirective", AstKnownSuffix.OMPLoopDirective },
                { "OMPMasterDirective", AstKnownSuffix.OMPMasterDirective },
                { "OMPMasterTaskLoopDirective", AstKnownSuffix.OMPMasterTaskLoopDirective },
                { "OMPMasterTaskLoopSimdDirective", AstKnownSuffix.OMPMasterTaskLoopSimdDirective },
                { "OMPOrderedDirective", AstKnownSuffix.OMPOrderedDirective },
                { "OMPParallelDirective", AstKnownSuffix.OMPParallelDirective },
                { "OMPParallelForDirective", AstKnownSuffix.OMPParallelForDirective },
                { "OMPParallelForSimdDirective", AstKnownSuffix.OMPParallelForSimdDirective },
                { "OMPParallelMasterDirective", AstKnownSuffix.OMPParallelMasterDirective },
                { "OMPParallelMasterTaskLoopDirective", AstKnownSuffix.OMPParallelMasterTaskLoopDirective },
                { "OMPParallelMasterTaskLoopSimdDirective", AstKnownSuffix.OMPParallelMasterTaskLoopSimdDirective },
                { "OMPParallelSectionsDirective", AstKnownSuffix.OMPParallelSectionsDirective },
                { "OMPScanDirective", AstKnownSuffix.OMPScanDirective },
                { "OMPSectionDirective", AstKnownSuffix.OMPSectionDirective },
                { "OMPSectionsDirective", AstKnownSuffix.OMPSectionsDirective },
                { "OMPSimdDirective", AstKnownSuffix.OMPSimdDirective },
                { "OMPSingleDirective", AstKnownSuffix.OMPSingleDirective },
                { "OMPTargetDataDirective", AstKnownSuffix.OMPTargetDataDirective },
                { "OMPTargetDirective", AstKnownSuffix.OMPTargetDirective },
                { "OMPTargetEnterDataDirective", AstKnownSuffix.OMPTargetEnterDataDirective },
                { "OMPTargetExitDataDirective", AstKnownSuffix.OMPTargetExitDataDirective },
                { "OMPTargetParallelDirective", AstKnownSuffix.OMPTargetParallelDirective },
                { "OMPTargetParallelForDirective", AstKnownSuffix.OMPTargetParallelForDirective },
                { "OMPTargetParallelForSimdDirective", AstKnownSuffix.OMPTargetParallelForSimdDirective },
                { "OMPTargetSimdDirective", AstKnownSuffix.OMPTargetSimdDirective },
                { "OMPTargetTeamsDirective", AstKnownSuffix.OMPTargetTeamsDirective },
                { "OMPTargetTeamsDistributeDirective", AstKnownSuffix.OMPTargetTeamsDistributeDirective },
                { "OMPTargetTeamsDistributeParallelForDirective", AstKnownSuffix.OMPTargetTeamsDistributeParallelForDirective },
                { "OMPTargetTeamsDistributeParallelForSimdDirective", AstKnownSuffix.OMPTargetTeamsDistributeParallelForSimdDirective },
                { "OMPTargetTeamsDistributeSimdDirective", AstKnownSuffix.OMPTargetTeamsDistributeSimdDirective },
                { "OMPTargetUpdateDirective", AstKnownSuffix.OMPTargetUpdateDirective },
                { "OMPTaskDirective", AstKnownSuffix.OMPTaskDirective },
                { "OMPTaskgroupDirective", AstKnownSuffix.OMPTaskgroupDirective },
                { "OMPTaskLoopDirective", AstKnownSuffix.OMPTaskLoopDirective },
                { "OMPTaskLoopSimdDirective", AstKnownSuffix.OMPTaskLoopSimdDirective },
                { "OMPTaskwaitDirective", AstKnownSuffix.OMPTaskwaitDirective },
                { "OMPTaskyieldDirective", AstKnownSuffix.OMPTaskyieldDirective },
                { "OMPTeamsDirective", AstKnownSuffix.OMPTeamsDirective },
                { "OMPTeamsDistributeDirective", AstKnownSuffix.OMPTeamsDistributeDirective },
                { "OMPTeamsDistributeParallelForDirective", AstKnownSuffix.OMPTeamsDistributeParallelForDirective },
                { "OMPTeamsDistributeParallelForSimdDirective", AstKnownSuffix.OMPTeamsDistributeParallelForSimdDirective },
                { "OMPTeamsDistributeSimdDirective", AstKnownSuffix.OMPTeamsDistributeSimdDirective },
                { "ReturnStmt", AstKnownSuffix.ReturnStmt },
                { "SEHExceptStmt", AstKnownSuffix.SEHExceptStmt },
                { "SEHFinallyStmt", AstKnownSuffix.SEHFinallyStmt },
                { "SEHLeaveStmt", AstKnownSuffix.SEHLeaveStmt },
                { "SEHTryStmt", AstKnownSuffix.SEHTryStmt },
                { "Stmt", AstKnownSuffix.Stmt },
                { "SwitchCase", AstKnownSuffix.SwitchCase },
                { "SwitchStmt", AstKnownSuffix.SwitchStmt },
                { "ValueStmt", AstKnownSuffix.ValueStmt },
                { "WhileStmt", AstKnownSuffix.WhileStmt },

                //Types
                { "AdjustedType", AstKnownSuffix.AdjustedType },
                { "ArrayType", AstKnownSuffix.ArrayType },
                { "AtomicType", AstKnownSuffix.AtomicType },
                { "AttributedType", AstKnownSuffix.AttributedType },
                { "AutoType", AstKnownSuffix.AutoType },
                { "BlockPointerType", AstKnownSuffix.BlockPointerType },
                { "BuiltinType", AstKnownSuffix.BuiltinType },
                { "ComplexType", AstKnownSuffix.ComplexType },
                { "ConstantArrayType", AstKnownSuffix.ConstantArrayType },
                { "ConstantMatrixType", AstKnownSuffix.ConstantMatrixType },
                { "DecayedType", AstKnownSuffix.DecayedType },
                { "DecltypeType", AstKnownSuffix.DecltypeType },
                { "DeducedTemplateSpecializationType", AstKnownSuffix.DeducedTemplateSpecializationType },
                { "DeducedType", AstKnownSuffix.DeducedType },
                { "DependentAddressSpaceType", AstKnownSuffix.DependentAddressSpaceType },
                { "DependentExtIntType", AstKnownSuffix.DependentExtIntType },
                { "DependentNameType", AstKnownSuffix.DependentNameType },
                { "DependentSizedArrayType", AstKnownSuffix.DependentSizedArrayType },
                { "DependentSizedExtVectorType", AstKnownSuffix.DependentSizedExtVectorType },
                { "DependentSizedMatrixType", AstKnownSuffix.DependentSizedMatrixType },
                { "DependentTemplateSpecializationType", AstKnownSuffix.DependentTemplateSpecializationType },
                { "DependentVectorType", AstKnownSuffix.DependentVectorType },
                { "ElaboratedType", AstKnownSuffix.ElaboratedType },
                { "EnumType", AstKnownSuffix.EnumType },
                { "ExtIntType", AstKnownSuffix.ExtIntType },
                { "ExtVectorType", AstKnownSuffix.ExtVectorType },
                { "FunctionNoProtoType", AstKnownSuffix.FunctionNoProtoType },
                { "FunctionProtoType", AstKnownSuffix.FunctionProtoType },
                { "FunctionType", AstKnownSuffix.FunctionType },
                { "IncompleteArrayType", AstKnownSuffix.IncompleteArrayType },
                { "InjectedClassNameType", AstKnownSuffix.InjectedClassNameType },
                { "LValueReferenceType", AstKnownSuffix.LValueReferenceType },
                { "MacroQualifiedType", AstKnownSuffix.MacroQualifiedType },
                { "MatrixType", AstKnownSuffix.MatrixType },
                { "MemberPointerType", AstKnownSuffix.MemberPointerType },
                { "ObjCInterfaceType", AstKnownSuffix.ObjCInterfaceType },
                { "ObjCObjectPointerType", AstKnownSuffix.ObjCObjectPointerType },
                { "ObjCObjectType", AstKnownSuffix.ObjCObjectType },
                { "ObjCTypeParamType", AstKnownSuffix.ObjCTypeParamType },
                { "PackExpansionType", AstKnownSuffix.PackExpansionType },
                { "ParenType", AstKnownSuffix.ParenType },
                { "PipeType", AstKnownSuffix.PipeType },
                { "PointerType", AstKnownSuffix.PointerType },
                { "RecordType", AstKnownSuffix.RecordType },
                { "ReferenceType", AstKnownSuffix.ReferenceType },
                { "RValueReferenceType", AstKnownSuffix.RValueReferenceType },
                { "SubstTemplateTypeParmPackType", AstKnownSuffix.SubstTemplateTypeParmPackType },
                { "SubstTemplateTypeParmType", AstKnownSuffix.SubstTemplateTypeParmType },
                { "TagType", AstKnownSuffix.TagType },
                { "TemplateSpecializationType", AstKnownSuffix.TemplateSpecializationType },
                { "TemplateTypeParmType", AstKnownSuffix.TemplateTypeParmType },
                { "Type", AstKnownSuffix.Type },
                { "TypedefType", AstKnownSuffix.TypedefType },
                { "TypeOfExprType", AstKnownSuffix.TypeOfExprType },
                { "TypeOfType", AstKnownSuffix.TypeOfType },
                { "TypeWithKeyword", AstKnownSuffix.TypeWithKeyword },
                { "UnaryTransformType", AstKnownSuffix.UnaryTransformType },
                { "UnresolvedUsingType", AstKnownSuffix.UnresolvedUsingType },
                { "VariableArrayType", AstKnownSuffix.VariableArrayType },
                { "VectorType", AstKnownSuffix.VectorType },



                //Own
                { "TypeVisibilityAttr", AstKnownSuffix.TypeVisibilityAttr },

                //own generated

                { "CXXRecord", AstKnownSuffix.CXXRecord },
                { "BuiltinAttr", AstKnownSuffix.BuiltinAttr },
                { "NoThrowAttr", AstKnownSuffix.NoThrowAttr },
                { "DefinitionData", AstKnownSuffix.DefinitionData },
                { "DefaultConstructor", AstKnownSuffix.DefaultConstructor },
                { "CopyConstructor", AstKnownSuffix.CopyConstructor },
                { "MoveConstructor", AstKnownSuffix.MoveConstructor },
                { "CopyAssignment", AstKnownSuffix.CopyAssignment },
                { "MoveAssignment", AstKnownSuffix.MoveAssignment },
                { "Destructor", AstKnownSuffix.Destructor },
                { "MaxFieldAlignmentAttr", AstKnownSuffix.MaxFieldAlignmentAttr },
                { "TemplateArgument", AstKnownSuffix.TemplateArgument },
                { "QualType", AstKnownSuffix.QualType },
                { "value:", AstKnownSuffix.Value },
                { "TemplateTypeParm", AstKnownSuffix.TemplateTypeParm },
                { "Typedef", AstKnownSuffix.Typedef },
                { "CXXCtorInitializer", AstKnownSuffix.CXXCtorInitializer },
                { "original", AstKnownSuffix.Original },
                { "VisibilityAttr", AstKnownSuffix.VisibilityAttr },
                { "ReturnsNonNullAttr", AstKnownSuffix.ReturnsNonNullAttr },
                { "AllocSizeAttr", AstKnownSuffix.AllocSizeAttr },
                { "WarnUnusedResultAttr", AstKnownSuffix.WarnUnusedResultAttr },
                { "MSAllocatorAttr", AstKnownSuffix.MSAllocatorAttr },
                { "AllocAlignAttr", AstKnownSuffix.AllocAlignAttr },
                { "Enum", AstKnownSuffix.Enum },
                { "DeprecatedAttr", AstKnownSuffix.DeprecatedAttr },
                { "PureAttr", AstKnownSuffix.PureAttr },
                { "AlwaysInlineAttr", AstKnownSuffix.AlwaysInlineAttr },
                { "Field", AstKnownSuffix.Field },
                { "ClassTemplateSpecialization", AstKnownSuffix.ClassTemplateSpecialization },
                { "AlignedAttr", AstKnownSuffix.AlignedAttr },
                { "ConstAttr", AstKnownSuffix.ConstAttr },
                { "DLLImportAttr", AstKnownSuffix.DLLImportAttr },
                { "public", AstKnownSuffix.Public },
                { "NoInlineAttr", AstKnownSuffix.NoInlineAttr },
                { "FormatAttr", AstKnownSuffix.FormatAttr },
                { "RestrictAttr", AstKnownSuffix.RestrictAttr },
                { "MSNoVTableAttr", AstKnownSuffix.MSNoVTableAttr },
                { "Overrides:", AstKnownSuffix.Overrides },
                { "NoAliasAttr", AstKnownSuffix.NoAliasAttr },
                { "UuidAttr", AstKnownSuffix.UuidAttr },
                { "Function", AstKnownSuffix.Function },
                { "MSInheritanceAttr", AstKnownSuffix.MSInheritanceAttr },
                { "SelectAnyAttr", AstKnownSuffix.SelectAnyAttr },
                { "<<<NULL>>>", AstKnownSuffix.NULL },
                { "TypeAlias", AstKnownSuffix.TypeAlias },
                { "PointerAttr", AstKnownSuffix.PointerAttr },
                { "MSGuid", AstKnownSuffix.MSGuid },
                { "Var", AstKnownSuffix.Var },
                { "ReturnsTwiceAttr", AstKnownSuffix.ReturnsTwiceAttr },
                { "inherited", AstKnownSuffix.Inherited },
                { "private", AstKnownSuffix.Private },
                { "SectionAttr", AstKnownSuffix.SectionAttr },
                { "OwnerAttr", AstKnownSuffix.OwnerAttr },
                { "CXX11NoReturnAttr", AstKnownSuffix.CXX11NoReturnAttr },
                { "FinalAttr", AstKnownSuffix.FinalAttr },
                { "ClassTemplatePartialSpecialization", AstKnownSuffix.ClassTemplatePartialSpecialization },
                { "array_filler:", AstKnownSuffix.Array_filler },
                { "element:", AstKnownSuffix.Element },
                { "elements:", AstKnownSuffix.Elements },
                { "target", AstKnownSuffix.Target },
                { "nominated", AstKnownSuffix.Nominated },
                { "constructed", AstKnownSuffix.Constructed },
                { "OverrideAttr", AstKnownSuffix.OverrideAttr },
                { "filler:", AstKnownSuffix.Filler },
                { "UnresolvedUsingTypename", AstKnownSuffix.UnresolvedUsingTypename },
                { "virtual", AstKnownSuffix.Virtual },
                { "MSVtorDispAttr", AstKnownSuffix.MSVtorDispAttr },
                { "FullComment", AstKnownSuffix.FullComment },
                { "ParagraphComment", AstKnownSuffix.ParagraphComment },
                { "TextComment", AstKnownSuffix.TextComment },
                { "VerbatimLineComment", AstKnownSuffix.VerbatimLineComment },
                { "BlockCommandComment", AstKnownSuffix.BlockCommandComment },
                { "ParamCommandComment", AstKnownSuffix.ParamCommandComment }


            };
        }
    }
}