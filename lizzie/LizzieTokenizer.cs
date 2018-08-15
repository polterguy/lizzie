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

namespace lizzie
{
    public class LizzieTokenizer : ITokenizer
    {
        /*
         * Occassionally we need to read more than one token ahead, at which point we
         * store these tokens in this stack.
         */
        Stack<string> _cachedTokens = new Stack<string>();

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

                    case '\'':
                    case '(':
                    case ')':

                        if (retVal == null) {

                            // This is our token.
                            return ((char)reader.Read()).ToString();

                        } else {

                            // This is the end of our token.
                            return retVal;
                        }

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

                    /*
                     * Single line comment token.
                     */

                    case ';':

                        Tokenizer.EatLine(reader);
                        if (retVal != null)
                            return retVal;
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
