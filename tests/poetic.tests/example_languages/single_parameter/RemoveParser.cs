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
using poetic.lambda.parser;
using poetic.lambda.utilities;
using poetic.lambda.collections;

namespace poetic.tests.example_languages.single_parameter
{
    /*
     * A simple parses example that mutates a simple string input with a
     * parametrised function tokenizer.
     */
    public class RemoveParser
    {
        readonly Tokenizer _tokenizer;

        public RemoveParser(Tokenizer tokenizer)
        {
            _tokenizer = tokenizer ?? throw new NullReferenceException(nameof(tokenizer));
        }

        public Actions<Mutable<string>> Parse()
        {
            var retVal = new Actions<Mutable<string>>();
            var enumerator = _tokenizer.GetEnumerator();
            while (enumerator.MoveNext()) {
                if (enumerator.Current == "remove") {
                    if (!enumerator.MoveNext()) {
                        throw new Exception("Unexpected EOF after function invocation");
                    }
                    if (enumerator.Current != "(") {
                        throw new Exception("No arguments supplied to function invocation");
                    }
                    if (!enumerator.MoveNext()) {
                        throw new Exception("Unexpected EOF after opening paranthesis");
                    }
                    var arg = enumerator.Current;
                    retVal.Add((ix) => ix.Value = ix.Value.Replace(arg, ""));
                    if (!enumerator.MoveNext()) {
                        throw new Exception("Unexpected EOF after function parameters");
                    }
                    if (enumerator.Current != ")") {
                        throw new Exception("No end of arguments supplied to function invocation");
                    }
                } else {
                    throw new Exception("Unsupported keyword");
                }
            }
            return retVal;
        }
    }
}
