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
using poetic.lambda.parser;
using poetic.lambda.collections;

namespace poetic.lizzie
{
    public class Function<TContext>
    {
        readonly Keywords<TContext> _keywords;

        public Function(Keywords<TContext> keywords = null)
        {
            _keywords = keywords ?? new Keywords<TContext>();
        }

        public Func<TContext, Arguments, Binder<TContext>, object> Parse(lambda.parser.Tokenizer tokenizer, Stream stream)
        {
            return Parse(tokenizer.Tokenize(stream));
        }

        public Func<TContext, Arguments, Binder<TContext>, object> Parse(lambda.parser.Tokenizer tokenizer, IEnumerable<Stream> streams)
        {
            return Parse(tokenizer.Tokenize(streams));
        }

        public Func<TContext, Arguments, Binder<TContext>, object> Parse(lambda.parser.Tokenizer tokenizer, string code)
        {
            return Parse(tokenizer.Tokenize(code));
        }

        public Func<TContext, Arguments, Binder<TContext>, object> Parse(lambda.parser.Tokenizer tokenizer, IEnumerable<string> code)
        {
            return Parse(tokenizer.Tokenize(code));
        }

        Func<TContext, Arguments, Binder<TContext>, object> Parse(IEnumerable<string> tokens)
        {
            // All statements in Lizzie are functions with the exact same signature.
            var functions = new List<Func<TContext, Arguments, Binder<TContext>, object>>();

            // Iterating as long as we have more statements.
            var en = tokens.GetEnumerator();
            while (en.MoveNext()) {

                // Checking if this is a keyword.
                if (_keywords.HasKeyword(en.Current)) {

                    // Finding keyword parser.
                    var keywordParser = _keywords[en.Current];

                    // Using keyword parser to parse keyword and add to our list of functions.
                    functions.Add(keywordParser(en));

                } else {

                    // Function invocation of some sort.
                    functions.Add(FunctionInvocation<TContext>.Create(en));
                }
            }

            // Creating our root level function object and returning it to caller.
            return new Func<TContext, Arguments, Binder<TContext>, object>(delegate (TContext ctx, Arguments args, Binder<TContext> binder) {

                // Iterating through each statement and evaluating it.
                // TODO: Make sure the "return" keyword can stop execution!
                object result = null;
                foreach (var ix in functions) {
                    result = ix(ctx, args, binder);
                }
                return result;
            });
        }
    }
}
