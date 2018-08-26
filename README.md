
# Lizzie, the world's smallest scripting language for the CLR

Lizzie is a dynamic scripting language for .Net based upon a design pattern
called _"Symbolic Delegates"_. This allows you to execute dynamically created
scripts, that does neither compile nor are interpreted, but instead _"compiles"_
directly down to managed CLR delegates. Below is an example of using Lizzie from
C#.

```csharp
using System;
using lizzie;

class MainClass
{
    public static void Main(string[] args)
    {
        // Some inline Lizzie code
        var code = @"

// Multiples 10 by 2 and adds to 57
+(57, *(10, 2))

";

        // Creating a Lizzie lambda object from the above code, and evaluating it
        var lambda = LambdaCompiler.Compile(code);
        var result = lambda();

        // Writing the result of the above evaluation to the console
        Console.WriteLine("Result was: " + result);

        // Waiting for user input
        Console.Read();
    }
}
```

Lizzie is highly influenced and inspired from Lisp, but without the unintuitive
_"Polish notation"_. In such a way, it arguably is dynamic Lisp for the CLR. Its
dynamic nature allows you to execute snippets of Lizzie code, inline in your C#
code, by loading your code from files, or by for instance fetching the code from
some database of some sort, or even transmit code over the network to have a
server endpoint (securely) evaluate your code.

You can easily create your own _"keywords"_ in Lizzie, which allows you to create
your own DSL or _"Domain Specific programming Languages"_. Lizzie hence easily
lends itself to richer rule based engines, and similar domain specific problems,
where your code needs to be more dynamic in nature than that which the CLR allows
you to through C#, VB.NET or F#.

## What is a Symbolic Delegate?

A Symbolic Delegate is a CLR delegate that is dynamically looked up during runtime
from a dictionary of delegates with the same signature. This allows you to dynamically
wire together delegates to an _"execution tree"_ during runtime, based upon whatever
delegate happens to be the value for your _"symbol"_. Lizzie is literally a
dictionary of delegates, where the key to lookup your delegates are of type string.
This allows you to easily extend Lizzie by simply creating a new delegate, and
associating it with a _"symbol"_, to such have access to execute CLR methods
from your Lizzie script code.

## Binding Lizzie to your own types

If you want to, you can _"bind"_ your Lizzie code to a CLR type. This
allows you to extend your Lizzie code with your own C# _"keywords"_, to
create your own _"DSL"_. Below is an example.

```csharp
using System;
using lizzie;

class MainClass
{
    // This method will be available as a function in your Lizzie code
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

write(""Hello World!!"")

";

        // Creating a lambda function from our code, and evaluating it
        var function = LambdaCompiler.Compile(new MainClass(), code);
        function();
    }
}
```

## How small is Lizzie?

The [entire reference documentation for Lizze](/docs/introduction.md) is roughly
12 pages if you choose to print it. _This is the entire reference documentation
for the language_. This allows you to learn the entire programming language
literally in 20 minutes. The _"compiler"_ for the language is less than 500 lines
of code, and all _"keywords"_ are less than 1,000 lines of code in total. The project
as a whole has roughly 2,200 lines of code, but 50% of these are comments. When built,
the DLL is roughly 45KB on disc. There are 7 public classes in the project, one
attribute, and one interface. There are less than 30 methods in total, and you don't
have to use more than a handful of these to start adding dynamic scripting abilities
to your CLR code. This arguably makes Lizzie the smallest (useful) programming
language on the planet, if you ignore languages such as _"brainfuck"_, arguably
created more or less as a joke.

The Lizzie tokenizer also contains only 7 different tokens. There are no operators
in the language, no keywords, and only one type of statement. In fact all
_"statements"_ are _"functions"_, that all have the same signature. Compare this
to the 500+ keywords, and 50+ operators of C#, and the 1,000+ pages of reference
documentation for C#, and hopefully you understand the advantage.

* [The entire language and its syntax explained in 12 pages](/docs/introduction.md)

## How fast is Lizzie

When profiling a language such as Lizzie, there are two important things to
measure.

* __Compilation speed__
* __Execution speed__

Compilation is **blistering fast**, something we can illustrate with the
following C# code, where we compile a snippet of Lizzie code 10,000 times.

```csharp
using System;
using System.Diagnostics;
using lizzie;

class MainClass
{
    public static void Main(string[] args)
    {
        // Some inline Lizzie code
        var code = @"
+(5, 2, 50)
-(100, 30, 3)
*(5, 3, 2)
/(100, 4)
%(18, 4)
";

        // Compiling the above code 10,000 times
        Console.WriteLine("Compiling some Lizzie code 10,000 times, please wait ...");
        Stopwatch sw = Stopwatch.StartNew();
        for (var idx = 0; idx < 10000; idx++) {
            var lambda = LambdaCompiler.Compile(code);
        }
        sw.Stop();
        Console.WriteLine($"We compiled the above code 10,000x in {sw.ElapsedMilliseconds} milliseconds");

        // Waiting for user input
        Console.Read();
    }
}
```

On my computer, which is a MacBook Air from 2016, the above code compiles 10,000
times in roughly 2,100 milliseconds. Since Lizzie is a dynamic scripting language,
intended to frequently retrieve snippets of dynamic code, and compile these, before
it executes the result - The compilation speed is hence arguably equally
important as its execution speed. On my computer, compiling Lizzie itself, and
its unit tests **once** requires 4.62 seconds! Compiling the above Lizzie code
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
        Console.WriteLine($"We executed the above code 10,000x in {sw.ElapsedMilliseconds} milliseconds!");

        // Waiting for user input.
        Console.Read();
    }
}
```

The results on my computer says 722 milliseconds.

Lizzie is not as fast as C#, since each function invocation also requires a lookup
into a `Dictionary`. Each function invocation also implies evaluating a
delegate, which has an additional overhead of 20% compared to invoking a virtual
method. So you can't expect a Lizzie lambda object to evaluate nearly as fast as the
equivalent C# method. However, compared to the execution speed of an interpreter
written on the CLR, and/or a _"true compiler"_ written on the CLR, Lizzie will
purely mathematically outperform both of these for all practical concerns,
assuming you have an interest in executing dynamic code. Since most practical
snippets of code does complex tasks, such as accessing the file system, and
reading/writing to databases, fetching data over sockets, etc - The execution
speed overhead of your Lizzie code for most practical concerns will be irrelevant.

**DISCLAIMER** - I would not encourage you to use Lizzie for extremely CPU resource
demanding tasks, such as polygon rendering, algorithmic intensive math
operations, complex parsing, etc. Because after all, it will never execute
as fast as the equivalent C# code, due to its dynamic nature.

## A 5 minutes introductory video to Lizzie

<p align="center">
<a href="https://www.youtube.com/watch?v=hgRaTRJ2nUc">
<img alt="A 5 minutes introduction video to Lizzie" title="A 5 minutes introduction video to Lizzie" src="https://phosphorusfive.files.wordpress.com/2018/08/intro-to-lizzie-video-screenshot.png" />
</a>
</p>

## Reference documentation

* [Reference documentation for Lizze (12 pages, 20 minute read)](/docs/introduction.md)

## Installation

```bash
PM > Install-Package lizzie
```

Or visit the [download page to get its source code](https://github.com/polterguy/lizzie/releases)
