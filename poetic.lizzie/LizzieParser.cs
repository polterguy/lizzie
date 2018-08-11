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
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using poetic.lambda.parser;
using poetic.lambda.exceptions;
using poetic.lambda.collections;

namespace poetic.lizzie
{
    /// <summary>
    /// Lizzie parser that creates a Lizzie execution object to be evaluated as
    /// a function.
    /// </summary>
    public class LizzieParser<TContext>
    {
        // Binder for this instance.
        readonly Binder<TContext> _binder = new Binder<TContext>();

        // Which keywords to use.
        readonly LizzieKeywords<TContext> _keywords;

        public LizzieParser(LizzieKeywords<TContext> keywords = null)
        {
            /*
             * If no explicit keywords override have been supplied, we use the default
             * CTOR, which will populate our keywords dictionary with the default Lizzie
             * keywords.
             */
            _keywords = keywords ?? new LizzieKeywords<TContext>();
        }

        /// <summary>
        /// Parses the code in the stream, using the tokenizer, and returns a function
        /// to caller.
        /// </summary>
        /// <returns>The function object being the result of the parse operation.</returns>
        /// <param name="tokenizer">Tokenizer to use.</param>
        /// <param name="stream">Stream containing your code.</param>
        public Func<TContext, object> Parse(Tokenizer tokenizer, Stream stream)
        {
            return Parse(tokenizer.Tokenize(stream));
        }

        /// <summary>
        /// Parses the code in all streams, using the tokenizer, and returns a function
        /// to caller.
        /// </summary>
        /// <returns>The function object being the result of the parse operation.</returns>
        /// <param name="tokenizer">Tokenizer to use.</param>
        /// <param name="streams">Streams containing your code.</param>
        public Func<TContext, object> Parse(Tokenizer tokenizer, IEnumerable<Stream> streams)
        {
            return Parse(tokenizer.Tokenize(streams));
        }

        /// <summary>
        /// Parses the specified code, using the tokenizer, and returns a function
        /// to caller.
        /// </summary>
        /// <returns>The function object being the result of the parse operation.</returns>
        /// <param name="tokenizer">Tokenizer to use.</param>
        /// <param name="code">The code you wish to parse.</param>
        public Func<TContext, object> Parse(Tokenizer tokenizer, string code)
        {
            return Parse(tokenizer.Tokenize(code));
        }

        /// <summary>
        /// Parses the specified code snippets, using the tokenizer, and returns a function
        /// to caller.
        /// </summary>
        /// <returns>The function object being the result of the parse operation.</returns>
        /// <param name="tokenizer">Tokenizer to use.</param>
        /// <param name="code">Snippets of code you wish to create an execution object out of.</param>
        public Func<TContext, object> Parse(Tokenizer tokenizer, IEnumerable<string> code)
        {
            return Parse(tokenizer.Tokenize(code));
        }

        /*
         * Parses the code in the specified enumerator and returns a function to caller.
         */
        Func<TContext, object> Parse(IEnumerable<string> tokens)
        {
            /*
             * Creating our actions that will be the actual content of our lambda
             * function object.
             */
            var actions = new Actions<FunctionStack<TContext>>();

            /*
             * Creating our actual actions, which are a bunch of dynamically
             * created delegates, created according to what the parser finds in
             * our code.
             */
            var en = tokens.GetEnumerator();
            while (en.MoveNext()) {
                var statement = CreateStatement(en);
                actions.Add(statement);
            }

            // Creating our root level function object and returning it to caller.
            var functor = new Func<TContext, object>(delegate (TContext context) {

                // Creating our root level binder.
                Binder<TContext> binder = new Binder<TContext>();

                // Creating our root level stack.
                var stack = new FunctionStack<TContext>(binder, context);

                // Executing our actions, passing in our stack to execution.
                actions.Execute(stack);

                // Returning return value to caller.
                return stack.Return;
            });

            // Returning root level function object to caller.
            return functor;
        }

