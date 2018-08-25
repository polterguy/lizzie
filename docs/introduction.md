
# Lizzie reference guide

Lizzie is a programming language based upon the _"good"_ ideas from Lisp, but
without the _"funny syntax"_. Although this arguably eliminates most of the
peculiarities from Lisp, it still keeps some of these around. The same way that
Lisp is based upon Symbolic Expressions, or S-Expressions, Lizzie is based upon
a similar construct which we refer to as _"Symbolic Delegates"_ in this documentation.
This arguably makes the ideas of Lisp, dynamically available to developers on
the CLR stack, without forcing an entirely new way of thinking down their throats.
However, let's start with the basics.

## Binding your Lizzie code to your domain types

The first really cool feature of Lizzie is that you can bind Lizzie code to a
CLR class. Imagine the following.

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
        var code = @"

// This invokes the above 'Foo' method.
foo()";

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
_"Symbolic Delegate"_ for each method that you have marked with the `Bind` attribute,
and make that method available as a _"function"_ to your Lizzie code.

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
        // Some inline Lizzie code.
        var code = "foo()";

        // Creating a lambda function from our code.
        var function = Compiler.Compile<MainClass>(new Tokenizer(new LizzieTokenizer()), code);

        // Creating an instance of our class, which we can bind to our code.
        var context = new MainClass();

        // Creating a binder, and adding the 'foo' function to it.
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

The last example above requires a slightly more manual job, but from a functional
point of view, the above two examples are identical, except that in the first
example the `this` reference is implicitly passed into your function, since this
is a member instance method of the `MainClass` - While in the second example,
the reference to your context is passed explicitly as the `ctx` argument.
The signature of the functions are still the same, and can be found below.

```csharp
delegate object Function<TContext>(TContext ctx, Binder<TContext> binder, Arguments arguments);
```

`TContext` is our `MainClass` above.

Every Lizzie function has the exact same signature. This is what makes it possible for
us to handle delegates _"symbolically"_. Since we know that every method/function/delegate
will have the same signature, we can treat them as interchangeable function objects. This
creates many advantages, and some disadvantages. The _"disadvantage"_ is that you
loose type safety while passing arguments around, since the `Arguments` class
is simply a wrapper around `List<object>`.

**Notice** - Lizzie is not type safe, but after a while, you will
realize that _is the whole point_, and its _main advantage_ in fact. If Lizzie
had type safety, it wouldn't have much practical use in fact, since the
whole idea is to create an extremely loosely coupling, allowing you to create
configurations and rule based engines, which can be dynamically stored any place,
and chained together to allow for complex rule based engines, through a
dynamically compiled script language. This also implies that the same piece of
Lizzie code, might in theory perform two distinct different tasks, depending
upon which class you are binding it towards. So you can completely change what
your code does, by simply choosing to bind it to something else, which of
course is extremely powerful once you realize its advantages. This trait also
makes Lizzie very easy to learn. In fact, the entire reference documentation
for the language, which is this page, is not more than roughly 10 pages if
you choose to print it out. These 10 pages are _everything_ you need to learn
in order to master Lizzie.

## Pre-defined Lizzie functions

Lizzie contains many pre-defined functions for different use cases, which you
can choose to use. In fact, if you want to have complete control over what
_"keywords"_ your Lizzie code has access to, you can control this, through a
slightly more manual process, resembling the example below.

```csharp
using System;
using lizzie;

class MainClass
{
    public static void Main(string[] args)
    {
        // Some inline Lizzie code
        var code = "+(10, 57)";

        // Creating a tokenizer and compiling our Lizzie code
        var tokenizer = new Tokenizer(new LizzieTokenizer());
        var function = Compiler.Compile<MainClass>(tokenizer, code);

        /*
         * Creating a context object, a binder, and adding up only one
         * single function to it
         */
        var ctx = new MainClass();
        var binder = new Binder<MainClass>();
        binder["+"] = Functions<MainClass>.Add;

        // Evaluating our Lizzie function
        var result = function(ctx, binder);
        Console.WriteLine($"Result: '{result}'");

        // Waiting for user input
        Console.Read();
    }
}
```

