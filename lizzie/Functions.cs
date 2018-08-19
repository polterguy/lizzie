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
        /// Expects the first argument to be a symbol, literally referenced by an '@'
        /// character, and the second argument to be the initial value for that symbol.
        /// If no second argument is provided, the initial value of the symbol
        /// will be set to 'null'.
        /// </summary>
        /// <value>The function wrapping the 'var' function.</value>
        public static Function<TContext> Var {
            get {
                return new Function<TContext>((ctx, binder, arguments) => {

                    // Expecting symbol name as the first argument.
                    var symbolName = arguments.Get<string>(0);

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

                    // Setting the variable
                    binder[symbolName] = value;
                    return value;
                });
            }
        }
    }
}
