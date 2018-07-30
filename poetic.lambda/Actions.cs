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

namespace poetic.lambda
{
    /// <summary>
    /// Class encapsulating a list of Actions taking no arguments.
    /// </summary>
    public class Actions : Lambdas<Action>
    {
        /// <summary>
        /// Executes all lambdas in a sequence on the calling thread.
        /// </summary>
        public void Sequence()
        {
            Sequence(delegate(Action action) {
                action();
            });
        }

        /// <summary>
        /// Creates one thread for each of your actions, and execute the action
        /// on this thread in "fire and forget" mode.
        /// </summary>
        public void Forget()
        {
            Forget(delegate(Action action) {
                action();
            });
        }

        /// <summary>
        /// Creates one thread for each of your actions, and execute the action
        /// on this thread not returning control to caller before all threads are
        /// finished with their work.
        /// </summary>
        public void Join()
        {
            Join(delegate (Action action) {
                action();
            });
        }

        /// <summary>
        /// Creates one thread for each of your actions, and execute the action
        /// on this thread not returning control to caller before all threads are
        /// finished with their work, unless milliseconds amount of time has passed,
        /// at which point it stops waiting for the thread to finish its work.
        /// </summary>
        public void Join(int milliseconds)
        {
            Join(delegate (Action action) {
                action();
            }, milliseconds);
        }
    }

    /// <summary>
    /// Class encapsulating a list of Actions taking one arguments.
    /// </summary>
    public class Actions<T1> : Lambdas<Action<T1>>
    {
        /// <summary>
        /// Executes all lambdas in a sequence on the calling thread.
        /// </summary>
        public void Sequence(T1 t1)
        {
            Sequence(delegate (Action<T1> action) {
                action(t1);
            });
        }

        /// <summary>
        /// Creates one thread for each of your actions, and execute the action
        /// on this thread in "fire and forget" mode.
        /// </summary>
        public void Forget(T1 t1)
        {
            Forget(delegate (Action<T1> action) {
                action(t1);
            });
        }

        /// <summary>
        /// Creates one thread for each of your actions, and execute the action
        /// on this thread not returning control to caller before all threads are
        /// finished with their work.
        /// </summary>
        public void Join(T1 t1)
        {
            Join(delegate (Action<T1> action) {
                action(t1);
            });
        }

        /// <summary>
        /// Creates one thread for each of your actions, and execute the action
        /// on this thread not returning control to caller before all threads are
        /// finished with their work, unless milliseconds amount of time has passed,
        /// at which point it stops waiting for the thread to finish its work.
        /// </summary>
        public void Join(T1 t1, int milliseconds)
        {
            Join(delegate (Action<T1> action) {
                action(t1);
            }, milliseconds);
        }
    }
}
