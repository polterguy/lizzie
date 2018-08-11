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
using System.Globalization;
using System.Collections.Generic;

namespace poetic.lambda.collections
{
    /// <summary>
    /// Class encapsulating a list of arguments.
    /// </summary>
    public class Arguments : Sequence<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.collections.Arguments`1"/> class.
        /// </summary>
        public Arguments()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.collections.Arguments`1"/> class.
        /// </summary>
        /// <param name="arguments">Initial arguments.</param>
        public Arguments(params object[] arguments)
            : base(arguments)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.collections.Arguments`1"/> class.
        /// </summary>
        /// <param name="arguments">Arguments.</param>
        public Arguments(IEnumerable<object> arguments)
            : base(arguments)
        { }

        /// <summary>
        /// Returns the number of arguments in this instance.
        /// </summary>
        /// <value>The count of arguments.</value>
        public int Count
        {
            get { return _list.Count; }
        }

        /// <summary>
        /// Returns the specified argument.
        /// </summary>
        /// <returns>The argument.</returns>
        /// <param name="index">Which argument to return.</param>
        public object Get (int index)
        {
            return _list[index];
        }

        /// <summary>
        /// Returns the specified argument and converts it is necessary.
        /// </summary>
        /// <returns>The argument at the specified index position.</returns>
        /// <param name="index">Indexof which argument to retrieve.</param>
        /// <typeparam name="T">Type to convert object into.</typeparam>
        public T Get<T>(int index)
        {
            // Retrieving argument and converting it to type specified by caller.
            var obj = Get(index);
            if (obj is T)
                return (T)obj; // No conversion is necessary.
            return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the specified argument and converts it is necessary.
        /// </summary>
        /// <returns>The argument at the specified index position.</returns>
        /// <param name="index">Indexof which argument to retrieve.</param>
        /// <param name="def">Default value to return if argument doesn't exist.</param>
        /// <typeparam name="T">Type to convert object into.</typeparam>
        public T Get<T>(int index, T def)
        {
            // If specified argument doesn't exist, we return the default given by caller.
            if (index >= Count)
                return def;

            // Retrieving argument and converting it to type specified by caller.
            var obj = Get(index);
            if (obj is T)
                return (T)obj; // No conversion is necessary.
            return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Throws an exception if number of arguments does not equal specified count.
        /// </summary>
        /// <param name="count">Number of arguments to expect.</param>
        public void AssertCount(int count)
        {
            if (Count < count)
                throw new ArgumentException("Too few arguments");
            if (Count > count)
                throw new ArgumentException("Too many arguments");
        }
    }
}