In our above example, we have created a `Binder` with only one single function
available for Lizzie, which of course is our `+` function. Anything you try
to do besides invoking `+` will throw an exception, because it doesn't contain
any other functions besides `+`. This gives you complete control over
what a piece of Lizzie code is legally allowed to do, and allows you to for
instance evaluate _"insecure"_ code in a highly restricted context, which does
not have access to negatively modify the state of your server/client in any
ways. The `Functions` class contains several pre-defined functions you might
want to use, ranging from math functions, to declaring variables, changing
values of variables, creating functions in your Lizzie code, etc, etc, etc. Due
to the way these functions are loaded into the Lizzie binder, you can also choose
to translate the entire language's syntax to for instance Japaneese or Greek if
you wish. Simply change the _"+"_ to _"foo"_, and there's no `+` function,
but rather a `foo` function, that does what `+` previously did.

In such a way, Lizzie is arguably a programming language chemically cleansed
for _"keywords"_, besides the ones you explicitly choose to load into its
binder. However, when you use the `LambdaCompiler` to compile your code, all
default _"functions"_ or _"keywords"_ are automatically added for you. Further
down on this page you can find the complete list of pre-defined functions, and
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

        // Creating a lambda function from our code
        var function = LambdaCompiler.Compile<MainClass>(new MainClass(), code);

        // Evaluates our Lizzie code making sure we bind it to our instance
        var result = function();

        // Waiting for user input
        Console.Read();
    }
}
```

In the above Lizzie code we create a _"variable"_ named `foo`, and set its initial
value to _"57"_, before we write out its content to the console by invoking our
WriteLine method, which is bound to our Lizzie code, using the `[Bind]` attribute.
The `var` function must be given at least on _"variable name"_, in addition to
optionally an initial value for that variable. The value can be anything ranging
from a function, to a string, or a number of some sort - Or the return value from
a bound C# method, allowing you to create complex objects and handle these within
your Lizzie code.

**FYI** - In the next examples, we will only include the Lizzie code - So keep the above
C# console program around, and simply replace its Lizzie code, to try out the
following examples.

#### What's with the funny '@' symbol?

**Warning** - Advanced concept coming up. If you don't understand the contents
of this sub-title, simply skip it, and come back to it later, at which point
it will make more sense to you. Understanding this part is not required to
understand the rest of this documentation.

Lizzie is based upon the ideas of Lisp. In Lisp, and hence in Lizzie, everything
is evaluated. In fact, even constants you include in your code, are wrapped inside
of functions, which we refer to as _"Symbolic Delegates"_, and when these constants
are de-referenced we simply evaluate the function wrapping our constants.

This creates a problem, which is that if we want to refer to a symbol by name,
instead of evaluating it, we need an additional layer of indirection.
Hence, when you refer to the actual symbol, instead of its value, we prefix
the symbol with an `@`. If you remove the '@' above, your code will throw an
exception, because it will try to evaluate the symbol `foo`, which at that point
is not declared, and your code will throw an exception.

If you know Lisp from before, realize that the `@` character in Lizzie equals
the `'` character in Lisp, or the `(quote foo)` in Lisp. Internally it simply
returns the string _"foo"_ instead of trying to evaluate _"foo"_ as a function
to retrieve its value. This is a necessary level of indirection since there are
no _"operators"_ or _"keywords"_ in Lizzie, and everything is a _"Symbolic Delegate"_.

This might seem a little bit weird in the beginning, but also have a lot of advantages,
such as the ability to declare an entire function invocation, which might be an
entire code tree for that matter, and pass that invocation into another function,
without actually evaluating it. Below is an example of this.

