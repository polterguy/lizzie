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

    [Bind(Name = "expensive")]
    object Expensive(Binder<MainClass> binder, Arguments arguments)
    {
        // Faking an expensive process by sleeping for 1 second.
        System.Threading.Thread.Sleep(1000);

        // Returning the 1st argument, if given.
        if (arguments.Count > 0)
            return arguments.Get(0);
        return null;
    }

    public static void Main(string[] args)
    {
        // Some inline Lizzie code
        var code = @"
var(@foo1)
var(@foo2)

// Remove the 57 value to have the if below yield false
var(@foo3)

/*
 * Yields true since foo3 contains a non-null value
 */
if(any(@foo1, @foo1, @foo3), {
  write(""Any yields true"")
}, {
  write(""Any yields false"")
})
";

        // Creating a lambda function from our code.
        var function = LambdaCompiler.Compile<MainClass>(new MainClass(), code);

        // Evaluates our Lizzie code making sure we bind it to our instance.
        var result = function();

        // Waiting for user input.
        Console.Read();
    }
}
