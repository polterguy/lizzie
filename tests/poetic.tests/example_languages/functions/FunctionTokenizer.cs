/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.IO;
using System.Collections.Generic;
using poetic.lambda.parser;

namespace poetic.tests.example_languages.functions
{
    /*
     * A simple word tokenizer that return each word kind of like string.Split
     * would do given " " as separate characters.
     */
    public class FunctionTokenizer : ITokenizer
    {
        public string Next (StreamReader reader)
        {
            Tokenizer.EatSpace(reader);
            var retVal = "";
            while (!reader.EndOfStream) {
                var ch = (char)reader.Read();
                switch (ch) {
                    case '(':
                        return "(";
                    case ')':
                        return ")";
                    default:
                        retVal += ch;
                        break;
                }
                if (Tokenizer.NextIsOfOrEOF(reader, '(', ')'))
                    break;
                if (Tokenizer.NextIsSpaceOrEOF(reader))
                    break;
            }
            return retVal == "" ? null : retVal;
        }
    }
}
