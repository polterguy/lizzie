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
using poetic.lambda.exceptions;

namespace poetic.lizzie
{
    /// <summary>
    /// Tokenizer for Lizzie.
    /// </summary>
    public class Tokenizer<TContext> : ITokenizer
    {
        // Binder to bind methods to functions.
        Binder<TContext> _binder = new Binder<TContext>();

        // Helper to track where in our grammar we are.
        enum Pos {
            Function,
            FunctionName,
            FunctionOpeningParanthesis,
            FunctionArgument,
            FunctionArgumentComma,
            FunctionClosingParanthesis
        };

        Stack<Pos> _position = new Stack<Pos>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lizzie.Tokenizer`1"/> class.
        /// 
        /// If no functionMap is specified, the method name will be used as the function name.
        /// </summary>
        public Tokenizer()
        { }

        /// <summary>
        /// Interface implementation returning next token.
        /// </summary>
        /// <returns>The next token from the given stream reader.</returns>
        /// <param name="reader">Reader.</param>
        public string Next (StreamReader reader)
        {
            // Checking our grammar position.
            if (_position.Count == 0) {

                // Root level, first eating all non-tokens.
                EatNonTokens(reader);

                // Getting word.
                var word = Tokenizer.ReadWord(reader);

                // Sanity checking word.
                if (word != "function") {

                    // At root level we only tolerate "function" keyword.
                    throw new PoeticParsingException("Only 'function' definitions are legal at root level of Lizzie.");
                }

                // Pushing position on stack, and returning token to caller.
                _position.Push(Pos.Function);
                return word;

            } else {

                // Figuring out where we are.
                var pos = _position.Peek();
                switch (pos) {
                    case Pos.Function:

                        // Just saw 'function' keyword, now expecting 'FunctionName'.
                        Tokenizer.EatSpace(reader);
                        var word = Tokenizer.ReadWord(reader);
                        _position.Push(Pos.FunctionName);
                        return word;

                    case Pos.FunctionName:

                        // Just saw 'FunctionName', now expecting '('.
                        Tokenizer.EatSpace(reader);
                        var ch = (char)reader.Read();
                        if (ch != '(')
                            throw new PoeticParsingException("Unexpected token while looking for function paranthesis.");
                        _position.Push(Pos.FunctionOpeningParanthesis);
                        return "(";

                    case Pos.FunctionOpeningParanthesis:

                        // Just saw '(', now expecting word or ')'.
                        Tokenizer.EatSpace(reader);
                        if ((char)reader.Peek() == ')') {

                            // This function does not take any arguments.
                            _position.Pop();
                            _position.Push(Pos.FunctionClosingParanthesis);
                            return ")";
                        }
                        word = Tokenizer.ReadWord(reader);
                        if (string.IsNullOrEmpty (word)) {
                            throw new PoeticParsingException("Error while figuring our name of function.");
                        }
                        _position.Push(Pos.FunctionArgument);
                        return word;

                    case Pos.FunctionArgument:

                        // Just saw argument name, now expecting either ',' or ')'.
                        Tokenizer.EatSpace(reader);
                        ch = (char)reader.Peek();
                        if (ch == ',') {
                            _position.Pop();
                            _position.Push(Pos.FunctionArgumentComma);
                            return ",";
                        } else if (ch == ')') {
                            _position.Pop();
                            _position.Pop();
                            _position.Push(Pos.FunctionClosingParanthesis);
                            return ",";
                        } else {
                            word = Tokenizer.ReadWord(reader);
                        }
                }
            }
        }

        /*
         * Eats spacing and comments from stream.
         */
        private void EatNonTokens(StreamReader reader)
        {
            while (true) {
                if (!reader.EndOfStream && Tokenizer.NextIsWhiteSpace(reader)) {

                    // Eating next white space character(s).
                    Tokenizer.EatSpace(reader);

                } else if (!reader.EndOfStream && Tokenizer.NextIsOf(reader, '/')) {

                    // Comment coming up next,ignoring initial '/'.
                    reader.Read();

                    // Checking type of comment.
                    if (Tokenizer.NextIsOf(reader, '*')) {

                        // Multiline comment.
                        Tokenizer.EatUntil(reader, "*/");

                    } else {

                        // Single line comment.
                        Tokenizer.EatLine(reader);
                    }
                } else {

                    // No more white space or comments.
                    return;
                }
            }
        }
    }
}
