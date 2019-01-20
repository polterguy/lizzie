
# Lizzie reference guide

Lizzie is a programming language based upon the (good) ideas from Lisp, but
without the _"funny syntax"_. Although this eliminates most of the
peculiarities from Lisp, some _"weird"_ constructs are still necessary to create
a powerful language such as Lisp. The same way that Lisp is based upon Symbolic
Expressions, or S-Expressions, Lizzie is based upon a similar construct which
we refer to as _"Symbolic Delegates"_. This makes the ideas of Lisp,
dynamically available to developers on the CLR stack, without forcing an
entirely new way of thinking down your throat. However, let's start with the basics.

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
        // This invokes the above 'Foo' method from Lizzie
        var code = "foo()";

        // Compiling the above code, 'binding' to a MainClass instance.
        var lambda = LambdaCompiler.Compile(new MainClass(), code);
        var result = lambda();

        // Waiting for user input.
        Console.Read();
    }
}
```

As you execute the above C# console program, you will realize that your Lizzie
code is able to execute your `Foo` C# method, as if it was a Lizzie function. This
is because of that the type of `MainClass` is the type argument to the
`LambdaCompiler.Compile` method. Internally, the Lizzie compiler will create a
_"Symbolic Delegate"_ for each method that you have marked with the `Bind` attribute
on your `MainClass`, and make this method available as a _"function"_ to your
Lizzie code. This allows you to extend Lizzie as you see fit, with your own _"keywords"_
created in C#, to create your own _"Domain Specific Language"_ - While
still keeping the Lizzie syntax and its dynamic model. Another way to
accomplish the same as above, is to choose to instead explicitly add your
functions to the binder, as delegates. This is particularly useful if you can't
change the type you are binding to. The above code is logically identically
to the following code.

```csharp
using System;
using lizzie;

class MainClass
{
    public static void Main(string[] args)
    {
        // Some inline Lizzie code
        var code = "foo()";

        // Creating a lambda function from our code
        var function = Compiler.Compile<MainClass>(new Tokenizer(new LizzieTokenizer()), code);

        // Creating a binder, and adding the 'foo' function to it
        var binder = new Binder<MainClass>();
        binder["foo"] = new Function<MainClass>((ctx, binder2, arguments) => {
            Console.WriteLine("Hello World");
            return null;
        });

        // Evaluates our Lizzie code making sure we bind it to our instance
        function(new MainClass(), binder);

        // Waiting for user input.
        Console.Read();
    }
}
```

The last example above requires a slightly more manual job, but from a functional
point of view, the above two examples are identical, except that in the first
example the `this` reference is implicitly passed into your function, since this
is an instance member method of our `MainClass` - While in the second example,
the reference to your context is passed explicitly in as the `ctx` argument.
The signature of the functions are still the same, and can be found below.

```csharp
delegate object Function<TContext>(TContext ctx, Binder<TContext> binder, Arguments arguments);
```

Every Lizzie function has the exact same signature. This is what makes it possible for
us to handle delegates _"symbolically"_. Since we know that every method/function/delegate
will have the same signature, we can treat them as interchangeable function objects. This
creates many advantages, and some disadvantages. The _"disadvantage"_ is that you
loose type safety while passing arguments around, since the `Arguments` class
is simply a wrapper around `List<object>`. The advantage is that you have 
_"implicit polymorphism"_ on all functions in Lizzie, and any function can be changed
with any other function.

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
for the language, which is this page, is not more than roughly 11 pages if
you choose to print it. These 11 pages is _everything_ you need to learn
in order to master Lizzie.

**Notice** - Instances of the `Binder` class are _not_ thread safe. Creating an
instance of the Binder class and binding it to your own context type also implies
some runtime overhead, since it includes reflection. However, you can still cache
a single binder, and then use its `Clone` method for each thread that needs to
bind towards the same type, to significantly reduce resource usage during
compilation of your Lizzie code.

The `Binder` also functions as a stack. At the global level, everything you
declare becomes available for every function, and all of your Lizzie code.
However, everything you declare as symbols/variables inside a function, will only
exist for that function. This is similar to how JavaScript works. If you declare
a symbol/variable inside a stack, that already exists at the global level, this
variable will locally override the variable for the duration of your function.

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
         * Creating a binder, and adding up only one
         * single function to it
         */
        var binder = new Binder<MainClass>();
        binder["+"] = Functions<MainClass>.Add;

        // Evaluating our Lizzie function
        var result = function(new MainClass(), binder);
        Console.WriteLine($"Result: '{result}'");

        // Waiting for user input
        Console.Read();
    }
}
```

