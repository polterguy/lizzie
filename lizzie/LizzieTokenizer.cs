/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System.IO;
using System.Collections.Generic;
using lizzie.exceptions;

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
                     * Reserved characters to make it possible to expand syntax in future releases,
                     * without making old code incompatible.
                     */

                    case '[':
                    case ']':
                    case '*':
                    case '?':
                    case '#':
                    case '$':

                        throw new LizzieTokenizerException($"'{ch}' is a reserved character for future addons.");

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

                    case '@':
                    case ',':
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
                     * Possible single line comment token.
                     */

                    case '/':

                        reader.Read(); // Discarding first "/".
                        ch = (char)reader.Peek();
                        if (ch == '/'){

                            // Single line comment.
                            Tokenizer.EatLine(reader);
                            if (retVal != null)
                                return retVal;

                        } else if (ch == '*') {

                            // Multiline comment.
                            Tokenizer.EatUntil(reader, "*/");
                        }

                        // There might be some spaces at the front of our stream now ...
                        Tokenizer.EatSpace(reader);
                        break;

                    /*
                     * Default, simply appending character to token buffer.
                     */

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
