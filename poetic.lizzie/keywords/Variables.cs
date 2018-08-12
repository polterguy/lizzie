/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using poetic.lambda.parser;
using poetic.lambda.exceptions;

namespace poetic.lizzie.keywords
{
    /// <summary>
    /// Contains the variable declaration keyword.
    /// </summary>
    public static class Variables<TContext>
    {
        /// <summary>
        /// Returns the branching keyword parser to caller.
        /// </summary>
        /// <value>The keyword-name/parsing-function mappings that are responsible for parsing the branching keywords.</value>
        public static IEnumerable<Tuple<string, Func<IEnumerator<string>, Action<FunctionStack<TContext>>>>> Keywords
        {
            get {
                return new Tuple<string, Func<IEnumerator<string>, Action<FunctionStack<TContext>>>> [] {

                    /*
                     * If keyword.
                     */
                    new Tuple<string, Func<IEnumerator<string>, Action<FunctionStack<TContext>>>>(
                        "var",
                        new Func<IEnumerator<string>, Action<FunctionStack<TContext>>> (delegate (IEnumerator<string> en) {

                        /*
                         * At this point the enumerator should be at "var" keyword.
                         * It might be a simple variable declaration, without an assignment,
                         * or it might be both a declaration and an assignment.
                         * 
                         * First we need to figure out the name for our variable.
                         */
                        if (!en.MoveNext())
                            throw new PoeticParsingException("Unexpected EOF after 'var' keyword.");

                        // Retrieving variable name and sanity checking its name.
                        var name = en.Current;
                        SanityCheckVariableName(name);

                        // Sanity checking code.
                        if (!en.MoveNext())
                            throw new PoeticParsingException($"Unexpected EOF after declaration of variable named '{name}'.");

                        if (en.Current == ";") {

                            // This is only a variable declaration, and not an assignment.
                            return new Action<FunctionStack<TContext>>(delegate (FunctionStack<TContext> fs) {

                                /*
                                 * Allocating place for our variable on the stack,
                                 * without setting its initial value.
                                 */
                                fs[name] = null;

                            });
                        } else if (en.Current == "=") {

                            /*
                             * This is both a variable declaration and an assignment
                             * of its intitial value. Since its initial value might
                             * be an expression or function invocation, and not only
                             * necessarily a const value, we use our expression parser
                             * to late bind its value. But first sanity checking its
                             * syntax.
                             */
                            if (!en.MoveNext())
                                throw new PoeticParsingException($"Syntax error after '{name}' variable declaration.");
                            var expression = ExpressionParser<TContext>.Create(en);

                            /*
                             * Sanity checking syntax.
                             */
                            if (en.Current != ";")
                                throw new PoeticParsingException($"Missing semicolon after '{name}' variable declaration.");

                            /*
                             * Incrementing tokenizer to next token.
                             */
                            return new Action<FunctionStack<TContext>>(delegate (FunctionStack<TContext> fs) {

                                /*
                                 * Allocating place for our variable on the stack,
                                 * and also setting its initial value at the same time.
                                 */
                                fs[name] = expression(fs);

                            });
                        } else {

                            // Oops ...!!
                            throw new PoeticParsingException($"Syntax error after declaring '{name}' variable.");
                        }
                        }))
                };
            }
        }

        private static void SanityCheckVariableName(string name)
        {
            if ("abcdefghijklmnopqrstuvwxyz".IndexOf(name[0]) == -1)
                throw new PoeticParsingException($"'{name}' is not a legal variable name.");
        }
    }
}