In our above example, we have created a `Binder` with only one single function
available for Lizzie, which is our `+` function. Anything you try
to do besides invoking `+` will throw an exception, because it doesn't contain
any other functions. This gives you complete control over
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
        var function = LambdaCompiler.Compile(new MainClass(), code);

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
The `var` function must be given at least a _"variable name"_, in addition to
optionally an initial value for that variable. The value can be anything ranging
from a function, to a string, or a number of some sort - Or the return value from
a bound C# method, allowing you to create complex objects and handle these within
your Lizzie code.

### What's with the funny '@' symbol?

Lizzie is based upon the ideas of Lisp. In Lisp, and hence in Lizzie, everything
is evaluated. In fact, even constants you include in your code, are wrapped inside
of functions, which we refer to as _"Symbolic Delegates"_, and when these constants
are de-referenced we simply evaluate the function wrapping our constants.

This creates a problem, which is that if we want to refer to a symbol by name,
instead of evaluating it, we need an additional layer of indirection.
Hence, when you refer to the actual symbol, instead of its value, we prefix
the symbol with an `@`. If you remove the '@' above, your code will throw an
exception, because it will try to evaluate the symbol `foo`, which at that point
is not declared, and your code will throw an exception. The `@` symbol hence
logically implies _"don't evaluate what follows"_.

If you know Lisp from before, realize that the `@` character in Lizzie equals
the `'` character in Lisp, or the `(quote foo)`. Internally it simply
returns the string _"foo"_ instead of trying to evaluate _"foo"_ as a function
to retrieve its value. This is a necessary level of indirection since there are
no _"operators"_ or _"keywords"_ in Lizzie, and everything is a _"Symbolic Delegate"_.

This might seem a little bit weird in the beginning, but also have a lot of advantages,
such as the ability to declare an entire function invocation, which might be an
entire code tree for that matter, and pass that invocation into another function,
without actually evaluating it. Below is an example of this. Don't worry if you
don't understand all of the following code, we will go through its elements
further down.

```javascript
var(@foo, function({
  write('foo is invoked ...')
  bar()
}, @bar))

/*
 * Notice, this function is passed into our function without
 * being evaluated
 */
foo(@write('This will be evaluated last ...'))
```

If you evaluate the Lizzie code above, you might be surprised to see that the
`@write(...)` invocation that we pass into our `foo` function is in fact not evaluated
before we pass it into our `foo` function. This allows you to decorate a function
invocation, and _"delay"_ its evaluation, to the point in time where you are
sure of that you actually want to evaluate it. Internally in Lizzie, this is
actually done by creating a wrapper function invocation, that decorates our
inner function invocation, and returns that decorated function invocation when
referencing the symbol.

Think of this in such a way that in Lizzie function invocations are also objects.
The above `@write(...)` syntax is logically similar to the following JavaScript.

```javascript
foo(function() { write("This will be evaluated last ...") });
```

### Changing a variable's value

To declare a value, you always use the `var` function. This allocates space for
your variable on the stack, which allows you to reference its value later. If
you for some reasons want to change the variable later, you can use the `set`
keyword. Below is an example.

```javascript
// Declaring 'foo' and giving it an initial value of 57
var(@foo, 57)
write(foo)

// Changing foo's value
set(@foo, 67)
write(foo)
```

The above code first declares the `foo` variable and assigns its initial value
to 57, for then to change its value to 67. The `set` keyword or function is what
we use to change a variable's value.

### Functions

So far we have used functions a little bit, but let's dive deeper into the syntax
of how to declare one. First of all, the following code will only create a function,
and actually not make it available for us in any ways.

```javascript
function({
  write('This function can never be invoked!')
})
```

