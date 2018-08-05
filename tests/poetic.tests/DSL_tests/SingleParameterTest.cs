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
using poetic.tests.example_languages.single_parameter;
using poetic.lambda.utilities;

namespace poetic.tests.DSL_tests
{
    [TestFixture]
    public class SingleParameterTest
    {
        [Test]
        public void FunctionsTest_1()
        {
            // Creating our tokenizer and parsing it to create a lambda object.
            var tokenizer = new Tokenizer("remove(x) remove(y)", new FunctionTokenizer());
            var list = new List<string>(tokenizer);
            var lambda = new RemoveParser(tokenizer).Parse();

            // Executes our lambda passing in an input string that mutates.
            var input = new Mutable<string>("thomasxx hanyysen");
            lambda.Execute(input);
            Assert.AreEqual("thomas hansen", input.Value);
        }

        [Test]
        public void FunctionsTest_2()
        {
            /*
             * Creating our tokenizer and parsing it to create a lambda object.
             * Notice, we intentionally try to put in superflous white spaces here
             * to test the white space logic of the Tokenizer class.
             */
            var tokenizer = new Tokenizer(@"    remove    (     x )   
  

 
  remove (y )   

", new FunctionTokenizer());
            var list = new List<string>(tokenizer);
            Assert.AreEqual(8, list.Count);
            var lambda = new RemoveParser(tokenizer).Parse();

            // Executes our lambda passing in an input string that mutates.
            var input = new Mutable<string>("thomasxx hanyysen");
            lambda.Execute(input);
            Assert.AreEqual("thomas hansen", input.Value);
        }
    }
}
