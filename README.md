
# Lizze, the world's smallest script language for the CLR

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
    [Bind(Name = "write")]
    object Write(Binder<MainClass> ctx, Arguments arguments)
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
write(foo)";

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

Lizzie is highly influenced and inspired from Lisp, but without the unintuitive
_"Polish notation"_. In such a way, it arguably is Lisp for the CLR. Its dynamic
nature allows you to execute snippets of Lizzie code, inline in your C# code,
by loading your code from files, or by for instance fetching the code from some
database of some sort.

You can easily create your own _"keywords"_ in Lizzie, which allows you to create
your own DSL or _"Domain Specific programming Languages"_. Lizzie hence easily
lends itself to richer rule based engines, and similar domain specific problems,
where your code needs to be more dynamic in nature than that which the CLR allows
you to through e.g. C#, VB.NET or F#.

## What is a Symbolic Delegate?

A Symbolic Delegate is a CLR delegate that is dynamically looked up during runtime,
from a dictionary of delegates with the same signature. This allows you to dynamically
wire together delegates to an _"execution tree"_ during runtime, based upon whatever
delegate happens to be the value for your key. Hence; _"Symbolic Delegates"_. In
the above code for instance, we create 4 such symbolic delagates.

* __var__
* __set__
* __add__
* __write__

The first 3 above are added directly to the binder by referencing pre-existing
_"keywords"_ that exists in the `Functions` class, while the last _"keyword"_
is bound to the lambda object, since it's a method in our `MainClass` marked with
the `Bind` attribute. The last function from above illustrates how easily you
can extend Lizzie with your own _"keywords"_ to create your own DSL.

## Convenience classes and methods

For slightly more simplified usage, you can use the `LambdaCompiler` class, to
reduce the amount of boiler plate C# code necessary to create a lambda function
from your Lizzie code. This class have two static methods, one allowing you to
provide your own code and your own context, and the other one allowing you to
simply ignore the context, at which point your Lizzie code won't be bound to
any particular context instance at all. Below is an example of the latter.

```csharp
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
```

## How small is Lizzie

The [entire reference documentation for Lizze](/docs/introduction.md) is roughly
10 pages if you choose to print them out. This is the _entire reference documentation
for the language_. This implies that you can learn the entire programming language
literally in 10 minutes. The _"compiler"_ for the language is less than 500 lines
of code, and all _"keywords"_ are less than 800 lines of code in total. The project
as a whole has roughly 2000 lines of code, but 50% of these are comments. When built,
the dll is roughly 40KB on disc. There are 7 public classes in the project, one
attribute, and one interface. There are less than 30 methods in total, and you don't
have to use more than a handful of these to start adding dynamic scripting abilities
to your CLR code. This arguably makes Lizzie the smallest (useful) programming
language on the planet, if you ignore languages such as _"brainfuck"_, arguably
created more or less as a joke.

## How fast is Lizzie

When profiling a language such as Lizzie, there are two important things to
measure.

* Compilation speed
* Execution speed

Compilation speed is _"blistering fast"_, something we can illustrate with the
following C# code, where we compile a snippet of Lizzie code, 10,000 times.

```csharp
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
```

On my computer, which is a MacBook Air from 2016, the above code compiles 10,000
times in roughly 2,100 milliseconds. Since Lizzie is a dynamic scripting language,
intended to frequently retrieve snippets of dynamic code, and compile these, before
it executes the result - The compilation speed is hence arguably equally
important as its execution speed. On my computer, compiling Lizzie itself, and
its unit tests, **once**, requires 4.62 seconds! Compiling the above Lizzie code
10,000 times took me only 2 seconds.

### Execution speed

If we slightly modify our above code, to execute the code 10,000 times, instead
of compiling it 10,000 times, such that it resembles the following ...

```csharp
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

        // Executing the above code 10,000 times!
        Console.WriteLine("Executing some Lizzie code 10,000 times, please wait ...");
        var lambda = LambdaCompiler.Compile(code);
        Stopwatch sw = Stopwatch.StartNew();
        for (var idx = 0; idx < 10000; idx++) {
            lambda();
        }
        sw.Stop();
        Console.WriteLine($"We executed the above Lizzie code 10,000 times in {sw.ElapsedMilliseconds} milliseconds!");

        // Waiting for user input.
        Console.Read();
    }
}
```

The results on my computer says 722 milliseconds. Lizzie is not as fast as C#,
since each function invocation also requires a lookup into a `Dictionary`. In
addition, each function invocation implies evaluating a delegate, which has an
additional overhead of 20% compared to invoking a virtual method. So you can't
expect a Lizzie lambda object to evaluate as fast as the equivalent C# function.
However, compared to the execution speed of an interpreter written on the CLR,
and/or a _"true compiler"_ written on the CLR, Lizzie will purely mathematically
outperform both of these for all practical concerns, assuming you have an interest
in executing dynamically created code.

Since most practical snippets of code does complex tasks, such as accessing the
file system, and reading/writing to databases, fetching data over sockets, etc -
The execution speed overhead of your Lizzie code for most practical concerns will
be irrelevant.

**Notice** - I would not encourage you to use Lizzie for extremely demanding
resource tasks though, such as polygon rendering, algorithmic intensive math
operations, complex parsing, etc. Because after all, it will never execute
as fast as the equivalent C# code, due to its dynamic nature.


## Installation

Lizzie is still not finished, but will probably be released in the near future.
However, if you're determined on trying it out, you can clone the repository.
The release found in the releases is completely irrelevant for its current code.

Documentation is work in progress. Feel free to view the unit tests for examples
of how Lizzie works. However, evaluation of a _"body"_ will always return the
result of the Symbolic Delegate that was last evaluated. This is why the above
code will return _"77"_, because the value of the `bar` symbol is 77 after we
have added the the value of `foo` to the result of `multiply(10,2)` and assigned
the result to `bar`.

## Introduction to Lizzie

* [Reference documentation for Lizze](/docs/introduction.md)
