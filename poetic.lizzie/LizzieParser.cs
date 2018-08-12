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
        Action<FunctionStack<TContext>> CreateFunctionInvocationStatement(string name, IEnumerator<string> en, bool forceClosing = true)
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

                /*
                 * Checking if this is a comma, and such is expected, at which point we
                 * discard it, and move to our next argument.
                 */
                if (en.Current == ",") {

                    // Cannot start a function invocation with a "," as its first parameter!
                    if (arguments.Count == 0)
                        throw new PoeticParsingException($"Syntax error after invocation to '{name}', unexpected comma as first argument.");
                    if (!en.MoveNext())
                        throw new PoeticParsingException($"Unexpected EOF while parsing invocation to '{name}'.");
                }

                // Figuring out type of parameter.
                if (en.Current == ")")
                    break; // End off function invocation.

                /*
                 * At this point we are at the beginning of an expression, which
                 * might be a constant, a variable reference, or a full expression.
                 */
                arguments.Add(CreateExpression(en));
            }

            /*
             * In Lizzie the semicolon is mandatory after each statement. 
             */
            if (forceClosing && (!en.MoveNext() || en.Current != ";"))
                throw new PoeticParsingException($"Missing semicolon after '{name}' function invocation.");

            // Creating our action and returning to caller.
            return new Action<FunctionStack<TContext>>(delegate (FunctionStack<TContext> st) {

                // Sanity checking invocation, that function actually exists.
                if (!(st[name] is Func<TContext, Arguments, object> func)) {

                    /*
                     * Ooops, no such function!
                     * 
                     * Checking if there exists an object with the same name at all,
                     * to throw slightly more informative exceptions.
                     */
                    if (st.HasKey(name)) {

                        // Variable exists but is not a function.
                        throw new PoeticExecutionException($"Tried to evaluate object '{name}' as a function, while object was '{st[name].GetType().Name}'.");
                    }

                    // Key doesn't exist at all.
                    throw new PoeticExecutionException($"Function '{name}' doesn't exist.");
                }

                /*
                 * Evaluating our late bound arguments, and evaluate our function
                 * with the results of the evaluation of our arguments.
                 */
                var fArgs = new Arguments(arguments.Evaluate(st));
                st.Return = func(st.Context, fArgs);
            });
        }

        /*
         * Creates an expression that is evaluated at runtime, which might be a
         * function invocation, a constant, or an actual expression.
         */
        Func<FunctionStack<TContext>, object> CreateExpression(IEnumerator<string> en)
        {
            /*
             * Figuring out what the iterator is currently pointing to.
             * 
             * Candidates are constant, function invocation and expressions.
             * First we retrieve the current token from our tokenizer, and do
             * the simple checks, which are for some sortof constant.
             */
            var cur = en.Current;

            // Checking if enumerator is pointing to a numeric constant.
            if ("0123456789".IndexOf(cur[0]) != -1) {

                /*
                 * Some sort of number constant.
                 * 
                 * PS!
                 * All numbers in Lizzie are double, which equals 64 bits floating
                 * point numbers. This is the same logic as JavaScript.
                 */
                var constNumber = double.Parse(cur, CultureInfo.InvariantCulture);
                var expression = new Func<FunctionStack<TContext>, object>(delegate (FunctionStack<TContext> fs) {
                    return constNumber;
                });
                if (!en.MoveNext())
                    throw new PoeticParsingException($"Unexpected EOF after parsing the '{cur}' numeric constant.");
                return expression;
            }

            // Checking if enumerator is pointing to a string constant.
            if (cur == "\"" || cur == "'") {

                /*
                 * A string constant.
                 * Discarding opening single or double quote character, and sanity
                 * checking code at the same time.
                 */
                if (!en.MoveNext())
                    throw new PoeticParsingException($"Unexpected EOF while parsing string literal.");
                var constString = en.Current;
                var expression = new Func<FunctionStack<TContext>, object>(delegate (FunctionStack<TContext> fs) {
                    return constString;
                });

                // Moving enumerator beyond currently handled token, and doing some more sanity checking.
                if (!en.MoveNext())
                    throw new PoeticParsingException($"Unexpected EOF after parsing the '{constString}' string constant.");
                if (en.Current != cur)
                    throw new PoeticParsingException($"Syntax error, expecting ({cur}) after '{constString}', found '{en.Current}'.");
                if (!en.MoveNext())
                    throw new PoeticParsingException($"Unexpected EOF after parsing the '{constString}' string literal constant.");
                return expression;
            }

            /*
             * Current token is not pointing to a string or numeric constant,
             * hence candidates now are function invocation or expression.
             * 
             * If the next token is a "(", it is a function invocation, otherwise
             * it is an expression. But first some more sanity checking.
             */
            if (!en.MoveNext())
                throw new PoeticParsingException($"Unexpected EOF while parsing expression close to '{cur}'");
            if (en.Current == "(") {

                // Function invocation.
                var invocation = CreateFunctionInvocationStatement(cur, en, false /* We don't want the parser to expect a semicolon here! */);
                Func<FunctionStack<TContext>, object> functor = new Func<FunctionStack<TContext>, object>(delegate (FunctionStack<TContext> fs) {
                    invocation (fs);
                    return fs.Return;
                });

                // Skipping closing paranthesis for function invocation.
                if (!en.MoveNext())
                    throw new PoeticParsingException($"Unexpected EOF while parsing expression close to '{cur}'");

                // Returning function invocation to caller.
                return functor;

            } else {

                // An actual expression.
                // TODO: Continue here!
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
