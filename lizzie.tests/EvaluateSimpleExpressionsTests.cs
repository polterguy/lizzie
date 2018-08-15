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
using NUnit.Framework;
using lizzie.tests.domain_objects;
using lizzie.types;

namespace lizzie.tests
{
    public class EvaluateSimpleExpressionsTests
    {
        [Test]
        public void Integer()
        {
            var code = "1";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            var functor = list.Compile<Nothing>();
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            var result = functor(ctx, binder, null);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void String()
        {
            var code = @"""1""";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            var functor = list.Compile<Nothing>();
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            var result = functor(ctx, binder, null);
            Assert.AreEqual("1", result);
        }

        [Test]
        public void Double()
        {
            var code = @"1.1";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            var functor = list.Compile<Nothing>();
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            var result = functor(ctx, binder, null);
            Assert.AreEqual(1.1D, result);
        }

        [Test]
        public void NonFormThrows()
        {
            var code = "(1 2)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            var success = false;
            try {
                var functor = list.Compile<Nothing>();
                var ctx = new Nothing();
                var binder = new Binder<Nothing>();
                var result = functor(ctx, binder, null);
            } catch (exceptions.LizzieExecutionException) {
                success = true;
            }
            Assert.AreEqual(true, success);
        }
    }
}
