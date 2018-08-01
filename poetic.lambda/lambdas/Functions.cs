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
        public IEnumerable<TResult> JoinParallel()
        {
            var result = new List<TResult>();
            Synchronizer<List<TResult>> synchronizer = new Synchronizer<List<TResult>>(result);
            var lambdas = this.Select(ix => new Thread(new ThreadStart(delegate {
                var ixRes = ix();
                synchronizer.Write(list => list.Add(ixRes));
            }))).ToList();
            lambdas.ForEach(ix => ix.Start());
            lambdas.ForEach(ix => ix.Join());
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
        /// Braids each Action such that the args are sequentially applied in
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
    }
}
