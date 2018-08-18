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
using System.Collections.Generic;
using lizzie.types;
using lizzie.exceptions;

namespace lizzie
{
    public static class Compiler
    {
        public static Func<TContext, Binder<TContext>, object> Compile<TContext>(Tokenizer tokenizer, Stream stream)
        {
            return Compile<TContext>(tokenizer.Tokenize(stream));
        }

        public static Func<TContext, Binder<TContext>, object> Compile<TContext>(Tokenizer tokenizer, IEnumerable<Stream> streams)
        {
            return Compile<TContext>(tokenizer.Tokenize(streams));
        }

        public static Func<TContext, Binder<TContext>, object> Compile<TContext>(Tokenizer tokenizer, string snippet)
        {
            return Compile<TContext>(tokenizer.Tokenize(snippet));
        }

        public static Func<TContext, Binder<TContext>, object> Compile<TContext>(Tokenizer tokenizer, IEnumerable<string> snippets)
        {
            return Compile<TContext>(tokenizer.Tokenize(snippets));
        }

        static Func<TContext, Binder<TContext>, object> Compile<TContext>(IEnumerable<string> tokens)
        {
            // Retrieving enumerator containing tokens.
            var en = tokens.GetEnumerator();

            // Sanity checking code.
            if (!en.MoveNext()) // Moving inside of body.
                throw new LizzieParsingException("No code was given to compile.");

            // Compiling main body of code, not expecting braces ('{}'), since this is the root level.
            var functions = Body<TContext>.Compile(en, false);

            // Creating a function wrapping evaluation of all of our root level functions in body.
            return new Func<TContext, Binder<TContext>, object>((ctx, binder) => {
                foreach (var ix in functions) {
                    ix(ctx, binder, null);
                }
                return null;
            });
        }
    }
}
