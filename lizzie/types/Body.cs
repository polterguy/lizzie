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

using System.Collections.Generic;
using lizzie.exceptions;

namespace lizzie.types
{
    static class Body<TContext>
    {
        internal static List<Function<TContext>> Compile(IEnumerator<string> en, bool forceClose = true)
        {
            // Iterating through each root level function invocation in code.
            var content = new List<Function<TContext>>();
            while (true) {

                // Expecting symbol name to be current value of enumerator.
                var symbol = en.Current;

                // Moving to "(" of function invocation and sanity checking code.
                if (!en.MoveNext())
                    throw new LizzieParsingException($"Unexpected EOF while parsing '{symbol}'");

                // Appending function invocation to return value.
                content.Add(FunctionInvocation<TContext>.Compile(symbol, en));

                // Checking if we're at end of body.
                if (en.Current == "}" || !en.MoveNext())
                    break;
            }

            // Sanity checking code before we return list of root level function invocations to caller.
            if (forceClose) {

                if (en.Current != "}" || !en.MoveNext())
                    throw new LizzieParsingException($"Unexpected and premature EOF while parsing code.");

            } else {

                if (en.Current == "}")
                    throw new LizzieParsingException("Syntax error, too many closing braces ('}') in code.");
            }

            // Returning list of functions to caller.
            return content;
        }
    }
}
