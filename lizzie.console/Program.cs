using System;
using System.Diagnostics;
using lizzie;

class MainClass
{
    public static int Foo(int input)
    {
        return input + 1;
    }

    public static void Main(string[] args)
    {
        // Executing some C# code 10,000 times!
        Console.WriteLine("Executing some C# code 10,000 times, please wait ...");
        Stopwatch sw = Stopwatch.StartNew();
        for (var idx = 0; idx < 10000; idx++) {
            var integer1 = 5 + 2 + 50;
            Foo(integer1);
            var integer2 = 100 - 30 - 3;
            Foo(integer2);
            var integer3 = 5 * 3 * 2;
            Foo(integer3);
            var integer4 = 100 / 4;
            Foo(integer4);
            var integer5 = 10 % 4;
            Foo(integer5);
        }
        sw.Stop();
        Console.WriteLine($"We executed the above Lizzie code 10,000 times in {sw.ElapsedMilliseconds} milliseconds!");

        // Waiting for user input.
        Console.Read();
    }
}