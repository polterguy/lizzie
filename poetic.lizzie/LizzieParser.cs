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

using System.IO;
using System.Collections.Generic;
using poetic.lambda.parser;
using poetic.lambda.collections;

namespace poetic.lizzie
{
    /// <summary>
    /// Lizzie parser that creates a Lizzie execution object to be evaluated as
    /// a function.
    /// </summary>
    public class LizzieParser<TContext>
    {
        // Binder for this instance.
        readonly Binder<TContext> _binder = new Binder<TContext>();

        // Which keywords to use.
        readonly LizzieKeywords<TContext> _keywords;

        public LizzieParser(LizzieKeywords<TContext> keywords = null)
        {
            /*
             * If no explicit keywords override have been supplied, we use the default
             * CTOR, which will populate our keywords dictionary with the default Lizzie
             * keywords.
             */
            _keywords = keywords ?? new LizzieKeywords<TContext>();
        }

        /// <summary>
        /// Parses the code in the stream, using the tokenizer, and returns a function
        /// to caller.
        /// </summary>
        /// <returns>The function object being the result of the parse operation.</returns>
        /// <param name="tokenizer">Tokenizer to use.</param>
        /// <param name="stream">Stream containing your code.</param>
        public Actions<FunctionStack<TContext>> Parse(Tokenizer tokenizer, Stream stream)
        {
            return Parse(tokenizer.Tokenize(stream));
        }

        /// <summary>
        /// Parses the code in all streams, using the tokenizer, and returns a function
        /// to caller.
        /// </summary>
        /// <returns>The function object being the result of the parse operation.</returns>
        /// <param name="tokenizer">Tokenizer to use.</param>
        /// <param name="streams">Streams containing your code.</param>
        public Actions<FunctionStack<TContext>> Parse(Tokenizer tokenizer, IEnumerable<Stream> streams)
        {
            return Parse(tokenizer.Tokenize(streams));
        }

        /// <summary>
        /// Parses the specified code, using the tokenizer, and returns a function
        /// to caller.
        /// </summary>
        /// <returns>The function object being the result of the parse operation.</returns>
        /// <param name="tokenizer">Tokenizer to use.</param>
        /// <param name="code">The code you wish to parse.</param>
        public Actions<FunctionStack<TContext>> Parse(Tokenizer tokenizer, string code)
        {
            return Parse(tokenizer.Tokenize(code));
        }

        /// <summary>
        /// Parses the specified code snippets, using the tokenizer, and returns a function
        /// to caller.
        /// </summary>
        /// <returns>The function object being the result of the parse operation.</returns>
        /// <param name="tokenizer">Tokenizer to use.</param>
        /// <param name="code">Snippets of code you wish to create an execution object out of.</param>
        public Actions<FunctionStack<TContext>> Parse(Tokenizer tokenizer, IEnumerable<string> code)
        {
            return Parse(tokenizer.Tokenize(code));
        }

        /*
         * Parses the code in the specified enumerator and returns a function to caller.
         */
        Actions<FunctionStack<TContext>> Parse(IEnumerable<string> tokens)
        {
            /*
             * Creating our actions that will be the actual content of our lambda
             * function object.
             */
            var actions = new Actions<FunctionStack<TContext>>();

            /*
             * Creating our actual actions, which are a bunch of dynamically
             * created delegates, created according to what the parser finds in
             * our code.
             */
            var en = tokens.GetEnumerator();
            while (en.MoveNext()) {
                var statement = StatementParser<TContext>.Create(_keywords, en);
                actions.Add(statement);
            }

            // Creating our root level function object and returning it to caller.
            return actions;
        }
    }
}
