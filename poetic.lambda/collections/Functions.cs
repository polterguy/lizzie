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
    /// Class encapsulating a list of Func delegates taking no arguments.
    /// </summary>
    public class Functions<TResult> : Sequence<Func<TResult>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.lambdas.Functions`1"/> class.
        /// </summary>
        public Functions()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.delegates.Functions`1"/> class.
        /// </summary>
        /// <param name="lambdas">Initial functions.</param>
        public Functions(params Func<TResult>[] lambdas)
            : base(lambdas)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.delegates.Functions`1"/> class.
        /// </summary>
        /// <param name="lambdas">Initial functions.</param>
        public Functions(IEnumerable<Func<TResult>> lambdas)
            : base(lambdas)
        { }

        /// <summary>
        /// Executes each function in a sequence on the calling thread.
        /// </summary>
        /// <returns>The result of each evaluation.</returns>
        public IEnumerable<TResult> Sequence()
        {
            return Evaluate<TResult>.Sequence(this);
        }

        /// <summary>
        /// Evaluates each action in parallel blocking the calling thread for a
        /// maximum amount of time, until execution of all actions are finished,
        /// or milliseconds have passed, whatever occurs first.
        /// </summary>
        /// <returns>The result of each evaluation.</returns>
        public IEnumerable<TResult> Sequence(int millisecondsTimeout)
        {
            return Evaluate<TResult>.Parallel(this, millisecondsTimeout);
        }

        /// <summary>
        /// Evaluates each function in parallel on a separate thread and returns
        /// the result of each function.
        /// </summary>
        /// <returns>The result of each function invocation.</returns>
        public IEnumerable<TResult> Parallel()
        {
            return Evaluate<TResult>.Parallel(this);
        }
    }

    /// <summary>
    /// Class encapsulating a list of Func delegates taking one argument.
    /// </summary>
    public class Functions<T1, TResult> : Sequence<Func<T1, TResult>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.lambdas.Functions`1"/> class.
        /// </summary>
        public Functions()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.delegates.Functions`1"/> class.
        /// </summary>
        /// <param name="lambdas">Initial functors.</param>
        public Functions(params Func<T1, TResult>[] lambdas)
            : base(lambdas)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.delegates.Functions`1"/> class.
        /// </summary>
        /// <param name="lambdas">Initial functors.</param>
        public Functions(IEnumerable<Func<T1, TResult>> lambdas)
            : base(lambdas)
        { }

        /// <summary>
        /// Executes all lambdas in a sequence on the calling thread.
        /// </summary>
        /// <returns>The sequence.</returns>
        /// <param name="t1">T1.</param>
        public IEnumerable<TResult> Sequence(T1 t1)
        {
            return Evaluate<TResult>.Sequence(this.Select((ix) => new Func<TResult>(() => ix(t1))));
        }

        /// <summary>
        /// Executes each action in sequence blocking the calling thread for a
        /// maximum amount of time, until execution of all actions are finished,
        /// or milliseconds have passed, whatever occurs first.
        /// </summary>
        /// <param name="millisecondsTimeout">Maximum amount of time to block calling thread.</param>
        public IEnumerable<TResult> Sequence(T1 t1, int millisecondsTimeout)
        {
            return Evaluate<TResult>.Parallel(this.Select((ix) => new Func<TResult>(() => ix (t1))), millisecondsTimeout);
        }

        /// <summary>
        /// Evaluates each function in parallel on a separate thread and returns
        /// the result of each function.
        /// </summary>
        /// <returns>The result of each function invocation.</returns>
        public IEnumerable<TResult> Parallel(T1 t1)
        {
            return Evaluate<TResult>.Parallel(this.Select((ix) => new Func<TResult>(() => ix(t1))));
        }
    }
}
