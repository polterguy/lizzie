/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using NUnit.Framework;
using lizzie.tests.context_types;
using lizzie.exceptions;

namespace lizzie.tests
{
    public class Math
    {
        [Test]
        public void AddTwoIntegers()
        {
            var code = "+(10, 57)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<LambdaCompiler.Nothing>(tokenizer, code);
            var ctx = new LambdaCompiler.Nothing();
            var binder = new Binder<LambdaCompiler.Nothing>();
            binder["+"] = Functions<LambdaCompiler.Nothing>.Add;
            var result = function(ctx, binder);
            Assert.AreEqual(67, result);
        }

        [Test]
        public void AddMultipleIntegers()
        {
            var code = "+(7, 30, 5, 15)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<LambdaCompiler.Nothing>(tokenizer, code);
            var ctx = new LambdaCompiler.Nothing();
            var binder = new Binder<LambdaCompiler.Nothing>();
            binder["+"] = Functions<LambdaCompiler.Nothing>.Add;
            var result = function(ctx, binder);
            Assert.AreEqual(57, result);
        }

        [Test]
        public void AddMultipleFloatingPointValues()
        {
            var code = "+(7.0, 30.0, 5.47, 15.10)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<LambdaCompiler.Nothing>(tokenizer, code);
            var ctx = new LambdaCompiler.Nothing();
            var binder = new Binder<LambdaCompiler.Nothing>();
            binder["+"] = Functions<LambdaCompiler.Nothing>.Add;
            var result = function(ctx, binder);
            Assert.AreEqual(57.57, result);
        }

        [Test]
        public void ConcatenateStrings()
        {
            var code = @"+(""hello"", "" "", ""worl"",""d"")";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<LambdaCompiler.Nothing>(tokenizer, code);
            var ctx = new LambdaCompiler.Nothing();
            var binder = new Binder<LambdaCompiler.Nothing>();
            binder["+"] = Functions<LambdaCompiler.Nothing>.Add;
            var result = function(ctx, binder);
            Assert.AreEqual("hello world", result);
        }

        [Test]
        public void SubtractTwoIntegers()
        {
            var code = "-(67, 10)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<LambdaCompiler.Nothing>(tokenizer, code);
            var ctx = new LambdaCompiler.Nothing();
            var binder = new Binder<LambdaCompiler.Nothing>();
            binder["-"] = Functions<LambdaCompiler.Nothing>.Subtract;
            var result = function(ctx, binder);
            Assert.AreEqual(57, result);
        }

        [Test]
        public void SubtractMultipleIntegers()
        {
            var code = "-(77, 10, 5, 5)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<LambdaCompiler.Nothing>(tokenizer, code);
            var ctx = new LambdaCompiler.Nothing();
            var binder = new Binder<LambdaCompiler.Nothing>();
            binder["-"] = Functions<LambdaCompiler.Nothing>.Subtract;
            var result = function(ctx, binder);
            Assert.AreEqual(57, result);
        }

        [Test]
        public void MultiplyTwoIntegers()
        {
            var code = "*(5, 7)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<LambdaCompiler.Nothing>(tokenizer, code);
            var ctx = new LambdaCompiler.Nothing();
            var binder = new Binder<LambdaCompiler.Nothing>();
            binder["*"] = Functions<LambdaCompiler.Nothing>.Multiply;
            var result = function(ctx, binder);
            Assert.AreEqual(35, result);
        }

        [Test]
        public void MultiplyMultipleIntegers()
        {
            var code = "*(5, 7, 2)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<LambdaCompiler.Nothing>(tokenizer, code);
            var ctx = new LambdaCompiler.Nothing();
            var binder = new Binder<LambdaCompiler.Nothing>();
            binder["*"] = Functions<LambdaCompiler.Nothing>.Multiply;
            var result = function(ctx, binder);
            Assert.AreEqual(70, result);
        }

        [Test]
        public void DivideTwoFloatingPointNumbers()
        {
            var code = "/(24.8, 8)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<LambdaCompiler.Nothing>(tokenizer, code);
            var ctx = new LambdaCompiler.Nothing();
            var binder = new Binder<LambdaCompiler.Nothing>();
            binder["/"] = Functions<LambdaCompiler.Nothing>.Divide;
            var result = function(ctx, binder);
            Assert.AreEqual(3.1, result);
        }

        [Test]
        public void DivideMultipleFloatingPointNumbers()
        {
            var code = "/(100.1, 5, 2)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<LambdaCompiler.Nothing>(tokenizer, code);
            var ctx = new LambdaCompiler.Nothing();
            var binder = new Binder<LambdaCompiler.Nothing>();
            binder["/"] = Functions<LambdaCompiler.Nothing>.Divide;
            var result = function(ctx, binder);
            Assert.AreEqual(10.01, result);
        }

        [Test]
        public void ModuloTwoIntegerNumbers()
        {
            var code = "%(7, 5)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<LambdaCompiler.Nothing>(tokenizer, code);
            var ctx = new LambdaCompiler.Nothing();
            var binder = new Binder<LambdaCompiler.Nothing>();
            binder["%"] = Functions<LambdaCompiler.Nothing>.Modulo;
            var result = function(ctx, binder);
            Assert.AreEqual(2, result);
        }

        [Test]
        public void ModuloMultipleIntegerNumbers()
        {
            var code = "%(13, 10, 2)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<LambdaCompiler.Nothing>(tokenizer, code);
            var ctx = new LambdaCompiler.Nothing();
            var binder = new Binder<LambdaCompiler.Nothing>();
            binder["%"] = Functions<LambdaCompiler.Nothing>.Modulo;
            var result = function(ctx, binder);
            Assert.AreEqual(1, result);
        }
    }
}
