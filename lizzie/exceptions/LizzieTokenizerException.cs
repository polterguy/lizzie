/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

namespace lizzie.exceptions
{
    /// <summary>
    /// Tokenizer exception when the tokenizer could not for some reasons tokenize
    /// your code.
    /// </summary>
    public class LizzieTokenizerException : LizzieException
    {
        /// <summary>
        /// Creates a new exception with the specified message.
        /// </summary>
        /// <param name="message">Message containing more information about the exception.</param>
        public LizzieTokenizerException(string message)
            : base(message)
        { }
    }
}
