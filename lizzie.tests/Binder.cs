/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using NUnit.Framework;
using lizzie.tests.context_types;

namespace lizzie.tests
{
    public class Binder
    {
        [Test]
        public void CloneWithoutStack()
        {
            var original = new Binder<SimpleValues>();
            var clone = original.Clone();
            foreach (var ix in original.StaticItems) {
                Assert.AreEqual(clone[ix], original[ix]);
            }
            foreach (var ix in clone.StaticItems) {
                Assert.AreEqual(clone[ix], original[ix]);
            }
            Assert.AreEqual(0, clone.StackCount);
        }

        [Test]
        public void CloneWithPushedStack()
        {
            var original = new Binder<SimpleValues>();
            original.PushStack();
            original["foo"] = 57;
            original.PushStack();
            original["bar"] = 77;
            var clone = original.Clone();
            foreach (var ix in original.StaticItems) {
                Assert.AreEqual(clone[ix], original[ix]);
            }
            foreach (var ix in clone.StaticItems) {
                Assert.AreEqual(clone[ix], original[ix]);
            }
            Assert.AreEqual(2, clone.StackCount);
            Assert.AreEqual(77, clone["bar"]);
            clone.PopStack();
            Assert.AreEqual(57, clone["foo"]);
        }

        [Test]
        public void StaticFunctions()
        {
            var lambda = LambdaCompiler.Compile<SimpleValues>(new SimpleValues(), "get-static()");
            var result = lambda();
            Assert.AreEqual(7, result);
        }
    }
}
