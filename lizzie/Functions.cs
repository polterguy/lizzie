/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System.Linq;
using lizzie.types;
using lizzie.exceptions;

namespace lizzie
{
    /// <summary>
    /// Contains all the standard 'keyword' functions in Lizzie.
    /// </summary>
    public static class Functions<TContext>
    {
        /// <summary>
        /// Allows you to change the value of an existing symbol.
        /// 
        /// Expects the first argument to be a symbol, literally de-referenced by an '@'
        /// character, and the optional second argument to be the new value for that symbol.
        /// If no second argument is provided, the new value of the symbol
        /// will be set to 'null'.
        /// When the function is invoked, it will return the new value of your symbol.
        /// </summary>
        /// <value>The function wrapping the 'set keyword'.</value>
        public static Function<TContext> Set => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking invocation.
            if (arguments.Count == 0)
                throw new LizzieRuntimeException("No arguments provided to 'set', provide at least a symbolic name, literally declared, e.g. 'set(@foo)'.");

            // Expecting symbol name as the first argument.
            var symbolName = arguments.Get<string>(0);

            // Sanity checking that symbol already exists.
            if (!binder.ContainsKey(symbolName))
                throw new LizzieRuntimeException($"The symbol '{symbolName}' has not been declared in the scope of where you are trying to 'set' it.");

            // More sanity checks.
            if (arguments.Count > 2)
                throw new LizzieRuntimeException($"The 'set' keyword can only handle at most two arguments, and you tried to pass in more than two arguments as you tried to change the value of the '{symbolName}' symbol.");

            /*
             * Retrieving the initial value, if any, for the symbol to push
             * onto the stack. If no variable is supplied, we default the
             * stack object's value to 'null'.
             */
            var value = arguments.Count > 1 ? arguments.Get(1) : null;

