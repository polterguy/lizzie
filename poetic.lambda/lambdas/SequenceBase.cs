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

namespace poetic.lambda.lambdas
{
    /// <summary>
    /// Abstract Sequence base class containing helpers for creating your own sequences.
    /// </summary>
    public abstract class SequenceBase<TAction> : Lambdas<TAction>
    {
        /*
         * Protected implementation to make inheriting easier.
         */
        /// <summary>
        /// Sequentially executes each Action in order.
        /// </summary>
        /// <param name="action">Action.</param>
        protected void Sequential(Action<TAction> action)
        {
            foreach (var ix in this) {
                action(ix);
            }
        }

        /// <summary>
        /// Executes the sequence by creating one thread for each Action and
        /// executing each Action on a separate thread.
        /// </summary>
        /// <param name="action">Action.</param>
        protected void Parallel(Action<TAction> action)
        {
            var lambdas = this.Select(ix => new Thread(new ThreadStart(delegate {
                action(ix);
            }))).ToList();
            lambdas.ForEach(ix => ix.Start());
        }

        /// <summary>
        /// Executes the sequence on a different thread.
        /// </summary>
        /// <param name="action">Action.</param>
        protected void Parallelize(Action action)
        {
            var thread = new Thread(new ThreadStart(delegate {
                action();
            }));
            thread.Start();
        }

        /// <summary>
        /// Executes the sequence on a different thread for then to join,
        /// waiting maximum milliseconds amount of time before returning control
        /// back to caller.
        /// </summary>
        /// <param name="millisecondsTimeout">Milliseconds timeout.</param>
        /// <param name="action">Action.</param>
        protected void Parallelize(int millisecondsTimeout, Action action)
        {
            var wait = new EventWaitHandle(false, EventResetMode.ManualReset);
            var thread = new Thread(new ThreadStart(delegate {
                action();
                wait.Set();
            }));
            thread.Start();
            wait.WaitOne(millisecondsTimeout);
        }

        /// <summary>
        /// Executes the sequence by creating one thread for each Action and waits
        /// for each thread to finish before returning control back to caller.
        /// </summary>
        /// <param name="action">Action.</param>
        protected void Join(Action<TAction> action)
        {
            var lambdas = this.Select(ix => new Thread(new ThreadStart(delegate {
                action(ix);
            }))).ToList();
            lambdas.ForEach(ix => ix.Start());
            lambdas.ForEach(ix => ix.Join());
        }

        /// <summary>
        /// Executes the sequence by creating one thread for each Action and waits
        /// for each thread to finish before returning control back to caller. Will
        /// never wait more than the specified milliseconds before returning.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="milliseconds">Maximum amount of milliseconds to wait before returning control back to caller.</param>
        protected void Join(int milliseconds, Action<TAction> action)
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
            var threads = this.Select(ix => new Thread(new ThreadStart(delegate {
                action(ix);
            }))).ToList();
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
    }
}
