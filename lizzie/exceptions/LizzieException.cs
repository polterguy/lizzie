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
    /// 
    /// NOTICE!
    /// Lizzie might throw non-Lizzie exceptions, during tokenization, compilation,
    /// and evaluation - Since it can be given bad data, which results in that
    /// a .Net exception is thrown instead. Such as when for instance your binded
    /// object throws exceptions, which Lizzie doesn't prevent, and should not
    /// prevent either. So you should also expect non-Lizzie exeptions in your
    /// code that evaluates Lizzie code.
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
