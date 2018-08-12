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
            var value = en.Current;

            // Checking if enumerator is pointing to a numeric constant.
            if ("0123456789".IndexOf(value[0]) != -1) {

                /*
                 * NOTICE!
                 * 
                 * This might still be an expression, such as "50 + 50" etc ...
                 * Exactly what it is, depends upon the next token.
                 */
                if (!en.MoveNext())
                    throw new PoeticParsingException($"Unexpected EOF after parsing '{value}'.");
                if (IsConstant(en.Current))
                    return CreateNumericConstant(value);

            } else if (value == "\"" || value == "'") {

                /*
                 * A string literal constant.
                 * Storing quote type to make sure literal ends with the same type
                 * of quotes.
                 */
                var quote = en.Current;
                if (!en.MoveNext())
                    throw new PoeticParsingException($"Unexpected EOF after parsing '{value}'.");

                // Finding our actual string literal constant.
                value = en.Current;

                // Making sure string literal ends with the same type of quote.
                if (!en.MoveNext() || en.Current != quote)
                    throw new PoeticParsingException($"Unexpected EOF after parsing '{value}'.");

                /*
                 * String literal might still be a part of an expression, which
                 * we won't know before we have fetched the next token.
                 * 
                 * Examples of expressions including string literal constants might be ("foo" + "bar")
                 * for instance.
                 */
                if (!en.MoveNext())
                    throw new PoeticParsingException($"Unexpected EOF after parsing '{value}'.");
                if (IsConstant(en.Current))
                    return CreateStringLiteralConstant(value);
            } else {

                // Moving forward.
                if (!en.MoveNext())
                    throw new PoeticParsingException($"Unexpected EOF after parsing '{value}'.");
            }

            /*
             * This is a "full expression", possibly a function invocation,
             * and not simply a numeric or string literal constant.
             */
            return CreateFullExpression(value, en);
        }

        /*
         * Creating a full expression and returning to caller.
         */
        static Func<FunctionStack<TContext>, object> CreateFullExpression(string firstToken, IEnumerator<string> en)
        {
            /*
             * Current token is not pointing to a string or numeric constant,
             * hence candidates now are function invocation or expression.
             * If it is a function invocation, en.Current equals "(".
             * If it is a full expression, en.Current might be one of the following.
             * "+"
             * "-"
             * "*"
             * "/"
             * "&&"
             * "||"
             */

            // Checking if this is a function invocation.
            if (en.Current == "(") {

                /*
                 * Function invocation.
                 *
                 * NOTICE!
                 * We don't expect a semicolon here after function invocation.
                 */
                var invocation = StatementParser<TContext>.CreateFunctionInvocation(firstToken, en, false);
                var functor = new Func<FunctionStack<TContext>, object>(delegate (FunctionStack<TContext> fs) {
                    invocation (fs);
                    return fs.Return;
                });

                // Skipping closing paranthesis for function invocation.
                if (!en.MoveNext())
                    throw new PoeticParsingException($"Unexpected EOF while parsing expression close to '{firstToken}'");

                // Returning function invocation to caller.
                return functor;

            } else {

                /*
                 * This might be a full expression, or a simple variable de-reference operation.
                 * Exactly what it is depends upon the next token.
                 */
                switch (en.Current) {

                    case ")":
                    case ";":

                        // Simple variable de-reference operation.
                        return new Func<FunctionStack<TContext>, object>(delegate (FunctionStack<TContext> fs) {
                            return fs[firstToken];
                        });
                }
            }

            // Oops ...!
            throw new PoeticParsingException($"Expected expression, constant or function invocation close to '{firstToken}'");
        }

        /*
         * Returns true if token defines the end of a constant.
         */
        static bool IsConstant(string token)
        {
            return token == "," || token == ")" || token == ";";
        }

        /*
         * Creates a numeric constant function, that simply returns the constant
         * number at runtime.
         */
        static Func<FunctionStack<TContext>, object> CreateNumericConstant (string value)
        {
            /*
             * Parsing our string to become a 64 bits double floating point value.
             * 
             * NOTICE!
             * All numeric value in Lizze are 64 bits floating point double types.
             * This is similar to JavaScript.
             */
            var constNumber = double.Parse(value, CultureInfo.InvariantCulture);
            var expression = new Func<FunctionStack<TContext>, object>(delegate (FunctionStack<TContext> fs) {
                return constNumber;
            });
            return expression;
        }

        /*
         * Creates a string literal constant function, that simply returns the constant
         * string at runtime.
         */
        static Func<FunctionStack<TContext>, object> CreateStringLiteralConstant(string value)
        {
            return new Func<FunctionStack<TContext>, object>(delegate (FunctionStack<TContext> fs) {
                return value;
            });
        }
    }
}
