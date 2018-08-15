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
using lizzie.types;

namespace lizzie
{
    public class Parser
    {
        public LizzieSequence Parse(Tokenizer tokenizer, Stream stream)
        {
            return Parse(tokenizer.Tokenize(stream));
        }

        public LizzieSequence Parse(Tokenizer tokenizer, IEnumerable<Stream> streams)
        {
            return Parse(tokenizer.Tokenize(streams));
        }

        public LizzieSequence Parse(Tokenizer tokenizer, string snippet)
        {
            return Parse(tokenizer.Tokenize(snippet));
        }

        public LizzieSequence Parse(Tokenizer tokenizer, IEnumerable<string> snippets)
        {
            return Parse(tokenizer.Tokenize(snippets));
        }

        public LizzieSequence Parse(IEnumerable<string> tokens)
        {
            var result = new LizzieSequence();
            var en = tokens.GetEnumerator();
            while (en.MoveNext()) {
                result.Add(LizzieType.Create(en));
            }
            return result;
        }
    }
}
