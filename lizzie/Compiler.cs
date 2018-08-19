/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using lizzie.types;

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
            // Compiling main body of code, not expecting braces ('{}'), since this is the root level.
            var tuples = Body<TContext>.Compile(tokens.GetEnumerator(), false);
            var functions = tuples.Item1;

            // Creating a function wrapping evaluation of all of our root level functions in body.
            return new Func<TContext, Binder<TContext>, object>((ctx, binder) => {
                object result = null;
                foreach (var ix in functions) {
                    result = ix(ctx, binder, null);
                }
                return result;
            });
        }
    }
}
