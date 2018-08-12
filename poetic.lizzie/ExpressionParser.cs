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
    /// Lizzie parser parsing a single expression.
    /// </summary>
    public static class ExpressionParser<TContext>
    {
        /*
         * Creates an expression that is evaluated at runtime, which might be a
         * function invocation, a constant, or an actual expression.
         */
        public static Func<FunctionStack<TContext>, object> Create(IEnumerator<string> en)
        {
            /*
             * Figuring out what the iterator is currently pointing to.
             * 
             * Candidates are a constant, a function invocation, or an expression.
             * First we retrieve the current token from our tokenizer, and do
             * the simple checks, which are for some sort of constant.
             */
            var cur = en.Current;

            // Checking if enumerator is pointing to a numeric constant.
            if ("0123456789".IndexOf(cur[0]) != -1) {

                // First character is a number from 0-9, hence this is a number of some sort.
                return CreateNumericConstant(en);
            }

            // Checking if enumerator is pointing to a string constant.
            if (cur == "\"" || cur == "'") {

                // A string literal constant.
                return CreateStringLiteralConstant(en);
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

            // Checking if this is a function invocation.
            if (en.Current == "(") {

                /*
                 * Function invocation.
                 *
                 * NOTICE!
                 * We don't expect a semicolon here after function invocation.
                 */
                var invocation = StatementParser<TContext>.CreateFunctionInvocation(cur, en, false);
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

                // Defaulting to expression.
                // TODO: Continue here!
            }

            return null;
        }

        /*
         * Creates a numeric constant function, that simply returns the constant
         * number at runtime.
         */
        static Func<FunctionStack<TContext>, object> CreateNumericConstant (IEnumerator<string> en)
        {
            /*
             * Parsing our string to become a 64 bits double floating point value.
             * 
             * NOTICE!
             * All numeric value in Lizze are 64 bits floating point double types.
             * This is similar to JavaScript.
             */
            var constNumber = double.Parse(en.Current, CultureInfo.InvariantCulture);
            var expression = new Func<FunctionStack<TContext>, object>(delegate (FunctionStack<TContext> fs) {
                return constNumber;
            });
            if (!en.MoveNext())
                throw new PoeticParsingException($"Unexpected EOF after parsing the '{constNumber}' numeric constant.");
            return expression;
        }

        /*
         * Creates a numeric constant function, that simply returns the constant
         * number at runtime.
         */
        static Func<FunctionStack<TContext>, object> CreateStringLiteralConstant(IEnumerator<string> en)
        {
            /*
             * We need to store the quote type, to verify our string literal is
             * closed with the same quote type that it was opened with.
             */
            var quote = en.Current;

            /*
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
            if (en.Current != quote)
                throw new PoeticParsingException($"Syntax error, expecting ({quote}) after '{constString}', found '{en.Current}'.");
            if (!en.MoveNext())
                throw new PoeticParsingException($"Unexpected EOF after parsing the '{constString}' string literal constant.");
            return expression;
        }
    }
}