        /*
         * Parses the next statement and creates an action out of it, returning
         * that action to the caller.
         */
        Action<FunctionStack<TContext>> CreateStatement(IEnumerator<string> en)
        {
            if (_keywords.HasKeyword(en.Current)) {

                // This is a registered keyword, hence parsing it as such.
                return _keywords[en.Current](en);

            } else {

                // Some sort of variable de-referencing operation (function invocation for instance?)
                return CreateVariableStatement(en);
            }
        }

        /* 
         * Creates a statement referencing some sort of variable, which can be either
         * a function invocation or a variable assignment.
         */
        Action<FunctionStack<TContext>> CreateVariableStatement(IEnumerator<string> en)
        {
            // Variable name.
            var cur = en.Current;

            // Sanity checking name.
            SanityCheckVariableName(cur);

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
                     */
                    return CreateFunctionInvocationStatement(cur, en);

                case "=":
                case "+=":
                case "-=":

                    /*
                     * Assignment statement.
                     */
                    return CreateAssignmentStatement(cur, en);

                default:
                    throw new PoeticParsingException($"Unexpected token {en.Current} after {cur}, expected function invocation or assignment.");
            }
        }

        /*
         * Sanity checks variable name.
         * 
         * A legal variable name must start out with the letters a-z or A-Z.
         */
        void SanityCheckVariableName(string name)
        {
            if ("abcdefghijklmnopqrstuvwxyz".IndexOf(char.ToLower(name[0])) == -1) {
                throw new PoeticParsingException($"{name} is not a legal variable name.");
            }
        }

        /*
         * Creates a function invocation statement and returns to caller.
         */
        Action<FunctionStack<TContext>> CreateFunctionInvocationStatement(string name, IEnumerator<string> en)
        {
            /*
             * Sanity checking code.
             * At this point Current should be "(", hence discarding opening paranthesis,
             * and sanity checking invocation.
             */
            if (!en.MoveNext())
                throw new PoeticParsingException($"Unexpected EOF while parsing function invocation to {name}");

            /*
             * Iterating as long as we have more arguments, and creating a list
             * of "late bound" arguments, which aren't evaluated before the function
             * is evaluated.
             */
            Functions<FunctionStack<TContext>, object> arguments = new Functions<FunctionStack<TContext>, object>();
            while (true) {

                // Figuring out type of parameter.
                if (en.Current == ")")
                    break; // End off function invocation.

                /*
                 * At this point we are at the beginning of an expression, which
                 * might be a constant, a variable reference, or a full expression.
                 */
                arguments.Add(CreateExpression(en));
            }

            // Creating our action and returning to caller.
            return new Action<FunctionStack<TContext>>(delegate (FunctionStack<TContext> st) {

                /*
                 * Retrieving our function, evaluating our expressions, and invoking
                 * the function with the result of our expressions as its arguments.
                 */
                var fArgs = new Arguments(arguments.Evaluate(st));
                var func = st[name] as Func<TContext, Arguments, object>;
                func(st.Context, fArgs);
            });
        }

        /*
         * Creates an expression that is evaluated at runtime, which might be a
         * function invocation, a constant, or an actual expression.
         */
        Func<FunctionStack<TContext>, object> CreateExpression(IEnumerator<string> en)
        {
            // Figuring out what current iterator is pointing to.
            var cur = en.Current;
            if ("0123456789".IndexOf(cur[0]) != -1) {

                /*
                 * Some sort of number constant.
                 * 
                 * Creating our function that evaluates to that constant number,
                 * as a double value, discarding the current iterator, and returning
                 * that function to caller.
                 */
                var constNumber = double.Parse(cur, CultureInfo.InvariantCulture);
                var expression = new Func<FunctionStack<TContext>, object>(delegate (FunctionStack<TContext> fs) {
                    return constNumber;
                });
                if (!en.MoveNext())
                    throw new PoeticParsingException($"Unexpected EOF after parsing the {cur} constant.");
                return expression;
            }

            return null;
        }

        /*
         * Creates an assignment statement, assigning some value to some variable, and
         * returning statement to caller.
         */
        Action<FunctionStack<TContext>> CreateAssignmentStatement(string name, IEnumerator<string> en)
        {
            // Creating our action and returning to caller.
            return new Action<FunctionStack<TContext>>(delegate (FunctionStack<TContext> st) {
                var stackObject = st[name];
            });
        }
    }
}
