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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace lizzie.types
{
    public class LizzieArguments : IEnumerable<object>
    {
        List<object> _list = new List<object>();

        public LizzieArguments()
        { }

        public LizzieArguments(params object[] arguments)
        {
            _list.AddRange(arguments);
        }

        public LizzieArguments(IEnumerable<object> arguments)
        {
            _list.AddRange(arguments);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public object Get(int index)
        {
            return _list[index];
        }

        public T Get<T>(int index)
        {
            // Retrieving argument and converting it to type specified by caller.
            var obj = Get(index);
            if (obj is T)
                return (T)obj; // No conversion is necessary.
            return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }

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

        #region [ -- Interface implementations -- ]

        public IEnumerator<object> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}