```javascript
var(@foo, function({
  write("foo is invoked ...")
  bar()
}, @bar))

/*
 * Notice, this function is passed into our function without
 * being evaluated!
 */
foo(@write("This will be evaluated last ..."))
```

If you evaluate the Lizzie code above, you might be surprised to see that the
`@write(...)` invocation that we pass into our `foo` function is in fact not evaluated
before we pass it into our `foo` function. This allows you to decorate a function
invocation, and _"delay"_ its evaluation, to the point in time where you are
sure of that you actually want to evaluate it. Internally in Lizzie, this is
actually done by creating a wrapper function invocation, that decorates our
inner function invocation, and returns that decorated function invocation when
referencing the symbol.

If the above code is all Greek to you, it simply declares a variable
named `foo`, and assigns the _"anonymous function"_ returned from the `function`
invocation to the value of `foo`. Our `foo` function can take one parameter,
and within our `foo` function we can de-reference this argument's value as `bar`.
Since `bar` happens to be a function, that internally invokes `write`, with an
argument that it has already been decorated with - We can evaluate `bar` in our
`foo` function without arguments - Which is what we do in the above code
_after_ we first write _"foo is invoked"_ to the console. For this reasons the
evaluation of our `bar` function actually occurs _after_ we have written
_"foo is invoked"_ to the console, even though we have created our function
invocation as an argument to our `foo` function.

An easy way to visualize this, is by realizing that the statement
`@write(...)` does not invoke `write`, but rather creates an anonymous function,
that once evaluated will invoke `write` with the arguments you have already
declared that you want to use for your `write` invocation. Think of this in
such a way that in Lizzie function invocations are also objects.

If you don't understand what the above code does, relax and keep on reading,
we will dive deep into functions further down in this document.

**FYI** - The `{...}` parts above simply creates a _"lambda object"_, that is
basically a delegate, that internally contains a list of other delegates, that
it sequentially evaluated once the _"lambda"_ is evaluated. `{}` simply creates
an object, that happens to be a function, which you can execute when needed.

### Changing a variable's value

To declare a value, you always use the `var` function. This allocates space for
your variable on the stack, which allows you to reference its value later. If
you for some reasons want to change the variable later, you can use the `set`
keyword. Below is an example.

```javascript
// Declaring 'foo' and giving it an initial value of 57
var(@foo, 57)
write(foo)

// Changing foo's value.
set(@foo, 67)
write(foo)
```

The above code first declares the `foo` variable and assigns its initial value
to 57, for then to change its value to 67. The `set` keyword or function is what
we use to change a variable's value.

#### Functions are 1st class objects in Lizzie

For the record, in Lizzie a function is a _"first class object"_, and can be
assigned to a variable, allowing you to pass it around to other parts of your
code, as you see fit. There are logically no differences between the number
_"57"_, the string _"hello world"_ or a function in Lizzie. In fact, internally
they are also treated exactly the same way. This is what allows us to handle
all Lizzie code the same way, since literally everything becomes a delegate
internally in Lizzie. Even constant values, such as the above 57 value, is in
fact wrapped inside a delegate, that returns that constant value back to the
caller.

### Functions

So far we have used functions a little bit, but let's dive deeper into the syntax
of how to declare one. First of all, the following code will only create a function,
and actually not make it available for us in any ways.

```javascript
function({
  write("This function can never be invoked!")
})
```

The above function can never be invoked, simply because we do not have
a reference to it, once we have passed beyond the line of code that creates it.
So we must assign it to a symbol, or pass it into another function somehow,
to be able to actually use it. Below is a slightly more useful example.

```javascript
/*
 * Declaring a variable named 'foo' and assigning a function to its value.
 */
var(@foo, 
  function({
    write("This function can be invoked!")
  })
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
    write("Hello ")
    write(name)
    write("you are ")
    write(age)
    write("years old ...")
  },

  // These are arguments our function can handle.
  @name,
  @age)
)

// Invoking our function.
foo("Thomas", 44)
```

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

### So what is a Symbolic Delegate anyway?

