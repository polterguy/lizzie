/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System;

namespace lizzie.exceptions
{
    /// <summary>
    /// Base exception class for all exceptions thrown by Lizzie.
    /// </summary>
    public class LizzieException : Exception
    {
        /// <summary>
        /// Creates a new exception with the specified message.
        /// </summary>
        /// <param name="message">Message containing more information about the exception.</param>
        public LizzieException(string message)
            : base(message)
        { }
    }
}
