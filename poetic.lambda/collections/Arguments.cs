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

namespace poetic.lambda.collections
{
    public class Arguments<T> : List<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.Arguments`1"/> class.
        /// </summary>
        public Arguments()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.Arguments`1"/> class.
        /// </summary>
        /// <param name="lambdas">Initial functors.</param>
        public Arguments(params T [] lambdas)
            : base(lambdas)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.Arguments`1"/> class.
        /// </summary>
        /// <param name="lambdas">Initial functors.</param>
        public Arguments(IEnumerable<T> lambdas)
            : base(lambdas)
        { }

        /// <summary>
        /// Applies all arguments to the specified Action sequentially.
        /// </summary>
        /// <param name="action">Action to apply arguments to.</param>
        public void Apply(Action<T> action)
        {
            foreach (var idx in this) {
                action(idx);
            }
        }

        /// <summary>
        /// Creates and starts a new thread with the given Action once for each
        /// parameter in collection.
        /// </summary>
        /// <param name="action">Action.</param>
        public void Parallel(Action<T> action)
        {
            var lambdas = this.Select(ix => new Thread(new ThreadStart(delegate {
                action(ix);
            }))).ToList();
            lambdas.ForEach(ix => ix.Start());
        }

        /// <summary>
        /// Creates one new thread which it sequentially executes the specified
        /// Action one once for each argument.
        /// </summary>
        /// <param name="action">Action.</param>
        public void Parallelize(Action<T> action)
        {
            var thread = new Thread(new ThreadStart(delegate {
                Apply(action);
            }));
            thread.Start();
        }

        /// <summary>
        /// Creates and starts a new thread with the given Action once for each
        /// parameter in collection and waits for all thrads to finish before
        /// return control back to caller.
        /// </summary>
        /// <param name="action">Action.</param>
        public void Join (Action<T> action)
        {
            var lambdas = this.Select(ix => new Thread(new ThreadStart(delegate {
                action(ix);
            }))).ToList();
            lambdas.ForEach(ix => ix.Start());
            lambdas.ForEach(ix => ix.Join());
        }

        /// <summary>
        /// Creates and starts a new thread with the given Action once for each
        /// parameter in collection and waits for all thrads to finish before
        /// return control back to caller, but never waiting formore than
        /// milliseconds before returning control back to caller.
        /// </summary>
        /// <param name="action">Action.</param>
        public void Join(int milliseconds, Action<T> action)
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
