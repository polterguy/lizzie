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
using System.Threading;
using System.Collections.Generic;
using NUnit.Framework;
using poetic.lambda.collections;
using poetic.lambda.utilities;

namespace poetic.tests
{
    [TestFixture]
    public class ArgumentsTest
    {
        [Test]
        public void Apply()
        {
            var arguments = new Arguments<int>(1, 2, 3);
            var result = 0;
            arguments.Apply(delegate (int input) {
                result += input;
            });

            Assert.AreEqual(6, result);
        }

        [Test]
        public void Parallel()
        {
            var arguments = new Arguments<int>(1, 2, 3);

            var wait = new EventWaitHandle(false, EventResetMode.ManualReset);
            var sync = new Synchronizer<List<int>>(new List<int>());

            var result = 0;
            arguments.Parallel(delegate (int input) {
                sync.Write(delegate (List<int> list) {
                    result += input;
                    list.Add(input);
                    if (list.Count == arguments.Count)
                        wait.Set();
                });
            });

            wait.WaitOne();
            Assert.AreEqual(6, result);
        }

        [Test]
        public void Parallelize()
        {
            var arguments = new Arguments<int>(1, 2, 3);

            var wait = new EventWaitHandle(false, EventResetMode.ManualReset);
            var sync = new Synchronizer<List<int>>(new List<int>());

            var result = 0;
            arguments.Parallelize(delegate (int input) {
                sync.Write(delegate (List<int> list) {
                    result += input;
                    list.Add(input);
                    if (list.Count == arguments.Count)
                        wait.Set();
                });
            });

            wait.WaitOne();
            Assert.AreEqual(6, result);
        }

        [Test]
        public void Join()
        {
            var arguments = new Arguments<int>(1, 2, 3);

            var sync = new Synchronizer<int>(0);
            arguments.Join(delegate (int input) {
                sync.Assign((ix) => ix + input);
            });
            var result = 0;
            sync.Read(delegate(int input) {
                result = input;
            });
            Assert.AreEqual(6, result);
        }

        [Test]
        public void JoinTimeout()
        {
            var arguments = new Arguments<int>(1, 2, 3);

            var sync = new Synchronizer<int>(0);
            arguments.Join(500, delegate (int input) {
                if (input == 3)
                    Thread.Sleep(2000);
                sync.Assign((ix) => ix + input);
            });
            var result = 0;
            sync.Read(delegate (int input) {
                result = input;
            });
            Assert.AreEqual(3, result);
        }
    }
}
