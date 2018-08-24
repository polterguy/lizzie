using System;
using lizzie;

class MainClass
{
    [Bind(Name = "write")]
    object Write(Binder<MainClass> binder, Arguments arguments)
    {
        Console.WriteLine(arguments.Get(0));
        return null;
    }

    public static void Main(string[] args)
    {
        // Some inline Lizzie code
        var code = @"
var(@foo, 
  function({
    write(""Hello "")
    write(name)
    write(""you are "")
    write(age)
    write(""years old ..."")
  },

  // These are arguments our function can handle.
  @name,
  @age)
)

// Invoking our function.
foo(""Thomas"", 44)
";

        // Compiling the above code, 'binding' to our MainClass instance.
        var lambda = LambdaCompiler.Compile<MainClass>(new MainClass(), code);
        var result = lambda();

        // Waiting for user input.
        Console.Read();
    }
}