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
using poetic.lambda.collections;

namespace poetic.lizzie.keywords
{
    /// <summary>
    /// The "return" keyword.
    /// </summary>
    public static class Return<TContext>
    {
        /// <summary>
        /// Returns the value to caller.
        /// </summary>
        /// <value>The keyword-name/parsing-function mappings that are responsible for parsing the branching keywords.</value>
        public static IEnumerable<Tuple<string, Func<IEnumerator<string>, Func<TContext, Arguments, Binder<TContext>, object>>>> Keywords
        {
            get {
                return new Tuple<string, Func<IEnumerator<string>, Func<TContext, Arguments, Binder<TContext>, object>>> [] {

                    /*
                     * Creates our "return" function parser.
                     */
                    new Tuple<string, Func<IEnumerator<string>, Func<TContext, Arguments, Binder<TContext>, object>>>(
                        "return",
                        new Func<IEnumerator<string>, Func<TContext, Arguments, Binder<TContext>, object>> (delegate (IEnumerator<string> en) {

                        // Skipping "return" token.
                        if (!en.MoveNext())
                            throw new PoeticParsingException("Unexpected EOF while parsing 'return' invocation.");

                        // Parsing return invocation's arguments, and sanity checking invocation.
                        var args = ArgumentsParser<TContext>.Parse("return", en);
                        if (args.Count > 1)
                            throw new PoeticParsingException("'return' keyword can only return one argument.");

                        // Returning function invocation to caller.
                        return new Func<TContext, Arguments, Binder<TContext>, object>(delegate (TContext ctx, Arguments arguments, Binder<TContext> binder) {
                            return args[0](ctx, arguments, binder);
                        });
                        }))
                };
            }
        }
    }
}
