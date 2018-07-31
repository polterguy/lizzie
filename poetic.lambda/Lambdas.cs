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
using System.Collections.Generic;

namespace poetic.lambda
{
    /// <summary>
    /// Base class for all delegate list types.
    /// </summary>
    public abstract class Lambdas<TLambda> : List<TLambda>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.delegates.Chain`2"/> class.
        /// </summary>
        public Lambdas()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.delegates.Chain`2"/> class.
        /// </summary>
        /// <param name="lambdas">Initial functors.</param>
        public Lambdas(params TLambda[] lambdas)
            : base (lambdas)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.delegates.Chain`2"/> class.
        /// </summary>
        /// <param name="lambdas">Initial functors.</param>
        public Lambdas(IEnumerable<TLambda> lambdas)
            : base (lambdas)
        { }
    }
}
