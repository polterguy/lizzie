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
using poetic.lambda.parser;

namespace poetic.lizzie
{
    /// <summary>
    /// Tokenizer for Lizzie.
    /// </summary>
    public class LizzieTokenizer : ITokenizer
    {
        /// <summary>
        /// Interface implementation returning next token.
        /// </summary>
        /// <returns>The next token from the given stream reader.</returns>
        /// <param name="reader">Reader to retrieve next token from.</param>
        public string Next (StreamReader reader)
        {
            // Eating non tokens from reader.
            EatNonTokens(reader);

            // Finding next token from reader.
            string retVal = null;
            while (!reader.EndOfStream) {
                var ch = (char)reader.Peek();
                switch(ch) {
                    case ' ':
                    case '\r':
                    case '\n':
                    case '\t':

                        // This is the end of our token.
                        return retVal;

                    case ',':
                    case '(':
                    case ')':
                    case '[':
                    case ']':
                    case '{':
                    case '}':
                    case ':':
                        if (retVal == null) {

                            // This is our token.
                            reader.Read();
                            return ch.ToString();

                        } else {

                            // This is the end of our token.
                            return retVal;
                        }
                    default:

                        // Eating next character, and appending to retVal.
                        retVal += (char)reader.Read();
                        break;
                }
            }
            return retVal;
        }

        /*
         * Eats spacing and comments from stream.
         */
        private void EatNonTokens(StreamReader reader)
        {
            while (!reader.EndOfStream && Tokenizer.EatSpace(reader) /* TODO: Implement 'EatComments' */) {}
        }
    }
}
