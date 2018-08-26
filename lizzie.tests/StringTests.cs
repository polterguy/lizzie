/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using NUnit.Framework;

namespace lizzie.tests
{
    public class StringTests
    {
        [Test]
        public void SubstringOnlyOffset()
        {
            var lambda = LambdaCompiler.Compile(@"substr(""foobarxyz"", 3)");
            var result = lambda();
            Assert.AreEqual("barxyz", result);
        }

        [Test]
        public void SubstringWithCount()
        {
            var lambda = LambdaCompiler.Compile(@"substr(""foobarxyz"", 3, 3)");
            var result = lambda();
            Assert.AreEqual("bar", result);
        }

        [Test]
        public void LengthOfString()
        {
            var lambda = LambdaCompiler.Compile(@"length(""foo"")");
            var result = lambda();
            Assert.AreEqual(3, result);
        }

        [Test]
        public void Replace()
        {
            var lambda = LambdaCompiler.Compile(@"replace(""foo"", ""o"", ""xx"")");
            var result = lambda();
            Assert.AreEqual("fxxxx", result);
        }

        [Test]
        public void SingleQuoteStrings()
        {
            var lambda = LambdaCompiler.Compile(@"replace('foo', 'o', 'xx')");
            var result = lambda();
            Assert.AreEqual("fxxxx", result);
        }

        [Test]
        public void EscapedSingleQuotedString()
        {
            var lambda = LambdaCompiler.Compile(@"'foo\'bar'");
            var result = lambda();
            Assert.AreEqual("foo'bar", result);
        }

        [Test]
        public void EscapedDoubleQuotedString()
        {
            var lambda = LambdaCompiler.Compile(@"""foo\""bar""");
            var result = lambda();
            Assert.AreEqual("foo\"bar", result);
        }
    }
}
