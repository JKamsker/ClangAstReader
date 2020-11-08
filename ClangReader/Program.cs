using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using ClangReader.LanguageTranslation;
using ClangReader.Lib.Ast;
using ClangReader.Lib.Ast.Interception;
using ClangReader.Lib.Ast.Models;
using ClangReader.Lib.Collections;
using ClangReader.Lib.Extensions;
using ClangReader.Pipelined;
using ClangReader.Tests;
using ClangReader.Types;
using ClangReader.Utilities;

using Microsoft.Extensions.Primitives;
using Microsoft.Toolkit.HighPerformance.Extensions;

using Newtonsoft.Json;

namespace ClangReader
{
    internal class MainClass
    {
        public static void Main(string[] args)
        {
            var fileLocation = @$"{Environment.CurrentDirectory}\..\..\..\..\..\.local\cssender-02.ast";
            if (!File.Exists(fileLocation))
            {
                Console.WriteLine($"File '{fileLocation}' doesnt exist");
                Console.ReadLine();
                return;
            }

            DoAstProcessing(fileLocation);
            Console.WriteLine("kkk");
            Console.ReadLine();
        }

        private static void DoAstProcessing(string fileLocation)
        {
            var forbidden = new HashSet<AstType>()
            {
                AstType.WhileStmt,
                AstType.ForStmt,
                AstType.DoStmt,
            };

            ProcessFile(forbidden, fileLocation);

            Console.WriteLine("[Wait for key press]");
            Console.ReadKey();
        }

        private static void ProcessFile(HashSet<AstType> forbidden, string file)
        {
            Console.WriteLine("Parsing");
            var sw = Stopwatch.StartNew();
            var reader = new AstFileReader(file);
            var rootTokens = reader.Parse(new TypeBasedFilterInterceptor(AstType.CXXMethodDecl));
            sw.Stop();
            Console.WriteLine($"Parsing took {sw.Elapsed.TotalMilliseconds} ms");

            foreach (var methodDecl in rootTokens.SelectMany(x => x.children))
            {
                var methodName = methodDecl.properties.FirstOrDefault();
                // Console.WriteLine(methodName);
                if (methodName != "SendCS_FINISHSKILL_ACK")
                {
                    continue;
                }

                // Should be able to handle all later...
                var saidMsg = GetVariableThatGetsSaid(methodDecl).FirstOrDefault();
                if (saidMsg == null)
                {
                    Console.WriteLine("\tNo message said");
                    return;
                }
                //ProcessMethod(forbidden, methodDecl, saidMsg);

                var ser = methodDecl.SerializeFriendly();

                VisitTest(methodDecl);

                Debugger.Break();
            }

            return;
        }

        private static void VisitTest(AstToken token)
        {
            if (token.Type == AstType.ForStmt)
            {
                Console.WriteLine("For statement");
            }

            foreach (var child in token.children)
            {
                VisitTest(child);
            }

            if (IsPacketShiftExpression(token))
            {
                //Type from token.children[0]
                //
                //|-CXXOperatorCallExpr 0x1f38c6b5468 <line:2997:2, line:3005:6> 'CPacket' lvalue '<<'
                //| |-ImplicitCastExpr 0x1f38c6b5450 <col:3> 'CPacket &(*)(int)' <FunctionToPointerDecay>
                //| | `-DeclRefExpr 0x1f38c6b5430 <col:3> 'CPacket &(int)' lvalue CXXMethod 0x1f386e73ac8 'operator<<' 'CPacket &(int)'

                if (TryGetPacketWriteOverloadType(token.children[0], out var typeName))
                {
                    Console.Write($"Write type : {typeName} Name: ");
                }

                if (token.children.Count > 1 && TryGetSimpleWriteFromVar(token.children[2], out var prop))
                {
                    Console.Write(prop.Name);

                    //var callPart = token.children[0].SerializeFriendly();

                    //Debugger.Break();
                }
                else if (TryGetMethodCallWrite(token.children[2], out var name))
                {
                    //Debugger.Break();

                    Console.Write(name);
                }
                else
                {
                    Console.Write("Unknown");

                    //var wholeBodySerialization = token.SerializeFriendly();
                    //var relevant = token.AsTokenDto();
                    //relevant.Children[1] = new AstTokenDto();
                    //var reser = relevant.SerializeFriendly();

                    //Debugger.Break();
                }

                //var accessor = GetChildAccesor(token.children[2]);
                Console.WriteLine();
            }
        }

        private static bool TryGetPacketWriteOverloadType(AstToken token, out string typeName)
        {
            switch (token.Type)
            {
                case AstType.ImplicitCastExpr:
                    foreach (var tokenChild in token.children)
                    {
                        if (TryGetPacketWriteOverloadType(tokenChild, out typeName))
                        {
                            return true;
                        }
                    }
                    break;

                case AstType.DeclRefExpr:
                    var match = Regex.Match(token.properties[0], "CPacket.*?\\((.*?)\\)");
                    if (match.Success)
                    {
                        typeName = match.Groups[1].Value;
                        return true;
                    }
                    break;

                default:
                    break;
            }

            typeName = string.Empty;
            return false;
        }

