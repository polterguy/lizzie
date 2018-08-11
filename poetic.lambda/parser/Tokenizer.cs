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
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using poetic.lambda.exceptions;

namespace poetic.lambda.parser
{
    /// <summary>
    /// Tokenizer class producing tokens from a stream or string input.
    /// </summary>
    public class Tokenizer
    {
        readonly ITokenizer _tokenizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.parser.Tokenizer"/> class.
        /// </summary>
        /// <param name="tokenizer">Tokenizer to use for tokenizing snippets.</param>
        public Tokenizer(ITokenizer tokenizer)
        {
            // Not passing in a tokenizer is a logical runtime error!
            _tokenizer = tokenizer ?? throw new NullReferenceException(nameof(tokenizer));
        }

        /// <summary>
        /// Returns all tokens from the specified stream.
        /// </summary>
        /// <returns>All tokens from the stream.</returns>
        /// <param name="stream">Stream to retrieve tokens from.</param>
        public IEnumerable<string> Tokenize(Stream stream)
        {
            // Notice! We do NOT take ownership over stream!
            StreamReader reader = new StreamReader(stream);
            while (true) {
                var token = _tokenizer.Next(reader);
                if (token == null)
                    break;
                yield return token;
            }
            yield break;
        }

        /// <summary>
        /// Returns all tokens from all streams specified.
        /// </summary>
        /// <returns>The tokens from all streams.</returns>
        /// <param name="streams">Streams to retrieve tokens from.</param>
        public IEnumerable<string> Tokenize(IEnumerable<Stream> streams)
        {
            foreach (var ixStream in streams) {
                foreach (var ixToken in Tokenize(ixStream)) {
                    yield return ixToken;
                }
            }
        }

