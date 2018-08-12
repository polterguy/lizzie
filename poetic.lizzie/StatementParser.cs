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
using System.Globalization;
using System.Collections.Generic;
using poetic.lambda.parser;
using poetic.lambda.exceptions;
using poetic.lambda.collections;

namespace poetic.lizzie
{
    /// <summary>
    /// Lizzie parser parsing a single statement.
    /// </summary>
    public static class StatementParser<TContext>
    {
        /// <summary>
        /// Creates a Lizzie statement.
        /// </summary>
        /// <returns>An action wrapping the execution of our statement.</returns>
        /// <param name="keywords">Keywords you want to use.</param>
        /// <param name="en">Enumerator containing tokens.</param>
        public static Action<FunctionStack<TContext>> Create(LizzieKeywords<TContext> keywords, IEnumerator<string> en)
        {
            if (keywords.HasKeyword(en.Current)) {

                // This is a registered keyword, hence parsing it as such.
                return keywords[en.Current](en);

            } else {

                // Some sort of variable de-referencing operation (function invocation for instance?)
                return Create(en);
            }
        }

        /// <summary>
        /// Creates a function invocation statement and returns it to caller as
        /// an action.
        /// </summary>
        /// <returns>The function invocation.</returns>
        /// <param name="name">Name.</param>
        /// <param name="en">En.</param>
        /// <param name="mustClose">If set to <c>true</c> must close.</param>
        public static Action<FunctionStack<TContext>> CreateFunctionInvocation(string name, IEnumerator<string> en, bool mustClose)
        {
            /*
             * Sanity checking code.
             * At this point Current should be "(", hence discarding opening paranthesis,
             * and sanity checking invocation.
             */
            if (!en.MoveNext())
                throw new PoeticParsingException($"Unexpected EOF while parsing function invocation to {name}");

            /*
             * Creating our arguments resolver, that will evaluate the arguments
             * during runtime, to retrieve their runtime value(s).
             */
            var arguments = CreateArgumentsEvaluator(en);

            /*
             * In Lizzie the semicolon is mandatory after each statement, unless
             * it's an "inner statement", implying it's a function invocation being
             * used as an argument to another function invocation.
             */
            if (mustClose && (!en.MoveNext() || en.Current != ";"))
                throw new PoeticParsingException($"Missing semicolon after '{name}' function invocation.");

            // Creating our action and returning to caller.
            return new Action<FunctionStack<TContext>>(delegate (FunctionStack<TContext> st) {

                // Sanity checking invocation, that function actually exists.
                if (!(st[name] is Func<TContext, Arguments, object> func))
                {

                    /*
                     * Ooops, no such function!
                     * Checking if there exists an object with the same name at all,
                     * to throw slightly more informative exceptions.
                     */
                    if (st.HasKey(name))
                        throw new PoeticExecutionException($"Tried to evaluate object '{name}' as a function, while object was '{st[name].GetType().Name}'.");

                    // Key doesn't exist at all.
                    throw new PoeticExecutionException($"Function '{name}' doesn't exist.");
                }

                /*
                 * Evaluating our late bound arguments, and evaluate our function
                 * with the results of the evaluation of our arguments, and making
                 * sure we put the function's return value onto the stack.
                 */
                var fArgs = new Arguments(arguments.Evaluate(st));
                st.Return = func(st.Context, fArgs);
            });
        }

        /* 
         * Creates a statement referencing some sort of variable, which can be either
         * a function invocation or a variable assignment.
         */
        static Action<FunctionStack<TContext>> Create(IEnumerator<string> en)
        {
            // Variable name.
            var cur = en.Current;

            /*
             * Figuring out what type of statement this is, candidates are assignments
             * and function invocations. But first sanity checking our tokens.
             * 
             * Notice, if the next token is "(" this is a function invocation, if
             * the next token is "=", "+=" or "-=", this is an assignment statement.
             */
            if (!en.MoveNext()) {
                throw new PoeticParsingException($"Unexpected EOF after {cur}, expected assignment or function invocation.");
            }

            // Figuring out type of statement.
            switch (en.Current) {
                case "(":

                    /*
                     * Function invocation.
                     * We don't expect semicolon here, and we don't care about its return value.
                     */
                    return CreateFunctionInvocation(cur, en, true);

                case "=":
                case "+=":
                case "-=":

                    /*
                     * Assignment statement.
                     */
                    return CreateAssignment(cur, en);

                default:
                    throw new PoeticParsingException($"Unexpected token {en.Current} after {cur}, expected function invocation or assignment.");
            }
        }

        /*
         * Creates an arguments evaluator, that returns a Functions object, which
         * will retrieve arguments to a function invocation during runtime.
         */
        static Functions<FunctionStack<TContext>, object> CreateArgumentsEvaluator(IEnumerator<string> en)
        {
            /*
             * Iterating as long as we have more arguments, and creating a list
             * of "late bound" arguments, which aren't evaluated before the function
             * is evaluated. The arguments are wrapped inside a Functions instance
             * for simplicity, which allows us to sequentially evaluate them as
             * late bound arguments, to evaluate their runtime values.
             */
            var arguments = new Functions<FunctionStack<TContext>, object>();
            while (true) {

                /*
                 * Checking if this is a comma, and such is expected, at which point we
                 * discard it, and move to our next argument.
                 */
                if (en.Current == ",") {

                    // Cannot start a function invocation with a "," as its first parameter!
                    if (arguments.Count == 0)
                        throw new PoeticParsingException("Syntax error after invocation to function, unexpected comma as first argument.");
                    if (!en.MoveNext())
                        throw new PoeticParsingException("Unexpected EOF while parsing invocation to function.");
                    if (en.Current == ",")
                        throw new PoeticParsingException("Unexpecte comma found while parsing arguments to function invocation.");
                }

                // Figuring out type of parameter.
                if (en.Current == ")")
                    break; // End off function invocation.

                /*
                 * At this point we are at the beginning of an expression, which
                 * might be a constant, a variable reference, or a full expression.
                 */
                arguments.Add(ExpressionParser<TContext>.Create(en));
            }
            return arguments;
        }

        /*
         * Creates an assignment statement, assigning some value to some variable, and
         * returning statement to caller.
         */
        static Action<FunctionStack<TContext>> CreateAssignment(string name, IEnumerator<string> en)
        {
            // Creating our action and returning to caller.
            return new Action<FunctionStack<TContext>>(delegate (FunctionStack<TContext> st) {
                var stackObject = st[name];
            });
        }
    }
}
