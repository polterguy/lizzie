
# Lizze a dynamic scripting language for .Net

Lizzie is a dynamic scripting language for .Net based upon the design pattern
called _"Symbolic Delegates"_. This allows you to execute dynamically created
scripts, that does neither compile nor are interpreted, but instead translates
directly down to delegates. Below is an example of using Lizzie.

```csharp
using System;
using lizzie;

class MainClass
{
    // This method will be available as a function in your Lizzie code.
    [Bind(Name = "write-line")]
    object ConsoleWriteLine(Binder<MainClass> ctx, Arguments arguments)
    {
        Console.WriteLine(arguments.Get<string>(0));
        return null;
    }

    public static void Main(string[] args)
    {
        // Some inline Lizzie code
        var code = @"

// Declaring "foo" as a variable and setting its initial value
var(@foo, 57)

// Adding 10 to "foo"
set(@foo, add(foo, 10))

// Writing out the value of "foo" on the console
write-line(foo)";

        // Creating a tokenizer to tokenize our Lizzie code.
        var tokenizer = new Tokenizer(new LizzieTokenizer());

        // Creating a lambda function from our code.
        var function = Compiler.Compile<MainClass>(tokenizer, code);

        // Creating an instance of our class, which we can bind to our code.
        var context = new MainClass();

        // Creating a binder, and adding some keywords to it.
        var binder = new Binder<MainClass>();
        binder["var"] = Functions<MainClass>.Var;
        binder["set"] = Functions<MainClass>.Set;
        binder["add"] = Functions<MainClass>.Add;

        // Evaluates our Lizzie code making sure we bind it to our instance.
        function(context, binder);

        // Waiting for user input.
        Console.Read();
    }
}
```

Lizzie is highly influenced and inspired from LISP, but without the unintuitive
_"Polish notation"_. In such a way, it arguably is LISP for .Net. Its dynamic
nature allows you to execute snippets of Lizzie code, inline in your C# code,
by loading your code from files, or by for instance fetching the code from some
database of some sort.

You can easily create your own _"keywords"_ in Lizzie, which allows you to create
your own DSL or _"Domain Specific programming Languages"_. Lizzie hence easily
lends itself to richer rule based engines, and similar domain specific problems.
