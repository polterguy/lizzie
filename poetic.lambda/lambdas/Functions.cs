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
using poetic.lambda.utilities;

namespace poetic.lambda.lambdas
{
    /// <summary>
    /// Class encapsulating a list of Func delegates taking no arguments.
    /// </summary>
    public class Functions<TResult> : Lambdas<Func<TResult>>
    {
        /// <summary>
        /// Executes all lambdas in a sequence on the calling thread.
        /// </summary>
        public IEnumerable<TResult> Sequential()
        {
            foreach (var ix in this) {
                yield return ix();
            }
        }

        /// <summary>
        /// Creates one thread for each of your Functions, and execute the function
        /// on this thread not returning control to caller before all threads are
        /// finished with their work. Returns the result of each thread back to caller
        /// afterwards.
        /// </summary>
        public IEnumerable<TResult> Join()
        {
            var result = new List<TResult>();
            Synchronizer<List<TResult>> synchronizer = new Synchronizer<List<TResult>>(result);
            var threads = this.Select(ix => new Thread(new ThreadStart(delegate {
                var ixRes = ix();
                synchronizer.Write(list => list.Add(ixRes));
            }))).ToList();
            threads.ForEach(ix => ix.Start());
            threads.ForEach(ix => ix.Join());
            foreach (var idxRes in result) {
                yield return idxRes;
            }
        }

        /// <summary>
        /// Creates one thread for each of your Functions, and execute the function
        /// on this thread not returning control to caller before all threads are
        /// finished with their work. Returns the result of each thread back to caller
        /// afterwards.
        /// </summary>
        public IEnumerable<TResult> Join(int milliseconds)
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

            // Starting threads, keeping track of returned values as we go.
            var result = new List<TResult>();
            Synchronizer<List<TResult>> synchronizer = new Synchronizer<List<TResult>>(result);
            var threads = this.Select(ix => new Thread(new ThreadStart(delegate {
                var ixRes = ix();
                synchronizer.Write(list => list.Add(ixRes));
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

            // Yielding results.
            foreach (var idxRes in result) {
                yield return idxRes;
            }
        }
    }

    /// <summary>
    /// Class encapsulating a list of Func delegates taking one arguments.
    /// </summary>
    public class Functions<T1, TResult> : Lambdas<Func<T1, TResult>>
    {
        /// <summary>
        /// Executes all lambdas in a sequence on the calling thread.
        /// </summary>
        public IEnumerable<TResult> Sequential(T1 t1)
        {
            foreach (var ix in this) {
                yield return ix(t1);
            }
        }

        /// <summary>
        /// Braids each Function such that the args are sequentially applied in
        /// order of appearance. Will stop once there are no more args or no
        /// more Actions, whatever occurs first.
        /// </summary>
        /// <param name="args">Arguments.</param>
        public IEnumerable<TResult> Braid(IEnumerable<T1> args)
        {
            var argsIterator = args.GetEnumerator();
            var actionsIterators = GetEnumerator();
            while (actionsIterators.MoveNext() && argsIterator.MoveNext()) {
                yield return actionsIterators.Current(argsIterator.Current);
            }
        }

        /// <summary>
        /// Braids each Action such that the args are sequentially applied in
        /// order of appearance. Will restart iteration of args or Actions,
        /// such that either all args are applied to an Action or all Actions
        /// are executed with an argument.
        /// </summary>
        /// <param name="args">Arguments.</param>
        public IEnumerable<TResult> Wrap(IEnumerable<T1> args)
        {
            var argsIterator = args.GetEnumerator();
            var actionsIterators = GetEnumerator();
            var moreArgs = argsIterator.MoveNext();
            var moreActions = actionsIterators.MoveNext();
            if (!moreActions || !moreActions)
                yield break; // No actions or no arguments.

            while (moreArgs || moreActions) {
                yield return actionsIterators.Current(argsIterator.Current);
                if (!argsIterator.MoveNext ()) {
                    moreArgs = false;
                    argsIterator = args.GetEnumerator();
                    argsIterator.MoveNext();
                }
                if (!actionsIterators.MoveNext ()) {
                    moreActions = false;
                    actionsIterators = GetEnumerator();
                    actionsIterators.MoveNext();
                }
            }
        }
    }
}
