/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using NUnit.Framework;
using poetic.lizzie;
using poetic.lambda.parser;
using poetic.lambda.collections;

namespace poetic.tests.lizzie_tests
{
    [TestFixture]
    public class ParserTest
    {
        class FooTest
        {
            public int Foo
            {
                get;
                set;
            }

            [Function(Name = "foo")]
            public object SetFoo(Arguments arguments)
            {
                Foo += arguments.Get<int>(0);
                return null;
            }
        }

        [Test]
        public void Parse_1()
        {
            // Creating tokenizer from code.
            const string code = @"foo(57)";
            var tokenizer = new Tokenizer(code, new LizzieTokenizer());

            // Creating parser.
            var lizzie_parser = new LizzieParser<FooTest>();

            // Creating our lambda object.
            var lambda = lizzie_parser.Parse(tokenizer);

            // Executing lambda with a new context instance.
            var foo = new FooTest();
            lambda.Execute(foo);

            // Verifying changes were applied.
            Assert.AreEqual(57, foo.Foo);
        }

        [Test]
        public void Parse_2()
        {
            // Creating tokenizer from code.
            const string code = @"foo(57)";
            var tokenizer = new Tokenizer(code, new LizzieTokenizer());

            // Creating parser.
            var lizzie_parser = new LizzieParser<FooTest>();

            // Creating our lambda object.
            var lambda = lizzie_parser.Parse(tokenizer);

            // Executing lambda with a new context instance.
            var foo = new FooTest();
            lambda.Execute(foo);

            // Verifying changes were applied.
            Assert.AreEqual(57, foo.Foo);

            /*
             * Executing lambda with ANOTHER context instance.
             * 
             * We do this to verify we can reuse the same lambda object for
             * multiple contexts.
             */
            var foo2 = new FooTest() {
                Foo = 10
            };
            lambda.Execute(foo2);

            // Verifying changes were applied.
            Assert.AreEqual(67, foo2.Foo);
        }
    }
}
