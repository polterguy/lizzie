/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System.Linq;
using lizzie.exceptions;

namespace lizzie
{
    /// <summary>
    /// Contains all the standard 'keyword' functions in Lizzie.
    /// </summary>
    public static class Functions<TContext>
    {
        /// <summary>
        /// Allows you to declare a variable.
        /// 
        /// Expects the first argument to be a symbol, referenced by prepending
        /// it with an '@' token, and the second argument to be the optional
        /// initial value for that symbol. If no second argument is provided,
        /// the initial value of the symbol will be set to 'null'.
        /// When the function is evaluated, it will return the value you initially
        /// set the symbol to.
        /// </summary>
        /// <value>The function wrapping the 'var keyword'.</value>
        public static Function<TContext> Var {
            get {
                return new Function<TContext>((ctx, binder, arguments) => {

                    // Sanity checking invocation.
                    if (arguments.Count == 0)
                        throw new LizzieRuntimeException("No arguments provided to 'var', provide at least a symbolic name, literally declared, e.g. 'var(@foo)'.");

                    // Expecting symbol name as the first argument.
                    var symbolName = arguments.Get<string>(0);

                    // More sanity checks.
                    if (arguments.Count > 2)
                        throw new LizzieRuntimeException($"The 'var' keyword can only handle at most two arguments, as you tried to declare the '{symbolName}' symbol, you passed in more than two arguments.");

                    /*
                     * Sanity checking that code is not trying to declare the same
                     * variable twice.
                     */
                    if (binder.ContainsKey(symbolName))
                        throw new LizzieRuntimeException($"You cannot declare the '{symbolName}' symbol since it was already previously declared in the same body.");

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
            }
        }

        /// <summary>
        /// Allows you to change the value of a previously declared variable.
        /// 
        /// Expects the first argument to be a symbol, literally de-referenced by an '@'
        /// character, and the second argument to be the new value for that symbol.
        /// If no second argument is provided, the new value of the symbol
        /// will be 'null'.
        /// When the function is invoked, it will return the value you changed
        /// the symbolic value to.
        /// </summary>
        /// <value>The function wrapping the 'set keyword'.</value>
        public static Function<TContext> Set {
            get {
                return new Function<TContext>((ctx, binder, arguments) => {

                    // Sanity checking invocation.
                    if (arguments.Count == 0)
                        throw new LizzieRuntimeException("No arguments provided to 'set', provide at least a symbolic name, literally declared, e.g. 'set(@foo)'.");

                    // Expecting symbol name as the first argument.
                    var symbolName = arguments.Get<string>(0);

                    // More sanity checks.
                    if (arguments.Count > 2)
                        throw new LizzieRuntimeException($"The 'set' keyword can only handle at most two arguments, as you tried to pass in more than two arguments as you tried to change the value of the '{symbolName}' symbol.");

                    /*
                     * Retrieving the initial value, if any, for the symbol to push
                     * onto the stack. If no variable is supplied, we default the
                     * stack object's value to 'null'.
                     */
                    var value = arguments.Count > 1 ? arguments.Get(1) : null;

                    // Sanity checking that symbol exists from before.
                    if (!binder.ContainsKey(symbolName))
                        throw new LizzieRuntimeException($"The symbol '{symbolName}' does not exist, please declare it using the 'var' keyword.");

                    // Setting the variable.
                    binder[symbolName] = value;
                    return value;
                });
            }
        }

        /// <summary>
        /// Adds a bunch of things together. Can be used either for string literals,
        /// integer numbers, or floating point numbers.
        /// 
        /// Will return the result of the addition to caller.
        /// </summary>
        /// <value>The function wrapping the 'add keyword'.</value>
        public static Function<TContext> Add
        {
            get {
                return new Function<TContext>((ctx, binder, arguments) => {

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
            }
        }

        /// <summary>
        /// Subtracts a bunch of numbers from the first number provided.
        /// 
        /// Will return the result of the subtraction to caller.
        /// </summary>
        /// <value>The function wrapping the 'subtract keyword'.</value>
        public static Function<TContext> Subtract
        {
            get
            {
                return new Function<TContext>((ctx, binder, arguments) => {

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
            }
        }

        /// <summary>
        /// Multiplies a bunch of numbers to each other.
        /// 
        /// Will return the result of the multiplication to caller.
        /// </summary>
        /// <value>The function wrapping the 'multiply keyword'.</value>
        public static Function<TContext> Multiply
        {
            get
            {
                return new Function<TContext>((ctx, binder, arguments) => {

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
            }
        }

        /// <summary>
        /// Divides a bunch of numbers from the first number.
        /// 
        /// Will return the result of the division to caller.
        /// </summary>
        /// <value>The function wrapping the 'divide keyword'.</value>
        public static Function<TContext> Divide
        {
            get
            {
                return new Function<TContext>((ctx, binder, arguments) => {

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
            }
        }

        /// <summary>
        /// Calculates the modulo of two or more numbers.
        /// 
        /// Will return the result to caller.
        /// </summary>
        /// <value>The function wrapping the 'modulo keyword'.</value>
        public static Function<TContext> Modulo
        {
            get
            {
                return new Function<TContext>((ctx, binder, arguments) => {

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
            }
        }
    }
}
