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
using poetic.lambda.collections;
using poetic.lambda.parser;

namespace poetic.lizzie
{
    /// <summary>
    /// Convenience class for creating Lizzie execution lambda objects.
    /// </summary>
    public static class LambdaBuilder<TContext> where TContext : class
    {
        /// <summary>
        /// Creates and returns a Lizzie execution lambda object from the given stream.
        /// </summary>
        /// <returns>The lambda execution object.</returns>
        /// <param name="stream">Stream containing your Lizzie code.</param>
        public static Actions<TContext> Build(Stream stream)
        {
            var tokenizer = new Tokenizer(stream, new LizzieTokenizer());
            var parser = new LizzieParser<TContext>();
            return parser.Parse(tokenizer);
        }

        /// <summary>
        /// Creates and returns a Lizzie execution lambda object from the given code.
        /// </summary>
        /// <returns>The lambda execution object.</returns>
        /// <param name="code">Lizzie code.</param>
        public static Actions<TContext> Build(string code)
        {
            var tokenizer = new Tokenizer(code, new LizzieTokenizer());
            var parser = new LizzieParser<TContext>();
            return parser.Parse(tokenizer);
        }
    }
}
