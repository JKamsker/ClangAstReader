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
            DoAstProcessing();
            Console.WriteLine("kkk");
            Console.ReadLine();
        }

        private static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            Debugger.Break();
            return null;
        }

        private static Assembly? AssemblyLoadContext_AssemblyResolve(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            if (arg2.FullName.StartsWith("SomeOrdinaryLib"))
            {
                return Assembly.LoadFile(@"C:\Users\Weirdo\source\repos\external\ClangAstReader\SomeOrdinaryLib\bin\Debug\netcoreapp3.1\out\SomeOrdinaryLib-mod-02.dll");
            }

            Debugger.Break();
            return null;
        }

        private static void DoSomeOrdinaryStufF()
        {
            var rstr = new SomeOrdinaryLib.StringResultStruct(20);

            rstr.SpanValue[0] = 'a';
            Console.WriteLine(rstr.StringValue);
        }

        private static void Splitfancy(bool skipEmpty = true)
        {
            var stringBuilder = new List<string>();
            var aspan = "as,da-sd,,f".AsSpan();

            var tempspan = aspan.Slice(0);
            while (tempspan.Length != 0)
            {
                var index = tempspan.IndexOfAny(',', '-');
                if (index == -1)
                {
                    stringBuilder.Add(tempspan.ToString());
                    tempspan = ReadOnlySpan<char>.Empty;
                    // Console.WriteLine(stringBuilder.ToString());
                    break;
                }

                if (index > 0 || (index == 0 && !skipEmpty))
                {
                    var window = tempspan[0..index];
                    stringBuilder.Add(window.ToString());
                }

                tempspan = tempspan[++index..];
            }
            Debugger.Break();
        }

        private static void PerfTestSingleThread()
        {
            var engine = new StringSplitEngine
            (
                new EnclosureOptions('\''),
                new EnclosureOptions('"'),
                new EnclosureOptions('<', '>')
            );

            var sw3 = Stopwatch.StartNew();

            long donotOptimize = 0;
            foreach (var line in File.ReadLines(@"C:\Users\Weirdo\source\repos\4Story\Agnares\4Story_5.0_Source\Client\astout\cssender-02.ast"))
            {
                var a = engine.SplitSegmented(line);
                donotOptimize += (int)a.FirstOrDefault().Length;
            }
            GC.Collect(3, GCCollectionMode.Forced);
            sw3.Stop();
            Console.WriteLine($"Singlethreaded Completed in {sw3.Elapsed.TotalMilliseconds}");

            Console.ReadLine();
        }

        private static bool running;
        private static Channel<IEnumerable<string>> inputChannel;

        private static async Task PerfTestMultiThreaded()
        {
            while (true)
            {
                running = true;
                var workers = new List<Task>();

                inputChannel = Channel.CreateBounded<IEnumerable<string>>(new BoundedChannelOptions(10 * 1024)
                {
                    SingleWriter = true,
                    SingleReader = false,
                    AllowSynchronousContinuations = false,
                });

                for (int i = 0; i < Environment.ProcessorCount; i++)
                {
                    workers.Add(Task.Factory.StartNew(async () => await DoWork()));
                }

                var sw = Stopwatch.StartNew();
                foreach (var lines in File.ReadLines(@"C:\Users\Weirdo\source\repos\4Story\Agnares\4Story_5.0_Source\Client\astout\cssender-02.ast").ChunkIn(512))
                {
                    await inputChannel.Writer.WriteAsync(lines);
                }

                inputChannel.Writer.Complete();
                running = false;

                await Task.WhenAll(workers);

                sw.Stop();
                Console.WriteLine($"Finished in {sw.Elapsed.TotalMilliseconds}");
            }
        }

        private static async Task DoWork()
        {
            var engine = new StringSplitEngine
            (
                new EnclosureOptions('\''),
                new EnclosureOptions('"'),
                new EnclosureOptions('<', '>')
            );

            long donotOptimize = 0;

            try
            {
                while (running)
                {
                    var lines = await inputChannel.Reader.ReadAsync();
                    foreach (var line in lines)
                    {
                        var res = engine.SplitSegmented(line);
                        donotOptimize += (int)res.FirstOrDefault().Length;
                    }
                }
            }
            catch (OperationCanceledException e)
            {
            }
        }

        private static void TestEngine()
        {
            var engine = new StringSplitEngine
            (
                new EnclosureOptions('\''),
                new EnclosureOptions('"'),
                new EnclosureOptions('<', '>')
            );

            //var reference =  "'OI BOY' Boy Oi <start ay ay <level oi d > end > this should be extra <start but no< end >";
            //var result = engine.Split(reference);
            //var result1 = StringArrayEnumerator.GetStringArray_Old(reference);

            var lines = File.ReadLines(@"C:\Users\Weirdo\source\repos\4Story\Agnares\4Story_5.0_Source\Client\astout\cssender-02.ast");
            foreach (var line in lines)
            {
                var result = engine.Split(line);
                var result1 = engine.SplitSegmented(line).Select(x => x.ToString()).ToArray();

                if (result.SequenceEqual(result1))
                {
                    continue;
                }

                if (result.Length != result1.Length)
                {
                    Console.WriteLine("Len unequal");
                    //Debugger.Break();
                    continue;
                }

                foreach (var (res1, res2, curi) in result.Zip(result1, (x, y) => (x, y)).Select((x, i) => (x.x, x.y, i)))
                {
                    if (res1.Equals(res2))
                    {
                        continue;
                    }

                    if (res1.Equals(res2.TrimStart('\'').TrimEnd('\'')))
                    {
                        continue;
                    }

                    Console.WriteLine($"|{res1}| != |{res2}| at {curi}");
                }

                //Debugger.Break();
            }
        }

        private static void DoAstProcessing()
        {
            var forbidden = new HashSet<AstKnownSuffix>()
            {
                AstKnownSuffix.WhileStmt,
                AstKnownSuffix.ForStmt,
                AstKnownSuffix.DoStmt,
            };

            ProcessFile(forbidden, @"C:\Users\Weirdo\source\repos\4Story\Agnares\4Story_5.0_Source\Client\astout\cssender-02.ast");

            Console.WriteLine("[Wait for key press]");
            Console.ReadKey();
        }

        private static void ProcessFile(HashSet<AstKnownSuffix> forbidden, string file)
        {
            var sw = Stopwatch.StartNew();
            var reader = new AstFileReader(file);
            var rootTokens = reader.Parse_02();

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

            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            return;
        }

        private static void VisitTest(AstToken token)
        {
            //Console.WriteLine($"{prefix}{token.Type}");
            foreach (var child in token.children)
            {
                VisitTest(child);
            }

            if (IsPacketShiftExpression(token))
            {
                if (token.children.Count > 1 && TryGetSimpleWriteFromVar(token.children[2], out var prop))
                {
                    //Type from
                    //
                    //|-CXXOperatorCallExpr 0x1f38c6b5468 <line:2997:2, line:3005:6> 'CPacket' lvalue '<<'
                    //| |-ImplicitCastExpr 0x1f38c6b5450 <col:3> 'CPacket &(*)(int)' <FunctionToPointerDecay>
                    //| | `-DeclRefExpr 0x1f38c6b5430 <col:3> 'CPacket &(int)' lvalue CXXMethod 0x1f386e73ac8 'operator<<' 'CPacket &(int)'
                    //
                    //token.children[0]
                    Debugger.Break();
                }
                else if (TryGetMethodCallWrite(token.children[2], out var name))
                {
                    Debugger.Break();
                }
                //var accessor = GetChildAccesor(token.children[2]);
            }
        }

        private static bool TryGetMethodCallWrite(AstToken token, out string name)
        {
            name = string.Empty;

            DeclRefExprProperties properties = null;
            switch (token.Type)
            {
                case AstKnownSuffix.MemberExpr when token.properties[0] == "<bound member function type>":
                    name = token.properties[1];
                    name = name.TrimStart("->").TrimStart("Get");
                    return true;

                case AstKnownSuffix.CXXMemberCallExpr:
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
            return token.Type == AstKnownSuffix.CXXOperatorCallExpr && token.properties.Length <= 3 && (token.properties[2] == "<<" || token.properties[2] == "'<<'");
        }

        private static bool TryGetSimpleWriteFromVar(AstToken token, out DeclRefExprProperties properties)
        {
            properties = null;
            switch (token.Type)
            {
                case AstKnownSuffix.DeclRefExpr when token.children.Count == 0:
                    properties = new DeclRefExprProperties(token);
                    return true;

                case AstKnownSuffix.CXXOperatorCallExpr when IsPacketShiftExpression(token):
                case AstKnownSuffix.ImplicitCastExpr:
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
            if (token.Type == AstKnownSuffix.DeclRefExpr && token.children.Count == 0)
            {
                return new DeclRefExprProperties(token);
            }

            if (token.Type == AstKnownSuffix.MemberExpr && token.children.Count > 0 && token.properties[0] == "<bound member function type>")
            {
                var func = token.properties[1];
                //Debugger.Break();
            }

            if (token.Type == AstKnownSuffix.CXXMemberCallExpr)
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

        private static void ProcessMethod(HashSet<AstKnownSuffix> forbidden, AstToken methodDecl, DeclRefExprProperties saidMsg)
        {
            // Should be able to handle all later...
            var usages = GetUsages(saidMsg, methodDecl).ToList();
            foreach (var usage in usages)
            {
                var parentsInMethod = usage.parent.TraverseParents()
                    .TakeWhile(x => x != methodDecl)
                    .ToList();

                var allOperatorExpressions = parentsInMethod
                    .Where(x => x.Type == AstKnownSuffix.CXXOperatorCallExpr)
                    .Where(x => x.properties.Length == 3 && (x.properties[2] == "<<" || x.properties[2] == "'<<'"))
                    .ToList();

                var goodParents = allOperatorExpressions
                    .Where(x => x.children.Count == 3)
                    .ToList();

                foreach (var parent in goodParents)
                {
                    var decl = parent.children[2].VisitEnumerable(x => x.Type == AstKnownSuffix.DeclRefExpr).FirstOrDefault();
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
                x => x.Type == AstKnownSuffix.DeclRefExpr && new DeclRefExprProperties(x).Equals(saidMsg)
            );
        }

        private static IEnumerable<DeclRefExprProperties> GetVariableThatGetsSaid(AstToken methodDecl)
        {
            var memberExpressions = methodDecl.VisitEnumerable
            (
                x => x.Type == AstKnownSuffix.MemberExpr && x.parent.Type == AstKnownSuffix.CXXMemberCallExpr && x.properties.Contains("->Say")
            );

            foreach (var memberExpression in memberExpressions)
            {
                var otherchildren = memberExpression.parent
                        .VisitEnumerable(x => x != memberExpression)
                        .Where(x => x.Type == AstKnownSuffix.DeclRefExpr)
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