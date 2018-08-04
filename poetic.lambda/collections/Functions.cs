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
        /// <param name="functions">Initial functions.</param>
        public Functions(params Func<TResult>[] functions)
            : base(functions)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.delegates.Functions`1"/> class.
        /// </summary>
        /// <param name="functions">Initial functions.</param>
        public Functions(IEnumerable<Func<TResult>> functions)
            : base(functions)
        { }

        /// <summary>
        /// Evaluate each function in a sequence on the calling thread.
        /// </summary>
        /// <returns>The result of each evaluation.</returns>
        public IEnumerable<TResult> Evaluate()
        {
            return Evaluator<TResult>.Sequentially(this);
        }

        /// <summary>
        /// Evaluates each function in parallel and returns the result to caller.
        /// </summary>
        /// <returns>The result of each function invocation.</returns>
        public IEnumerable<TResult> EvaluateParallel()
        {
            return Evaluator<TResult>.Parallel(this);
        }
    }
}
