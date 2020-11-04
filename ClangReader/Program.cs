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
            var forbidden = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "WhileStmt",
                "ForStmt",
                "DoStmt",
            };

            ProcessFile(forbidden, @"C:\Users\Weirdo\source\repos\4Story\Agnares\4Story_5.0_Source\Client\astout\cssender-02.ast");

            Console.WriteLine("[Wait for key press]");
            Console.ReadKey();
        }

        private static void ProcessFile(HashSet<string> forbidden, string file)
        {
            var sw = Stopwatch.StartNew();
            //var reader = new AstFileReader(file);

            //reader.ParseAsync().Wait();
            //sw.Stop();
            //Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            //return;

            AstTextFile dumpFile = new AstTextFile(file);
            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            return;
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
                var parentsInMethod = EnumerableExtensions.TakeWhile(usage.parent
                        .TraverseParents(), x => x != methodDecl)
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

                    var illegalOperation = EnumerableExtensions.TakeWhile(decl.TraverseParents(), x => x != methodDecl).FirstOrDefault(x => forbidden.Contains(x.name));
                    if (illegalOperation != null)
                    {
                        Console.WriteLine($"\tIllegal operation - {illegalOperation.name}");

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

//Console.WriteLine(typeof(string).Assembly.FullName);
//Console.WriteLine(typeof(string).Assembly.Location);

//var fastAlloc = (Func<int, string>)typeof(String)
//    .GetMethod("FastAllocateString", BindingFlags.Static | BindingFlags.NonPublic)
//    .CreateDelegate(typeof(Func<int, string>))
//    ;

//var res = fastAlloc(20);
//var asrefed = Unsafe.AsRef<char>(res.GetPinnableReference());

//var constructor = typeof(Span<>)
//        .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
//    ;

////var ins = constructor.First();
////var obj = ins.Invoke(ref res.GetPinnableReference());

//var shared = MemoryPool<byte>.Shared.Rent(1024);
//var memory = shared.Memory;
//var span = memory.Span;

//var str = new ArraySegment<char>("asdf".ToCharArray());

//var str1 = str.ToString();
//var str2 = string.Concat(str.AsSpan(), str.AsSpan());

//while (true)
//{
//    var sw1 = Stopwatch.StartNew();
//    for (int i = 0; i < 100_000_000; i++)
//    {
//        var as1 = memory.AsArraySegment();
//    }
//    sw1.Stop();
//    Console.WriteLine(sw1.Elapsed.TotalMilliseconds);

//    var sw2 = Stopwatch.StartNew();
//    for (int i = 0; i < 100_000_000; i++)
//    {
//        var ok = MemoryMarshal.TryGetArray(memory, out ArraySegment<byte> as2);
//    }
//    sw2.Stop();
//    Console.WriteLine(sw2.Elapsed.TotalMilliseconds);
//    Console.WriteLine();
//    Thread.Sleep(1000);
//}

//span.GetDjb2HashCode();

////var sp1 = "henlo".AsSpan();
////var sp2 = "henlo".AsSpan();
////var sw = Stopwatch.StartNew();
////for (int i = 0; i < 1_000_000_000; i++)
////{
////    sp1.GetDjb2HashCode();
////}
////sw.Stop();
////Console.WriteLine(sw.Elapsed.TotalMilliseconds);
////DoAstProcessing();

//Console.ReadLine();
//return;
////await PerfTestMultiThreaded();