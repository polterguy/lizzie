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
using System.Linq;
using System.Collections.Generic;
using lizzie.exceptions;

namespace lizzie.types
{
    static class FunctionInvocation<TContext>
    {
        internal static Function<TContext> Compile(string symbol, IEnumerator<string> en)
        {
            // Retrieving symbol name and doing some basic sanity checking.
            SanityCheckSymbolName(symbol);

            // Sanity checking function invocation.
            if (en.Current != "(" || !en.MoveNext())
                throw new LizzieParsingException($"Missing arguments after '{symbol}'");

            // Compiling arguments, such that we can evaluate them lazily.
            var lazy = CompileLazyArguments(symbol, en);

            // Discarding ")" token.
            en.MoveNext();

            // Returning function invocation to caller.
            return new Function<TContext>((ctx, binder, arguments) => {

                // Checking if this is a function symbolically referenced.
                string reference = symbol.TrimStart('@');
                while (symbol.StartsWith("@", StringComparison.InvariantCulture)) {
                    reference = binder[reference] as string;
                    symbol = symbol.Substring(1);
                }

                // Evaluating function invocation hopefully found in binder now, evaluating our lazy arguments.
                return binder.Get<Function<TContext>>(reference)(
                    ctx, 
                    binder, 
                    new Arguments (lazy.Select(ix => ix(ctx, binder, arguments))));
            });
        }

        static List<Function<TContext>> CompileLazyArguments(string symbol, IEnumerator<string> en)
        {
            // Iterating while we have more arguments, and appending into returned Arguments collection.
            var arguments = new List<Function<TContext>>();
            while (en.Current != ")") {

                // Late binding our argument, and appending it to returned Arguments collection.
                arguments.Add(CompileLazyArgument(en));

                // Checking if we have more arguments.
                if (en.Current == ")")
                    break;

                // Sanity checking.
                if (en.Current != "," || !en.MoveNext() || en.Current == ")")
                    throw new LizzieParsingException($"Syntax error after argument found in '{symbol}'");
            }

            // Returning arguments to caller.
            return arguments;
        }

        static Function<TContext> CompileLazyArgument(IEnumerator<string> en)
        {
            // Figuring out what type of argument we're dealing with.
            object value = en.Current;
            if (en.Current == "\"") {

                // Constant string argument.
                if (!en.MoveNext())
                    throw new LizzieParsingException("Unexpected EOF while parsing string literal.");

                // Updating value and moving past closing double quote.
                value = en.Current;
                if (!en.MoveNext() || en.Current != "\"" || !en.MoveNext())
                    throw new LizzieParsingException("Unexpected EOF while parsing string literal.");

            } else if (IsNumeric(en.Current)) {

                // Numeric constant.
                if (en.Current.Count(ix => ix == '.') == 1) {

                    // Floating point numeric value.
                    value = double.Parse(en.Current);

                } else {

                    // Integer numeric value.
                    value = long.Parse(en.Current);
                }
                if (!en.MoveNext())
                    throw new LizzieParsingException("Unexpected EOF after parsing numeric constant.");

            } else if (IsSymbol(en.Current)) {

                // Symbol.
                var symbolName = en.Current;
                if (!en.MoveNext())
                    throw new LizzieParsingException($"Unexpected EOF after parsing '{symbolName}' symbol.");
                if (en.Current == "(") {

                    // Nested function invocation.
                    return Compile(symbolName, en);
                }

                // Symbolic reference.
                if (!symbolName.StartsWith("@", StringComparison.InvariantCulture)) {

                    // Literally implying the symbol (its name).
                    return new Function<TContext>((ctx, binder, arguments) => {
                        return symbolName;
                    });

                } else {

                    // Referencing symbol's value, possibly recursively.
                    return new Function<TContext>((ctx, binder, arguments) => {

                        object reference = symbolName.TrimStart('@');
                        while (symbolName.StartsWith ("@", StringComparison.InvariantCulture)) {
                            symbolName = symbolName.Substring(1);
                            reference = binder[reference.ToString()];
                        }
                        return reference;
                    });
                }
            }

            // Returning function being wrapping argument such that it is evaluated lazily.
            return new Function<TContext>((ctx, binder, arguments) => {
                return value;
            });
        }

        static bool IsNumeric (string value)
        {
            if (value.Any(ix => "0123456789.".IndexOf(ix) == -1))
                return false;
            if (value.Count(ix => ix == '.') > 1)
                return false;
            return true; // Number of some sort.
        }

        static bool IsSymbol(string name)
        {
            if (IsNumeric(name))
                return false;
            if (name == "{")
                return false;
            return true;
        }

        static void SanityCheckSymbolName(string name)
        {
            if ("0123456789".IndexOf(name[0]) == 0)
                throw new LizzieParsingException($"'{name}' is not a legal symbol name, you cannot start a symbol with a number.");
        }
    }
}
