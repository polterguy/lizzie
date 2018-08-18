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

using System.Collections.Generic;
using NUnit.Framework;

namespace lizzie.tests
{
    public class TokenizerTests
    {
        [Test]
        public void FunctionInvocationInteger()
        {
            var code = "a(1)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(4, list.Count);
        }

        [Test]
        public void FunctionInvocationString()
        {
            var code = @"a(""foo"")";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
        }

        [Test]
        public void FunctionInvocationMixed()
        {
            var code = @"a(""foo"", 5, ""bar"")";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(12, list.Count);
        }

        [Test]
        public void FunctionInvocationMixedNested()
        {
            var code = @"a(""foo"", bar(5), ""bar"")";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(15, list.Count);
        }

        [Test]
        public void WeirdSpacing()
        {
            var code = @"  a (   ""\""fo\""o""   ,bar(  5 )     ,""bar""   )   ";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(15, list.Count);
        }

        [Test]
        public void SingleLineComment()
        {
            var code = @"a(""foo"", bar(5)) // , ""bar"")";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(11, list.Count);
        }

        [Test]
        public void MultiLineComment()
        {
            var code = @"
a(""foo"" /* comment */,
     bar(5)/* FOO!! *** */ ) // , ""bar"")
   /* 
 * hello
 */
jo()";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(14, list.Count);
        }
    }
}