        /// <summary>
        /// Tokenizes the specified code
        /// </summary>
        /// <returns>The tokens from the given code.</returns>
        /// <param name="code">Code to retrieve tokens from.</param>
        public IEnumerable<string> Tokenize(string code)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(code))) {
                foreach (var ix in Tokenize(stream)) {
                    yield return ix;
                }
            }
        }

        /// <summary>
        /// Tokenizes the specified code snippets.
        /// </summary>
        /// <returns>The tokens retrieved from all the specified code snippets.</returns>
        /// <param name="code">Code snippets to retrieve tokens from.</param>
        public IEnumerable<string> Tokenize(IEnumerable<string> code)
        {
            foreach (var ixCode in code) {
                foreach (var ixToken in Tokenize(ixCode)) {
                    yield return ixToken;
                }
            }
        }

        /// <summary>
        /// Eats initial whitespace from reader.
        /// </summary>
        /// <returns><c>true</c>, if spacing characters were encountered, <c>false</c> otherwise.</returns>
        /// <param name="reader">Reader to eat from.</param>
        public static bool EatSpace(StreamReader reader)
        {
            var retVal = false;
            while (!reader.EndOfStream) {
                var ch = (char)reader.Peek();
                if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n') {
                    reader.Read();
                    retVal = true;
                } else {
                    break;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Eats the rest of the line.
        /// </summary>
        /// <returns><c>true</c>, if characters were encountered before the end of the line, <c>false</c> otherwise.</returns>
        /// <param name="reader">Reader to eat from.</param>
        public static bool EatLine(StreamReader reader)
        {
            var retVal = false;
            while(!reader.EndOfStream) {
                var ch = (char)reader.Read();
                if (ch == '\r' || ch == '\n') {
                    if (!reader.EndOfStream) {
                        ch = (char)reader.Peek();
                        if (ch == '\r' || ch == '\n')
                            reader.Read();
                    }
                    break;
                } else {
                    retVal = true;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Eats until the sequence of characters in found consecutively in stream.
        /// 
        /// Useful for discarding multiline comments among other things.
        /// </summary>
        /// <param name="reader">Reader.</param>
        /// <param name="sequence">Sequence.</param>
        public static void EatUntil(StreamReader reader, string sequence)
        {
            if (string.IsNullOrEmpty(sequence))
                throw new PoeticTokenizerException("Can't read until empty sequence is found");
            var first = sequence[0];
            var buffer = "";
            while (true) {
                var ix = reader.Read();
                if (ix == -1)
                    return;
                if (buffer != "") {

                    // Seen first character.
                    buffer += (char)ix;
                    if (buffer == sequence) {
                        return; // Found sequence
                    } else if (buffer.Length == sequence.Length) {
                        buffer = ""; // Throwing away matches found so far, starting over again.
                    }
                } else {
                    if (first == (char)ix) {

                        // Beginning of sequence.
                        buffer += (char)ix;
                        if (buffer == sequence) {
                            return; // Sequence found
                        } else if (buffer.Length == sequence.Length) {
                            buffer = ""; // Throwing away matches found so far, starting over again.
                            if (first == (char)ix) {
                                buffer += (char)ix; // This is the opening token for sequence.
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if next character in stream is whitespace character,
        /// or EOF has been reached.
        /// </summary>
        /// <returns><c>true</c>, if next is space, <c>false</c> otherwise.</returns>
        /// <param name="reader">Reader.</param>
        public static bool NextIsWhiteSpace(StreamReader reader)
        {
            return NextIsOf(reader, ' ', '\t', '\r', '\n');
        }

        /// <summary>
        /// Returns true if next character in stream is any of the specified characters,
        /// or EOF has been reached.
        /// </summary>
        /// <returns><c>true</c>, if next is one of specified characters, <c>false</c> otherwise.</returns>
        /// <param name="reader">Reader.</param>
        /// <param name="characters">Characters.</param>
        public static bool NextIsOf (StreamReader reader, params char[] characters)
        {
            var ch = (char)reader.Peek();
            return characters.Any((ix) => ix == ch);
        }

        /// <summary>
        /// Reads the specified string from the reader and returns it to caller.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="reader">Reader.</param>
        public static string ReadString (StreamReader reader, char stop = '"')
        {
            var builder = new StringBuilder ();
            for (var c = reader.Read (); c != -1; c = reader.Read ()) {
                switch (c) {
                    case '\\':
                        builder.Append (GetEscapeCharacter (reader, stop));
                        break;
                    case '\n':
                    case '\r':
                        throw new PoeticTokenizerException (string.Format ("String literal container CR or LF character."));
                    default:
                        if (c == stop)
                            return builder.ToString();
                        builder.Append ((char)c);
                        break;
                }
            }
            throw new ApplicationException (string.Format ("Syntax error, string literal not closed before end of input near '{0}'", builder));
        }

        /*
         * Returns escape character.
         */
        static string GetEscapeCharacter (StreamReader reader, char stop)
        {
            var ch = reader.Read();
            if (ch == -1)
                throw new PoeticTokenizerException("EOF found before string literal was closed");
            switch ((char)ch) {
                case '\\':
                    return "\\";
                case 'a':
                    return "\a";
                case 'b':
                    return "\b";
                case 'f':
                    return "\f";
                case 't':
                    return "\t";
                case 'v':
                    return "\v";
                case 'n':
                    return "\n";
                case 'r':
                    return "\r";
                case 'x':
                    return HexCharacter (reader);
                default:
                    if (ch == stop)
                        return stop.ToString();
                    throw new PoeticTokenizerException ("Invalid escape sequence found in string literal");
            }
        }

        /*
         * Returns hex encoded character.
         */
        static string HexCharacter(StreamReader reader)
        {
            var hexNumberString = "";
            for (var idxNo = 0; idxNo < 4; idxNo++)
                hexNumberString += (char)reader.Read();
            var integerNo = Convert.ToInt32(hexNumberString, 16);
            return Encoding.UTF8.GetString(BitConverter.GetBytes(integerNo).Reverse().ToArray());
        }
    }
}