A _"Symbolic Delegate"_ is exactly what it sounds like. It's a delegate, associated
with a _"symbol"_. The symbol is basically just a string, which serves as a key
into a dictionary, where the values are delegates. Below is how these are more
or less implemented in Lizzie.

```csharp
// The delegate type.
delegate object Function<TContext>(TContext ctx, Binder<TContext> binder, Arguments arguments);

// Dictionary containing our symbolix delegates.
Dictionary<string, Function<TContext>>
```

This allows us to lookup functions from a dictionary using the symbol as a key.
Since a dictionary lookup is an O(1) operation, this creates little overhead
for us compared to native CLR code, while also allowing us to dynamically
parse Lizzie's syntax, to dynamically build and modify our delegate dictionary.
And since every _"function"_ has the exact same signature, we can treat all
functions exactly the same way.

This allows us to create a programming language (Lizzie), that is Turing complete,
without neither any compilation nor any interpretation being necessary to
_"execute"_ our end result. Which results in a **blistering fast** process for
dynamically parsing Lizzie code, and creating a _"lambda object"_ out of it
during runtime. Compiling C# code often requires seconds, in addition to often
also _"reloading"_ the process such that it can execute our CLR code. Interpreting
script code is often a process too expensive to be able to adequately implement
in a managed language such as C#. However, parsing a bunch of _"Symbolic Delegates"_,
and create a lambda object out of it, is not only blistering fast, but the end
result is also rarely significantly slower to executing compiled C# code for all
practical concerns. Even though there is one additional layer of indirection to
lookup the delegates from your dictionary, this rarely have any practical concerns
for most apps, unless you require insane amounts of speed, at which point you
probably wouldn't choose a managed environment in the first place anyway.

### Branching

To branch in Lizzie you can use the `if` function. Below is an example.

```javascript
var(@foo, "Value of foo")
if(foo,{
  write("Foo has a value")
})
```

Since the `foo` variable has a value, the lambda which is the second argument
to our `if` invocation will be evaluated. If you remove the above initial value
to `foo` it won't evaluate the parts in between `{` and `}` above. If you supply
an additional lambda as the third argument, this will become the `if` statement's
associated `else` lambda, that is evaluated if the condition of your `if` returns
a null value of some sort.

```javascript
var(@foo)
if(foo,{
  write("Foo has a value")
},{
  write("Foo is not defined")
})
```

#### The definition of truth in Lizzie

Lizzie does not have any explicit _"true"_ or _"false"_ boolean types or values.
The definition of something that is _"true"_ in Lizzie, is anything returning
something that is not null. So basically, every object that is not null, has an
implicit conversion to _"true"_ in Lizzie. Let's illustrate with an example.

```javascript
// Creating a function that returns 57
var(@foo, function({
  57
}))

// Evaluating the above function, and checking if it returned anything.
if(foo(),{
  write("Foo returned something")
})
```

If you remove the `57` parts in the above code, the `if` will evaluate to false.
This is called _"implicit conversion to boolean"_, and everything in Lizzie,
including the boolean value of _"false"_, will in fact evaluate to true.
The only thing that evaluates to _"false"_ is a _"null"_ reference.

#### Wait, where's the return keyword?

Well, it doesn't exist! This is because inside of a lambda, whatever is
evaluated last, before the lambda returns, will be implicitly returned as the
_"value"_ of the lambda. Let's illustrate this with an example.

```javascript
/*
 * Creating a function named 'foo', that takes one argument.
 */
var(@foo, function({

  /*
   * Checking value of input argument, and returning 57 if it has
   * a value, otherwise we return 67.
   */
  if(input, {
    57
  }, {
    67
  })

}, @input))

/*
 * Evaluating the above function twice, with and without an argument,
 * and writing out what it returns on the console.
 */
var(@tmp1, foo("some value"))
write(+("Foo returned ", tmp1))

// Notice! No value passed in to foo here ...
var(@tmp2, foo())
write(+("Foo returned ", tmp2))
```

