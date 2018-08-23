using System;
using lizzie;

class MainClass
{
    [Bind(Name = "write")]
    object WriteLine(Binder<MainClass> binder, Arguments arguments)
    {
        Console.WriteLine(arguments.Get(0));
        return null;
    }

    public static void Main(string[] args)
    {
        // Some inline Lizzie code
        var code = @"
// Creating a function.
var(@foo, function({

  /*
   * Checking value of input argument, and returning 57 if it has
   * a value, otherwise we return 67.
   */
  if(not(eq(input, ""Thomas"")), {
    ""Welcome stranger""
  }, {
    ""Welcome home boss!!""
  })

}, @input))

// Evaluating the above function.
var(@tmp1, foo(""Thomas""))
write(add(""Foo returned "", tmp1))
";

        // Creating a lambda function from our code.
        var function = LambdaCompiler.Compile<MainClass>(new MainClass(), code);

        // Evaluates our Lizzie code making sure we bind it to our instance.
        var result = function();

        // Waiting for user input.
        Console.Read();
    }
}