            // Setting the variable.
            binder[symbolName] = value;
            return value;
        });

        /// <summary>
        /// Declares a symbolic value, optionally setting its initial value
        /// 
        /// Expects the first argument to be a symbol, literally de-referenced by an '@'
        /// character, and the optional second argument to be the initial value for that symbol.
        /// If no second argument is provided, the initial value of the symbol
        /// will be set to 'null'.
        /// When the function is invoked, it will return the value of your symbol.
        /// </summary>
        /// <value>The function wrapping the 'var keyword'.</value>
        public static Function<TContext> Var => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking invocation.
            if (arguments.Count == 0)
                throw new LizzieRuntimeException("No arguments provided to 'var', provide at least a symbolic name, literally declared, e.g. 'var(@foo)'.");

            // Expecting symbol name as the first argument.
            var symbolName = arguments.Get<string>(0);

            // Sanity checking that symbol already exists.
            if (binder.ContainsKey(symbolName))
                throw new LizzieRuntimeException($"The symbol '{symbolName}' has already been declared in the scope of where you are trying to declare it using the 'var' keyword.");

            // More sanity checks.
            if (arguments.Count > 2)
                throw new LizzieRuntimeException($"The 'var' keyword can only handle at most two arguments, and you tried to pass in more than two arguments as you tried to declare the '{symbolName}' symbol.");

            /*
             * Retrieving the initial value, if any, for the symbol to push
             * onto the stack. If no variable is supplied, we default the
             * stack object's value to 'null'.
             */
            var value = arguments.Count > 1 ? arguments.Get(1) : null;

            // Setting the variable.
            binder[symbolName] = value;
            return value;
        });

        /// <summary>
        /// Adds a bunch of things together. Can be used either for string literals,
        /// integer numbers, or floating point numbers.
        /// 
        /// Will return the result of the addition to caller.
        /// </summary>
        /// <value>The function wrapping the 'add keyword'.</value>
        public static Function<TContext> Add => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking code.
            if (arguments.Count < 2)
                throw new LizzieRuntimeException("The 'add' keyword requires at least 2 arguments, and you tried to invoke it with fewer.");

            // Retrieving the first value, making sure we retrieve it as a "dynamic type".
            dynamic result = arguments.Get(0);
            foreach (dynamic ix in arguments.Skip(1)) {
                result += ix;
            }

            // Returning the result of addition operation to caller.
            return result;
        });

        /// <summary>
        /// Subtracts a bunch of numbers from the first number provided.
        /// 
        /// Will return the result of the subtraction to caller.
        /// </summary>
        /// <value>The function wrapping the 'subtract keyword'.</value>
        public static Function<TContext> Subtract => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking code.
            if (arguments.Count < 2)
                throw new LizzieRuntimeException("The 'subtract' keyword requires at least 2 arguments, and you tried to invoke it with fewer.");

            // Retrieving the first value, making sure we retrieve it as a "dynamic type".
            dynamic result = arguments.Get(0);
            foreach (dynamic ix in arguments.Skip(1)) {
                result -= ix;
            }

            // Returning the result of addition operation to caller.
            return result;
        });

        /// <summary>
        /// Multiplies a bunch of numbers to each other.
        /// 
        /// Will return the result of the multiplication to caller.
        /// </summary>
        /// <value>The function wrapping the 'multiply keyword'.</value>
        public static Function<TContext> Multiply => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking code.
            if (arguments.Count < 2)
                throw new LizzieRuntimeException("The 'multiply' keyword requires at least 2 arguments, and you tried to invoke it with fewer.");

            // Retrieving the first value, making sure we retrieve it as a "dynamic type".
            dynamic result = arguments.Get(0);
            foreach (dynamic ix in arguments.Skip(1)) {
                result *= ix;
            }

            // Returning the result of addition operation to caller.
            return result;
        });

        /// <summary>
        /// Divides a bunch of numbers from the first number.
        /// 
        /// Will return the result of the division to caller.
        /// </summary>
        /// <value>The function wrapping the 'divide keyword'.</value>
        public static Function<TContext> Divide => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking code.
            if (arguments.Count < 2)
                throw new LizzieRuntimeException("The 'multiply' keyword requires at least 2 arguments, and you tried to invoke it with fewer.");

            // Retrieving the first value, making sure we retrieve it as a "dynamic type".
            dynamic result = arguments.Get(0);
            foreach (dynamic ix in arguments.Skip(1)) {
                result /= ix;
            }

            // Returning the result of addition operation to caller.
            return result;
        });

        /// <summary>
        /// Calculates the modulo of two or more numbers.
        /// 
        /// Will return the result to caller.
        /// </summary>
        /// <value>The function wrapping the 'modulo keyword'.</value>
        public static Function<TContext> Modulo => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking code.
            if (arguments.Count < 2)
                throw new LizzieRuntimeException("The 'multiply' keyword requires at least 2 arguments, and you tried to invoke it with fewer.");

            // Retrieving the first value, making sure we retrieve it as a "dynamic type".
            dynamic result = arguments.Get(0);
            foreach (dynamic ix in arguments.Skip(1)) {
                result %= ix;
            }

            // Returning the result of addition operation to caller.
            return result;
        });

        /// <summary>
        /// Creates a function and returns it to the caller.
        /// </summary>
        /// <value>The function created.</value>
        public static Function<TContext> Function => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking code.
            if (arguments.Count == 0)
                throw new LizzieRuntimeException("The 'function' keyword requires at least 1 argument, and you tried to invoke it with fewer.");

            // Retrieving body of function and doing some basic sanity checks.
            var lambda = arguments.Get(0) as Function<TContext>;
            if (lambda == null)
                throw new LizzieRuntimeException("When declaring a function, the first argument must be a body.");

            // Retrieving argument declarations and sanity checking them.
            var args = arguments.Skip(1).Select(ix => ix as string).ToList();
            foreach (var ix in args) {
                if (!(ix is string))
                    throw new LizzieRuntimeException("When declaring a function, all argument declarations must be valid symbols.");
                Symbol<TContext>.SanityCheckSymbolName(ix as string);
            }

            // Returning function to caller.
            return new Function<TContext>((ctx2, binder2, arguments2) => {
                binder.PushStack();
                try {
                    for (var ix = 0; ix < arguments2.Count && ix < args.Count; ix++) {
                        binder[args[ix]] = arguments2.Get(ix);
                    }

                    // Applying the rest of our arguments as null values.
                    for (var ix = arguments2.Count; ix < args.Count; ix++) {
                        binder[args[ix]] = null;
                    }
                    return lambda(ctx2, binder2, arguments2);
                } finally {
                    binder.PopStack();
                }
            });
        });

        /// <summary>
        /// Creates an if condition and returns it to the caller.
        /// </summary>
        /// <value>The function created.</value>
        public static Function<TContext> If => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking code.
            if (arguments.Count < 2)
                throw new LizzieRuntimeException("The 'if' keyword requires at least 2 arguments, and you tried to invoke it with fewer.");

            // Retrieving condition, if body, and sanity checking invocation.
            var condition = arguments.Get(0);
            var lambdaIf = arguments.Get(1) as Function<TContext>;
            if (lambdaIf == null)
                throw new LizzieRuntimeException("The 'if' keyword requires a lambda argument as its second argument.");

            // Checking if condition is to be lazily evaluated.
            if (condition is Function<TContext> conditionFunction) {
                condition = conditionFunction(ctx, binder, arguments);
            }

            // Retrieving an sanity checking else body, if it exists.
            var arg3 = arguments.Count > 2 ? arguments.Get(2) : null;
            Function<TContext> lambdaElse = null;
            if (arg3 != null) {
                lambdaElse = arg3 as Function<TContext>;
                if (lambdaElse == null)
                    throw new LizzieRuntimeException("The 'if' keyword requires a lambda argument as its third (else) argument if you supply a third argument.");
            }

            // Checking if we should evaluate if body.
            if (condition != null) {

                // If yields true, evaluating if body.
                return lambdaIf(ctx, binder, arguments);

            } else if (lambdaElse != null) {

                // If yields false, and we have an else body, hence we evaluate it.
                return lambdaElse(ctx, binder, arguments);
            }
            return null;
        });
    }
}
