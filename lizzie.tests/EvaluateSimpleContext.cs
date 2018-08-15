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
    public class EvaluateSimpleContext
    {
        [Test]
        public void IntegerFunctionNullReturn()
        {
            var code = "(set-value 57)";
            var tokenizer = new lizzie.generic.Tokenizer(new lizzie.Tokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            var functor = list.Compile<SimpleInteger>();
            var ctx = new SimpleInteger();
            var binder = new Binder<SimpleInteger>();
            var result = functor(ctx, binder);
            Assert.IsNull(result);
            Assert.AreEqual(57, ctx.Value);
        }

        [Test]
        public void IntegerFunctionIntegerReturn()
        {
            var code = "(set-and-return-value 57)";
            var tokenizer = new lizzie.generic.Tokenizer(new lizzie.Tokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            var functor = list.Compile<SimpleInteger>();
            var ctx = new SimpleInteger();
            var binder = new Binder<SimpleInteger>();
            var result = functor(ctx, binder);
            Assert.AreEqual(57, result);
            Assert.AreEqual(57, ctx.Value);
        }

        [Test]
        public void IntegerFunctionTwice()
        {
            var code = @"
(set-value 57)
(set-value 67)
";
            var tokenizer = new lizzie.generic.Tokenizer(new lizzie.Tokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            var functor = list.Compile<SimpleInteger>();
            var ctx = new SimpleInteger();
            var binder = new Binder<SimpleInteger>();
            var result = functor(ctx, binder);
            Assert.IsNull(result);
            Assert.AreEqual(67, ctx.Value);
        }

        [Test]
        public void MixedFunctionsReturns()
        {
            var code = @"
(set-value 57)
(set-and-return-value 67)
";
            var tokenizer = new lizzie.generic.Tokenizer(new lizzie.Tokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            var functor = list.Compile<SimpleInteger>();
            var ctx = new SimpleInteger();
            var binder = new Binder<SimpleInteger>();
            var result = functor(ctx, binder);
            Assert.AreEqual(67, result);
            Assert.AreEqual(67, ctx.Value);
        }

        [Test]
        public void NotExistsThrows()
        {
            var code = "(does-not-exist 57)";
            var tokenizer = new lizzie.generic.Tokenizer(new lizzie.Tokenizer());
            var parser = new Parser();
            var list = parser.Parse(tokenizer, code);
            var functor = list.Compile<SimpleInteger>();
            var ctx = new SimpleInteger();
            var binder = new Binder<SimpleInteger>();
            var success = false;
            try {
                var result = functor(ctx, binder);
            } catch (lizzie.exceptions.LizzieExecutionException) {
                success = true;
            }
            Assert.AreEqual(true, success);
        }
    }
}
