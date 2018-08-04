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

using System.IO;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using poetic.lambda.parser;
using poetic.tests.halpers;

namespace poetic.tests
{
    [TestFixture]
    public class ParserTest
    {
        [Test]
        public void Tokenize_1()
        {
            var code = "foo bar";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(code))) {
                var tokenizer = new WordTokenizer();
                var tokens = new List<string>(Parser.Tokenize(stream, tokenizer));
                Assert.AreEqual(2, tokens.Count);
                Assert.AreEqual("foo", tokens[0]);
                Assert.AreEqual("bar", tokens[1]);
            }
        }
    }
}
