using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

using ClangReader.LanguageTranslation;
using ClangReader.Models;
using ClangReader.Types;
using ClangReader.Utilities;

using Newtonsoft.Json;

namespace ClangReader
{
    internal class MainClass
    {
        //public static Dictionary<string, TranslationFile> translation = new Dictionary<string, TranslationFile>();

        public static void Main(string[] args)
        {
            //var engine = new StringSplitEngine
            //(
            //    new EnclosureOptions('\''),
            //    new EnclosureOptions('"'),
            //    new EnclosureOptions('<', '>')
            //);

            //var result = engine.Split("'OI BOY' Boy Oi <start ay ay <level oi d > end > this should be extra <start but no< end >");

            string input = "one \"two two\" three \"four four\" five six";
            var parts = Regex.Matches(input, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToList();

            //var splitArray = Regex.Split(subjectString, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            var forbidden = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "WhileStmt",
                "ForStmt",
                "DoStmt",
            };

            ProcessFile(forbidden, @"C:\Users\Weirdo\source\repos\4Story\Agnares\4Story_5.0_Source\Client\astout\cssender-02.ast");
            //var files = System.IO.Directory.GetFiles(@"C:\Users\Weirdo\source\repos\4Story\Agnares\4Story_5.0_Source\Client\astout", "*.ast");
            //int counter = 0;
            //foreach (var file in files)
            //{
            //    Console.WriteLine("[{0}/{1}] {2}", counter++, files.Length, file);
            //    //if (counter < 25) continue;

            //    ProcessFile(forbidden, file);
            //}
            Console.WriteLine("[Wait for key press]");
            Console.ReadKey();
        }

        private static void ProcessFile(HashSet<string> forbidden, string file)
        {
            AstTextFile dumpFile = new AstTextFile(file);
            //string astDumpPath = "/home/misha/Projects/cache/functionExecute.cpp.dump";
            //string astDumpPath = "F:/cache/jsmn.cpp.dump";
            //AstTextFile dumpFile = new AstTextFile(astDumpPath);

            foreach (var rootToken in dumpFile.rootTokens)
            {
                var methodDecls = rootToken.children
                    .Where(x => x.name == "CXXMethodDecl")
                    .Where(x => x.context.sourceFile?.EndsWith("TNetSender.cpp") == true || x.context.sourceFile?.Contains("CSSender") == true)
                    .ToList();

                foreach (var methodDecl in methodDecls)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    ProcessMethod(forbidden, methodDecl);
                }
            }
        }

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
                var parentsInMethod = usage.parent
                    .TraverseParents()
                    .TakeWhile(x => x != methodDecl)
                    .ToList();

                var goodParents = parentsInMethod
                    .Where(x => x.name == "CXXOperatorCallExpr")
                    .Where(x => x.properties.Length == 3 && (x.properties[2] == "<<" || x.properties[2] == "'<<'"))
                    .Where(x => x.children.Count == 3)
                    .ToList();

                foreach (var parent in goodParents)
                {
                    var decl = parent.children[2].VisitEnumerable(x => x.name == "DeclRefExpr").FirstOrDefault();
                    if (decl == null)
                    {
                        continue;
                    }

                    var properties = new DeclRefExprProperties(decl);

                    Console.WriteLine($"\t[{properties.InstanceId}] {properties.Name} ({properties.Type})");

                    var illegalOperation = decl.TraverseParents().TakeWhile(x => x != methodDecl).FirstOrDefault(x => forbidden.Contains(x.name));
                    if (illegalOperation != null)
                    {
                        Console.WriteLine($"\tIllegal operation - {illegalOperation.name}");

                        DebugNodes(decl.TraverseParents().TakeWhile(x => x != methodDecl));

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
                x => x.name == "DeclRefExpr" && new DeclRefExprProperties(x).Equals(saidMsg)
            );
        }

        private static IEnumerable<DeclRefExprProperties> GetVariableThatGetsSaid(AstToken methodDecl)
        {
            var memberExpressions = methodDecl.VisitEnumerable
            (
                x => x.name == "MemberExpr" && x.parent.name == "CXXMemberCallExpr" && x.properties.Contains("->Say")
            );

            foreach (var memberExpression in memberExpressions)
            {
                var otherchildren = memberExpression.parent
                    .VisitEnumerable(x => x != memberExpression)
                    .Where(x => x.name == "DeclRefExpr")
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

                //if (otherchildren.Count > 1)
                //{
                //    throw new NotSupportedException("Multiple decls are not supported");
                //}

                //if (otherchildren.Count == 1)
                //{
                //}
            }
        }

        private static void ProcessSetInstruction(AstToken methodDecl)
        {
            var setId = methodDecl.VisitEnumerable(x => x.name == "MemberExpr" && x.properties.Contains(".SetID")).FirstOrDefault();

            if (setId != null)
            {
                var literals = setId.parent.VisitEnumerable(x => x.name == "BinaryOperator");
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