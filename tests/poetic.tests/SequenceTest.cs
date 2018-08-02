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
using poetic.lambda.lambdas;
using poetic.lambda.utilities;

namespace poetic.tests
{
    [TestFixture]
    public class SequenceTest
    {
        /// <summary>
        /// Verifies that a Sequence with one argument is correctly
        /// executed when using Braid.
        /// </summary>
        [Test]
        public void Braid_1()
        {
            /*
             * Used to hold our result.
             */
            var result = "initial_";

            /*
             * Creating a Sequence and adding two Actions to it.
             */
            var sequence = new Sequence<string>();
            sequence.Add((arg) => result += arg);
            sequence.Add((arg) => result += arg);
            sequence.Add((arg) => result += arg);

            /*
             * Braiding our Sequence.
             */
            sequence.Braid(new string[] { "1", "2", "3" });

            /*
             * Making sure we got the result we expected.
             */
            Assert.AreEqual("initial_123", result);
        }

        /// <summary>
        /// Verifies that a Sequence with one argument is correctly
        /// executed when using Braid.
        /// </summary>
        [Test]
        public void Braid_2()
        {
            /*
             * Used to hold our result.
             */
            var result = "initial_";

            /*
             * Creating a Sequence and adding two Actions to it.
             */
            var sequence = new Sequence<string>();
            sequence.Add((arg) => result += arg);
            sequence.Add((arg) => result += arg);
            sequence.Add((arg) => result += arg + "ERROR");

            /*
             * Braiding our Sequence, passing in one argument too little.
             */
            sequence.Braid(new string[] { "1", "2" });

            /*
             * Making sure we got the result we expected.
             */
            Assert.AreEqual("initial_12", result);
        }

        /// <summary>
        /// Verifies that a Sequence with one argument is correctly
        /// executed when using Braid.
        /// </summary>
        [Test]
        public void Braid_3()
        {
            /*
             * Used to hold our result.
             */
            var result = "initial_";

            /*
             * Creating a Sequence and adding two Actions to it.
             */
            var sequence = new Sequence<string>();
            sequence.Add((arg) => result += arg);
            sequence.Add((arg) => result += arg);

            /*
             * Braiding our Sequence, passing in one argument too much.
             */
            sequence.Braid(new string[] { "1", "2", "3" });

            /*
             * Making sure we got the result we expected.
             */
            Assert.AreEqual("initial_12", result);
        }

        /// <summary>
        /// Verifies that a Sequence with no generic arguments executes correctly
        /// when using Parallel execution.
        /// </summary>
        [Test]
        public void Join_1()
        {
            /*
             * Used to hold our result and synchronise access to it.
             * Notice, this time we'll need synchronised access to our result
             * value, since each Action will be executed on a separate thread.
             */
            var sync = new Synchronizer<string>("initial_");

            // Creating a Sequence and adding two Actions to it.
            var sequence = new Sequence();
            sequence.Add(() => sync.Assign((res) => res + "foo"));
            sequence.Add(() => sync.Assign((res) => res + "bar"));

            /*
             * Executing Sequence parallel making sure we wait for all threads
             * to finish.
             */
            sequence.Join();

            /*
             * Notice, this time the execution of the Actions are undetermined,
             * hence we've got two possibly results that might occur.
             */
            var result = sync.Get();
            Assert.AreEqual(true, "initial_foobar" == result || result == "initial_barfoo");
        }

        /// <summary>
        /// Verifies that a Sequence with no generic arguments executes correctly
        /// when using Parallel execution.
        /// </summary>
        [Test]
        public void Join_2()
        {
            /*
             * Used to hold our result and synchronise access to it.
             */
            var result = "";

            // Making sure we can determine order of execution of our Parallel.
            var wait = new EventWaitHandle(false, EventResetMode.ManualReset);

            // Creating a Sequence and adding two Actions to it.
            Sequence sequence = new Sequence();
            sequence.Add(delegate {

                /*
                 * Updating reference and allowing the other thread to do its work.
                 */
                result += "foo";
                wait.Set();

            });
            sequence.Add(delegate {

                /*
                 * Waiting until the other thread has done its work.
                 */
                wait.WaitOne(1000);
                result += "bar";

            });

            /*
             * Executing Sequence parallel making sure we wait for all threads
             * to finish.
             */
            sequence.Join();

            /*
             * Notice, this time the execution of the Actions are determined due to our WaitHandle.
             */
            Assert.AreEqual("foobar", result);
        }

