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
using System.Collections;
using System.Collections.Generic;

namespace poetic.lambda.collections
{
    /// <summary>
    /// Contains a sequence of TLambda instances.
    /// </summary>
    public abstract class Sequence<T> : IEnumerable<T>
    {
        protected List<T> _list;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.lambdas.Sequence`1"/> class.
        /// </summary>
        public Sequence()
        {
            _list = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.lambdas.Sequence`1"/> class.
        /// </summary>
        /// <param name="items">Initial items.</param>
        public Sequence(params T[] items)
        {
            _list = new List<T>(items);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.lambdas.Sequence`1"/> class.
        /// </summary>
        /// <param name="items">Initial items.</param>
        public Sequence(IEnumerable<T> items)
        {
            _list = new List<T>(items);
        }

        /// <summary>
        /// Returns the number of items in current instance.
        /// </summary>
        /// <value>The number of items in the current instance.</value>
        public int Count
        {
            get { return _list.Count; }
        }

        /// <summary>
        /// Appends the specified item to the sequence.
        /// </summary>
        /// <param name="item">Item to append.</param>
        public void Add(T item)
        {
            _list.Add(item);
        }

        /// <summary>
        /// Adds a range of items to the sequence.
        /// </summary>
        /// <param name="items">Items to add.</param>
        public void AddRange(IEnumerable<T> items)
        {
            _list.AddRange(items);
        }

        /// <summary>
        /// Adds a range of items to the sequence.
        /// </summary>
        /// <param name="items">Items to add.</param>
        public void AddRange(params T [] items)
        {
            _list.AddRange(items);
        }

        #region [ -- Interface implementations -- ]

        /// <summary>
        /// Gets the enumerator for the items.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        // Private implementation.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