In our first function invocation above, `input` has a value, hence it will
evaluate the line `57`, which of course simply _"returns"_ the constant value
of 57 to caller. In the second invocation, `input` does **not** have a value,
and hence the else parts of our `if` invocation will be evaluated, which returns
67. Hence, by intelligently structuring your code, there is no need for an
explicit `return` keyword in Lizzie.

**Notice** - If you for some reasons wants to explicitly use null for some reasons,
you can do so with the null constant. Below is an example.

```javascript
var(@foo)
if(eq(null, foo), {
  write("Yup,it's null!")
})
```

#### Testing for equality

Sometimes you need to check if a variable has a specific value, and not only
if it is defined. For those cases there's the `eq` function.

```javascript
// Creating a function.
var(@foo, function({

  /*
   * Checking value of input argument, and returning 57 if it has
   * a value, otherwise we return 67.
   */
  if(eq(input, "Thomas"), {
    "Welcome home boss!!"
  }, {
    "Welcome stranger"
  })

}, @input))

// Evaluating the above function.
var(@tmp1, foo("Thomas"))
write(+("Foo returned ", tmp1))
```

The above code of course writes _"Welcome home boss"_ on the console. If you
change the value you invoke `foo` with above to e.g. _"John Doe"_, it will
write _"Welcome stranger"_ instead. If you wish to _"negate"_ the check,
implying _"not equals"_, you can simply wrap your `eq` invocation inside of
a `not` function invocation, which will negate the value of `eq`, or any other
values for that matter. Below is an example, that logically is the same as our
previous example, but where the return value of our `eq` is negated using a `not`
invocation.

```javascript
// Creating a function.
var(@foo, function({

  /*
   * Checking value of input argument, and returning 57 if it has
   * a value, otherwise we return 67.
   */
  if(not(eq(input, "Thomas")), {
    "Welcome stranger"
  }, {
    "Welcome home boss!!"
  })

}, @input))

// Evaluating the above function.
var(@tmp1, foo("Thomas"))
write(+("Foo returned ", tmp1))
```

In addition to `eq` and `not` you also have the following comparison functions.

* `mt` implying _"more than"_
* `lt` implying _"less than"_
* `mte` implying _"more than or equal to"_
* `lte` implying _"less than or equal to"_

The above 4 functions can only be used for types that have overloaded the
equivalent operators for these types of comparisons.

#### OR and AND

Since Lizzie doesn't have operators, neither the OR nor the AND keywords exists
in Lizzie. However, you can accomplish the same result by using the `any` and the
`all` functions. The `any` is the equivalent of OR in a traditional programming
language, while `all` is the equivalent of AND. The `any` function will return
true if any of its arguments evaluates to true, while `all` will only return
true if all of its arguments yields true. This allows you to combine `any` and
`all` to accomplish the same as OR and AND would do for you normally. Consider
the following.

```javascript
var(@foo1)
var(@foo2)

// Remove the 57 value to have the if below yield false.
var(@foo3, 57)

/*
 * Yields true since foo3 contains a non-null value.
 */
if(any(foo1, foo1, foo3), {
  write("Any yields true")
})
```

If you exchange the above `any` with an `all`, it will not yield true, since not
all of the arguments it needs to test will evaluate to true.

#### Boolean shortcutting your code

Since everything in Lizzie is evaluated, this creates a dilemma for us, where
the previously mentioned `@` becomes crucially important due to something that
is called _"boolean conditional shortcut"_, which implies that both the OR and
the AND operator does not need to check its arguments, if the first argument
returns true for `any`, or the first argument returns null for `all`. This is
because when we test for `any`, and the first argument yields true, we don't need
to check anymore arguments to `any` to know that our `any` function will evaluate to true.
While for `all`, if the first argument yields null, we know that `all` as a
whole will not yield true. This implies that for expensive functions, that have
a significant cost to evaluate, we can use the `@` symbol to avoid evaluating
the condition, before we know for a fact that we need to. And since the value
of the n-1 argument always decides if we need to evaluate the n argument, we
can significantly conserve resources by postponing the evaluation of the
condition in both our `any` functions and our `all` functions.

