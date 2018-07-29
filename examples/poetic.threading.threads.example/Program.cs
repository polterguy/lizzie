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
using System;
using System.Threading;
using poetic.threading;

namespace poetic.threading.threads.example
{
    /// <summary>
    /// An dummy example of a class where instances of the class needs to be shared
    /// between multiple threads, and hence access to the instance needs to be
    /// synchronized.
    /// </summary>
    class Shared
    {
        string _data = "initial";

        /// <summary>
        /// Returns the state of the instance.
        /// </summary>
        /// <returns>The read.</returns>
        public string Read()
        {
            return _data;
        }

        /// <summary>
        /// Modifies the state of the instance.
        /// </summary>
        /// <param name="value">Value.</param>
        public void Write(string value)
        {
            _data += value;
        }
    }

    class MainClass
    {
        /// <summary>
        /// An example of how to use the Threads class to spawn of multiple threads,
        /// with some slightly simplified syntax.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            /*
             * Creating our Threads instance, and adding two delegates to it.
             */
            var threads = new Threads();
            threads = threads.Add(delegate {
                Thread.Sleep(100);
            });
            threads = threads.Add(delegate {
                Thread.Sleep(100);
            });

            /*
             * Fire and forget execution of our threads.
             */
            threads.Start();

            /*
             * Creating multiple threads, waiting for each of our threads to finish
             * their execution, before we proceed.
             */
            string thread1_result = "", thread2_result = "";
            threads = new Threads();
            threads = threads.Add(delegate {
                thread1_result = "Thread 1 done";
            });
            threads = threads.Add(delegate {
                thread2_result = "Thread 2 done";
            });

            /*
             * This will start all of our delegates on a separate thread, and
             * wait for all threads to finish.
             */
            threads.Join();

            /*
             * Proving that our threads actually modified our strings.
             */
            Console.WriteLine(thread1_result);
            Console.WriteLine(thread2_result);
            Console.WriteLine();

            /*
             * Creating multiple threads, waiting for each of our threads to finish
             * their execution, before we proceed. But never waiting more than
             * 1 second in total, before allowing execution to continue, without
             * waiting for our threads to finish their work.
             * 
             * NOTICE!
             * Since we are not waiting indefinitely here, we arguably SHOULD have
             * used a Synchronizer instance to wrap our string result, since we
             * could in theory have a race condition between one of our threads,
             * and our Console.WriteLine invocation further down. However, to
             * keep the example easily understood, and only illustrating our
             * Threads class, without any additional confusion added to our code,
             * we have avoided doing this, for clarity reasons.
             * 
             * However, as a general rule, if you use the Join override, with a
             * timeout value, you should ALWAYS Synchronise access to any shared
             * resources if you have an instance of an object that is shared
             * between two of your threads, and/or one of your threads, and your
             * "main thread" - Which is what we are actually doing here.
             */
            thread1_result = "Thread 1 is not done (which is an error)";
            thread2_result = "Thread 2 is not done (CORRECT!)";
            var thread3_result = "Thread 3 is not done (which is an error)";
            threads = new Threads();
            threads = threads.Add(delegate {

                /*
                 * Sleeping thread for 500 milliseconds, which is not enough in
                 * isolation to passover the total wait time.
                 */
                Thread.Sleep(500);
                thread1_result = "Thread 1 done (CORRECT!)";
            });
            threads = threads.Add(delegate {

                /*
                 * Making sure we sleep the current thread, such that it won't
                 * be finished before our Join invocation below has timed out.
                 */
                Thread.Sleep(2500);

                /*
                 * Access to this string SHOULD have been synchronised.
                 * 
                 * Read the comment above for details about why this is not done.
                 */
                thread2_result = "Thread 2 done (ERROR!)";
            });
            threads = threads.Add(delegate {

                /*
                 * Sleeping thread for 500 milliseconds, which is not enough in
                 * isolation to passover the total wait time.
                 */
                Thread.Sleep(500);
                thread3_result = "Thread 3 done (CORRECT!)";
            });

            /*
             * This will start all of our delegates on a separate thread, and
             * wait for all threads to finish.
             */
            threads.Join(1000);

            /*
             * Proving that our threads actually modified our strings.
             */
            Console.WriteLine(thread1_result);
            Console.WriteLine(thread2_result);
            Console.WriteLine(thread3_result);
            Console.WriteLine();

            /*
             * Example of synchronised sharing some object (securely) between
             * multiple threads.
             * 
             * Notice, this time we instantiate our Threads instance such that we
             * can pass in a Shared instance to our threads, synchronising access
             * to our instance at the same time.
             */
            var threads2 = new Threads<Synchronizer<Shared>>();

            /*
             * Adding three thread delegates to our Threads instance.
             */
            threads2 = threads2.Add(delegate (Synchronizer<Shared> synchronizer) {

                // Accessing Shared instance synchronised.
                synchronizer.Read (delegate (Shared shared_instance) {
                    thread1_result = shared_instance.Read();
                });
            });
            threads2 = threads2.Add(delegate (Synchronizer<Shared> synchronizer) {

                // Making sure we don't fetch value before thread 2 is done.
                Thread.Sleep(1000);

                // Accessing Shared instance synchronised.
                synchronizer.Write(delegate (Shared shared_instance) {
                    shared_instance.Write(" thread 2 was here!");
                });
            });
            threads2 = threads2.Add(delegate (Synchronizer<Shared> synchronizer) {

                // Making sure we don't fetch value before thread 2 is done.
                Thread.Sleep(2000);

                // Accessing Shared instance synchronised.
                synchronizer.Read(delegate (Shared shared_instance) {
                    thread3_result = shared_instance.Read();
                });
            });

            /*
             * Executing all of our threads, such that we'll wait the main thread
             * until all threads are finished.
             */
            threads2.Join(new Synchronizer<Shared> (new Shared()));

            /*
             * Writing results to Console.
             */
            Console.WriteLine(thread1_result);
            Console.WriteLine(thread3_result);
        }
    }
}
