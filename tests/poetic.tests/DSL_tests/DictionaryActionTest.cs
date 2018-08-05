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
using poetic.lambda.parser;
using poetic.tests.example_languages.no_parameters;
using poetic.lambda.utilities;

namespace poetic.tests.DSL_tests
{
    [TestFixture]
    public class DictionaryActionTest
    {
        [Test]
        public void ParseTest_1()
        {
            // Creating our tokenizer and parsing it to create a lambda object.
            var tokenizer = new Tokenizer("remove_x remove_y", new WordTokenizer());
            var lambda = new RemoveParser(tokenizer).Parse();

            // Executes our lambda passing in an input string that mutates.
            var input = new Mutable<string>("thomasx hanseny");
            lambda.Execute(input);
            Assert.AreEqual("thomas hansen", input.Value);
        }

        [Test]
        public void ParseTest_2()
        {
            // Creating our tokenizer and parsing it to create a lambda object.
            var tokenizer = new Tokenizer("remove_y", new WordTokenizer());
            var lambda = new RemoveParser(tokenizer).Parse();

            // Executes our lambda passing in an input string that mutates.
            var input = new Mutable<string>("thomasx hanseny");
            lambda.Execute(input);
            Assert.AreEqual("thomasx hansen", input.Value);
        }

        [Test]
        public void ParseTest_3()
        {
            // Creating our tokenizer and parsing it to create a lambda object.
            var tokenizer = new Tokenizer("remove_x", new WordTokenizer());
            var lambda = new RemoveParser(tokenizer).Parse();

            // Executes our lambda passing in an input string that mutates.
            var input = new Mutable<string>("thoxxxmasx hanyysenyxx");
            lambda.Execute(input);
            Assert.AreEqual("thomas hanyyseny", input.Value);
        }

        [Test]
        public void ParseTest_4()
        {
            // Creating our tokenizer and parsing it to create a lambda object.
            var tokenizer = new Tokenizer("remove_x", new WordTokenizer());
            var lambda = new RemoveParser(tokenizer).Parse();

            // Executes our lambda passing in an input string that mutates.
            var input = new Mutable<string>("thoxxxmasx hanyysenyxx");
            lambda.Execute(input);
            Assert.AreEqual("thomas hanyyseny", input.Value);

            // Passing in the same argument to another instance of our lambda.
            tokenizer = new Tokenizer("remove_y", new WordTokenizer());
            lambda = new RemoveParser(tokenizer).Parse();
            lambda.Execute(input);
            Assert.AreEqual("thomas hansen", input.Value);
        }

        [Test]
        public void ParseTest_5()
        {
            // Creating our tokenizer and parsing it to create a lambda object.
            var tokenizer = new Tokenizer("remove_x", new WordTokenizer());
            var lambda = new RemoveParser(tokenizer).Parse();

            // Executes our lambda passing in an input string that mutates.
            var input = new Mutable<string>("thomasxxx hansxxen");
            lambda.Execute(input);
            Assert.AreEqual("thomas hansen", input.Value);

            // Reusing the same lambda object on another input.
            input = new Mutable<string>("Thomxxas Hansxxxen");
            lambda.Execute(input);
            Assert.AreEqual("Thomas Hansen", input.Value);
        }
    }
}
