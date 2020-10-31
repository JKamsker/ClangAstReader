using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using ClangReader.Utilities;

namespace ClangReader.Tests
{
    public class StringSplitterPerfTests
    {
        public static void RunPerformanceTests()
        {
            long donotOptimize = 0;

            var lines = File.ReadAllLines(@"C:\Users\Weirdo\source\repos\4Story\Agnares\4Story_5.0_Source\Client\astout\cssender-02.ast");

            Console.WriteLine("Warming up");
            for (int i = 0; i < 20; i++)
            {
                RunTestRound(donotOptimize, lines);
            }

            Console.WriteLine("Ready to test");
            Console.ReadLine();

            while (true)
            {
                donotOptimize = RunTestRound(donotOptimize, lines);

                return;
            }

            Console.ReadLine();
        }

        private static long RunTestRound(long donotOptimize, string[] lines)
        {
            Console.WriteLine(nameof(TestNewSplit));
            var newSplitRes = TestNewSplit(ref donotOptimize, lines);

            Console.WriteLine(nameof(TestNewCachedLength));
            var chachedLenRes = TestNewCachedLength(ref donotOptimize, lines);

            Console.WriteLine(nameof(TestNewEngine));
            var newEngineRes = TestNewEngine(ref donotOptimize, lines);

            Console.WriteLine("TestOld");
            var oldRes = TestOld(ref donotOptimize, lines);

            Console.WriteLine(nameof(TestReallyOld));
            var reallyOldRes = TestReallyOld(ref donotOptimize, lines);

            Console.WriteLine(donotOptimize);

            Console.Write($"New: {newSplitRes.TotalMilliseconds} ");
            Console.Write($"New_Cached_Len: {chachedLenRes.TotalMilliseconds} ");
            Console.Write($"newEngineRes: {newEngineRes.TotalMilliseconds} ");

            Console.Write($"Old: {oldRes.TotalMilliseconds} ");
            Console.Write($"ReallyOld: {reallyOldRes.TotalMilliseconds} ");
            Console.WriteLine();
            return donotOptimize;
        }

        private static TimeSpan TestNewCachedLength(ref long donotOptimize, string[] lines)
        {
            var sw3 = Stopwatch.StartNew();
            foreach (var line in lines)
            {
                var a = StringArrayEnumerator.GetStringArray_CachedLength(line);
                donotOptimize += (int)a.FirstOrDefault().Length;
            }
            GC.Collect(3, GCCollectionMode.Forced);
            sw3.Stop();
            return sw3.Elapsed;
        }

        private static TimeSpan TestNewEngine(ref long donotOptimize, string[] lines)
        {
            var engine = new StringSplitEngine
            (
                new EnclosureOptions('\''),
                new EnclosureOptions('"'),
                new EnclosureOptions('<', '>')
            );

            var sw3 = Stopwatch.StartNew();
            foreach (var line in lines)
            {
                var a = engine.SplitSegmented(line);
                donotOptimize += (int)a.FirstOrDefault().Length;
            }
            GC.Collect(3, GCCollectionMode.Forced);
            sw3.Stop();
            return sw3.Elapsed;
        }

        private static TimeSpan TestReallyOld(ref long donotOptimize, string[] lines)
        {
            var sw2 = Stopwatch.StartNew();
            foreach (var line in lines)
            {
                var a = StringArrayEnumerator.GetStringArray_reallyold(line);
                donotOptimize += (int)(a.FirstOrDefault()?.Length ?? 0);
            }
            GC.Collect(3, GCCollectionMode.Forced);
            sw2.Stop();
            return sw2.Elapsed;
        }

        private static TimeSpan TestOld(ref long donotOptimize, string[] lines)
        {
            var sw1 = Stopwatch.StartNew();
            foreach (var line in lines)
            {
                var a = StringArrayEnumerator.GetStringArray_Old(line);
                donotOptimize += (int)(a.FirstOrDefault()?.Length ?? 0);
            }

            GC.Collect(3, GCCollectionMode.Forced);
            sw1.Stop();
            return sw1.Elapsed;
        }

        private static TimeSpan TestNewSplit(ref long donotOptimize, string[] lines)
        {
            var sw = Stopwatch.StartNew();
            foreach (var line in lines)
            {
                var a = StringArrayEnumerator.GetStringArray(line);
                donotOptimize += (int)a.FirstOrDefault().Length;
            }
            GC.Collect(3, GCCollectionMode.Forced);

            sw.Stop();
            return sw.Elapsed;
        }
    }
}