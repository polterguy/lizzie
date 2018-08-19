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
    /// <summary>
    /// Class responsible for compiling Lizzie code, and create a Lambda object
    /// out of it, which you can evaluate from your CLR code.
    /// </summary>
    public static class Compiler
    {
        /// <summary>
        /// Compile the Lizzie code found in the specified stream.
        /// </summary>
        /// <returns>The compiled lambda object.</returns>
        /// <param name="tokenizer">The tokenizer to use.</param>
        /// <param name="stream">Stream containing Lizzie code. Notice, this method does not claim ownership over
        /// your stream, and you are responsible for correctly disposing it yourself</param>
        /// <typeparam name="TContext">The type of your context object.</typeparam>
        public static Lambda<TContext> Compile<TContext>(Tokenizer tokenizer, Stream stream)
        {
            return Compile<TContext>(tokenizer.Tokenize(stream));
        }

        /// <summary>
        /// Compile the Lizzie code found in the specified streams.
        /// </summary>
        /// <returns>The compiled lambda object.</returns>
        /// <param name="tokenizer">The tokenizer to use.</param>
        /// <param name="streams">Streams containing Lizzie code. Notice, this method does not claim ownership over
        /// your streams, and you are responsible for correctly disposing your streams yourself</param>
        /// <typeparam name="TContext">The type of your context object.</typeparam>
        public static Lambda<TContext> Compile<TContext>(Tokenizer tokenizer, IEnumerable<Stream> streams)
        {
            return Compile<TContext>(tokenizer.Tokenize(streams));
        }

        /// <summary>
        /// Compile the specified Lizzie code.
        /// </summary>
        /// <returns>The compiled lambda object.</returns>
        /// <param name="tokenizer">The tokenizer to use.</param>
        /// <param name="snippet">Your Lizzie code.</param>
        /// <typeparam name="TContext">The type of your context object.</typeparam>
        public static Lambda<TContext> Compile<TContext>(Tokenizer tokenizer, string snippet)
        {
            return Compile<TContext>(tokenizer.Tokenize(snippet));
        }

        /// <summary>
        /// Compile the specified Lizzie code snippets.
        /// </summary>
        /// <returns>The compiled lambda object.</returns>
        /// <param name="tokenizer">The tokenizer to use.</param>
        /// <param name="snippets">Snippets containing your Lizzie code.</param>
        /// <typeparam name="TContext">The type of your context object.</typeparam>
        public static Lambda<TContext> Compile<TContext>(Tokenizer tokenizer, IEnumerable<string> snippets)
        {
            return Compile<TContext>(tokenizer.Tokenize(snippets));
        }

        /*
         * Common helper method for above methods, that does the heavy lifting,
         * and actually compiles our code down to a lambda object.
         */
        static Lambda<TContext> Compile<TContext>(IEnumerable<string> tokens)
        {
            // Compiling main body of code, not expecting braces ('{}'), since this is the root level.
            var tuples = Body<TContext>.Compile(tokens.GetEnumerator(), false);
            var functions = tuples.Item1;

            /*
             * Creating a function wrapping evaluation of all of our root level functions in body,
             * making sure we always return the result of the last function invocation to caller.
             */
            return new Lambda<TContext>((ctx, binder) => {
                object result = null;
                foreach (var ix in functions) {
                    result = ix(ctx, binder, null);
                }
                return result;
            });
        }
    }
}
