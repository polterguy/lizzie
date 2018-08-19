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
    }
}
