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
  +(""Hello "", input)
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
  +(""Hello "", name, "" it seems you are "", old, "" years old"")
}, @name, @old))
foo(""Thomas"", 44)");
            var result = lambda();
            Assert.AreEqual("Hello Thomas it seems you are 44 years old", result);
        }

        [Test]
        public void EvaluateFunctionFromWithinFunction()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@bar, function({
  77
}))
var(@foo, function({
  bar()
}))
foo()");
            var result = lambda();
            Assert.AreEqual(77, result);
        }

        [Test]
        public void ChangeStackFromWithinFunction()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@bar, function({
  var(@arg, 50)
}))
var(@foo, function({
  var(@arg, 27)
  +(arg, bar())
}))
foo()");
            var result = lambda();
            Assert.AreEqual(77, result);
        }

        [Test]
        public void VariableDeclaredWithinFunctionDoesNotExistThrows()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@foo, function({
  var(@bar, 50)
}))
foo()
bar");
            var success = false;
            try {
                lambda();
            } catch(LizzieRuntimeException) {
                success = true;
            }
            Assert.AreEqual(true, success);
        }
    }
}
