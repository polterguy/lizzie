/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
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
