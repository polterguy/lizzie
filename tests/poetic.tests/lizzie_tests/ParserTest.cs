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
using poetic.tests.lizzie_tests.contexts;

namespace poetic.tests.lizzie_tests
{
    [TestFixture]
    public class ParserTest
    {
        [Test]
        public void Parse_01()
        {
            // Creating our function.
            var functor = new LizzieParser<SimpleNumericValue>().Parse(new Tokenizer(new LizzieTokenizer()), "add(57);");

            // Evaluating our function.
            var ctx = new SimpleNumericValue();
            var st = new FunctionStack<SimpleNumericValue>(new Binder<SimpleNumericValue>(), ctx);
            functor.Execute(st);

            // Verifying it behaved as expected.
            Assert.AreEqual(57, ctx.Value);
        }

        [Test]
        public void Parse_02()
        {
            // Creating our function.
            var functor = new LizzieParser<SimpleNumericValue>().Parse(new Tokenizer(new LizzieTokenizer()), "add(57);");

            // Evaluating our function with our first context.
            var ctx1 = new SimpleNumericValue();
            var st = new FunctionStack<SimpleNumericValue>(new Binder<SimpleNumericValue>(), ctx1);
            functor.Execute(st);

            // Verifying first evaluation behaved as expected.
            Assert.AreEqual(57, ctx1.Value);

            // Evaluating our function with our second context.
            var ctx2 = new SimpleNumericValue() { Value = 10 };
            var st2 = new FunctionStack<SimpleNumericValue>(new Binder<SimpleNumericValue>(), ctx2);
            functor.Execute(st2);

            // Verifying second evaluation behaved as expected.
            Assert.AreEqual(67, ctx2.Value);

            // Verifying evaluation of lambda with the second context did not change our first context.
            Assert.AreEqual(57, ctx1.Value);
        }

        [Test]
        public void Parse_03()
        {
            // Creating our function.
            var functor = new LizzieParser<SimpleStringValue>().Parse(new Tokenizer(new LizzieTokenizer()), "add('foo');");

            // Evaluating our function.
            var ctx = new SimpleStringValue();
            var st = new FunctionStack<SimpleStringValue>(new Binder<SimpleStringValue>(), ctx);
            functor.Execute(st);

            // Verifying it behaved as expected.
            Assert.AreEqual("foo", ctx.Value);

            /*
             * Executing functor another time with the same context, verifying it
             * concatenates the string yet another time.
             */
            functor.Execute(st);
            Assert.AreEqual("foofoo", ctx.Value);
        }

        [Test]
        public void Parse_04()
        {
            // Creating our function.
            var functor = new LizzieParser<SimpleStringValue>().Parse(new Tokenizer(new LizzieTokenizer()), @"
add('foo');
add(""bar"");");

            // Evaluating our function.
            var ctx = new SimpleStringValue();
            var st = new FunctionStack<SimpleStringValue>(new Binder<SimpleStringValue>(), ctx);
            functor.Execute(st);

            // Verifying it behaved as expected.
            Assert.AreEqual("foobar", ctx.Value);
        }

        [Test]
        public void Parse_05()
        {
            // Creating our function.
            var functor = new LizzieParser<SimpleStringValue>().Parse(new Tokenizer(new LizzieTokenizer()), @"
add(add('howdy'));");

            // Evaluating our function.
            var ctx = new SimpleStringValue();
            var st = new FunctionStack<SimpleStringValue>(new Binder<SimpleStringValue>(), ctx);
            functor.Execute(st);

            // Verifying it behaved as expected.
            Assert.AreEqual("howdyhowdy", ctx.Value);
        }

        [Test]
        public void Parse_06()
        {
            // Creating our function.
            var functor = new LizzieParser<TwoNumericValues>().Parse(new Tokenizer(new LizzieTokenizer()), @"add(2, 55);");

            // Evaluating our function.
            var ctx = new TwoNumericValues();
            var st = new FunctionStack<TwoNumericValues>(new Binder<TwoNumericValues>(), ctx);
            functor.Execute(st);

            // Verifying it behaved as expected.
            Assert.AreEqual(57, st.Context.Value);
        }
    }
}
