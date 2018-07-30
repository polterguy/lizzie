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
using System.Collections;
using System.Collections.Generic;

namespace poetic.lambda
{
    /// <summary>
    /// Base class for all Lambdas generic classes.
    /// </summary>
    public class Lambdas<TLambda> : IEnumerable<TLambda>
    {
        // List of delegates.
        readonly List<TLambda> _lambdas;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.delegates.Chain`2"/> class.
        /// </summary>
        public Lambdas()
        {
            _lambdas = new List<TLambda>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.delegates.Chain`2"/> class.
        /// </summary>
        /// <param name="functors">Initial functors.</param>
        public Lambdas(params TLambda[] functors)
        {
            _lambdas = new List<TLambda>(functors);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.delegates.Chain`2"/> class.
        /// </summary>
        /// <param name="functors">Initial functors.</param>
        public Lambdas(IEnumerable<TLambda> functors)
        {
            _lambdas = new List<TLambda>(functors);
        }

        /// <summary>
        /// Adds the specified functor to the list of delegates.
        /// </summary>
        /// <param name="functor">Functor to add to chain.</param>
        public void Add(TLambda functor)
        {
            _lambdas.Add(functor);
        }

        /// <summary>
        /// Adds the specified functors to the functors.
        /// </summary>
        /// <param name="functors">Functors to add.</param>
        public void AddRange(params TLambda[] functors)
        {
            _lambdas.AddRange(functors);
        }

        /// <summary>
        /// Adds the specified functors to the functors.
        /// </summary>
        /// <param name="functors">Functors to add.</param>
        public void AddRange(IEnumerable<TLambda> functors)
        {
            _lambdas.AddRange(functors);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<TLambda> GetEnumerator()
        {
            return _lambdas.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _lambdas.GetEnumerator();
        }

        /*
         * Protected implementation to make inheriting easier.
         */
        protected void Sequence(Action<TLambda> action)
        {
            foreach (var ix in this) {
                action(ix);
            }
        }

        /*
         * Protected implementation to make inheriting easier.
         */
        protected void Forget(Action<TLambda> action)
        {
            var lambdas = this.Select(ix => new Thread(new ThreadStart(delegate {
                action(ix);
            }))).ToList();
            lambdas.ForEach(ix => ix.Start());
        }

        /*
         * Protected implementation to make inheriting easier.
         */
        protected void Join(Action<TLambda> action)
        {
            var lambdas = this.Select(ix => new Thread(new ThreadStart(delegate {
                action(ix);
            }))).ToList();
            lambdas.ForEach(ix => ix.Start());
            lambdas.ForEach(ix => ix.Join());
        }

        /*
         * Protected implementation to make inheriting easier.
         */
        protected void Join(Action<TLambda> action, int milliseconds)
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
