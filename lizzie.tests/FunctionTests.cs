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
    public class FunctionTests
    {
        [Test]
        public void ReturnsNumberConstant()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@foo, function({
  57
}))
foo()");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void ReturnsStringConstant()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@foo, function({
  ""Hello World""
}))
foo()");
            var result = lambda();
            Assert.AreEqual("Hello World", result);
        }

        [Test]
        public void SingleParameter()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@foo, function({
  add(""Hello "", input)
}, @input))
foo(""Thomas"")");
            var result = lambda();
            Assert.AreEqual("Hello Thomas", result);
        }

        [Test]
        public void MultipleParameters()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@foo, function({
  add(""Hello "", name, "" it seems you are "", old, "" years old"")
}, @name, @old))
foo(""Thomas"", 44)");
            var result = lambda();
            Assert.AreEqual("Hello Thomas it seems you are 44 years old", result);
        }
    }
}
