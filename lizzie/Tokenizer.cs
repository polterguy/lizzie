/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using lizzie.exceptions;

namespace lizzie
{
    /// <summary>
    /// Main tokenizer instance, used as input to the compilation process.
    /// 
    /// If you implement your own tokenizer, you might benefit from taking
    /// advantage of someof the static methods in this class.
    /// </summary>
    public class Tokenizer
    {
        readonly ITokenizer _tokenizer;

        /// <summary>
        /// Creates a new tokenizer instance that is used as input to the compiler.
        /// </summary>
        /// <param name="tokenizer">Tokenizer implementation, normally an instance of the LizzieTokenizer class.</param>
        public Tokenizer(ITokenizer tokenizer)
        {
            // Not passing in a tokenizer is a logical runtime error!
            _tokenizer = tokenizer ?? throw new LizzieTokenizerException("No tokenizer implementation given to tokenizer.");
        }

        /// <summary>
        /// Main method invoked by the compiler to request tokens from a stream.
        /// </summary>
        /// <returns>Each token found in your code.</returns>
        /// <param name="stream">Stream containing Lizzie code. Notice, this method does not claim ownership over
        /// your stream, and you are responsible for correctly disposing it yourself</param>
        public IEnumerable<string> Tokenize(Stream stream)
        {
            // Notice! We do NOT take ownership over stream!
            StreamReader reader = new StreamReader(stream);
            while (true) {
                var token = _tokenizer.Next(reader);
                if (token == null)
                    break;
                yield return token;
            }
            yield break;
        }

        /// <summary>
        /// Main method invoked by the compiler to request tokens from multiple streams.
        /// </summary>
        /// <returns>Each token found in your code.</returns>
        /// <param name="streams">Streams containing Lizzie code. Notice, this method does not claim ownership over
        /// your streams, and you are responsible for correctly disposing the streams yourself</param>
        public IEnumerable<string> Tokenize(IEnumerable<Stream> streams)
        {
            foreach (var ixStream in streams) {
                foreach (var ixToken in Tokenize(ixStream)) {
                    yield return ixToken;
                }
            }
        }

        /// <summary>
        /// Main method invoked by the compiler to request tokens from a single string.
        /// </summary>
        /// <returns>Each token found in your code.</returns>
        /// <param name="code">Code to tokenize.</param>
        public IEnumerable<string> Tokenize(string code)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(code))) {
                foreach (var ix in Tokenize(stream)) {
                    yield return ix;
                }
            }
        }

        /// <summary>
        /// Main method invoked by the compiler to request tokens from multiple strings.
        /// </summary>
        /// <returns>Each token found in your code snippets.</returns>
        /// <param name="snippets">Code snippets to tokenize.</param>
        public IEnumerable<string> Tokenize(IEnumerable<string> snippets)
        {
            foreach (var ixCode in snippets) {
                foreach (var ixToken in Tokenize(ixCode)) {
                    yield return ixToken;
                }
            }
        }

        /// <summary>
        /// Eats and discards all whitespace characters found at the beginning of your reader.
        /// 
        /// A white space character is any of the following characters; ' ', '\t',
        /// '\r' and '\n'.
        /// </summary>
        /// <returns><c>true</c>, if space was eaten, <c>false</c> otherwise.</returns>
        /// <param name="reader">Reader.</param>
        public static bool EatSpace(StreamReader reader)
        {
            var retVal = false;
            while (!reader.EndOfStream) {
                var ch = (char)reader.Peek();
                if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n') {
                    reader.Read();
                    retVal = true;
                } else {
                    break;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Eats and discards the rest of the line from your reader.
        /// </summary>
        /// <returns><c>true</c>, if line was eaten, <c>false</c> otherwise.</returns>
        /// <param name="reader">Reader.</param>
        public static void EatLine(StreamReader reader)
        {
            while (!reader.EndOfStream) {
                var ch = (char)reader.Read();
                if (ch == '\r' || ch == '\n') {
                    if (!reader.EndOfStream) {
                        ch = (char)reader.Peek();
                        if (ch == '\r' || ch == '\n')
                            reader.Read();
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Eats and discards characters from the reader until the specified sequence is found.
        /// </summary>
        /// <param name="reader">Reader to eat from.</param>
        /// <param name="sequence">Sequence to look for that will end further eating.</param>
        public static void EatUntil(StreamReader reader, string sequence)
        {
            if (string.IsNullOrEmpty(sequence))
                throw new LizzieTokenizerException("No stop sequence specified to EatUntil.");
            var first = sequence[0];
            var buffer = "";
            while (true) {
                var ix = reader.Read();
                if (ix == -1)
                    return;
                if (buffer != "") {

                    // Seen first character.
                    buffer += (char)ix;
                    if (buffer == sequence) {
                        return; // Found sequence
                    } else if (buffer.Length == sequence.Length) {
                        buffer = ""; // Throwing away matches found so far, starting over again.
                    }
                } else {
                    if (first == (char)ix) {

                        // Beginning of sequence.
                        buffer += (char)ix;
                        if (buffer == sequence) {
                            return; // Sequence found
                        } else if (buffer.Length == sequence.Length) {
                            buffer = ""; // Throwing away matches found so far, starting over again.
                            if (first == (char)ix) {
                                buffer += (char)ix; // This is the opening token for sequence.
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reads a single line string literal from the reader, escaping characters if necessary,
        /// and also supporting UNICODE hex syntax to reference UNICODE characters.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="reader">Reader.</param>
        /// <param name="stop">Stop.</param>
        public static string ReadString(StreamReader reader, char stop = '"')
        {
            var builder = new StringBuilder();
            for (var c = reader.Read(); c != -1; c = reader.Read()) {
                switch (c) {
                    case '\\':
                        builder.Append(GetEscapedCharacter(reader, stop));
                        break;
                    case '\n':
                    case '\r':
                        throw new LizzieTokenizerException($"String literal contains CR or LF characters close to '{builder.ToString()}'.");
                    default:
                        if (c == stop)
                            return builder.ToString();
                        builder.Append ((char)c);
                        break;
                }
            }
            throw new LizzieTokenizerException($"Syntax error, string literal not closed before EOF near '{builder.ToString()}'");
        }

        /*
         * Returns escape character.
         */
        static string GetEscapedCharacter (StreamReader reader, char stop)
        {
            var ch = reader.Read();
            if (ch == -1)
                throw new LizzieTokenizerException("EOF found before string literal was closed");
            switch ((char)ch) {
                case '\\':
                    return "\\";
                case 'a':
                    return "\a";
                case 'b':
                    return "\b";
                case 'f':
                    return "\f";
                case 't':
                    return "\t";
                case 'v':
                    return "\v";
                case 'n':
                    return "\n";
                case 'r':
                    return "\r";
                case 'x':
                    return HexCharacter(reader);
                default:
                    if (ch == stop)
                        return stop.ToString();
                    throw new LizzieTokenizerException($"Invalid escape sequence character '{Convert.ToInt32(ch)}' found in string literal");
            }
        }

        /*
         * Returns hex encoded character.
         */
        static string HexCharacter(StreamReader reader)
        {
            var hexNumberString = "";
            for (var idxNo = 0; idxNo < 4; idxNo++)
                hexNumberString += (char)reader.Read();
            var integerNo = Convert.ToInt32(hexNumberString, 16);
            return Encoding.UTF8.GetString(BitConverter.GetBytes(integerNo).Reverse().ToArray());
        }
    }
}
