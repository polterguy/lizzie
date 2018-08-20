using System;
using lizzie;

class MainClass
{
    public static void Main(string[] args)
    {
        // Some inline Lizzie code
        var code = @"
var(@foo, 57)
var(@bar, add(foo, multiply(10,2)))
bar";

        /*
         * Creating a lambda object not even caring about anything but the
         * result of our code.
         */
        var lambda = LambdaCompiler.Compile(code);
        var result = lambda();

        // Writing the result of the above evaluation to the console.
        Console.WriteLine("Result was: " + result);

        // Waiting for user input.
        Console.Read();
    }
}