        /// <summary>
        /// Verifies that a Sequence with no generic arguments executes correctly
        /// when using Parallel execution.
        /// </summary>
        [Test]
        public void Join_3()
        {
            /*
             * Used to hold our result and synchronise access to it.
             */
            var result = "";

            // Making sure we can determine order of execution of our Parallel.
            var wait = new EventWaitHandle(false, EventResetMode.ManualReset);

            // Creating a Sequence and adding two Actions to it.
            Sequence sequence = new Sequence();
            sequence.Add(delegate {

                /*
                 * Waiting until the other thread has done its work.
                 */
                wait.WaitOne(1000);
                result += "foo";

            });
            sequence.Add(delegate {

                /*
                 * Updating reference and allowing the other thread to do its work.
                 */
                result += "bar";
                wait.Set();

            });

            /*
             * Executing Sequence parallel making sure we wait for all threads
             * to finish.
             */
            sequence.Join();

            /*
             * Notice, this time the execution of the Actions are determined due to our WaitHandle.
             */
            Assert.AreEqual("barfoo", result);
        }

        /// <summary>
        /// Verifies that a Sequence with no generic arguments executes correctly
        /// when using Parallel execution.
        /// </summary>
        [Test]
        public void JoinTimeout()
        {
            /*
             * Used to hold our result and synchronise access to it.
             */
            var result = "";

            // Creating a Sequence and adding two Actions to it.
            Sequence sequence = new Sequence();
            sequence.Add(delegate {

                /*
                 * Updating reference and allowing the other thread to do its work.
                 */
                result += "foo";

            });
            sequence.Add(delegate {

                /*
                 * Waiting until the other thread has done its work.
                 */
                Thread.Sleep(1000);
                result += "bar";

            });

            /*
             * Executing Sequence parallel making sure we wait for all threads
             * to finish.
             */
            sequence.Join(500);

            /*
             * Notice, this time the execution of the Actions are determined due to our WaitHandle.
             */
            Assert.AreEqual("foo", result);
        }

        /// <summary>
        /// Verifies that a Sequence with no generic arguments executes correctly
        /// when using Parallel execution without Join.
        /// </summary>
        [Test]
        public void Parallel()
        {
            // Used to hold our result.
            var result = "";

            // Making sure we can determine order of execution of threads.
            var waits = new EventWaitHandle[] {
                new EventWaitHandle (false, EventResetMode.ManualReset),
                new EventWaitHandle (false, EventResetMode.ManualReset),
                new EventWaitHandle (false, EventResetMode.ManualReset)
            };

            // Creating a Sequence and adding 3 Actions to it.
            var sequence = new Sequence();
            sequence.Add(delegate {

                /*
                 * Waiting for second thread, updating reference, and doing this
                 * thread's work.
                 */
                waits[1].WaitOne(1000);
                result += "1";
                waits[0].Set();

            });
            sequence.Add(delegate {

                /*
                 * Waiting for third thread, updating reference, and doing this
                 * thread's work.
                 */
                waits[2].WaitOne(1000);
                result += "2";
                waits[1].Set();

            });
            sequence.Add(delegate {

                /*
                 * Updating reference and allowing the other thread to do its work.
                 */
                result += "3";
                waits[2].Set();

            });

            /*
             * Executing Sequence parallel making sure we wait for all threads
             * to finish.
             */
            sequence.Parallel();

            /*
             * Making sure we wait until all threads have finished.
             */
            WaitHandle.WaitAll(waits);

            /*
             * Notice, this time the execution of the Actions are determined due to our WaitHandle.
             */
            Assert.AreEqual("321", result);
        }

