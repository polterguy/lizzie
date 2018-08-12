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
    public static class ArgumentsParser<TContext>
    {
        public static List<Func<TContext, Arguments, Binder<TContext>, object>> Parse(string functionName, IEnumerator<string> en)
        {
            // Sanity checking invocation, and skipping past initial "(".
            if (en.Current != "(")
                throw new PoeticParsingException($"Unexpected token while parsing '{functionName}'");
            if (!en.MoveNext())
                throw new PoeticParsingException($"Unexpected EOF while parsing arguments to '{functionName}'");

            // Return value, containing functions that will evaluate arguments during runtime.
            var arguments = new List<Func<TContext, Arguments, Binder<TContext>, object>>();

            // Iterating for as long as we have arguments.
            while (en.Current != ")") {

                // Adding currently iterated argument to return value.
                arguments.Add(Create(en));

                // Sanity checking.
                if (!en.MoveNext())
                    throw new PoeticParsingException($"Unexpected EOF while parsing arguments to '{functionName}'");
            }
            return arguments;
        }

        private static Func<TContext, Arguments, Binder<TContext>, object> Create(IEnumerator<string> en)
        {
            var argument = en.Current;
            if ("0123456789".IndexOf(argument[0]) != -1) {

                // Numeric constant.
                double constDouble = double.Parse(argument, CultureInfo.InvariantCulture);
                return new Func<TContext, Arguments, Binder<TContext>, object> (delegate (TContext ctx, Arguments arguments, Binder<TContext> binder) {
                    return constDouble;
                });
            }
            if (argument == "\"" || argument == "'") {

                // String literal constant.
                if (!en.MoveNext())
                    throw new PoeticParsingException("Unexpected EOF while parsing a string literal constant.");
                var strLiteral = en.Current;
                if (!en.MoveNext())
                    throw new PoeticParsingException("Unexpected EOF while parsing a string literal constant.");
                if (en.Current != argument)
                    throw new PoeticParsingException("Unexpected character while parsing a string literal constant.");
                return new Func<TContext, Arguments, Binder<TContext>, object>(delegate (TContext ctx, Arguments arguments, Binder<TContext> binder) {
                    return strLiteral;
                });
            }

            /*
             * Some sort of function invocation.
             */
            return FunctionInvocation<TContext>.Create(en);
        }
    }
}
