using System;
using System.Diagnostics;
using lizzie;

class MainClass
{
    public static void Main(string[] args)
    {
        // Some inline Lizzie code.
        var code = @"
+(5, 2, 50)
-(100, 30, 3)
*(5, 3, 2)
/(100, 4)
%(18, 4)
";

        // Compiling the above code 10,000 times!
        Console.WriteLine("Compiling some Lizzie code 10,000 times, please wait ...");
        Stopwatch sw = Stopwatch.StartNew();
        for (var idx = 0; idx < 10000; idx++) {
            var lambda = LambdaCompiler.Compile(code);
        }
        sw.Stop();
        Console.WriteLine($"We compiled the above Lizzie code 10,000 times in {sw.ElapsedMilliseconds} milliseconds!");

        // Waiting for user input.
        Console.Read();
    }
}