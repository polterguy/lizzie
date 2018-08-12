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
using poetic.tests.lizzie_tests.contexts;

namespace poetic.tests.lizzie_tests
{
    [TestFixture]
    public class ParserTest
    {
        [Test]
        public void Parse_01()
        {
            // Creating our parser.
            var functionParser = new Function<SimpleNumericValue>();

            // Parsing the code below to create a function, using the Lizzie tokenizer.
            var code = "return(57)";
            var function = functionParser.Parse(new lambda.parser.Tokenizer(new lizzie.Tokenizer()), code);

            // Creating the context which our evaluation will be evaluated within.
            var ctx = new SimpleNumericValue();

            // Creating our binder that binds our CLR methods to functions in our script.
            var binder = new Binder<SimpleNumericValue>();

            // Evaluating our function, passing in no arguments.
            var result = function(ctx, null, binder);

            // Verifying our result is as expected.
            Assert.AreEqual(57, result);
        }

        [Test]
        public void Parse_02()
        {
            // Creating our parser.
            var functionParser = new Function<SimpleNumericValue>();

            // Parsing the code below to create a function, using the Lizzie tokenizer.
            var code = @"return(""57"")";
            var function = functionParser.Parse(new lambda.parser.Tokenizer(new lizzie.Tokenizer()), code);

            // Creating the context which our evaluation will be evaluated within.
            var ctx = new SimpleNumericValue();

            // Creating our binder that binds our CLR methods to functions in our script.
            var binder = new Binder<SimpleNumericValue>();

            // Evaluating our function, passing in no arguments.
            var result = function(ctx, null, binder);

            // Verifying our result is as expected.
            Assert.AreEqual("57", result);
        }

        [Test]
        public void Parse_03()
        {
            // Creating our parser.
            var functionParser = new Function<SimpleNumericValue>();

            // Parsing the code below to create a function, using the Lizzie tokenizer.
            var code = "return('57')";
            var function = functionParser.Parse(new lambda.parser.Tokenizer(new lizzie.Tokenizer()), code);

            // Creating the context which our evaluation will be evaluated within.
            var ctx = new SimpleNumericValue();

            // Creating our binder that binds our CLR methods to functions in our script.
            var binder = new Binder<SimpleNumericValue>();

            // Evaluating our function, passing in no arguments.
            var result = function(ctx, null, binder);

            // Verifying our result is as expected.
            Assert.AreEqual("57", result);
        }

        [Test]
        public void Parse_04()
        {
            // Creating our parser.
            var functionParser = new Function<SimpleNumericValue>();

            // Parsing the code below to create a function, using the Lizzie tokenizer.
            var code = "return(get())";
            var function = functionParser.Parse(new lambda.parser.Tokenizer(new lizzie.Tokenizer()), code);

            // Creating the context which our evaluation will be evaluated within.
            var ctx = new SimpleNumericValue() { Value = 10 };

            // Creating our binder that binds our CLR methods to functions in our script.
            var binder = new Binder<SimpleNumericValue>();

            // Evaluating our function, passing in no arguments.
            var result = function(ctx, null, binder);

            // Verifying our result is as expected.
            Assert.AreEqual(10, result);
        }
    }
}
