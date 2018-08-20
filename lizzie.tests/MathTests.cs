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
    public class MathTests
    {
        [Test]
        public void AddTwoIntegers()
        {
            var code = "add(10, 57)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["add"] = Functions<Nothing>.Add;
            var result = function(ctx, binder);
            Assert.AreEqual(67, result);
        }

        [Test]
        public void AddMultipleIntegers()
        {
            var code = "add(7, 30, 5, 15)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["add"] = Functions<Nothing>.Add;
            var result = function(ctx, binder);
            Assert.AreEqual(57, result);
        }

        [Test]
        public void AddMultipleFloatingPointValues()
        {
            var code = "add(7.0, 30.0, 5.47, 15.10)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["add"] = Functions<Nothing>.Add;
            var result = function(ctx, binder);
            Assert.AreEqual(57.57, result);
        }

        [Test]
        public void ConcatenateStrings()
        {
            var code = @"add(""hello"", "" "", ""worl"",""d"")";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["add"] = Functions<Nothing>.Add;
            var result = function(ctx, binder);
            Assert.AreEqual("hello world", result);
        }

        [Test]
        public void SubtractTwoIntegers()
        {
            var code = "subtract(67, 10)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["subtract"] = Functions<Nothing>.Subtract;
            var result = function(ctx, binder);
            Assert.AreEqual(57, result);
        }

        [Test]
        public void SubtractMultipleIntegers()
        {
            var code = "subtract(77, 10, 5, 5)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["subtract"] = Functions<Nothing>.Subtract;
            var result = function(ctx, binder);
            Assert.AreEqual(57, result);
        }

        [Test]
        public void MultiplyTwoIntegers()
        {
            var code = "multiply(5, 7)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["multiply"] = Functions<Nothing>.Multiply;
            var result = function(ctx, binder);
            Assert.AreEqual(35, result);
        }

        [Test]
        public void MultiplyMultipleIntegers()
        {
            var code = "multiply(5, 7, 2)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["multiply"] = Functions<Nothing>.Multiply;
            var result = function(ctx, binder);
            Assert.AreEqual(70, result);
        }

        [Test]
        public void DivideTwoFloatingPointNumbers()
        {
            var code = "divide(24.8, 8)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["divide"] = Functions<Nothing>.Divide;
            var result = function(ctx, binder);
            Assert.AreEqual(3.1, result);
        }

        [Test]
        public void DivideMultipleFloatingPointNumbers()
        {
            var code = "divide(100.1, 5, 2)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["divide"] = Functions<Nothing>.Divide;
            var result = function(ctx, binder);
            Assert.AreEqual(10.01, result);
        }

        [Test]
        public void ModuloTwoIntegerNumbers()
        {
            var code = "modulo(7, 5)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["modulo"] = Functions<Nothing>.Modulo;
            var result = function(ctx, binder);
            Assert.AreEqual(2, result);
        }

        [Test]
        public void ModuloMultipleIntegerNumbers()
        {
            var code = "modulo(13, 10, 2)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["modulo"] = Functions<Nothing>.Modulo;
            var result = function(ctx, binder);
            Assert.AreEqual(1, result);
        }
    }
}
