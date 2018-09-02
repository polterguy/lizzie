/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System.Collections.Generic;
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

        [Test]
        public void JSONString_01()
        {
            var lambda = LambdaCompiler.Compile(@"
string(map(
  'foo', 57,
  'bar', 67
))
");
            var result = lambda();
            Assert.AreEqual(@"{""foo"":57,""bar"":67}", result);
        }

        [Test]
        public void JSONString_02()
        {
            var lambda = LambdaCompiler.Compile(@"
string(map(
  'foo', 'howdy',
  'bar', 'world'
))
");
            var result = lambda();
            Assert.AreEqual(@"{""foo"":""howdy"",""bar"":""world""}", result);
        }

        [Test]
        public void JSONString_03()
        {
            var lambda = LambdaCompiler.Compile(@"
string(map(
  'foo', 'howdy',
  'bar', 'wor""ld'
))
");
            var result = lambda();
            Assert.AreEqual(@"{""foo"":""howdy"",""bar"":""wor\""ld""}", result);
        }

        [Test]
        public void JSONString_04()
        {
            var lambda = LambdaCompiler.Compile(@"
string(list(
  'foo',
  'bar'
))
");
            var result = lambda();
            Assert.AreEqual(@"[""foo"",""bar""]", result);
        }

        [Test]
        public void JSONString_05()
        {
            var lambda = LambdaCompiler.Compile(@"
string(list(
  'foo',
  map(
    'bar1',57,
    'bar2',77,
    'bar3',list(1,2,map('hello','world'))
  )
))
");
            var result = lambda();
            Assert.AreEqual(@"[""foo"",{""bar1"":57,""bar2"":77,""bar3"":[1,2,{""hello"":""world""}]}]", result);
        }

        [Test]
        public void JSONStringToObject_01()
        {
            var lambda = LambdaCompiler.Compile(@"
json(""{'foo':57}"")
");
            var result = lambda();
            var map = result as Dictionary<string, object>;
            Assert.IsNotNull(map);
            Assert.AreEqual(57, map["foo"]);
        }

        [Test]
        public void JSONStringToObject_02()
        {
            var lambda = LambdaCompiler.Compile(@"
json(""[0,1,2]"")
");
            var result = lambda();
            var list = result as List<object>;
            Assert.IsNotNull(list);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(2, list[2]);
        }

        [Test]
        public void JSONStringToObject_03()
        {
            var lambda = LambdaCompiler.Compile(@"
json(""[0,1,{'foo':57,'bar':77,'hello':'world'}]"")
");
            var result = lambda();
            var list = result as List<object>;
            Assert.IsNotNull(list);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            var map = list[2] as Dictionary<string, object>;
            Assert.IsNotNull(map);
            Assert.AreEqual(3, map.Count);
            Assert.AreEqual(57, map["foo"]);
            Assert.AreEqual(77, map["bar"]);
            Assert.AreEqual("world", map["hello"]);
        }
    }
}
