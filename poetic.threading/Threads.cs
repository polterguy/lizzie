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
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace poetic.threading
{
    /// <summary>
    /// Class allowing you create and start multiple threads.
    /// </summary>
    public class Threads
    {
        // List of delegates, where each delegate will become one thread.
        List<ThreadDelegate> _functors;

        /// <summary>
        /// A single thread delegate.
        /// </summary>
        public delegate void ThreadDelegate();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.threading.Threads"/> class.
        /// </summary>
        public Threads()
        {
            _functors = new List<ThreadDelegate>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.threading.Threads"/> class.
        /// </summary>
        /// <param name="functors">List of delegates, each becoming one thread.</param>
        public Threads(IEnumerable<ThreadDelegate> functors)
        {
            _functors = new List<ThreadDelegate>(functors);
        }

        /// <summary>
        /// Adds a delegate to the list of threads.
        /// </summary>
        /// <param name="functor">Functor.</param>
        public void Add(ThreadDelegate functor)
        {
            _functors.Add(functor);
        }

        /// <summary>
        /// Executes each of your delegates on a separate thread.
        /// </summary>
        public void Start()
        {
            _functors.ForEach(ix => new Thread(new ThreadStart(ix)).Start());
        }

        /// <summary>
        /// Executes each of your delegates on a separate thread, and waits for
        /// all your threads to finish their tasks.
        /// </summary>
        public void Join()
        {
            // Starting each thread, and waiting for all threads to finish before returning.
            var threads = _functors.Select(ix => new Thread(new ThreadStart(ix))).ToList();
            threads.ForEach(ix => ix.Start());
            threads.ForEach(ix => ix.Join());
        }

        /// <summary>
        /// Executes each of your delegates on a separate thread, and waits for
        /// all your threads to finish their tasks, making sure we never wait more
        /// than milliseconds amount of time, before returning.
        /// </summary>
        public void Join(int milliseconds)
        {
            // Sanity checking argument.
            if (milliseconds <= 0)
                throw new ArgumentException("Time must be a positive integer value", nameof(milliseconds));

            /*
             * Used to keep track of whether or not total amount of milliseconds have
             * passed or not.
             * 
             * NOTICE! Since starting a bunch of threads carries some overhead, we
             * do this before we create our threads, such that the total amount of
             * time we actually measure, becomes the total amount of time we spend
             * in this method, and not the total amount of "Join time".
             */
            var sw = Stopwatch.StartNew();

            // Starting each thread.
            var threads = _functors.Select(ix => new Thread(new ThreadStart(ix))).ToList();
            threads.ForEach(ix => ix.Start());

            /*
             * Iterating through each of our threads, making sure we never
             * wait more than milliseconds amount of time, before we give up, and
             * return control to caller.
             */
            foreach (var idx in threads) {

                /*
                 * Stopping stopwatch and decrementing time spent so far.
                 */
                sw.Stop();
                milliseconds -= (int)sw.ElapsedMilliseconds;

                /*
                 * Checking if total amount of time has elapsed.
                 */
                if (milliseconds <= 0)
                    break; // Time has left the rest of our threads hanging ...

                /*
                 * Restarting our Stopwatch to accurately time the Join time of
                 * our next thread's Join invocation.
                 */
                sw = Stopwatch.StartNew();

                // Making sure we never wait beyond our maximum amount of time.
                idx.Join(milliseconds);
            }
        }

        /// <summary>
        /// Executes each of your delegates on a separate thread, and waits for
        /// all your threads to finish their tasks, making sure we never wait more
        /// than time amount of time, before returning.
        /// </summary>
        public void Join(TimeSpan time)
        {
            // Sanity checking argument.
            if (time.TotalMilliseconds > (double)int.MaxValue)
                throw new ArgumentException("Maximum amount of time exceeded. " + int.MaxValue + " milliseconds is max value.", nameof(time));

            // Invoking common implementation method.
            Join((int)time.TotalMilliseconds);
        }
    }
}
