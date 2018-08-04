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
using System.Collections.Generic;
using poetic.lambda.utilities;

namespace poetic.lambda.collections
{
    /// <summary>
    /// Class encapsulating a list of Action delegates taking no arguments.
    /// </summary>
    public class Actions : Sequence<Action>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.lambdas.Sequence"/> class.
        /// </summary>
        public Actions()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.delegates.Sequence"/> class.
        /// </summary>
        /// <param name="lambdas">Initial functors.</param>
        public Actions(params Action[] lambdas)
            : base(lambdas)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.delegates.Sequence"/> class.
        /// </summary>
        /// <param name="lambdas">Initial functors.</param>
        public Actions(IEnumerable<Action> lambdas)
            : base(lambdas)
        { }

        /// <summary>
        /// Sequentially executes each action not returning before execution is finished.
        /// </summary>
        public void Sequentially()
        {
            Execute.Sequentially(this);
        }

        /// <summary>
        /// Sequentially executes each action on a different thread, and blocking
        /// the calling thread until done, or millisecondsTimeout have passed.
        /// </summary>
        /// <param name="millisecondsTimeout">Maximum amount of time to block calling thread.</param>
        public void Sequentially(int millisecondsTimeout)
        {
            Execute.Sequentially(this, millisecondsTimeout);
        }

        /// <summary>
        /// Sequentially executes each action on a different thread without blocking
        /// the calling thread.
        /// </summary>
        public void SequentiallyUnblocked()
        {
            Execute.SequentiallyUnblocked(this);
        }

        /// <summary>
        /// Executes each action in parallel blocking the calling thread until
        /// all actions are finished executing.
        /// </summary>
        public void Parallel()
        {
            Execute.Parallel(this);
        }

        /// <summary>
        /// Executes each action in parallel blocking the calling thread for a
        /// maximum amount of time, until execution of all actions are finished,
        /// or milliseconds have passed, whatever occurs first.
        /// </summary>
        /// <param name="millisecondsTimeout">Maximum amount of time to block calling thread.</param>
        public void Parallel(int millisecondsTimeout)
        {
            Execute.Parallel(this, millisecondsTimeout);
        }

        /// <summary>
        /// Executes each action in parallel without blocking the calling thread.
        /// </summary>
        public void ParallelUnblocked()
        {
            Execute.ParallelUnblocked(this);
        }
    }

    /// <summary>
    /// Class encapsulating a list of Action delegates taking one arguments.
    /// </summary>
    public class Actions<T1> : Sequence<Action<T1>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.lambdas.Sequence"/> class.
        /// </summary>
        public Actions()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.delegates.Sequence"/> class.
        /// </summary>
        /// <param name="lambdas">Initial functors.</param>
        public Actions(params Action<T1>[] lambdas)
            : base(lambdas)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.delegates.Sequence"/> class.
        /// </summary>
        /// <param name="lambdas">Initial functors.</param>
        public Actions(IEnumerable<Action<T1>> lambdas)
            : base(lambdas)
        { }

        /// <summary>
        /// Sequentially executes each action not returning before execution is finished.
        /// </summary>
        public void Sequentially(T1 t1)
        {
            Execute.Sequentially(this.Select((ix) => new Action(() => ix(t1))));
        }

        /// <summary>
        /// Sequentially executes each action on a different thread, and blocking
        /// the calling thread until done, or millisecondsTimeout have passed.
        /// </summary>
        /// <param name="millisecondsTimeout">Maximum amount of time to block calling thread.</param>
        public void Sequentially(T1 t1, int millisecondsTimeout)
        {
            Execute.Sequentially(this.Select((ix) => new Action(() => ix(t1))), millisecondsTimeout);
        }

        /// <summary>
        /// Sequentially executes each action on a different thread without blocking
        /// the calling thread.
        /// </summary>
        public void SequentiallyUnblocked(T1 t1)
        {
            Execute.SequentiallyUnblocked(this.Select((ix) => new Action(() => ix(t1))));
        }

        /// <summary>
        /// Executes each action in parallel blocking the calling thread until
        /// all actions are finished executing.
        /// </summary>
        public void Parallel(T1 t1)
        {
            Execute.Parallel(this.Select((ix) => new Action(() => ix(t1))));
        }

        /// <summary>
        /// Executes each action in parallel blocking the calling thread for a
        /// maximum amount of time, until execution of all actions are finished,
        /// or milliseconds have passed, whatever occurs first.
        /// </summary>
        /// <param name="millisecondsTimeout">Maximum amount of time to block calling thread.</param>
        public void Parallel(T1 t1, int millisecondsTimeout)
        {
            Execute.Parallel(this.Select((ix) => new Action(() => ix(t1))), millisecondsTimeout);
        }

        /// <summary>
        /// Executes each action in parallel without blocking the calling thread.
        /// </summary>
        public void ParallelUnblocked(T1 t1)
        {
            Execute.ParallelUnblocked(this.Select((ix) => new Action(() => ix(t1))));
        }
    }
}
