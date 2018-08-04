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

namespace poetic.lambda.collections
{
    /// <summary>
    /// Class encapsulating a list of Func delegates where the output from one
    /// is being passed in as input to the next.
    /// </summary>
    public class Chain<T> : Functions<T, T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.lambdas.Chain`1"/> class.
        /// </summary>
        public Chain()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.lambdas.Chain`1"/> class.
        /// </summary>
        /// <param name="functions">Initial functors.</param>
        public Chain(params Func<T, T>[] functions)
            : base(functions)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.lambdas.Chain`1"/> class.
        /// </summary>
        /// <param name="functions">Initial functors.</param>
        public Chain(IEnumerable<Func<T, T>> functions)
            : base(functions)
        { }

        /// <summary>
        /// Evaluates the chain, and returns the result to caller.
        /// </summary>
        public T Foo(T t1)
        {
            foreach (var ix in this) {
                t1 = ix(t1);
            }
            return t1;
        }
    }
}
