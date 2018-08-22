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
    public class BranchingTests
    {
        [Test]
        public void IfVariableHasValueTrue()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@foo, 1)
if(foo, {
  57
})
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void IfVariableHasValueFalse()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@foo)
if(foo, {
  57
})
");
            var result = lambda();
            Assert.IsNull(result);
        }

        [Test]
        public void LazyIfConditionYieldsTrue()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@foo, function({
  67
}))
if(@foo(), {
  57
})
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void LazyIfConditionYieldsFalse()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@foo,function({}))
if(@foo(), {
  57
})
");
            var result = lambda();
            Assert.IsNull(result);
        }

        [Test]
        public void DeclaredArgumentNotPassedIn()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@foo,function({
  input
}, @input))
if(@foo(), {
  57
})
");
            var result = lambda();
            Assert.IsNull(result);
        }

        [Test]
        public void DeclaredArgumentPassedIn()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@foo,function({
  input
}, @input))
if(@foo(""howdy""), {
  57
})
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void ElseYieldsTrue()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@foo)
if(foo, {
  67
}, {
  57
})
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void ElseYieldsFalse()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@foo, 1)
if(foo, {
  67
}, {
  57
})
");
            var result = lambda();
            Assert.AreEqual(67, result);
        }
    }
}
