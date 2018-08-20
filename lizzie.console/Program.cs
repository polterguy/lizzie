using System;
using lizzie;

class MainClass
{
    public static void Main(string[] args)
    {
        // Some inline Lizzie code
        var code = @"
function(@foo, @{
  57
})
";

        // Creating a lambda function from our code.
        var function = LambdaCompiler.Compile(new Tokenizer(new LizzieTokenizer()), code);

        // Evaluates our Lizzie code making sure we bind it to our instance.
        var result = function();

        // Waiting for user input.
        Console.Read();
    }
}
