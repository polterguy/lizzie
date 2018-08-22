
# Lizzie introduction

Lizzie is a programming language based upon the (good) ideas from Lisp, but
without the _"funny syntax"_. Although this arguably eliminates most of the
peculiarities from Lisp, it still keeps some of these around. The same way that
Lisp is based upon Symbolix Expressions, or S-Expressions, Lizzie is arguably
based upon similar constructs. So hence some things will come very natural for
every developer that has done any development, in languages such as JavaScript
and C#, while some things will feel different in the beginning in Lizzie.
However, let's start with the basics.

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
        Console.WriteLine("Hello World");
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
it available as a _"function"_ to your Lizzie code.

This allows you to extend Lizzie as you see fit, with your own _"keywords"_
created in C#, to create your own _"Domain Specific Language"_, while
still keeping the Lizzie syntax and its dynamic model. Another way to
accomplish the same as above, is to choose to instead explicitly add your
functions to the binder, as delegates. This is particularly useful if you can't
for some reasons change the type you are binding to. The above code is logically
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
            Console.WriteLine("Hello World");
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
except that in the first example the `this` reference is implicitly passed into
your method, since this is a member instance method of the `MainClass` - While
in the second example, the reference to your context is passed explicitly as the
`ctx` argument. The signature of the functions are still the same, and can be
found below.

```csharp
delegate object Function<TContext>(TContext ctx, Binder<TContext> binder, Arguments arguments);
```

TContext above in our second example above is our `MainClass`.

Every Lizzie function has the exact same signature. This is what makes it possible for
us to handle delegates symbolically, since we know that every method/function/delegate
will have the same signature, we can treat them as interchangeable objects. This
creates many advantages, and some disadvantages. The disadvantage is that you
loose type safety while passing arguments around, since the `Arguments` class
is simply a wrapper around `List<object>`. This means you are responsible yourself
for making sure you don't try to perform an illegal cast or conversion inside of
your C# code. However, internally in Lizzie type safety doesn't really matter,
since it will just as happily accept anything, and convert back and forth from
almost anything to anything, as long as there exists a legal conversion somehow.
Although in your C# code, you must be slightly more careful. You can use the
`Get<T>(int index)` method though on your `Arguments` instance to perform
automatic conversion, if you know what types your method is expecting.

**Notice! Lizzie does not include type safety**, but after a while, you will
realize that _is the whole point_, and its _main advantage_ in fact! If Lizzie
had type safety, it wouldn't have much practical use in fact, since the
whole idea is to create an extremely loosely coupling, allowing you to create
configurations and rule based engines, which can be dynamically stored any place,
and chained together to allow for complex rule based engines, through a
dynamically compiled script language.

## Pre-defined Lizzie functions

Lizzie contains many pre-defined functions for different use cases, which you
can choose to use, or choose to not use if you want to. In fact, if you want to
have complete controlover what _"keywords"_ your Lizzie code has access to, you
can control this, through a slightly more manual process, resembling the example
below.

```csharp
using System;
using lizzie;

class MainClass
{
    public static void Main(string[] args)
    {
        // Some inline Lizzie code.
        var code = "add(10, 57)";

        // Creating a tokenizer and compiling our Lizzie code.
        var tokenizer = new Tokenizer(new LizzieTokenizer());
        var function = Compiler.Compile<MainClass>(tokenizer, code);

        /*
         * Creating a context object, a binder, and adding up only one
         * single function to it.
         */
        var ctx = new MainClass();
        var binder = new Binder<MainClass>();
        binder["add"] = Functions<MainClass>.Add;

        // Evaluating our Lizzie function.
        var result = function(ctx, binder);
        Console.WriteLine($"Result: '{result}'");

        // Waiting for user input.
        Console.Read();
    }
}
```

In our above example, we have created a Binder with only one single function
available for Lizzie, which of course is our `add` function. Anything you try
to do besides invoking `add` will throw an exception, because it doesn't contain
any other functions but the add function. This gives you complete control over
what a piece of Lizzie code is legally allowed to do, and allows you to for
instance evaluate _"insecure"_ code in a highly restricted context, which does
not have access to negatively modify the state of your server/client in any
ways. The `Functions` class contains several pre-defined functions you might
want to use though, ranging from math functions, to declaring variables, changing
values of variables, creating functions in your Lizzie code, etc, etc, etc.

In such a way, Lizzie is arguably a programming language checmically cleansed
for _"keywords"_, besides the ones you explicitly choose to load into its
binder. However, when you use the `LambdaCompiler` to compile your code, all
default _"functions"_ or _"keywords"_ are automatically loaded for you. Further
down on this page you can find the complete listof pre-defined functions, and
what they do for you.

### Declaring variables

To declare a variable in Lizzie you use the `var` function. This function
requires the name of the variable as its first argument, and an optional initial
value as its second argument. Below is an example.

```csharp
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
var(@foo, 57)
write(foo)
";

        // Creating a lambda function from our code.
        var function = LambdaCompiler.Compile<MainClass>(new MainClass(), code);

        // Evaluates our Lizzie code making sure we bind it to our instance.
        var result = function();

        // Waiting for user input.
        Console.Read();
    }
}
```

In the above Lizzie code we create a _"variable"_ named `foo`, and set its initial
value to _"57"_, before we write out its content to the console by invoking our
WriteLine method, which is bound to our Lizzie code, using the `[Bind]` attribute.

