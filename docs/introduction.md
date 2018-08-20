
# Lizzie introduction

Lizzie is a programming language based upon the (good) ideas from Lisp, but
without the _"funny syntax"_. Although this arguably eliminates most of the
peculiarities from Lisp, it still keeps some of these around. The same way that
Lisp is based upon Symbolix Expressions, or S-Expressions, Lizzie is arguably
based upon similar constructs. So hence some things will come very natural for
every developer that has done any development, in languages such as JavaScript
and C#, while some things will feel different in the beginning. However, let's
start with the basics.

## Binding your Lizzie code to your domain types

The first really cool feature of Lizzie is that you can bind it with a CLR class.
Imagine the following.

```csharp
using System;
using lizzie;

class MainClass
{
    [Bind(Name = "foo")]
    object Foo(Binder<MainClass> binder, Arguments arguments)
    {
        Console.WriteLine("foo was here!");
        return null;
    }

    public static void Main(string[] args)
    {
        // Some inline Lizzie code
        var code = "foo()";

        // Compiling the above code, 'binding' to our MainClass instance.
        var lambda = LambdaCompiler.Compile<MainClass>(new MainClass(), code);
        var result = lambda();

        // Waiting for user input.
        Console.Read();
    }
}
```

As you execute the above C# console program, you will see that clearly your Lizzie
code is able to execute your `Foo` C# method, as if it was a Lizzie function. This
is because of that the type of `MainClass` is the type argument to the
`LambdaCompiler.Compile` line. Internally, the Lizzie compiler will create a
delegate for each method that you have marked with the Bind attribute, and make
it available as a function in your Lizzie code.

This allows you to extend Lizzie as you see fit, with your own _"keywords"_
created in C#, to create your own _"Domain Specific Language"_, although while
still keeping the Lizzie syntax.

Another way to accomplish the same as above, is to choose to instead explicitly
add your functions to the binder, as delegates. The above code is logically
identically to the following code.

```csharp
using System;
using lizzie;

class MainClass
{
    public static void Main(string[] args)
    {
        // Some inline Lizzie code
        var code = "foo()";

        // Creating a lambda function from our code.
        var function = Compiler.Compile<MainClass>(new Tokenizer(new LizzieTokenizer()), code);

        // Creating an instance of our class, which we can bind to our code.
        var context = new MainClass();

        // Creating a binder, and adding some keywords to it.
        var binder = new Binder<MainClass>();
        binder["foo"] = new Function<MainClass>((ctx, binder2, arguments) => {
            Console.WriteLine("foo was here!");
            return null;
        });

        // Evaluates our Lizzie code making sure we bind it to our instance.
        function(context, binder);

        // Waiting for user input.
        Console.Read();
    }
}
```

Although to gain access to the binder, you must currently do a slightly more
manual job, and can't use the `LambdaBuilder` convenience methods at the moment.
But from a functional point of view, the above two examples are identical,
except that in the first example the this reference is implicitly passed into
your method, since this is a member method of the `MainClass` - While in the
second example the reference is passed explicitly as the `ctx` argument.
The signature of the functions are still the same, and can be found below.

```csharp
delegate object Function<TContext>(TContext ctx, Binder<TContext> binder, Arguments arguments);
```

TContext in the second example above is your `MainClass`.

Every Lizzie function has the exact signature. This is what makes it possible for
us to handle delegates symbolically, since we know that every method/function/delegate
will have the same signature, we can treat them as interchangeable objects. This
creates many advantages, and some disadvantages. The disadvantage is that you
loose type safety while passing arguments around, since the `Arguments` class
is simply a wrapper around `List<object>`. This means you are responsible yourself
for making sure you don't try to perform an illegal cast or conversion inside of
your code. However, internally in Lizzie type safety doesn't matter, since it
will just as happily accept anything, and convert back and forth from anything
to anything, as long as there exists a legal conversion somehow. Although in your
C# code, you must be slightly more careful. You can use the `Get<T>(int index)`
method though on your `Arguments` instance to perform automatic conversion, if
you know what types your method is expecting.

**Notice! Lizzie does not include type safety**, but after a while, you will
realize that _is the whole point_, and its _main advantage_ in fact!If Lizzie
had type safety, it wouldn't have any practical usecases in fact, since the
whole idea is to create an extremely loosely coupling, allowing you to create
configurations and rule based engines, which can be dynamically stored any place,
and chained together to perform more complex rules.