        /// <summary>
        /// Verifies that a Sequence with one argument executes correctly
        /// when using Parallel execution without Join.
        /// </summary>
        [Test]
        public void ParallelArgs()
        {
            // Used to hold our result.
            var sync = new Synchronizer<string>("initial_");

            // Making sure we can determine order of execution of threads.
            var waits = new EventWaitHandle[] {
                new EventWaitHandle (false, EventResetMode.ManualReset),
                new EventWaitHandle (false, EventResetMode.ManualReset),
                new EventWaitHandle (false, EventResetMode.ManualReset)
            };

            // Creating a Sequence and adding 3 Actions to it.
            var sequence = new Sequence<Synchronizer<string>>();
            sequence.Add(delegate (Synchronizer<string> input) {

                /*
                 * Waiting for second thread, updating reference, and doing this
                 * thread's work.
                 */
                waits[1].WaitOne(1000);
                input.Assign((current) => current + "1");
                waits[0].Set();

            });
            sequence.Add(delegate (Synchronizer<string> input) {

                /*
                 * Waiting for third thread, updating reference, and doing this
                 * thread's work.
                 */
                waits[2].WaitOne(1000);
                input.Assign((current) => current + "2");
                waits[1].Set();

            });
            sequence.Add(delegate (Synchronizer<string> input) {

                /*
                 * Updating reference and allowing the other thread to do its work.
                 */
                input.Assign((current) => current + "3");
                waits[2].Set();

            });
            /*
             * Executing Sequence parallel making sure we wait for all threads
             * to finish.
             */
            sequence.Parallel(sync);

            /*
             * Making sure we wait until all threads have finished.
             */
            WaitHandle.WaitAll(waits);

            /*
             * Notice, this time the execution of the Actions are determined due to our WaitHandle.
             */
            Assert.AreEqual("initial_321", sync.Get());
        }

        /// <summary>
        /// Verifies that a Sequence can be parallelized.
        /// </summary>
        [Test]
        public void Parallelize()
        {
            // Used to hold our result.
            var result = "";

            // Making sure we are able to wait for thread to finish execution.
            var wait = new EventWaitHandle(false, EventResetMode.ManualReset);

            // Creating a Sequence and adding 3 Actions to it.
            var sequence = new Sequence();
            sequence.Add(delegate {
                result += "1";
            });
            sequence.Add(delegate {
                result += "2";

            });
            sequence.Add(delegate {
                result += "3";
                wait.Set();

            });

            /*
             * Executing Sequence parallelized.
             */
            sequence.Parallelize();

            /*
             * Making sure we wait until thread is finished.
             */
            wait.WaitOne();

            /*
             * Verifying order of execution.
             */
            Assert.AreEqual("123", result);
        }

        /// <summary>
        /// Verifies that a Sequence can be parallelized.
        /// </summary>
        [Test]
        public void ParallelizeTimeout()
        {
            // Used to hold our result.
            var result = "";

            // Creating a Sequence and adding 3 Actions to it.
            var sequence = new Sequence();
            sequence.Add(delegate {
                result += "1";
            });
            sequence.Add(delegate {
                result += "2";
            });
            sequence.Add(delegate {
                result += "3";
                Thread.Sleep(2000);
            });
            sequence.Add(delegate {
                result += "4";
            });

            /*
             * Executing Sequence parallelized.
             */
            sequence.Parallelize(500);

            /*
             * Verifying order of execution.
             */
            Assert.AreEqual("123", result);
        }

        /// <summary>
        /// Verifies that a Sequence with no arguments behaves as expected
        /// when executing it sequentially.
        /// </summary>
        [Test]
        public void Sequential()
        {
            // Used to hold our result.
            var result = "";

            // Creating a Sequence and adding two Actions to it.
            var sequence = new Sequence();
            sequence.Add(() => result += "foo");
            sequence.Add(() => result += "bar");

            /*
             * Executing Sequence sequentially.
             */
            sequence.Sequential();

            /*
             * Making sure we got the result we expected.
             */
            Assert.AreEqual("foobar", result);
        }