The above function can never be invoked, simply because we do not have
a reference to it, once we have passed beyond the line that creates it.
So we must assign our function to a symbol, or pass it into another function
somehow, to be able to actually use it. Below is a slightly more useful example.

```javascript
/*
 * Declaring a symbol named 'foo' and assigning a function to its value
 */
var(@foo, 
  function({
    write('This function can be invoked!')
  })
)

// Invoking our function.
foo()
```

To pass arguments into your functions, simply declare the symbols you wish to
use for your arguments internally within your function, as additional arguments
to the `function` function. Below is an example.

```javascript
// Declaring 'foo' to be a function
var(@foo, 
  function({
    write('Hello')
    write(name)
    write('you are')
    write(age)
    write('years old ...')
  },

  // These are arguments our function can handle
  @name,
  @age)
)

// Invoking our function with two arguments
foo('Thomas', 44)
```

When you declare a function, you must declare all arguments you want
to handle inside your function with an `@` sign in front of the argument's
name. Otherwise you're not actually declaring the argument, but rather evaluating
the symbol with the name of the argument you are trying to declare.

The rule of thumb is as follows.

* If you refer to the variable, use an `@`
* If you refer to the variable's value, __do not__ use an `@`

To understand the difference, you might want to run the following program.

```javascript
var(@howdy, 'John Doe')
write(@howdy)
write(howdy)
```

The above program of course produces the following result.

```bash
howdy
John Doe
```

The `{ ... code ...}` notation is what is necessary to create a _"lambda object"_.
Such lambda objects are used when we need multiple statements that are to be
evaluated sequentially, such as we do when creating a function, or when we
create a loop, or when we create an `if` statement. This is similar to JavaScript
and C#.

### So what is a Symbolic Delegate anyway?

A _"Symbolic Delegate"_ is exactly what it sounds like. It's a delegate, associated
with a _"symbol"_. The symbol is basically just a string, which serves as a key
into a dictionary, where the values are delegates. Below is how these are more
or less implemented in Lizzie.

```csharp
// Pseudo code
Dictionary<string, Function> _stack;
```

This allows us to lookup functions from a dictionary using the symbol as a key.
Since a dictionary lookup is an O(1) operation, this creates little overhead
for us compared to native CLR code, while also allowing us to dynamically
parse Lizzie's syntax, to dynamically build and modify our delegate dictionary.
And since every _"function"_ has the exact same signature, we can treat all
functions interchangeable.

### Branching

To branch in Lizzie you can use the `if` function. Below is an example.

```javascript
var(@foo, 'Value of foo')
if(foo,{
  write('Foo has a value')
})
```

Since the `foo` variable has a value, the lambda which is the second argument
to our `if` invocation will be evaluated. If you remove the above initial value
to `foo` it won't evaluate the parts in between `{` and `}` above. If you supply
an additional lambda as the third argument, this will become the `else` lambda,
that is evaluated if the condition of your `if` returns null.

```javascript
var(@foo)
if(foo,{
  write('Foo has a value')
},{
  write('Foo is null')
})
```

### The definition of truth in Lizzie

Lizzie does not have any explicit _"true"_ or _"false"_ boolean types or values.
The definition of something that is _"true"_ in Lizzie, is anything returning
something that is not null. So basically, every object that is not null, has an
implicit conversion to _"true"_ in Lizzie. Let's illustrate with an example.

```javascript
// Creating a function that returns 57
var(@foo, function({
  57
}))

// Evaluating the above function, and checking if it returned anything
if(foo(),{
  write('Foo returned something')
})
```

If you remove the `57` parts in the above code, the `if` will evaluate to false.
This is called _"implicit conversion to boolean"_, and everything in Lizzie,
including the boolean value of _"false"_, will in fact evaluate to true.
The only thing that evaluates to _"false"_ is null.

### Wait, where's the return keyword?

Lizzie does not have a return keyword. This is because inside of a lambda object,
whatever is evaluated last, before the lambda returns, will be implicitly returned
as the _"value"_ of the lambda. Let's illustrate this with an example.

