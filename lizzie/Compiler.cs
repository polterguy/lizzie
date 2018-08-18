/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
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
