/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System.Collections.Generic;
using NUnit.Framework;
using lizzie.tests.context_types;

namespace lizzie.tests
{
    public class Null
    {
        [Test]
        public void AssertNullIsNull()
        {
            var lambda = LambdaCompiler.Compile("null");
            var result = lambda();
            Assert.IsNull(result);
        }

        [Test]
        public void ComparisonToNullYieldsTrue()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo)
if(eq(null, foo), {
  57
})");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void ComparisonToNullYieldsFalse()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo, 100)
if(not(eq(null, foo)), {
  57
})");
            var result = lambda();
            Assert.AreEqual(57, result);
        }
    }
}
