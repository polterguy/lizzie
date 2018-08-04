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
using System.Threading;
using poetic.lambda.utilities;
using poetic.lambda.collections;

namespace poetic.tests
{
    [TestFixture]
    public class FunctionsTest
    {
        [Test]
        public void EvaluateSequentiallyBlocked()
        {
            var functions = new Functions<string>();
            functions.Add(() => "1");
            functions.Add(() => "2");
            functions.Add(() => "3");

            var result = "";
            foreach (var idx in functions.EvaluateSequentiallyBlocked()) {
                result += idx;
            }
            Assert.AreEqual("123", result);
        }

        [Test]
        public void EvaluateParallelBlocked()
        {
            var functions = new Functions<string>();
            functions.Add(() => "1");
            functions.Add(() => "2");
            functions.Add(() => "3");

            var sync = new Synchronizer<string> ("");
            foreach (var idx in functions.EvaluateParallelBlocked()) {
                sync.Assign((input) => input + idx);
            }
            bool assert = false;
            sync.Read((result) => assert = result == "123" || result == "132" || result == "231" ||
                result == "213" || result == "312" || result == "321");
            Assert.AreEqual(true, assert);
        }
    }
}
