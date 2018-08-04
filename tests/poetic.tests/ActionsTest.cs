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
using NUnit.Framework;
using poetic.lambda.utilities;
using poetic.lambda.collections;

namespace poetic.tests
{
    [TestFixture]
    public class ActionsTest
    {
        [Test]
        public void Execute()
        {
            var result = "";

            var actions = new Actions();
            actions.Add(() => result += "foo");
            actions.Add(() => result += "bar");

            actions.Execute();
            Assert.AreEqual("foobar", result);
        }

        [Test]
        public void ExecuteArgs()
        {
            var sequence = new Actions<Mutable<string>>();
            sequence.Add((arg) => arg.Value += "foo");
            sequence.Add((arg) => arg.Value += "bar");

            var mutable = new Mutable<string>("initial_");
            sequence.Execute(mutable);
            Assert.AreEqual("initial_foobar", mutable.Value);
        }

        [Test]
        public void ExecuteParallel_1()
        {
            var sync = new Synchronizer<string>("initial_");

            var actions = new Actions();
            actions.Add(() => sync.Assign((res) => res + "foo"));
            actions.Add(() => sync.Assign((res) => res + "bar"));

            actions.ExecuteParallel();

            string result = null;
            sync.Read(delegate (string val) { result = val; });
            Assert.AreEqual(true, "initial_foobar" == result || result == "initial_barfoo");
        }

        [Test]
        public void ExecuteParallel_2()
        {
            var result = "";
            var wait = new ManualResetEvent(false);

            Actions actions = new Actions();
            actions.Add(delegate {

                result += "foo";
                wait.Set();
            });
            actions.Add(delegate {

                wait.WaitOne();
                result += "bar";
            });

            actions.ExecuteParallel();
            Assert.AreEqual("foobar", result);
        }

        [Test]
        public void ExecuteParallel_3()
        {
            var result = "";
            var wait = new ManualResetEvent(false);

            Actions actions = new Actions();
            actions.Add(delegate {

                wait.WaitOne();
                result += "foo";
            });
            actions.Add(delegate {

                result += "bar";
                wait.Set();
            });

            actions.ExecuteParallel();
            Assert.AreEqual("barfoo", result);
        }

        [Test]
        public void ExecuteParallelUnblocked()
        {
            var result = "";
            var waits = new ManualResetEvent[] {
                new ManualResetEvent(false),
                new ManualResetEvent(false),
                new ManualResetEvent(false)
            };

            var actions = new Actions();
            actions.Add(delegate {
                result += "1";
                waits[0].Set();
            });
            actions.Add(delegate {
                result += "2";
                waits[1].Set();
            });
            actions.Add(delegate {
                result += "3";
                waits[2].Set();
            });

            actions.ExecuteParallelUnblocked();
            WaitHandle.WaitAll(waits);
            var assert = result == "123" || result == "132" || result == "231" || result == "213" || result == "312" || result == "321";
            Assert.AreEqual(true, assert);
        }

        [Test]
        public void ExecuteParallelUnblockedArg()
        {
            var sync = new Synchronizer<string>("initial_");
            var waits = new EventWaitHandle[] {
                new EventWaitHandle (false, EventResetMode.ManualReset),
                new EventWaitHandle (false, EventResetMode.ManualReset),
                new EventWaitHandle (false, EventResetMode.ManualReset)
            };

            var actions = new Actions<Synchronizer<string>>();
            actions.Add(delegate (Synchronizer<string> input) {

                waits[1].WaitOne();
                input.Assign((current) => current + "1");
                waits[0].Set();
            });
            actions.Add(delegate (Synchronizer<string> input) {

                waits[2].WaitOne();
                input.Assign((current) => current + "2");
                waits[1].Set();
            });
            actions.Add(delegate (Synchronizer<string> input) {

                input.Assign((current) => current + "3");
                waits[2].Set();
            });

            actions.ExecuteParallelUnblocked(sync);
            WaitHandle.WaitAll(waits);
            string res = null;
            sync.Read(delegate (string val) { res = val; });
            Assert.AreEqual("initial_321", res);
        }
    }
}
