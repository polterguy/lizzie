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
    /// <summary>
    /// Convenience class for passing arguments around to Lizzie function objects.
    /// </summary>
    public class Arguments : IEnumerable<object>
    {
        List<object> _list = new List<object>();

        /// <summary>
        /// Creates an empty arguments instance.
        /// </summary>
        public Arguments()
        { }

        /// <summary>
        /// Initializes the instance with the specified initial arguments.
        /// </summary>
        /// <param name="arguments">Arguments to initialize instance with.</param>
        public Arguments(params object[] arguments)
        {
            _list.AddRange(arguments);
        }

        /// <summary>
        /// Initializes the instance with the specified initial arguments.
        /// </summary>
        /// <param name="arguments">Arguments to initialize instance with.</param>
        public Arguments(IEnumerable<object> arguments)
        {
            _list.AddRange(arguments);
        }

        /// <summary>
        /// Returns the number of arguments in this instance.
        /// </summary>
        /// <value>The number of arguments this instance holds.</value>
        public int Count
        {
            get { return _list.Count; }
        }

        /// <summary>
        /// Adds the specified argument to this instance.
        /// </summary>
        /// <param name="value">Argument to add.</param>
        public void Add(object value)
        {
            _list.Add(value);
        }

        /// <summary>
        /// Returns the argument at the specified instance. Will throw if you
        /// try to retrieve arguments beyond its size.
        /// </summary>
        /// <returns>The argument at the specified index.</returns>
        /// <param name="index">The index of the argument you want to retrieve.</param>
        public object Get(int index)
        {
            return _list[index];
        }

        /// <summary>
        /// Returns the argument at the specified instance, and tries to convert
        /// it the the requested TConvert type. Will throw if you
        /// try to retrieve arguments beyond its size.
        /// </summary>
        /// <returns>The argument at the specified index.</returns>
        /// <param name="index">The index of the argument you want to retrieve.</param>
        /// <typeparam name="TConvert">The type you want to convert the argument to.</typeparam>
        public TConvert Get<TConvert>(int index)
        {
            // Retrieving argument and converting it to type specified by caller.
            var obj = Get(index);
            if (obj is TConvert)
                return (TConvert)(object)obj; // No conversion is necessary.
            return (TConvert)Convert.ChangeType(obj, typeof(TConvert), CultureInfo.InvariantCulture);
        }

        #region [ -- Interface implementations -- ]

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
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
