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
        /// Creates one new thread and execute all Actions on this single thread.
        /// </summary>
        public void Parallelize()
        {
            Parallelize(Sequential);
        }

        /// <summary>
        /// Creates one new thread and execute all Actions on this single thread
        /// waiting for amaximum millisecondsTimeout before returning control back
        /// to caller.
        /// </summary>
        /// <param name="millisecondsTimeout">Milliseconds timeout.</param>
        public void Parallelize(int millisecondsTimeout)
        {
            Parallelize(millisecondsTimeout, Sequential);
        }

        /// <summary>
        /// Creates one thread for each of your actions, and execute the action
        /// on this thread not returning control to caller before all threads are
        /// finished with their work.
        /// </summary>
        public void Join()
        {
            Join((action) => action());
        }

        /// <summary>
        /// Creates one thread for each of your actions, and execute the action
        /// on this thread not returning control to caller before all threads are
        /// finished with their work, unless milliseconds amount of time has passed,
        /// at which point it stops waiting for the thread to finish its work.
        /// </summary>
        public void Join(int milliseconds)
        {
            Join(milliseconds, (action) => action());
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
        /// Braids each Action such that the args are sequentially applied in
        /// order of appearance. Will restart iteration of args or Actions,
        /// such that either all args are applied to an Action or all Actions
        /// are executed with an argument.
        /// </summary>
        /// <param name="args">Arguments.</param>
        public void Wrap(IEnumerable<T1> args)
        {
            var argsIterator = args.GetEnumerator();
            var actionsIterators = GetEnumerator();
            var moreArgs = argsIterator.MoveNext();
            var moreActions = actionsIterators.MoveNext();
            if (!moreActions || !moreActions)
                return; // No actions or no arguments.

            while (moreArgs || moreActions) {
                actionsIterators.Current(argsIterator.Current);
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

        /// <summary>
        /// Creates one thread for each of your actions, and execute the action
        /// on this thread in "fire and forget" mode.
        /// </summary>
        public void Parallel(T1 t1)
        {
            Parallel((action) => action(t1));
        }

        /// <summary>
        /// Creates one new thread and execute all Actions on this single thread.
        /// </summary>
        public void Parallelize(T1 t1)
        {
            Parallelize(() => Sequential(t1));
        }

        /// <summary>
        /// Creates one new thread and execute all Actions on this single thread,
        /// waiting for a maximum of millisecondsTimeout before returning control
        /// to caller.
        /// </summary>
        public void Parallelize(int millisecondsTimeout, T1 t1)
        {
            Parallelize(millisecondsTimeout, () => Sequential(t1));
        }

        /// <summary>
        /// Creates one thread for each of your actions, and execute the action
        /// on this thread not returning control to caller before all threads are
        /// finished with their work.
        /// </summary>
        public void Join(T1 t1)
        {
            Join((action) => action(t1));
        }

        /// <summary>
        /// Creates one thread for each of your actions, and execute the action
        /// on this thread not returning control to caller before all threads are
        /// finished with their work, unless milliseconds amount of time has passed,
        /// at which point it stops waiting for the thread to finish its work.
        /// </summary>
        public void Join(int milliseconds, T1 t1)
        {
            Join(milliseconds, (action) => action(t1));
        }
    }
}