#### What's with the funny '@' symbol?

Lizzie is based upon the ideas of Lisp. In Lisp, and hence in Lizzie, everything
is evaluated. In fact, even constants you include in your code, are wrapped inside
of functions, which we refer to as _"Symbolic Delegates"_, and when these constants
are de-referenced we simply evaluate the function wrapping our constants.

This creates a problem, which is that if we instead of wanting to evaluate a variable,
or more accurately a _"symbol"_, we need to inform Lizzie about that we're not
interested in evaluating it, but rather that we literally mean the symbol's name.
Hence, when you refer to the actual symbol, instead of its value, we prefix
the symbol with an `@`. If you remove the '@' above, your code will throw an
exception, because it will try to evaluate the symbol `foo`, which at that point
is not declared, and hence throw an exception.

If you know Lisp from before, realize that the `@` character in Lizzie equals
the `'` character in Lisp, or the `(quote foo)` in Lisp. Internally, it simply
return the string _"foo"_ instead of trying to evaluate _"foo"_ as a function,
to retrieve its value.

This seems a little bit weird in the beginning, but also have a lot of advantages,
such as the ability to declare an entire function invocation, which might be an
entire code tree for that matter, and pass that into another function, without
evaluating it. Below is an example of this. Notice, from now on, we will only
write out the Lizzie code, to reduce the size of our sample.

```javascript
var(@foo, function({
  write("foo is invoked ...")
  bar()
}, @bar))
foo(@write("This will be evaluated last ,,,"))
```

If you evaluate the Lizzie code above, you might be surprised to see that the
`@write` invocation that we pass into our `foo` function is in fact not evaluated
as we pass it into our function before we explicitly evaluate this function after
we have written our _"foo is invoked"_ line. This allows us to _"delay evaluation"_
of arguments, and function invocations, and literally anything in fact, until
we are really certain about that we actually want to evaluate a function.

For expensive functions, that might perform expensive IO operations for instance,
this little trick can significantly improve performance. For the record, if the
above code is all Greek to you, it simply declares a variable named `foo`, and
assigns the _"anonymous function"_ returned from the `function` invocation to
the value of `foo`. Our function again can take one parameter, and internally
within our function we can de-reference this argument's value as `bar`. Since
`bar` happens to be a function, we can evaluate it, which we do in the `bar()`
line above.

The above syntax might seem a little bit weird, but realize that Lizzie is
entirely built upon _"Symbolic Delegates"_, which are kind of like s-expressions
from Lisp, which implies that everything related to a _"keyword"_ must always
be within the paranthesis of the invocation to that keyword.

We will dive closer into functions later down in this chapter,so just relax if
it doesn't make sense to you yet ...

### Changing a variable's value

To declare a value,you always use the `var` function. This allocates space for
your variable on the stack, which allows you to reference its value later. If
you for some reasons want to change the variable later, you can use the `set`
keyword. Below is an example.

```javascript
// Assigning the value of 57 to our variable.
var(@foo, 57)
write(foo) // Writing the variable out

// Changing our variable's value, and writing out its new value.
set(@foo, 67)
write(foo)
```

The above code first set the `foo` variable to 57, for then to change its value
to 67. The `set` keyword or function is what we use to change a variable's value.

#### Functions are 1st class objects

For the record, in Lizzie, as you might have realized already, a function is a
first class object, and can be assigned to a variable, allowing you to pass it
around to other parts of your code, as you see fit. There are logically no
difference between the number _"57"_, the string _"hello world"_ or a function
in Lizzie. In fact, internally they are also treated exactly the same way.

### Functions

So far we have used functions a little bit, but let's dive deeper into the syntax
of how to declare one. First of all, the following code will only create a function,
and actually not make it available for us in any ways.

```javascript
function({write("This function can never be invoked!")})
```

The above function can actually never be invoked, simply because we do not have
a reference to it, once we have passed beyond the line of code that creates it.
So we must assign it to a variable, or pass it into another function somehow,
to be able to actually use it. Below is a slightly more useful example.

```javascript
var(@foo, 
  function({write("This function can be invoked!")})
)

// Invoking our function.
foo()
```

To pass arguments into your function, simply declare the symbols you wish to
use for your arguments, internally in your function, as additional arguments
to the `function` function. Below is an example.

```javascript
var(@foo, 
  function({
    write(add("Hello ", name, " you are ", age, " years"))
  }, @name, @age)
)

// Invoking our function.
foo("Thomas", 44)
```

**Notice** how we perform string concatenating above, by using the `add` function.
Most functions in Lizzie will just as happily accept any object type as input,
and try its best to intelligently use that input, to perform what it believes
the caller wants to do. Adding two strings together is hence one of those things
you can use the `add` function to, even though it's probably more useful for adding
numbers together.

Yet again, when you declare a function, all arguments you want to handle inside
of your functions, you must always declare with an `@` sign in front of the argument's
name. Otherwise you're not actually declaring the argument, but rather evaluating
the symbol with the name of the argument you are trying to declare.

The rule of thumb is as follows.

* If you refer to the variable, use an `@`
* If you refer to the variable's value, __do not__ use an `@`

To understand the difference, you might want to run the following program.

```javascript
var(@howdy, "John Doe")
write(@howdy)
write(howdy)
```

The above program of course produces the following result.

```bash
howdy
John Doe
```
