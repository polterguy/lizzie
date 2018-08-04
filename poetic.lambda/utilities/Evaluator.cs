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
using System.Collections.Generic;

namespace poetic.lambda.utilities
{
    /// <summary>
    /// Class allowing you to evaluate a list of functions.
    /// </summary>
    public static class Evaluator<TResult>
    {
        /// <summary>
        /// Evaluates each specified function in order and returns the result to caller.
        /// </summary>
        /// <returns>The sequence.</returns>
        /// <param name="functions">Functions.</param>
        public static IEnumerable<TResult> EvaluateSequentiallyBlocked(IEnumerable<Func<TResult>> functions)
        {
            // Sequentially execute each action on calling thread.
            foreach (var ix in functions) {
                yield return ix();
            }
        }

        /// <summary>
        /// Executes each function in parallel blocking the calling thread until
        /// all actions are finished executing, returning the results of the evaluation
        /// of each function.
        /// TODO: Make sure each thread returns its value immediately by using a list of ManualWaitHandles.
        /// </summary>
        /// <returns>The result of each function.</returns>
        /// <param name="functions">Functions to evaluate.</param>
        public static IEnumerable<TResult> EvaluateParallelBlocked(IEnumerable<Func<TResult>> functions)
        {
            // Sanity checking argument.
            if (!functions.Any())
                yield break;

            // Synchronising access to return values.
            var result = new List<TResult>();
            var sync = new Synchronizer<List<TResult>>(result);

            // Creates and starts a new thread for each action.
            var threads = functions.Select(ix => new Thread(new ThreadStart((delegate {
                var res = ix();
                sync.Write((shared) => shared.Add(res));
            })))).ToList();
            threads.ForEach(ix => ix.Start());
            threads.ForEach(ix => ix.Join());
            foreach (var ix in result) {
                yield return ix;
            }
        }

        /// <summary>
        /// Executes each function in parallel blocking the calling thread until
        /// all actions are finished executing, returning the results of the evaluation
        /// of each function.
        /// TODO: Make sure each thread returns its value immediately by using a list of ManualWaitHandles.
        /// </summary>
        /// <returns>The result of each function.</returns>
        /// <param name="functions">Functions to evaluate.</param>
        public static IEnumerable<TResult> EvaluateParallelBlocked(IEnumerable<Func<TResult>> functions, int millisecondsTimeout)
        {
            // Sanity checking argument.
            if (!functions.Any())
                yield break;

            // Sanity checking argument.
            if (millisecondsTimeout <= 0)
                throw new ArgumentException("Must be a positive integer value", nameof(millisecondsTimeout));

            // Synchronising access to return values.
            var list = new List<TResult>();
            var sync = new Synchronizer<List<TResult>>(list);

            // Making sure we're able to wait.
            ManualResetEvent wait = new ManualResetEvent(false);

            // Creates and starts a new thread for each action.
            var threads = functions.Select(ix => new Thread(new ThreadStart((delegate {
                var res = ix();
                sync.Write((shared) => shared.Add(res));
            })))).ToList();
            threads.ForEach(ix => ix.Start());
            wait.WaitOne(millisecondsTimeout);
            foreach (var ix in list) {
                yield return ix;
            }
        }
    }
}