This is possible because internally inside of `any` and `all`, Lizzie will check
to see if the argument it should check is a function, and if it is a function,
it will evaluate that function to see if it yields something, and only if that
function yields something, it will return null or _"something"_ to caller - Or
continue checking the rest of its arguments.

Consider the following entire console program.

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
if(all(expensive(), expensive(), expensive(), expensive(), expensive(5)), {
  write(""And we're done with TRUE!"")
}, {
  write(""And we're done with FALSE!"")
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
```

In the above program it will take 5 seconds before our `all` invocation is
finished evaluating, because each of our `expensive` functions will take 1
second to evaluate, and all of our invocations are evaluated before we invoke
our `all` function. If we change its Lizzie code to the following, it will
only require 1 second, because only the first argument needs to be evaluated,
before we know that `all` will for a fact evaluate to null.

```javascript
if(all(@expensive(), @expensive(), @expensive(), @expensive(), @expensive(5)), {
  write("And we're done with TRUE!")
}, {
  write("And we're done with FALSE!")
})
```

This is because in the first example all of our invocations to `expensive` are
evaluated before we even invoke our `any` function. While in our second example,
we delay evaluation by passing in our function invocations by reference to our
`any` function, which will inspect the values of its arguments, and if it finds
that these are function invocations, evaluate these functions before it determines
whether or not it's a null value or a true value. You can (and _should_) also apply
the same trick for `any` invocations if you know they will be expensive to evaluate.

### Lists

Lizzie has good support for handling lists of objects. To create a list you can
use the `list` function. To add to a list you can use `add`. To get an item you
can use `get`. To count items in a list you can use `count`, and to slice a list
you can use `slice`, which will return a sub list of your original list.

```javascript
// Declare a list.
var(@foo, list(57, 67, 77))
write(+("list count ", count(foo)))

// Returns the 3rd item.
write(+("list 3rd item ", get(foo, 2)))

// Adds two new items to the list.
add(foo, 88, 99)
write(+("list count ", count(foo)))

// Slice the list, and puts the new list into 'bar'.
var(@bar, slice(foo, 1, 3))
write(+("bar list count ", count(bar)))
```

#### Iterating lists

The `each` function allows you to evaluate a lambda once for each value in a list.
The first argument is expected to be a symbol prefixed with an `@` character,
which will be used to de-reference the currently iterated value inside of your
lambda. The second argument is expected to be the list to iterate. The third argument
must be a lambda block, which will be evaluated once for each item in your list.

```javascript
var(@foo, list(57, 67, 77, 88.88, 97))
each(@ix, foo, {
  write(ix)
})
```

### Conversion

Sometimes you need to convert an object from its string representatio to a number,
or vice versa. For such cases you have the `number` and `string` functions. Below
is an example.

```javascript
write(+(number("55"), 2))
write(+(string(55), 5))
```

### Math

Lizzie contains all the basic math functions, these are as follows.

* `+` adds two or more _"things"_ together.
* `-` subtracts one or more _"things"_ from its first argument.
* `/` divides one or more _"things"_ from its first argument.
* `*` multiplies one or more _"things"_ to each other.
* `%` calculate the modulo (remainder) after division.

Notice, we say _"things"_ above, because these functions works with all types
that have somehow overloaded the equivalent operators. This allows you to use
the `+` function to concatenate strings for instance, in addition to that you
can use the other operators for all types that have an operator overload for
that particular operator. All of the above functions can handle multiple
parameters, and will act accordingly.

```javascript
write(+(5, 2, 50))
write(-(100, 30, 3))
write(*(5, 3, 2))
write(/(100, 4))
write(%(18, 4))
```
