/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System.IO;

namespace lizzie
{
    /// <summary>
    /// Common tokenizer interface, in case you want to implement your own tokenizer,
    /// and override the default implementation, that expects Lizzie code.
    /// 
    /// If you do, you'll probably also want to implement your own Compiler class.
    /// </summary>
    public interface ITokenizer
    {
        /// <summary>
        /// Returns the next token available from the specified reader.
        /// </summary>
        /// <returns>The next token.</returns>
        /// <param name="reader">Reader to read tokens from.</param>
        string Next(StreamReader reader);
    }
}
