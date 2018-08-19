/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

namespace lizzie.exceptions
{
    /// <summary>
    /// Runtime execution exception thrown by Lizzie, when some severe error
    /// occurs during runtime, from which Lizzie cannot recover from.
    /// </summary>
    public class LizzieRuntimeException : LizzieException
    {
        /// <summary>
        /// Creates a new exception with the specified message.
        /// </summary>
        /// <param name="message">Message containing more information about the exception.</param>
        public LizzieRuntimeException(string message)
            : base(message)
        { }
    }
}
