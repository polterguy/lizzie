/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using lizzie.exceptions;

namespace lizzie
{
    /// <summary>
    /// Contains all the standard functions in Lizzie.
    /// </summary>
    public static class Functions<TContext>
    {
        /// <summary>
        /// Allows you to declare a variable.
        /// 
        /// Expects the first argument to be a symbol, literally de-referenced by an '@'
        /// character, and the second argument to be the initial value for that symbol.
        /// If no second argument is provided, the initial value of the symbol
        /// will be set to 'null'.
        /// When the function is invoked, it will return the value you initially
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

                    // Setting the variable.
                    binder[symbolName] = value;
                    return value;
                });
            }
        }
    }
}
