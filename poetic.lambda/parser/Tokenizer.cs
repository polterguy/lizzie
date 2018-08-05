﻿/*
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
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections;

namespace poetic.lambda.parser
{
    /// <summary>
    /// Tokenizer class producing tokens from a stream or string input.
    /// 
    /// Implement your own ITokenizer instance and pass in to CTOR as argument
    /// to create your own DSL.
    /// </summary>
    public class Tokenizer : IEnumerable<string>
    {
        StreamReader _reader;
        ITokenizer _tokenizer;
        List<string> _tokens;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.parser.Tokenizer"/> class.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="tokenizer">Tokenizer.</param>
        public Tokenizer(Stream stream, ITokenizer tokenizer)
        {
            // Sanity check.
            if (stream == null)
                throw new NullReferenceException(nameof(stream));

            /*
             * Notice, we don't take ownership over stream, so caller must make
             * sure the stream is disposed!
             */
            _reader = new StreamReader(stream);
            _tokenizer = tokenizer ?? throw new NullReferenceException(nameof(tokenizer));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.parser.Tokenizer"/> class.
        /// </summary>
        /// <param name="code">Code.</param>
        /// <param name="tokenizer">Tokenizer.</param>
        public Tokenizer(string code, ITokenizer tokenizer)
        {
            // Sanity check.
            if (string.IsNullOrEmpty(code))
                throw new NullReferenceException(nameof(code));

            // MemoryStream doesn't need to be disposed!
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
            _reader = new StreamReader(stream);
            _tokenizer = tokenizer ?? throw new NullReferenceException(nameof(tokenizer));
        }

        /// <summary>
        /// Eats initial whitespace from reader.
        /// </summary>
        /// <param name="reader">Reader.</param>
        public static void EatSpace(StreamReader reader)
        {
            while (!reader.EndOfStream) {
                var ch = (char)reader.Peek();
                if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n') {
                    reader.Read();
                } else {
                    break;
                }
            }
        }

        /// <summary>
        /// Returns true if next character in stream is whitespace character,
        /// or EOF has been reached.
        /// </summary>
        /// <returns><c>true</c>, if is space or EOF was nexted, <c>false</c> otherwise.</returns>
        /// <param name="reader">Reader.</param>
        public static bool NextIsWhiteSpace(StreamReader reader)
        {
            return NextIsOf(reader, ' ', '\t', '\r', '\n');
        }

        /// <summary>
        /// Returns true if next character in stream is any of the specified characters,
        /// or EOF has been reached.
        /// </summary>
        /// <returns><c>true</c>, if if next is was ended, <c>false</c> otherwise.</returns>
        /// <param name="reader">Reader.</param>
        /// <param name="characters">Characters.</param>
        public static bool NextIsOf (StreamReader reader, params char[] characters)
        {
            var ch = (char)reader.Peek();
            return characters.Any((ix) => ix == ch);
        }

        public IEnumerator<string> GetEnumerator()
        {
            if (_tokens == null)
                Tokenize();
            return _tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /*
         * Helper method to ensure we have retrieved all our tokens.
         */
        private void Tokenize()
        {
            _tokens = new List<string>();
            while (true) {
                var token = _tokenizer.Next(_reader);
                if (token == null)
                    break;
                _tokens.Add(token);
            }
        }
    }
}
