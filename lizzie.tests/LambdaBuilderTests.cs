/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using NUnit.Framework;
using lizzie.exceptions;
using lizzie.tests.context_types;

namespace lizzie.tests
{
    public class LambdaBuilderTests
    {
        [Test]
        public void WithType()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), "57");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void WithoutType()
        {
            var lambda = LambdaCompiler.Compile("57");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void MathTest()
        {
            var lambda = LambdaCompiler.Compile("modulo(divide(add(5, multiply(5,subtract(20,15))),3),7)");
            var result = lambda();
            Assert.AreEqual(3, result);
        }

        [Test]
        public void VariableTest()
        {
            var code = @"
set(@foo, 57)
set(@bar, add(foo, multiply(10,2)))
bar";
            var lambda = LambdaCompiler.Compile(code);
            var result = lambda();
            Assert.AreEqual(77, result);
        }

        [Test]
        public void SettingVariableToResultOfFunctionInvocation()
        {
            var lambda = LambdaCompiler.Compile(@"
set(@foo, set(@bar, 67))
bar");
            var result = lambda();
            Assert.AreEqual(67, result);
        }

        [Test]
        public void SettingVariableToFunctionInvocation_01()
        {
            var lambda = LambdaCompiler.Compile(@"
set(@foo, @set(@bar, 67))
foo");
            var result = lambda();
            Assert.IsTrue(result is Function<LambdaCompiler.Nothing>);
        }

        [Test]
        public void SettingVariableToFunctionInvocation_02()
        {
            var lambda = LambdaCompiler.Compile(@"
set(@foo, @set(@bar, 67))
bar");
            var success = false;
            try {
                lambda();
            } catch (LizzieRuntimeException) {
                success = true;
            }
            Assert.IsTrue(success);
        }

        [Test]
        public void SettingVariableToFunctionInvocation_03()
        {
            var lambda = LambdaCompiler.Compile(@"
set(@foo, @set(@bar, 67))
foo()
bar");
            var result = lambda();
            Assert.AreEqual(67, result);
        }
    }
}
