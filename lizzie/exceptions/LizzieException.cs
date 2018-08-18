/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System;

namespace lizzie.exceptions
{
    public class LizzieException : Exception
    {
        public LizzieException(string message)
            : base(message)
        { }
    }
}
