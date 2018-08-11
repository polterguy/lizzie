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

namespace poetic.lizzie.keywords
{
    /// <summary>
    /// Branching keywords such as "if" and "else".
    /// </summary>
    public static class Branching<TContext>
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
                        "if",
                        new Func<IEnumerator<string>, Action<FunctionStack<TContext>>> (delegate (IEnumerator<string> en) {
                            return null;
                        })),

                    /*
                     * Else keyword.
                     */
                    new Tuple<string, Func<IEnumerator<string>, Action<FunctionStack<TContext>>>>(
                        "else",
                        new Func<IEnumerator<string>, Action<FunctionStack<TContext>>> (delegate (IEnumerator<string> en) {
                            return null;
                        }))
                };
            }
        }
    }
}
