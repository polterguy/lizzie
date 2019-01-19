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
    /// <summary>
    /// The main Lizzie tokenizer, that will tokenize Lizzie code, which you can
    /// then use as input for the compiler that compiles a lambda object from your
    /// code.
    /// </summary>
    public class LizzieTokenizer : ITokenizer
    {
        /*
         * Occassionally we might need to read more than one token ahead, at which point we
         * store these tokens in this stack, and returns them once the next token is requested.
         */
        readonly Stack<string> _cachedTokens = new Stack<string>();

        /// <summary>
        /// Gets or sets the maximum size of strings the tokenizer will allow  before throwing an exception.
        /// </summary>
        /// <value>The size of the max string.</value>
        public int MaxStringSize { get; set; } = -1;

        #region [ -- Interface implementation -- ]

        /// <summary>
        /// Retrieves the next token from the specified reader.
        /// </summary>
        /// <returns>The next token from the reader.</returns>
        /// <param name="reader">Reader to retrieve token from.</param>
        public string Next(StreamReader reader)
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
                switch (ch) {
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
                    case '(':
                    case ')':
                    case '{':
                    case '}':

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
                    case '\'':

                        reader.Read(); //  Skipping '"'.
                        var strLiteral = Tokenizer.ReadString(reader, ch, MaxStringSize);

                        /*
                         * This time we need to cache our tokens for future invocations.
                         * 
                         * Notice, we need to push tokens in reversed order (LIFO).
                         */
                        _cachedTokens.Push(ch == '\'' ? "'" : "\"");
                        _cachedTokens.Push(strLiteral);
                        return ch == '\'' ? "'" : "\"";

                    /*
                     * Possible single line comment token.
                     */

                    case '/':

                        reader.Read(); // Discarding "/" first.
                        ch = (char)reader.Peek();
                        if (ch == '/') {

                            // Single line comment.
                            Tokenizer.EatLine(reader);

                            // There might be some spaces at the front of our stream now ...
                            Tokenizer.EatSpace(reader);

                            // Checking if we currently have a token.
                            if (retVal != null)
                                return retVal;

                        } else if (ch == '*') {

                            // Multiline comment, making sure we discard opening "*" character from stream.
                            reader.Read();
                            Tokenizer.EatUntil(reader, "*/", true);

                            // There might be some spaces at the front of our stream now ...
                            Tokenizer.EatSpace(reader);

                            // Checking if we currently have a token.
                            if (!string.IsNullOrEmpty(retVal))
                                return retVal;

                        } else {

                            // Returning '/' as a token.
                            return "/";
                        }
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

        #endregion
    }
}