```javascript
/*
 * Creating a function named 'foo', that takes one argument
 */
var(@foo, function({

  /*
   * Checking value of input argument, and returning 57 if it has
   * a value, otherwise we return 67
   */
  if(input, {
    57
  }, {
    67
  })

}, @input))

/*
 * Evaluating the above function twice, with and without an argument,
 * and writing out what it returns on the console
 */
var(@tmp1, foo('some value'))
write(+('Foo returned ', tmp1))

// Notice! No value passed in to foo here ...
var(@tmp2, foo())
write(+('Foo returned ', tmp2))
```

In our first function invocation above, `input` has a value, hence it will
evaluate the line `57`, which of course simply _"returns"_ the constant numeric
value of 57 to caller. In the second invocation, `input` does **not** have a value,
and hence the else parts of our `if` invocation will be evaluated, which _"returns"_
67. Hence, by intelligently structuring your code, there is no need for an
explicit `return` keyword in Lizzie. Notice also how the above code illustrates
that all arguments to your functions are optional by default. If you for some
reasons need to explicitly return null, you can use the `null` constant.

### Testing for equality

Sometimes you need to check if a variable has a specific value, and not only
if it is defined. For those cases there's the `eq` function.

```javascript
// Creating a function.
var(@foo, function({

  // Checking if 'input' contains 'Thomas'
  if(eq(input, 'Thomas'), {
    'Welcome home boss!!'
  }, {
    'Welcome stranger'
  })

}, @input))

// Evaluating the above function
write(foo('Thomas'))
write(foo('John Doe'))
```

If you wish to _"negate"_ the check, implying _"not equals"_, you can simply
wrap your `eq` invocation inside of a `not` function invocation, which will
negate the value of `eq`, or any other values for that matter. Below is an
example, that logically is the same as our previous example, but where the
return value of our `eq` is negated using a `not` invocation.

```javascript
// Creating a function.
var(@foo, function({

  // Checking if 'input' contains 'Thomas'
  if(not(eq(input, 'Thomas')), {
    'Welcome stranger'
  }, {
    'Welcome home boss!!'
  })

}, @input))

// Evaluating the above function
write(foo('Thomas'))
write(foo('John Doe'))
```

In addition to `eq` and `not` you also have the following comparison functions.

* `mt` implying _"more than"_
* `lt` implying _"less than"_
* `mte` implying _"more than or equal to"_
* `lte` implying _"less than or equal to"_

The above 4 functions can only be used for types that have overloaded the
equivalent operators for these types of comparisons.

### OR and AND

Lizzie doesn't have operators, neither OR nor AND keywords exists in Lizzie.
However, you can accomplish the same result by using the `any` and the
`all` functions. The `any` is the equivalent of OR in a traditional programming
language, while `all` is the equivalent of AND. `any` will return the first
non-null argument that it is given, or null if all arguments are null.
`all` will return the first null argument it is given, otherwise it will
return the value of its last argument. This allows you to combine `any` and `all`
to accomplish the same as OR and AND would do for you normally. Consider the
following.

```javascript
var(@foo1)
var(@foo2)

// Remove the 57 value to have the 'any' below yield false
var(@foo3, 57)

// Yields true since foo3 contains a non-null value
if(any(@foo1, @foo1, @foo3), {
  write('Any yields true')
}, {
  write('Any yields false')
})
```

If you exchange the above `any` with `all`, it will yield null, since some of
its arguments are null.

### Lazy condition evaluation

Since everything in Lizzie is evaluated, this creates a dilemma for us, where
the previously mentioned `@` character becomes important due to something that
is called _"short-circuit evaluation"_, which implies that both the `any` and
the `all` functions do _not need to check more arguments_, if the first argument
returns anything but null for `any`, or the first argument returns null for
`all`. This is because when we test for `any`, and the first argument yields
non-null, we don't need to check anymore arguments to `any` to know that our
`any` function will evaluate to its first argument. While for `all`, if the
first argument yields null, we know that `all` as a whole will always yield null.

