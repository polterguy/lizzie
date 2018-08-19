/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using lizzie.exceptions;

namespace lizzie
{
    /// <summary>
    /// Binds your context type such that all methods marked with the BindAttribute
    /// becomes available for you as functions in your Lizzie code, by referencing
    /// them as symbols.
    /// </summary>
    public class Binder<TContext>
    {
        readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        /// <summary>
        /// Creates a default binder, binding all bound methods in your context type.
        /// </summary>
        public Binder()
        {
            BindTypeMethods();
        }

        /// <summary>
        /// Gets or sets the value with the given key. You can set the content
        /// to either a constant or a Lizzie function, at which point you can
        /// retrieve the object by referencing it symbolically in your Lizzie code.
        /// </summary>
        /// <param name="symbolName">Name or symbol for your value.</param>
        public object this[string symbolName]
        {
            get => _values[symbolName];
            set => _values[symbolName] = value;
        }

        /// <summary>
        /// Returns true if the named symbol exists. Notice, the symbol's value might
        /// still be null, even if the symbol exists.
        /// </summary>
        /// <returns><c>true</c>, if symbol exists, <c>false</c> otherwise.</returns>
        /// <param name="symbolName">Symbol name.</param>
        public bool ContainsKey(string symbolName)
        {
            return _values.ContainsKey(symbolName);
        }

        /*
         * Binds all methods in your TContext type that is marked with the
         * BindAttribute, and make these available for you as symbolic functions
         * in your Lizzie code.
         */
        void BindTypeMethods()
        {
            var methods = typeof(TContext).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var ix in methods) {

                var attribute = ix.GetCustomAttribute<BindAttribute>();
                if (attribute != null) {

                    BindMethod(ix, attribute.Name ?? ix.Name);
                }
            }
        }

        /*
         * Binds a single method.
         */
        void BindMethod(MethodInfo method, string functionName)
        {
            // Sanity checking function name.
            if (string.IsNullOrEmpty(functionName))
                throw new LizzieBindingException("Can't bind to functions unless you choose a non-empty function name.");

            // Sanity checking method.
            var methodArgs = method.GetParameters();
            if (methodArgs.Length != 2)
                throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't take exactly two arguments");
            if (methodArgs[0].ParameterType != typeof(Binder<TContext>))
                throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't take a '{nameof(Binder<TContext>)}' type of argument as its first argument.");
            if (methodArgs[1].ParameterType != typeof(Arguments))
                throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't take an '{nameof(Arguments)}' type of argument as its first argument.");
            if (method.ContainsGenericParameters)
                throw new LizzieBindingException($"Can't bind to {method.Name} since it requires a generic argument.");
            if (method.ReturnType != typeof(object))
                throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't return '{nameof(Object)}'.");

            /*
             * Success, creating our delegate wrapping our method, and adding it to our dictionary with the specified
             * symbolic function name.
             */
            _values[functionName] = (Function<TContext>)
                Delegate.CreateDelegate(typeof(Function<TContext>), method);
        }
    }
}
