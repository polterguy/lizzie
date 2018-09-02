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
    public class DictionaryTests
    {
        [Test]
        public void CreateEmpty()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), "map()");
            var result = lambda();
            Assert.IsTrue(result is Dictionary<string, object>);
        }

        [Test]
        public void CreateWithInitialValues()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
map(
  'foo', 57,
  'bar', 77
)
");
            var result = lambda();
            var map = result as Dictionary<string, object>;
            Assert.AreEqual(2, map.Count);
            Assert.AreEqual(57, map["foo"]);
            Assert.AreEqual(77, map["bar"]);
        }

        [Test]
        public void Count()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
count(map(
  'foo', 57,
  'bar', 77
))
");
            var result = lambda();
            Assert.AreEqual(2, result);
        }

        [Test]
        public void RetrieveValues()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@my-map, map(
  'foo', 57,
  'bar', 77
))
get(my-map, 'foo')
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void Add()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@my-map, map(
  'foo', 57
))
add(my-map, 'bar', 77, 'howdy', 99)
my-map
");
            var result = lambda();
            var map = result as Dictionary<string, object>;
            Assert.AreEqual(3, map.Count);
            Assert.AreEqual(57, map["foo"]);
            Assert.AreEqual(77, map["bar"]);
            Assert.AreEqual(99, map["howdy"]);
        }

        [Test]
        public void Each_01()
        {
            var lambda = LambdaCompiler.Compile<Nothing>(new Nothing(), @"
var(@my-map, map(
  'foo', 47,
  'bar', 10
))
var(@result, 0)
each(@ix, my-map, {
  set(@result, +(result, get(my-map, ix)))
})
result
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }
    }
}