To consistently support this in Lizzie, and to avoid sub-optimal code being created,
you _must_ use the `@` symbol to avoid evaluating the condition before Lizzie knows
that it needs to evaluate your argument. This allows for something called _"lazy evaluation"_ of
conditions. And since the value of the n-1 argument always decides if
we need to evaluate the n argument, we can significantly conserve resources by
postponing the evaluation of the condition in both our `any` functions and our
`all` functions by evaluating the conditions _"lazy"_. Hence, both of these
two functions requires you to use _"lazy evaluation"_ of their arguments, by
appending your arguments to them with an `@` character.

If this sounds like Greek to you, simply remember that you must **always** prefix your
arguments to `any` and `all` with an `@` character.

### Lists

Lizzie has good support for handling lists of objects. To create a list you can
use the `list` function. To add to a list you can use `add`. To get an item you
can use `get`. To count items in a list you can use `count`. To slice a list
you can use `slice`, which will return a sub-list of your original list. In
addition you can also `apply` a list of arguments to another function invocation,
such that the content of your list, becomes the arguments to your other function
invocation.

```javascript
// Declare a list
var(@foo, list(57, 67, 77))
write(+('list count ', count(foo)))

// Returns the 3rd item
write(+('list 3rd item ', get(foo, 2)))

// Adds two new items to the list
add(foo, 88, 99)
write(+('list count ', count(foo)))

// Slice the list, and puts the new list into 'bar'
var(@bar, slice(foo, 1, 3))
write(+('bar list count ', count(bar)))

/*
 * Apply arguments from a list
 * This will result in that your + function will be invoked with
 * 3 arguments; 57, 10 and 10, instead of a single argument being a list.
 */
write(+(apply(list(57, 10, 10))))
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

### Maps

A map is a dictionary of string/object values, allowing you to create a more
efficient way of retrieving values than a list, since a map retrieval operation
is an O(1) operation. It shares most of the functions from `list`, such as `add`,
`get`, `count`, and `each`, but instead of using integer values to
refer to items, you use strings to refer to values in your `map`. In
addition when creating a map, or adding items to an existing map, you're expected
to provide a key in addition to a value. Below is an example.

```javascript
var(@my-map, map(
  'foo', 47, // Key 'foo', value 47
  'bar', 10  // Key 'bar', value 10
))
write(get(my-map, 'foo')) // Writes 47
```

### JSON conversion

By combining the `map`, `list` and `string` functions, you can easily create
JSON using Lizzie. Below is an example.

```javascript
write(string(list(
  'foo',
  map(
    'bar1',57,
    'bar2',77,
    'bar3',list(
      1,
      2,
      map(
        'hello','world'))))))
```

The above results in the following JSON.

```javascript
["foo",{"bar1":57,"bar2":77,"bar3":[1,2,{"hello":"world"}]}]
```

You can also reverse the process, and create an object out of JSON, using `json`.

```javascript
/*
 * Notice, make sure you escape the " characters below
 * if you paste this code into a C# string literal
 */
var(@foo, json("{'bar':57,'howdy':[1,2,3]}"))
write(get(foo,'bar')) // Writes 57
write(get(get(foo,'howdy'),2)) // Writes 3
```

`string` will also convert a simple object to its string representation, in
addition to that you can use `number` to convert a string to a numeric value.

```javascript
write(+(number('55'), 2))
write(+(string(55), 5))
```

### Math

Lizzie contains all the basic math functions, these are as follows.

* `+` adds two or more _"things"_ together
* `-` subtracts one or more _"things"_ from its first argument
* `/` divides one or more _"things"_ from its first argument
* `*` multiplies one or more _"things"_ to each other
* `%` calculate the modulo (remainder) after division

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

### String manipulation

Lizzie contains the following functions for manipulating strings.

* __substr__ returns a substring of the specified string, arguments are string, start, and (optional) count
* __length__ returns the length of the string
* __replace__ replaces all occurrencies of the specified 1st arg value with the 2nd arg value

```javascript
var(@foo, 'Hello World')
write(length(foo))
write(replace(foo, 'World', 'Sirius'))
write(substr(foo, 6, 2))
write(substr(foo, 6)) // The count is optional
```

### Eval

No script language is complete without an `eval` function, that allows for
dynamically creating code, that is evaluated dynamically by the code that
creates it. Below you can find an example of Lizzie's `eval` function.

```javascript
write(eval('+(57,10,10)'))
```

This function requires one argument, which must be a valid piece of Lizzie code,
which it compiles, evaluates, for then to return the result of the evaluation
back to caller. It will share the context object, but it will create a new stack,
not having access to the already dynamically declared variables. Notice that `eval`
will load up the default keywords from the `LambdaCompiler` from you.

### Lizzie types

Lizzie is extremely weakly typed, and arguably only contains a handful of types.
All numeric values are internally treated as `long`, unless they contain a decimal,
at which point they're treated as `double`. A string can be created either with a
`"` double quote or a `'` single quote string literal. These are the most important
types that Lizzie supports. However, if you create extension methods or delegates,
you can create more complex types, such as `DateTime` instances, and still to
some extent have Lizzie work with these. This is possible because of that the
math functions will use the `dynamic` type as it is doing its thing. This allows
you to create methods that instantiates stuff such as `BigInteger`, `DateTime`,
or `TimeSpan` instances, and still handle these internally quite well in Lizzie.
The default conversion to string in Lizzie uses `CultureInfo.InvariantCulture`,
allowing you to convert complex objects consistently to their string representations.

