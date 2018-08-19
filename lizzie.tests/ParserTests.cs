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
        public void InlineIntegerSymbol()
        {
            var code = "57";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            var result = function(ctx, binder);
            Assert.AreEqual(57, result);
        }

        [Test]
        public void InlineFloatingPointSymbol()
        {
            var code = "57.67";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            var result = function(ctx, binder);
            Assert.AreEqual(57.67, result);
        }

        [Test]
        public void InlineStringSymbol()
        {
            var code = @"""57""";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            var result = function(ctx, binder);
            Assert.AreEqual("57", result);
        }

        [Test]
        public void SymbolicallyReferencedConstantNumber()
        {
            var code = "foo";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["foo"] = 57;
            var result = function(ctx, binder);
            Assert.AreEqual(57, result);
        }

        [Test]
        public void SymbolicallyReferencedConstantString()
        {
            var code = "foo";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["foo"] = "bar";
            var result = function(ctx, binder);
            Assert.AreEqual("bar", result);
        }

        [Test]
        public void SymbolicallyReferencedConstantInteger()
        {
            var code = "foo";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["foo"] = 57;
            var result = function(ctx, binder);
            Assert.AreEqual(57, result);
        }

        [Test]
        public void SymbolicallyReferencedFunctionInvocationReturningInteger()
        {
            var code = "foo()";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["foo"] = new Function<Nothing>((ctx2, binder2, arguments) => {
                return 57;
            });
            var result = function(ctx, binder);
            Assert.AreEqual(57, result);
        }

        [Test]
        public void SymbolicallyReferencedComplexType()
        {
            var code = "foo";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["foo"] = new Nothing();
            var result = function(ctx, binder);
            Assert.IsTrue(result is Nothing);
        }

        [Test]
        public void SymbolicallyReferencedFunctionInvocationReturningComplexType()
        {
            var code = "foo()";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["foo"] = new Function<Nothing>((ctx2, binder2, arguments) => {
                return new Nothing();
            });
            var result = function(ctx, binder);
            Assert.IsTrue(result is Nothing);
        }

        [Test]
        public void LiterallyReferencingSymbolName()
        {
            var code = "@foo";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["foo"] = 57;
            var result = function(ctx, binder);
            Assert.AreEqual("foo", result);
        }

        [Test]
        public void BodyReturningConstantInteger()
        {
            var code = "{57}";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            var result = function(ctx, binder);
            Assert.AreEqual(57, result);
        }

        [Test]
        public void BinderGetFunctionInvocation()
        {
            var code = "get-constant-integer()";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<SimpleValues>(tokenizer, code);
            var ctx = new SimpleValues() { ValueInteger = 57 };
            var binder = new Binder<SimpleValues>();
            var result = function(ctx, binder);
            Assert.AreEqual(57, result);
        }

        [Test]
        public void BinderSetFunctionInvocation()
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
        public void BinderNestedFunctionGetAndSetInvocations()
        {
            var code = "set-value-string(get-constant-integer())";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<SimpleValues>(tokenizer, code);
            var ctx = new SimpleValues() { ValueInteger = 57 };
            var binder = new Binder<SimpleValues>();
            var result = function(ctx, binder);
            Assert.IsNull(result);
            Assert.AreEqual("57", ctx.ValueString);
        }

        [Test]
        public void BinderMultipleSetFunctionInvocations()
        {
            var code = @"
set-value-integer(57)
set-value-integer(67)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<SimpleValues>(tokenizer, code);
            var ctx = new SimpleValues();
            var binder = new Binder<SimpleValues>();
            var result = function(ctx, binder);
            Assert.IsNull(result);
            Assert.AreEqual(67, ctx.ValueInteger);
        }

        [Test]
        public void BinderTwoNestedFunctionInvocationArguments()
        {
            var code = @"
add-integers(get-constant-integer(), get-value-integer())
";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<SimpleValues>(tokenizer, code);
            var ctx = new SimpleValues() { ValueInteger = 10 };
            var binder = new Binder<SimpleValues>();
            var result = function(ctx, binder);
            Assert.IsNull(result);
            Assert.AreEqual(67, ctx.ValueInteger);
        }

        [Test]
        public void BinderComplexInvocation()
        {
            var code = @"
set-value-string(""2"")
set-value-integer(2)
add-integers(
  get-constant-integer(), 
  8, 
  get-value-integer(), 
  6, 
  mirror(
    get-value-integer()), 
  get-value-string())
";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<SimpleValues>(tokenizer, code);
            var ctx = new SimpleValues();
            var binder = new Binder<SimpleValues>();
            var result = function(ctx, binder);
            Assert.IsNull(result);
            Assert.AreEqual(77, ctx.ValueInteger);
        }
    }
}
