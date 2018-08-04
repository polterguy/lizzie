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

namespace poetic.lambda.utilities
{
    /// <summary>
    /// Class allowing you to execute a list of actions.
    /// </summary>
    public static class Execute
    {
        /// <summary>
        /// Sequentially executes each action not returning before execution is finished.
        /// </summary>
        /// <param name="actions">Actions to execute.</param>
        public static void Sequentially(IEnumerable<Action> actions)
        {
            // Sequentially execute each action on calling thread.
            foreach (var ix in actions) {
                ix();
            }
        }

        /// <summary>
        /// Sequentially executes each action on a different thread, and blocking
        /// the calling thread until done, or millisecondsTimeout have passed.
        /// </summary>
        /// <param name="actions">Actions to execute.</param>
        /// <param name="millisecondsTimeout">Maximum amount of time to block calling thread.</param>
        public static void Sequentially(IEnumerable<Action> actions, int millisecondsTimeout)
        {
            // Making sure we have any work to do.
            if (!actions.Any())
                return; // Nothing to do here.

            // Checking that we have a valid timeout.
            if (millisecondsTimeout == -1) {

                // No threads needed, wait time is "forever".
                Sequentially(actions);
                return;
            }

            // Sanity checking arguments.
            if (millisecondsTimeout <= 0)
                throw new ArgumentException("Must be a positive integer value or -1 indicating blocked execution", nameof(millisecondsTimeout));

            // Executing actions on a different thread.
            var thread = new Thread(new ThreadStart(delegate {
                Sequentially(actions);
            }));
            thread.Start();

            // Waiting for execution to finish, or time to pass.
            thread.Join(millisecondsTimeout);
        }

        /// <summary>
        /// Sequentially executes each action on a different thread without blocking
        /// the calling thread.
        /// </summary>
        /// <param name="actions">Actions to execute.</param>
        public static void SequentiallyUnblocked(IEnumerable<Action> actions)
        {
            // Sanity checking argument.
            if (!actions.Any())
                return;

            // Executes actions on another thread.
            var thread = new Thread(new ThreadStart(delegate {
                Sequentially(actions);
            }));
            thread.Start();
        }

        /// <summary>
        /// Executes each action in parallel blocking the calling thread until
        /// all actions are finished executing.
        /// </summary>
        /// <param name="actions">Actions to execute.</param>
        public static void Parallel(IEnumerable<Action> actions)
        {
            // Sanity checking argument.
            if (!actions.Any())
                return;

            // Creates and starts a new thread for each action.
            var threads = actions.Select(ix => new Thread(new ThreadStart((() => ix())))).ToList();
            threads.ForEach(ix => ix.Start());
            threads.ForEach(ix => ix.Join());
        }

        /// <summary>
        /// Executes each action in parallel blocking the calling thread for a
        /// maximum amount of time, until execution of all actions are finished,
        /// or milliseconds have passed, whatever occurs first.
        /// </summary>
        /// <param name="actions">Actions to execute.</param>
        /// <param name="millisecondsTimeout">Maximum amount of time to block calling thread.</param>
        public static void Parallel(IEnumerable<Action> actions, int millisecondsTimeout)
        {
            // Making sure we have any work to do.
            if (!actions.Any())
                return; // Nothing to do here.

            // Checking that we have a valid timeout.
            if (millisecondsTimeout == -1) {

                // No synchronisation needed, wait time is "forever".
                Parallel(actions);
                return;
            }

            // Sanity checking argument.
            if (millisecondsTimeout <= 0)
                throw new ArgumentException("Must be a positive integer value or -1 indicating blocked execution", nameof(millisecondsTimeout));

            // Tracking time.
            var sw = Stopwatch.StartNew();

            // Starting each thread.
            var threads = actions.Select(ix => new Thread(new ThreadStart(delegate {
                ix();
            }))).ToList();
            threads.ForEach(ix => ix.Start());

            /*
             * Iterating through each of our threads, making sure we never
             * wait more than milliseconds amount of time, before we give up, and
             * return control to caller.
             */
            foreach (var idx in threads) {

                // Stopping stopwatch and decrementing time spent so far.
                sw.Stop();
                millisecondsTimeout -= (int)sw.ElapsedMilliseconds;

                // Checking if total amount of time has elapsed.
                if (millisecondsTimeout <= 0)
                    break; // Time has left the rest of our threads hanging ...

                // Restarting our Stopwatch to accurately measure time.
                sw = Stopwatch.StartNew();

                // Making sure we never wait beyond our maximum amount of time.
                idx.Join(millisecondsTimeout);
            }
        }

        /// <summary>
        /// Executes each action in parallel without blocking the calling thread.
        /// </summary>
        /// <param name="actions">Actions to execute.</param>
        public static void ParallelUnblocked(IEnumerable<Action> actions)
        {
            foreach (var ix in actions) {
                var thread = new Thread(new ThreadStart(delegate {
                    ix();
                }));
                thread.Start();
            }
        }
    }
}
