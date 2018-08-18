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

namespace lizzie
{
    public class Arguments<T> : IEnumerable<T>
    {
        List<T> _list = new List<T>();

        public Arguments()
        { }

        public Arguments(params T[] arguments)
        {
            _list.AddRange(arguments);
        }

        public Arguments(IEnumerable<T> arguments)
        {
            _list.AddRange(arguments);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public void Add(T value)
        {
            _list.Add(value);
        }

        public T Get(int index)
        {
            return _list[index];
        }

        public TConvert Get<TConvert>(int index)
        {
            // Retrieving argument and converting it to type specified by caller.
            var obj = Get(index);
            if (obj is TConvert)
                return (TConvert)(object)obj; // No conversion is necessary.
            return (TConvert)Convert.ChangeType(obj, typeof(TConvert), CultureInfo.InvariantCulture);
        }

        public TConvert Get<TConvert>(int index, TConvert def)
        {
            // If specified argument doesn't exist, we return the default given by caller.
            if (index >= Count)
                return def;

            // Retrieving argument and converting it to type specified by caller.
            var obj = Get(index);
            if (obj is TConvert)
                return (TConvert)(object)obj; // No conversion is necessary.
            return (TConvert)Convert.ChangeType(obj, typeof(TConvert), CultureInfo.InvariantCulture);
        }

        #region [ -- Interface implementations -- ]

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    public class Arguments : Arguments<object>
    {
        public Arguments()
        { }

        public Arguments(params object[] arguments)
            : base (arguments)
        { }

        public Arguments(IEnumerable<object> arguments)
            : base (arguments)
        { }
    }
}