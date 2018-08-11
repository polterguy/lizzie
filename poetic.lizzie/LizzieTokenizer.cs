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
    public class LizzieTokenizer : ITokenizer
    {
        /*
         * Occassionally we need to read more than one token ahead, at which point we
         * store these tokens in this stack.
         */
        Stack<string> _cachedTokens = new Stack<string>();

        /// <summary>
        /// Interface implementation returning next token.
        /// </summary>
        /// <returns>The next token from the given stream reader.</returns>
        /// <param name="reader">Reader to retrieve next token from.</param>
        public string Next (StreamReader reader)
        {
            // Checking if we have cached tokens.
            if (_cachedTokens.Count > 0)
                return _cachedTokens.Pop(); // Returning cached token and popping it off our stack.

            // Eating white space from stream.
            Tokenizer.EatSpace(reader);
            if (reader.EndOfStream)
                return null; // No more tokens.

            // Finding next token from reader.
            string retVal = null;
            while (!reader.EndOfStream) {

                // Peeking next character in stream, and checking its classification.
                var ch = (char)reader.Peek();
                switch(ch) {

                    /*
                     * End of token characters.
                     */

                    case ' ':
                    case '\r':
                    case '\n':
                    case '\t':

                        /*
                         * This is the end of our token.
                         *
                         * Notice, since we start out method by eating white space,
                         * purely logically "retVal" must contain something now.
                         */
                        return retVal;

                    /*
                     * Single character tokens.
                     */

                    case ';':
                    case ',':
                    case '(':
                    case ')':
                    case '[':
                    case ']':
                    case '{':
                    case '}':
                    case ':':
                    case '%':
                    case '*':

                        if (retVal == null) {

                            // This is our token.
                            return ((char)reader.Read()).ToString();

                        } else {

                            // This is the end of our token.
                            return retVal;
                        }

                    /*
                     * Either one or two character tokens.
                     */

                    case '>':

                        if (retVal != null)
                            return retVal; // This is our token.
                        reader.Read(); // Skipping '>'
                        if ((char)reader.Peek() == '=') {
                            reader.Read();
                            return ">=";
                        } else {
                            return ">";
                        }

                    case '<':

                        if (retVal != null)
                            return retVal; // This is our token.
                        reader.Read(); // Skipping '<'
                        if ((char)reader.Peek() == '=') {
                            reader.Read();
                            return "<=";
                        } else {
                            return "<";
                        }

                    case '!':

                        if (retVal != null)
                            return retVal; // This is our token.
                        reader.Read(); // Skipping '!'
                        if ((char)reader.Peek() == '=') {
                            reader.Read();
                            return "!=";
                        } else {
                            return "!";
                        }

                    case '=':

                        if (retVal != null)
                            return retVal; // This is our token.
                        reader.Read(); // Skipping '='
                        if ((char)reader.Peek() == '=') {
                            reader.Read();
                            return "==";
                        } else {
                            return "=";
                        }

                    case '-':

                        if (retVal != null)
                            return retVal; // This is our token.
                        reader.Read(); // Skipping '-'
                        var tmp = (char)reader.Peek();
                        if (tmp == '=') {
                            reader.Read();
                            return "-=";
                        } else if (tmp == '-') {
                            reader.Read();
                            return "--";
                        } else {
                            return "-";
                        }

                    case '+':

                        if (retVal != null)
                            return retVal; // This is our token.
                        reader.Read(); // Skipping '-'
                        tmp = (char)reader.Peek();
                        if (tmp == '=') {
                            reader.Read();
                            return "+=";
                        } else if (tmp == '+') {
                            reader.Read();
                            return "++";
                        } else {
                            return "+";
                        }

                    /*
                     * For sure two character tokens.
                     * 
                     * Notice, we don't have any single character tokens (bit manipulations, etc).
                     */

                    case '&':
                        reader.Read(); //  Skipping '&'.
                        if ((char)reader.Read() != '&')
                            throw new PoeticTokenizerException("Missing & after &.");
                        return "&&";

                    case '|':
                        reader.Read(); //  Skipping '&'.
                        if ((char)reader.Read() != '|')
                            throw new PoeticTokenizerException("Missing | after |.");
                        return "||";

                    /*
                     * String literal token.
                     */

                    case '"':

                        reader.Read(); //  Skipping '"'.
                        var strLiteral = Tokenizer.ReadString(reader);

                        /*
                         * This time we need to cache our tokens for future invocations.
                         * 
                         * Notice, we need to push tokens in reversed order (LIFO).
                         */
                        _cachedTokens.Push("\"");
                        _cachedTokens.Push(strLiteral);
                        return "\"";

                    case '\'':

                        reader.Read(); //  Skipping '\''.
                        strLiteral = Tokenizer.ReadString(reader, '\'');

                        /*
                         * This time we need to cache our tokens for future invocations.
                         * 
                         * Notice, we need to push tokens in reversed order (LIFO).
                         */
                        _cachedTokens.Push("'");
                        _cachedTokens.Push(strLiteral);
                        return "'";

                    /*
                     * Possibly some sort of comment, but can also be a math operator.
                     */

                    case '/':

                        reader.Read(); //  Skipping initial '/'.
                        ch = (char)reader.Peek();
                        if (ch == '/') {

                            /*
                             * Single line comment.
                             */
                            var hasMore = Tokenizer.EatLine(reader);
                            if (!string.IsNullOrEmpty(retVal)) {
                                return retVal;
                            } else if (hasMore) {
                                return Next(reader);
                            }
                            return null; // No more tokens in stream.

                        } else if (ch == '*') {

                            /*
                             * Multiline comment
                             */
                            reader.Read(); // Ignoring '*' character.
                            Tokenizer.EatUntil(reader, "*/");
                            if (!string.IsNullOrEmpty(retVal)) {
                                return retVal;
                            } else {
                                return Next(reader);
                            }

                        } else {

                            // Math token.
                            if (!string.IsNullOrEmpty(retVal)) {
                                _cachedTokens.Push("/"); // Need to store initial '/' token for the next invocation.
                                return retVal;
                            }
                            return "/";
                        }

                    /*
                     * Possibly a decimal separator for a number, but can also be a token by itself.
                     */

                    case '.':

                        if (!string.IsNullOrEmpty(retVal)) {

                            /*
                             * Checking if what we are parsing is a number, and if so,
                             * we assume this is its decimal separator.
                             */
                            if ("0123456789".IndexOf(retVal[0]) == -1)
                                return retVal;
                            retVal += (char)reader.Read(); // Decimal separator.

                        } else {

                            // The '.' is our next token.
                            reader.Read(); // Discarding '.' character.
                            return ".";
                        }
                        break;


                    default:

                        // Eating next character, and appending to retVal.
                        retVal += (char)reader.Read();
                        break;
                }
            }
            return retVal;
        }
    }
}
