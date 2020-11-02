using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Toolkit.HighPerformance.Extensions;

namespace SomeOrdinaryLib
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var mySpan = "My sample source string".AsSpan();
            char dangerousRef = mySpan.DangerousGetReference();

            //new Span<char>(dangerousRef,)



            var nst = new string('\0',20);
            var charr = Unsafe.As<char[]>( nst);//;.AsSpan();

            var x = Unsafe.Add(ref charr, -1);


            //var aaa = (char[])nst;

            charr[-1] = 'o';
            charr[0] = 'o';
            charr[1] = 'i';


            var a = new ModifyableString(20);
            var b = new ModifyableString(20);

            a.SpanValue[0] = 'h';

            Console.WriteLine("Hello World! " + a.StringValue);

            "ayayay".AsSpan().CopyTo(a.SpanValue);

            Console.WriteLine("Hello World! " + a.StringValue);
            Console.WriteLine("Hello World! " + b.StringValue);
        }
    }



    public class FakeStringClass
    {
        private static char _firstChar;

        public static ref char GetRawStringData() => ref _firstChar;

        public static string FastAllocateString(int length)
        {
            return new string(' ', length);
        }

        public static char[] SomeChar => new char[20];
    }

    public readonly ref struct ModifyableString
    {

        public unsafe ModifyableString(int length)
        {
            var allocated = new string('\0', length);
            StringValue = allocated;

            fixed (char* destinationPtr = allocated)
            {
                SpanValue = new Span<char>(destinationPtr, length);
            }
        }

        public readonly string StringValue;
        public readonly Span<char> SpanValue;
    }
}