        /// <summary>
        /// Verifies that a Sequence with one argument is correctly
        /// executed when using Sequential.
        /// </summary>
        [Test]
        public void SequentialArgs()
        {
            /*
             * Creating a Sequence and adding two Actions to it.
             *
             * Notice, since string is immutable, we'll need to create a Mutable
             * instance wrapping our string, such that we can assign the reference
             * as we proceed through our Actions.
             * 
             * Otherwise we would not change the string itself, but only update
             * the reference, pointing to a new string, not allowing us to
             * actually change the string, but only update the reference we're
             * given inside of our Action.
             */
            var sequence = new Sequence<Mutable<string>>();
            sequence.Add((arg) => arg.Value += "foo");
            sequence.Add((arg) => arg.Value += "bar");

            /*
             * Creating our Mutable instance and executing our Sequence
             * sequentially, passing in our Mutable as its sequential argument.
             */
            var mutable = new Mutable<string>("initial_");
            sequence.Sequential(mutable);

            /*
             * Making sure we got the result we expected.
             */
            Assert.AreEqual("initial_foobar", mutable.Value);
        }

        /// <summary>
        /// Verifies that a Sequence with no arguments behaves as expected
        /// when executing it sequentially after first having reversed it.
        /// </summary>
        [Test]
        public void SequentialReverse()
        {
            // Used to hold our result.
            var result = "";

            // Creating a Sequence and adding two Actions to it.
            var sequence = new Sequence();
            sequence.Add(() => result += "foo");
            sequence.Add(() => result += "bar");

            /*
             * Reversing content of Sequence.
             */
            sequence.Reverse();

            /*
             * Executing Sequence sequentially.
             */
            sequence.Sequential();

            /*
             * Making sure we got the result we expected.
             */
            Assert.AreEqual("barfoo", result);
        }

        /// <summary>
        /// Verifies that a Sequence with one argument is correctly
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
            var sequence = new Sequence<string>();
            sequence.Add((arg) => result += arg);
            sequence.Add((arg) => result += arg);
            sequence.Add((arg) => result += arg);

            /*
             * Wrapping our Sequence.
             */
            sequence.Wrap(new string[] { "1", "2", "3" });

            /*
             * Making sure we got the result we expected.
             */
            Assert.AreEqual("initial_123", result);
        }

        /// <summary>
        /// Verifies that a Sequence with one argument is correctly
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
            var sequence = new Sequence<string>();
            sequence.Add((arg) => result += arg);
            sequence.Add((arg) => result += arg);
            sequence.Add((arg) => result += arg);

            /*
             * Braiding our Sequence, passing in one argument too little.
             */
            sequence.Wrap(new string[] { "1", "2" });

            /*
             * Making sure we got the result we expected.
             */
            Assert.AreEqual("initial_121", result);
        }

        /// <summary>
        /// Verifies that a Sequence with one argument is correctly
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
            var sequence = new Sequence<string>();
            sequence.Add((arg) => result += arg + "x");
            sequence.Add((arg) => result += arg);

            /*
             * Braiding our Sequence, passing in one argument too much.
             */
            sequence.Wrap(new string[] { "1", "2", "3" });

            /*
             * Making sure we got the result we expected.
             */
            Assert.AreEqual("initial_1x23x", result);
        }

        /// <summary>
        /// Verifies that a Sequence can be parallelized with one argument.
        /// </summary>
        [Test]
        public void ParallelizeArgs()
        {
            // Used to hold our result.
            var result = new Mutable<string>();

            // Making sure we are able to wait for thread to finish execution.
            var wait = new EventWaitHandle(false, EventResetMode.ManualReset);

            // Creating a Sequence and adding 3 Actions to it.
            var sequence = new Sequence<Mutable<string>>();
            sequence.Add((input) => input.Value += "1");
            sequence.Add((input) => input.Value += "2");
            sequence.Add(delegate (Mutable<string> input) {
                input.Value += "3";
                wait.Set();
            });

            /*
             * Executing Sequence parallelized.
             */
            sequence.Parallelize(result);

            /*
             * Making sure we wait until thread is finished.
             */
            wait.WaitOne();

            /*
             * Verifying order of execution.
             */
            Assert.AreEqual("123", result.Value);
        }
    }
}