        private static bool TryGetMethodCallWrite(AstToken token, out string name)
        {
            //ImplicitCastExpr

            name = string.Empty;

            DeclRefExprProperties properties = null;
            switch (token.Type)
            {
                case AstType.MemberExpr when token.properties[0] == "<bound member function type>":
                    name = token.properties[1];
                    name = name.TrimStart("->").TrimStart("Get");
                    return true;

                case AstType.CXXMemberCallExpr:
                    foreach (var tokenChild in token.children)
                    {
                        if (TryGetMethodCallWrite(tokenChild, out name))
                        {
                            return true;
                        }
                    }
                    break;

                default:
                    break;
            }
            return false;
            return false;
        }

        private static bool IsPacketShiftExpression(AstToken token)
        {
            return token.Type == AstType.CXXOperatorCallExpr && token.properties.Length <= 3 && (token.properties[2] == "<<" || token.properties[2] == "'<<'");
        }

        private static bool TryGetSimpleWriteFromVar(AstToken token, out DeclRefExprProperties properties)
        {
            properties = null;
            switch (token.Type)
            {
                case AstType.DeclRefExpr when token.children.Count == 0:
                    properties = new DeclRefExprProperties(token);
                    return true;

                case AstType.CXXOperatorCallExpr when IsPacketShiftExpression(token):
                case AstType.ImplicitCastExpr:
                    foreach (var tokenChild in token.children)
                    {
                        if (TryGetSimpleWriteFromVar(tokenChild, out properties))
                        {
                            return true;
                        }
                    }
                    break;

                default:
                    break;
            }
            return false;
        }

        private static DeclRefExprProperties GetChildAccesor(AstToken token)
        {
            if (token.Type == AstType.DeclRefExpr && token.children.Count == 0)
            {
                return new DeclRefExprProperties(token);
            }

            if (token.Type == AstType.MemberExpr && token.children.Count > 0 && token.properties[0] == "<bound member function type>")
            {
                var func = token.properties[1];
                //Debugger.Break();
            }

            if (token.Type == AstType.CXXMemberCallExpr)
            {
                Debugger.Break();
            }

            foreach (var tokenChild in token.children)
            {
                var accessor = GetChildAccesor(tokenChild);
                if (accessor != null)
                {
                    return accessor;
                }
            }

            return null;
        }

        private static void ProcessMethod(HashSet<AstType> forbidden, AstToken methodDecl, DeclRefExprProperties saidMsg)
        {
            // Should be able to handle all later...
            var usages = GetUsages(saidMsg, methodDecl).ToList();
            foreach (var usage in usages)
            {
                var parentsInMethod = usage.parent.TraverseParents()
                    .TakeWhile(x => x != methodDecl)
                    .ToList();

                var allOperatorExpressions = parentsInMethod
                    .Where(x => x.Type == AstType.CXXOperatorCallExpr)
                    .Where(x => x.properties.Length == 3 && (x.properties[2] == "<<" || x.properties[2] == "'<<'"))
                    .ToList();

                var goodParents = allOperatorExpressions
                    .Where(x => x.children.Count == 3)
                    .ToList();

                foreach (var parent in goodParents)
                {
                    var decl = parent.children[2].VisitEnumerable(x => x.Type == AstType.DeclRefExpr).FirstOrDefault();
                    if (decl == null)
                    {
                        continue;
                    }

                    var properties = new DeclRefExprProperties(decl);

                    Console.WriteLine($"\t[{properties.InstanceId}] {properties.Name} ({properties.Type})");

                    var illegalOperation = EnumerableExtensions.TakeWhile(decl.TraverseParents(), x => x != methodDecl).FirstOrDefault(x => forbidden.Contains(x.Type));
                    if (illegalOperation != null)
                    {
                        Console.WriteLine($"\tIllegal operation - {illegalOperation.Type}");

                        DebugNodes(decl.TraverseParents().TakeWhile(x => x != methodDecl));

                        Console.WriteLine();
                    }
                }
            }
        }

        private static void DebugNodes(IEnumerable<AstToken> parents)
        {
            // parents.First().parent.parent.parent == par
            /*	for (BYTE i = 0; i < (BYTE) vTarget.size(); ++i)
            {
	            pMSG
		            << vTarget[i]->m_dwID <---
		            << vTarget[i]->m_bType;
            }
            */

            foreach (var par in parents)
            {
                try
                {
                    var json = par.SerializeFriendly();
                    //var translated = AstTranslator.GetFunctionBody(par);
                    Debugger.Break();
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
                x => x.Type == AstType.DeclRefExpr && new DeclRefExprProperties(x).Equals(saidMsg)
            );
        }

        private static IEnumerable<DeclRefExprProperties> GetVariableThatGetsSaid(AstToken methodDecl)
        {
            var memberExpressions = methodDecl.VisitEnumerable
            (
                x => x.Type == AstType.MemberExpr && x.parent.Type == AstType.CXXMemberCallExpr && x.properties.Contains("->Say")
            );

            foreach (var memberExpression in memberExpressions)
            {
                var otherchildren = memberExpression.parent
                        .VisitEnumerable(x => x != memberExpression)
                        .Where(x => x.Type == AstType.DeclRefExpr)
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
    }
}