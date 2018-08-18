/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
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
