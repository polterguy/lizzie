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

using NUnit.Framework;

namespace lizzie.tests
{
    public class ParserTests
    {
        [Test]
        public void SimpleListOneIntegerValue()
        {
            var code = "1";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            Assert.AreEqual("1", list.ToString());
        }

        [Test]
        public void SimpleListOneDoubleValue()
        {
            var code = "1.0";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            Assert.AreEqual("1.0", list.ToString());
        }

        [Test]
        public void SimpleListOneLongDoubleValue()
        {
            var code = "1.0005";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            Assert.AreEqual("1.0005", list.ToString());
        }

        [Test]
        public void SimpleListOneSymbolValue()
        {
            var code = "x";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            Assert.AreEqual("x", list.ToString());
        }

        [Test]
        public void SimpleListOneStringValue()
        {
            var code = @"""1""";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            Assert.AreEqual(@"""1""", list.ToString());
        }

        [Test]
        public void SimpleListTwoIntegerValues()
        {
            var code = "1 2";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            Assert.AreEqual("1 2", list.ToString());
        }

        [Test]
        public void SimpleListTwoStringValues()
        {
            var code = @"""1"" ""2""";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            Assert.AreEqual(@"""1"" ""2""", list.ToString());
        }

        [Test]
        public void SimpleListTwoMixedValues()
        {
            var code = @"""1"" 2";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            Assert.AreEqual(@"""1"" 2", list.ToString());
        }

        [Test]
        public void InnerListOneIntegerValue()
        {
            var code = "(1)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            Assert.AreEqual("(1)", list.ToString());
        }

        [Test]
        public void InnerListTwoIntegerValues()
        {
            var code = "(1 2)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            Assert.AreEqual("(1 2)", list.ToString());
        }

        [Test]
        public void InnerListTwoMixedValues()
        {
            var code = @"(1 ""2"")";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            Assert.AreEqual(@"(1 ""2"")", list.ToString());
        }

        [Test]
        public void NestedListsIntegerValues()
        {
            var code = "(1 (2 3))";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            Assert.AreEqual("(1 (2 3))", list.ToString());
        }

        [Test]
        public void NestedListsMixedValues()
        {
            var code = @"(1 (""2"" 3))";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            Assert.AreEqual(@"(1 (""2"" 3))", list.ToString());
        }

        [Test]
        public void NestedListsMixedSymbolValues()
        {
            var code = @"(1 (""2"" 3x))";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            Assert.AreEqual(@"(1 (""2"" 3x))", list.ToString());
        }

        [Test]
        public void QuotedSymbol()
        {
            var code = "'a";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            Assert.AreEqual("(quote a)", list.ToString());
        }

        [Test]
        public void QuotedInnerSymbols()
        {
            var code = "(a 'b ('c d)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            Assert.AreEqual("(a (quote b) ((quote c) d))", list.ToString());
        }
    }
}