* [Download Lizzie here](https://github.com/polterguy/lizzie/releases)
* Or add the NuGet _"lizzie"_ package

### Dependency Injection (IoC)

As of version 0.8, Lizzie has support for _"deep binding"_ on your type, which
will traverse the entire object graph for Lizzie functions. An example of this
can be found below.

```csharp
class BaseClass
{
    [Bind(Name = "foo1")]
    protected object Foo1(Binder<BaseClass> ctx, Arguments arguments)
    {
        return 50;
    }
}

class SuperClass : BaseClass
{
    [Bind(Name = "foo2")]
    object Foo2(Binder<BaseClass> ctx, Arguments arguments)
    {
        return 7;
    }
}

/*
 * Somewhere else in your code ...
 *
 * NOTICE!
 * Type inference here will make sure your Binder uses "BaseClass" as
 * its generic argument, yet still you're able to invoke methods on "SuperClass".
 */
BaseClass simple = new SuperClass();

/*
 * The last "true" argument is important to "bind deeply" to your instance type!
 * Without the "true", it will bind towards the inferred type, which for this example
 * becomes "BaseClass".
 */
var lambda = LambdaCompiler.Compile(simple, "+(foo1(), foo2())", true);
var result = lambda();

/*
 * result is now ==> 57
 */
```

This allows you to among other things use Lizzie in scenarios where you only have
an interface, while still be able to invoke Lizzie functions that are `Bind`ed in
your derived type(s).

**Notice** - Your Lizzie functions must (somehow) be available for instance methods
on your most derived type(s), and the `Binder` will be required to be declared
in your Lizzie methods with its generic type argument being the type inferred
as the Binder is created. See the `Foo2` method above to understand what this
implies, and notice how it's taking a `Binder<BaseClass>` instance, even though
it is declared in the `SuperClass`.

### Optimizing Lizzie

Internally when you create a `Binder`, which is responsible for binding your Lizzie
code to your context instance, Lizzie will use reflection, and also compile lambda
expressions. This process is relatively expensive in terms of CPU usage. However,
if you know that you will bind to the same type frequently, you can internally
cache your binder, for then to `Clone` your _"master"_ binder each time you want to
evaluate a piece of Lizzie code bound towards the same `Binder` type. For a web
application, frequently compiling a new piece of Lizzie snippet, this would probably
increase the performance of the compilation process by several orders of magnitudes.

If you know that you will evaluate the same snippet of Lizzie code multiple
times, you can also cache the lambda returned by the compilation process itself.
Which would further increase your performance. However, Lizzie will never be as
fast as compiled C# code, due to its dynamic nature.

### Donate

If you feel Lizzie has given you value, I would
[appreciate some dollars](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&amp;hosted_button_id=4RL5XLLZYNBF2).
I am working on Lizzie out of my spare time, and your donations, even the smaller
ones, makes me feel that my work is appreciated, and allows me to justify continue
working on it.

* [Donate](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&amp;hosted_button_id=4RL5XLLZYNBF2)
