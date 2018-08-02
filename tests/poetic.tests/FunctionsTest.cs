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
using System.Linq;
using NUnit.Framework;
using System.Threading;
using poetic.lambda.lambdas;

namespace poetic.tests
{
    [TestFixture]
    public class FunctionsTest
    {
        /// <summary>
        /// Ensures that Sequential evaluates correctly.
        /// </summary>
        [Test]
        public void Sequential()
        {
            // Creating a Functions object.
            var functions = new Functions<string>();
            functions.Add(() => "1");
            functions.Add(() => "2");
            functions.Add(() => "3");

            // Executing and verifying result.
            var result = "";
            foreach (var idx in functions.Sequential()) {
                result += idx;
            }
            Assert.AreEqual("123", result);
        }

        /// <summary>
        /// Ensures that Sequential evaluates correctly.
        /// </summary>
        [Test]
        public void Join()
        {
            // Creating a Functions object.
            var functions = new Functions<string>();
            functions.Add(() => "1");
            functions.Add(() => "2");
            functions.Add(() => "3");

            // Executing and verifying result.
            var result = "";
            foreach (var idx in functions.Join()) {
                result += idx;
            }
            var assert = 
                result == "123" || 
                result == "132" || 
                result == "231" || 
                result == "213" || 
                result == "312" || 
                result == "321";
            Assert.AreEqual(true, assert);
        }

        /// <summary>
        /// Ensures that Sequential evaluates correctly.
        /// </summary>
        [Test]
        public void JoinTimeout()
        {
            // Creating a Functions object.
            var functions = new Functions<string>();
            functions.Add(() => "1");
            functions.Add(delegate () {
                Thread.Sleep(2000);
                return "3";
            });
            functions.Add(() => "2");

            // Executing and verifying result.
            var result = "";
            foreach (var idx in functions.Join(500)) {
                result += idx;
            }
            var assert = result == "12" || result == "21";
            Assert.AreEqual(true, assert);
        }

        /// <summary>
        /// Ensures that Braid evaluates correctly.
        /// </summary>
        [Test]
        public void Braid_1()
        {
            // Creating a Functions object.
            var functions = new Functions<int, string>();
            functions.Add((input) => (input + 1).ToString());
            functions.Add((input) => (input + 2).ToString());
            functions.Add((input) => (input + 3).ToString());

            // Executing and verifying result.
            var result = "";
            foreach (var idx in functions.Braid(new int[]{1,2,3})) {
                result += idx;
            }
            Assert.AreEqual("246", result);
        }

        /// <summary>
        /// Ensures that Braid evaluates correctly.
        /// </summary>
        [Test]
        public void Braid_2()
        {
            // Creating a Functions object.
            var functions = new Functions<int, string>();
            functions.Add((input) => (input + 1).ToString());
            functions.Add((input) => (input + 2).ToString());
            functions.Add((input) => (input + 3).ToString());

            // Executing and verifying result.
            var result = "";
            foreach (var idx in functions.Braid(new int[] { 1, 3 })) {
                result += idx;
            }
            Assert.AreEqual("25", result);
        }

        /// <summary>
        /// Ensures that Braid evaluates correctly.
        /// </summary>
        [Test]
        public void Braid_3()
        {
            // Creating a Functions object.
            var functions = new Functions<int, string>();
            functions.Add((input) => (input + 1).ToString());
            functions.Add((input) => (input + 2).ToString());
            functions.Add((input) => (input + 3).ToString());
            functions.Add((input) => (input + 4).ToString());

            // Executing and verifying result.
            var result = "";
            foreach (var idx in functions.Braid(new int[] { 1, 2, 3 })) {
                result += idx;
            }
            Assert.AreEqual("246", result);
        }

        /// <summary>
        /// Verifies that a Functions with one argument is correctly
        /// executed when using Wrap.
        /// </summary>
        [Test]
        public void Wrap_1()
        {
            /*
             * Used to hold our result.
             */
            var result = "initial_";

            /*
             * Creating a Sequence and adding two Actions to it.
             */
            var functions = new Functions<string, string>();
            functions.Add((arg) => arg + "x");
            functions.Add((arg) => arg + "xx");
            functions.Add((arg) => arg + "xxx");

            /*
             * Wrapping our Sequence.
             */
            foreach (var idx in functions.Wrap(new string[] { "1", "2", "3" })) {
                result += idx;
            }

            /*
             * Making sure we got the result we expected.
             */
            Assert.AreEqual("initial_1x2xx3xxx", result);
        }

        /// <summary>
        /// Verifies that a Functions with one argument is correctly
        /// executed when using Wrap.
        /// </summary>
        [Test]
        public void Wrap_2()
        {
            /*
             * Used to hold our result.
             */
            var result = "initial_";

            /*
             * Creating a Sequence and adding two Actions to it.
             */
            var functions = new Functions<string, string>();
            functions.Add((arg) => arg + "x");
            functions.Add((arg) => arg + "xx");
            functions.Add((arg) => arg + "xxx");

            /*
             * Wrapping our Sequence.
             */
            foreach (var idx in functions.Wrap(new string[] { "1", "2", "3", "4" })) {
                result += idx;
            }

            /*
             * Making sure we got the result we expected.
             */
            Assert.AreEqual("initial_1x2xx3xxx4x", result);
        }

        /// <summary>
        /// Verifies that a Functions with one argument is correctly
        /// executed when using Wrap.
        /// </summary>
        [Test]
        public void Wrap_3()
        {
            /*
             * Used to hold our result.
             */
            var result = "initial_";

            /*
             * Creating a Sequence and adding two Actions to it.
             */
            var functions = new Functions<string, string>();
            functions.Add((arg) => arg + "x");
            functions.Add((arg) => arg + "xx");
            functions.Add((arg) => arg + "xxx");
            functions.Add((arg) => arg + "xxxx");

            /*
             * Wrapping our Sequence.
             */
            foreach (var idx in functions.Wrap(new string[] { "1", "2", "3" })) {
                result += idx;
            }

            /*
             * Making sure we got the result we expected.
             */
            Assert.AreEqual("initial_1x2xx3xxx1xxxx", result);
        }
    }
}
