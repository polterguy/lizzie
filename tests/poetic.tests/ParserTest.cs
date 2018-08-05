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
        public void TokenizeWordsStream()
        {
            // Example code.
            var code = "foo  bar howdy ";

            // Using Stream as input to our tokenizer, and our "WordTokenizer".
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
            var tokenizer = new Tokenizer(stream, new WordTokenizer());

            // Retrieving tokens.
            var words = new List<string>(tokenizer);

            // Sanity checking tokens that were produced as a result from our tokenizer.
            Assert.AreEqual(3, words.Count);
            Assert.AreEqual("foo", words[0]);
            Assert.AreEqual("bar", words[1]);
            Assert.AreEqual("howdy", words[2]);
        }

        [Test]
        public void TokenizeWordsCode()
        {
            // Example code.
            var code = "foo  bar howdy ";

            // Using raw code (string) as input to our tokenizer, and our "WordTokenizer".
            var tokenizer = new Tokenizer(code, new WordTokenizer());

            // Retrieving tokens.
            var words = new List<string>(tokenizer);

            // Sanity checking tokens that were produced as a result from our tokenizer.
            Assert.AreEqual(3, words.Count);
            Assert.AreEqual("foo", words[0]);
            Assert.AreEqual("bar", words[1]);
            Assert.AreEqual("howdy", words[2]);
        }
    }
}
