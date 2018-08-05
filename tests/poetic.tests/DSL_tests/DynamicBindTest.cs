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

using System.Collections.Generic;
using NUnit.Framework;
using poetic.lambda.parser;
using poetic.tests.example_languages.dynamic_bind;
using poetic.tests.example_languages.single_parameter;

namespace poetic.tests.DSL_tests
{
    [TestFixture]
    public class DynamicBindTest
    {
        [Test]
        public void DynamicBindTest_1()
        {
            // Creating our tokenizer and parsing it to create a lambda object.
            var tokenizer = new Tokenizer("set_foo(xyz) set_bar(57)", new FunctionTokenizer());
            var list = new List<string>(tokenizer);
            var lambda = new DynamicBindParser<DynamicBinder>(tokenizer).Parse();

            // Creates an instance of our Binder and passes it into our lambda execution.
            var binder = new DynamicBinder();

            // Executes our lambda passing in an input string that mutates.
            lambda.Execute(binder);
            Assert.AreEqual("xyz", binder.FooValue);
            Assert.AreEqual(57, binder.BarValue);
        }
    }
}
