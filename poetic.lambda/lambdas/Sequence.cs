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
using System.Collections.Generic;

namespace poetic.lambda.lambdas
{
    /// <summary>
    /// Class encapsulating a list of Action delegates taking no arguments.
    /// </summary>
    public class Sequence : SequenceBase<Action>
    {
        /// <summary>
        /// Executes all lambdas in a sequence on the calling thread.
        /// </summary>
        public void Sequential()
        {
            Sequential((action) => action());
        }

        /// <summary>
        /// Creates one thread for each of your actions, and execute the action
        /// on this thread in "fire and forget" mode.
        /// </summary>
        public void Parallel()
        {
            Parallel((action) => action());
        }

        /// <summary>
        /// Creates one thread for each of your actions, and execute the action
        /// on this thread not returning control to caller before all threads are
        /// finished with their work.
        /// </summary>
        public void JoinParallel()
        {
            JoinParallel((action) => action());
        }

        /// <summary>
        /// Creates one thread for each of your actions, and execute the action
        /// on this thread not returning control to caller before all threads are
        /// finished with their work, unless milliseconds amount of time has passed,
        /// at which point it stops waiting for the thread to finish its work.
        /// </summary>
        public void JoinParallel(int milliseconds)
        {
            JoinParallel(milliseconds, (action) => action());
        }
    }

    /// <summary>
    /// Class encapsulating a list of Action delegates taking one arguments.
    /// </summary>
    public class Sequence<T1> : SequenceBase<Action<T1>>
    {
        /// <summary>
        /// Executes all lambdas in a sequence on the calling thread.
        /// </summary>
        public void Sequential(T1 t1)
        {
            Sequential((action) => action(t1));
        }

        /// <summary>
        /// Braids each Action such that the args are sequentially applied in
        /// order of appearance. Will stop once there are no more args or no
        /// more Actions, whatever occurs first.
        /// </summary>
        /// <param name="args">Arguments.</param>
        public void Braid(IEnumerable<T1> args)
        {
            var argsIterator = args.GetEnumerator();
            var actionsIterators = GetEnumerator();
            while (actionsIterators.MoveNext() && argsIterator.MoveNext()) {
                actionsIterators.Current(argsIterator.Current);
            }
        }

        /// <summary>
        /// Creates one thread for each of your actions, and execute the action
        /// on this thread in "fire and forget" mode.
        /// </summary>
        public void Parallel(T1 t1)
        {
            Parallel((action) => action(t1));
        }

        /// <summary>
        /// Creates one thread for each of your actions, and execute the action
        /// on this thread not returning control to caller before all threads are
        /// finished with their work.
        /// </summary>
        public void JoinParallel(T1 t1)
        {
            JoinParallel((action) => action(t1));
        }

        /// <summary>
        /// Creates one thread for each of your actions, and execute the action
        /// on this thread not returning control to caller before all threads are
        /// finished with their work, unless milliseconds amount of time has passed,
        /// at which point it stops waiting for the thread to finish its work.
        /// </summary>
        public void JoinParallel(int milliseconds, T1 t1)
        {
            JoinParallel(milliseconds, (action) => action(t1));
        }
    }
}
