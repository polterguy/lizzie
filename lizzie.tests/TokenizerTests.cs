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
        public void SimpleSExpression()
        {
            var code = "(a 1 2)";
            var tokenizer = new lizzie.generic.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(5, list.Count);
        }

        [Test]
        public void NestedSExpression()
        {
            var code = "(a (a b c) 2)";
            var tokenizer = new lizzie.generic.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(9, list.Count);
        }

        [Test]
        public void StringLiteral()
        {
            var code = @"(a ""this is a string"")";
            var tokenizer = new lizzie.generic.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
        }

        [Test]
        public void EscapedStringLiteral()
        {
            var code = @"(a ""this is a \""string"")";
            var tokenizer = new lizzie.generic.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
        }

        [Test]
        public void WeirdCharacters()
        {
            var code = @"(a $%&--&|#@)";
            var tokenizer = new lizzie.generic.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(4, list.Count);
        }

        [Test]
        public void LiteralCharacter()
        {
            // Notice, we do NOT support single quote character!
            var code = @"(a 'b)";
            var tokenizer = new lizzie.generic.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(5, list.Count);
        }
    }
}
