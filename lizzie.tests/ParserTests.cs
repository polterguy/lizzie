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
using lizzie.tests.domain_objects;

namespace lizzie.tests
{
    public class ParserTests
    {
        [Test]
        public void SetIntegerValueToIntegerConstant()
        {
            var code = "set-value-integer(57)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<SimpleValues>(tokenizer, code);
            var ctx = new SimpleValues();
            var binder = new Binder<SimpleValues>();
            var result = function(ctx, binder);
            Assert.AreEqual(57, ctx.ValueInteger);
        }

        [Test]
        public void SetStringValueToStringConstant()
        {
            var code = @"set-value-string(""foo"")";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<SimpleValues>(tokenizer, code);
            var ctx = new SimpleValues();
            var binder = new Binder<SimpleValues>();
            var result = function(ctx, binder);
            Assert.AreEqual("foo", ctx.ValueString);
        }

        [Test]
        public void SetStringValueToSymbol()
        {
            var code = "set-value-string(foo)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<SimpleValues>(tokenizer, code);
            var ctx = new SimpleValues();
            var binder = new Binder<SimpleValues>();
            binder["foo"] = 57;
            var result = function(ctx, binder);
            Assert.AreEqual("foo", ctx.ValueString);
        }

        [Test]
        public void SetStringValueToSymbolValue()
        {
            var code = "set-value-string(@foo)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<SimpleValues>(tokenizer, code);
            var ctx = new SimpleValues();
            var binder = new Binder<SimpleValues>();
            binder["foo"] = "bar";
            var result = function(ctx, binder);
            Assert.AreEqual("bar", ctx.ValueString);
        }

        [Test]
        public void SetIntegerValueToSymbolValue()
        {
            var code = "set-value-integer(@foo)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<SimpleValues>(tokenizer, code);
            var ctx = new SimpleValues();
            var binder = new Binder<SimpleValues>();
            binder["foo"] = 57;
            var result = function(ctx, binder);
            Assert.AreEqual(57, ctx.ValueInteger);
        }

        [Test]
        public void SetIntegerValueToRecursivelyReferencedSymbolValue()
        {
            var code = "set-value-integer(@@foo)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<SimpleValues>(tokenizer, code);
            var ctx = new SimpleValues();
            var binder = new Binder<SimpleValues>();
            binder["foo"] = "bar";
            binder["bar"] = 57;
            var result = function(ctx, binder);
            Assert.AreEqual(57, ctx.ValueInteger);
        }

        [Test]
        public void SetIntegerValueToValueOfFunction()
        {
            var code = "set-value-integer(get-constant-integer())";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<SimpleValues>(tokenizer, code);
            var ctx = new SimpleValues();
            var binder = new Binder<SimpleValues>();
            var result = function(ctx, binder);
            Assert.AreEqual(57, ctx.ValueInteger);
        }

        [Test]
        public void AddTwoIntegers()
        {
            var code = "add-integers(57, 10)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<SimpleValues>(tokenizer, code);
            var ctx = new SimpleValues();
            var binder = new Binder<SimpleValues>();
            var result = function(ctx, binder);
            Assert.AreEqual(67, ctx.ValueInteger);
        }

        [Test]
        public void EvaluatingFunctionBySymbolicReference()
        {
            var code = "@foo(57)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<SimpleValues>(tokenizer, code);
            var ctx = new SimpleValues();
            var binder = new Binder<SimpleValues>();
            binder["foo"] = "set-value-integer";
            var result = function(ctx, binder);
            Assert.AreEqual(57, ctx.ValueInteger);
        }

        [Test]
        public void EvaluatingFunctionBySymbolicReferenceRecursively()
        {
            var code = "@@foo(57)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<SimpleValues>(tokenizer, code);
            var ctx = new SimpleValues();
            var binder = new Binder<SimpleValues>();
            binder["foo"] = "bar";
            binder["bar"] = "set-value-integer";
            var result = function(ctx, binder);
            Assert.AreEqual(57, ctx.ValueInteger);
        }
    }
}
