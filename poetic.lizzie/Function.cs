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
using poetic.lambda.exceptions;

namespace poetic.lizzie
{
    internal class Function<TContext> where TContext : class
    {
        public string Name {
            get;
            private set;
        }

        static internal Function<TContext> Create (IEnumerator<string> enumerator)
        {
            // Sanity checks.
            if (enumerator.Current != "function")
                throw new PoeticParsingException("That is not a function");
            if (!enumerator.MoveNext())
                throw new PoeticParsingException("No function name provided");
            SanityCheckFunctionName(enumerator.Current);

            // Creating return value and decorating its name.
            var retVal = new Function<TContext>() {
                Name = enumerator.Current
            };

            // Returning function to caller.
            return retVal;
        }

        static void SanityCheckFunctionName(string name)
        {
            // Function name must start with [a-zA-Z].
            if ("abcdefghijklmnopqrstuvwxyz".IndexOf(char.ToLower(name[0])) == -1)
                throw new PoeticParsingException($"Function {name} did not start with a-z");
        }
    }
}
