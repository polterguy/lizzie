/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System.IO;

namespace lizzie
{
    public interface ITokenizer
    {
        string Next(StreamReader reader);
    }
}
