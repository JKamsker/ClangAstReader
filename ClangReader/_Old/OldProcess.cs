using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ClangReader.LanguageTranslation;
using ClangReader.Lib.Ast.Models;
using ClangReader.Lib.Extensions;

namespace ClangReader._Old
{
    class OldProcess
    {
        private static void ProcessMethod(HashSet<string> forbidden, AstToken methodDecl)
        {
            //  var meth = JsonConvert.SerializeObject(methodDecl.AsTokenDto(),Formatting.Indented);

            var methodName = methodDecl.properties.FirstOrDefault();
            Console.WriteLine($"Name: {methodName}");

            if (methodName == "SendCS_FINISHSKILL_ACK")
            {
                Debugger.Break();
            }

            var saidMsg = GetVariableThatGetsSaid(methodDecl).FirstOrDefault();
            if (saidMsg == null)
            {
                Console.WriteLine("\tNo message said");
                return;
            }

            var usages = GetUsages(saidMsg, methodDecl)
                // .Where(x => x.TraverseParents(true).Any(m => m.name == "MemberExpr" && m.properties.Contains(".SetID")))
                .ToList();

            foreach (var usage in usages)
            {
                //var allParents = usage.parent.TraverseParents().ToList();
                var parentsInMethod = EnumerableExtensions.TakeWhile(usage.parent
                        .TraverseParents(), x => x != methodDecl)
                    .ToList();

                var goodParents = parentsInMethod
                    .Where(x => x.unknownName == "CXXOperatorCallExpr")
                    .Where(x => x.properties.Length == 3 && (x.properties[2] == "<<" || x.properties[2] == "'<<'"))
                    .Where(x => x.children.Count == 3)
                    .ToList();

                foreach (var parent in goodParents)
                {
                    var decl = parent.children[2].VisitEnumerable(x => x.unknownName == "DeclRefExpr").FirstOrDefault();
                    if (decl == null)
                    {
                        continue;
                    }

                    var properties = new DeclRefExprProperties(decl);

                    Console.WriteLine($"\t[{properties.InstanceId}] {properties.Name} ({properties.Type})");

                    var illegalOperation = EnumerableExtensions.TakeWhile(decl.TraverseParents(), x => x != methodDecl).FirstOrDefault(x => forbidden.Contains(x.unknownName));
                    if (illegalOperation != null)
                    {
                        Console.WriteLine($"\tIllegal operation - {illegalOperation.unknownName}");

                        DebugNodes(EnumerableExtensions.TakeWhile(decl.TraverseParents(), x => x != methodDecl));

                        Console.WriteLine();
                    }
                }
            }

            //var found = new List<AstToken>();
            //methodDecl.Visit
            //(

            //    x => (x.name == "DeclRefExpr" && x.properties[2] == "ParmVar"),
            //    x => found.Add(x)
            //);

            //ProcessSetInstruction(methodDecl);

            //var compounts = found.Select(x => new
            //{
            //    Name = x.properties[4],
            //    Type = x.properties[5],
            //    Ast = x
            //}).ToList();

            //foreach (var compount in compounts)
            //{
            //    var illegalStatement = compount.Ast.TraverseParents().FirstOrDefault(x => forbidden.Contains(x.name));

            //    var addition = illegalStatement != null ? $"- Illegal statement found: {illegalStatement.name}" : string.Empty;
            //    Console.WriteLine($"\t{compount.Name} - {compount.Type} {addition}");

            //    if (addition != string.Empty)
            //    {
            //        //foreach (var cAst in compount.Ast.TraverseParents())
            //        //{
            //        //    try
            //        //    {
            //        //        Console.WriteLine();
            //        //    }
            //        //    catch (Exception e)
            //        //    {
            //        //    }
            //        //}

            //        Console.WriteLine();
            //        Debugger.Break();
            //    }
            //}
        }

        private static void DebugNodes(IEnumerable<AstToken> parents)
        {
            foreach (var par in parents)
            {
                try
                {
                    var json = par.SerializeFriendly();
                    var translated = AstTranslator.GetFunctionBody(par);
                    // Debugger.Break();
                }
                catch (Exception e)
                {
                }
            }
        }

        private static IEnumerable<AstToken> GetUsages(DeclRefExprProperties saidMsg, AstToken methodDecl)
        {
            return methodDecl.VisitEnumerable
            (
                x => x.unknownName == "DeclRefExpr" && new DeclRefExprProperties(x).Equals(saidMsg)
            );
        }

        private static IEnumerable<DeclRefExprProperties> GetVariableThatGetsSaid(AstToken methodDecl)
        {
            var memberExpressions = methodDecl.VisitEnumerable
            (
                x => x.unknownName == "MemberExpr" && x.parent.unknownName == "CXXMemberCallExpr" && x.properties.Contains("->Say")
            );

            foreach (var memberExpression in memberExpressions)
            {
                var otherchildren = memberExpression.parent
                    .VisitEnumerable(x => x != memberExpression)
                    .Where(x => x.unknownName == "DeclRefExpr")
                    .Select(x => new DeclRefExprProperties(x))
                    .ToList()
                    ;

                //Just take the last one

                if (otherchildren.Count == 1)
                {
                    yield return otherchildren.FirstOrDefault();
                }
                else
                {
                    //Multiple (path: pSession->Say( &vMSG ))
                    yield return otherchildren.FirstOrDefault(x => x.Token.properties[0] == "CPacket");
                }
            }
        }

        private static void ProcessSetInstruction(AstToken methodDecl)
        {
            var setId = methodDecl.VisitEnumerable(x => x.unknownName == "MemberExpr" && x.properties.Contains(".SetID")).FirstOrDefault();

            if (setId != null)
            {
                var literals = setId.parent.VisitEnumerable(x => x.unknownName == "BinaryOperator");
                foreach (var literal in literals)
                {
                    var exp = literal.AsExpression();
                    Console.WriteLine($"Set: {exp}");
                }
            }
        }

        //private static void Visit(AstToken token, Func<AstToken, bool> predicate, Action<AstToken> onFound)
        //{
        //    foreach (var tokenChild in token.children)
        //    {
        //        if (predicate(tokenChild))
        //        {
        //            onFound(tokenChild);
        //        }
        //        Visit(tokenChild, predicate, onFound);
        //    }
        //}

        //private static IEnumerable<AstToken> VisitEnumerable(AstToken token, Func<AstToken, bool> predicate)
        //{
        //    foreach (var tokenChild in token.children)
        //    {
        //        if (predicate(tokenChild))
        //        {
        //            yield return tokenChild;
        //        }
        //        var results = VisitEnumerable(tokenChild, predicate);
        //        foreach (var astToken in results)
        //        {
        //            yield return astToken;
        //        }
        //    }
        //}
    }
}
