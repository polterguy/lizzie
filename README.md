
# Lizze a dynamic scripting language for .Net

Lizzie is a dynamic scripting language for .Net based upon the design pattern
called _"Symbolic Delegates"_. This allows you to execute dynamically created
scripts, that does neither compile nor are interpreted, but instead translates
directly down to delegates. Below is some example code.

```javascript
// This code will return 67 when evaluated.
var(@foo, 57)
set(@foo, add(@foo, 10))
foo
```

Lizzie is highly influenced and inspired from LISP, but with a much more familiar
notation, without the weird and unintuitive _"Polish notation"_. In such a way,
it arguably is LISP for .Net. Its dynamic nature allows you to execute snippets
of Lizzie code, inline in your C# code, by loading your code from files, or by for
instance fetching the code from some database of some sort. Below is a complete
example of how this process might look like.

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
var(@foo, 57)
set(@foo, add(foo, 10))
write-line(foo)
";

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
