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
    /// If you implement your own tokenizer and compiler, you can still take
    /// advantage of the helper methods found in the generic Tokenizer class.
    /// </summary>
    public interface ITokenizer
    {
        /// <summary>
        /// Returns the next token available from the specified reader.
        ///
        /// Will return null if no more tokens are found, and EOF have been
        /// encountered.
        /// </summary>
        /// <returns>The next token found in the reader, if any.</returns>
        /// <param name="reader">Reader to read tokens from.</param>
        string Next(StreamReader reader);
    }
}
