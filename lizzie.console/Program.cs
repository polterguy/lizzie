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
write(+(5, 2, 50))
write(-(100, 30, 3))
write(*(5, 3, 2))
write(/(100, 4))
write(%(18, 4))
";

        // Compiling the above code, 'binding' to our MainClass instance.
        var lambda = LambdaCompiler.Compile<MainClass>(new MainClass(), code);
        var result = lambda();

        // Waiting for user input.
        Console.Read();
    }
}