/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

namespace lizzie.exceptions
{
    /// <summary>
    /// Parsing exception thrown when parsing of your Lizzie code could not be
    /// accomplished for some reasons.
    /// </summary>
    public class LizzieParsingException : LizzieException
    {
        /// <summary>
        /// Creates a new exception with the specified message.
        /// </summary>
        /// <param name="message">Message containing more information about the exception.</param>
        public LizzieParsingException(string message)
            : base(message)
        { }
    }
}
