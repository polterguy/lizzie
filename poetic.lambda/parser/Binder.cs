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
using System.Reflection;
using System.Collections.Generic;
using poetic.lambda.collections;
using poetic.lambda.exceptions;

namespace poetic.lambda.parser
{
    /// <summary>
    /// Binds variable names to objects and functions.
    /// </summary>
    public class Binder<TContext>
    {
        // All variables that have been binded to this instance.
        readonly Dictionary<string, object> _variables;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.parser.Binder`1"/> class.
        /// </summary>
        public Binder()
        {
            _variables = new Dictionary<string, object>();
            BindTypeMethods();
        }

        /// <summary>
        /// Gets or sets the object with the specified name.
        /// </summary>
        /// <param name="name">Name of object to retrieve or set.</param>
        public object this[string name] {
            get {
                if (!_variables.ContainsKey(name)) {
                    return null;
                }
                return _variables[name];
            }
            set {
                _variables[name] = value;
            }
        }

        /// <summary>
        /// Returns true if the specified key exists.
        /// </summary>
        /// <returns><c>true</c>, if object exists, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        public bool HasKey(string name)
        {
            return _variables.ContainsKey(name);
        }

        /*
         * Binds all instance methods found in type that have been marked with the Function attribute.
         */
        private void BindTypeMethods()
        {
            // Finding all relevant methods in type.
            var methods = typeof(TContext).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var ix in methods) {

                // Checking if this is a mapped method.
                var attribute = ix.GetCustomAttribute<FunctionAttribute>();
                if (attribute != null) {

                    /*
                     * Binding method, defaulting its function name to method
                     * name if no explicit name was given in its attribute.
                     */
                    BindMethod(ix, attribute.Name ?? ix.Name);
                }
            }
        }

        /*
         * Private helper that binds one MethodInfo to a function name by creating a Func delegate.
         */
        private void BindMethod(MethodInfo method, string functionName)
        {
            // Sanity checking function name.
            if (string.IsNullOrEmpty(functionName))
                throw new PoeticBindingException("Can't have functions with empty names");

            // Sanity checking method.
            var methodArgs = method.GetParameters();
            if (methodArgs.Length != 2)
                throw new PoeticParsingException($"Can't bind to {method.Name} since it doesn't take exactly one argument");
            if (methodArgs[0].ParameterType != typeof(Arguments))
                throw new PoeticParsingException($"Can't bind to {method.Name} since it doesn't take an '{nameof(Arguments)}' type of argument as first argument.");
            if (methodArgs[1].ParameterType != typeof(Binder<TContext>))
                throw new PoeticParsingException($"Can't bind to {method.Name} since it doesn't take a '{nameof(Binder<TContext>)}' type of argument as second argument.");
            if (method.ContainsGenericParameters)
                throw new PoeticParsingException($"Can't bind to {method.Name} since it requires a generic argument.");
            if (method.ReturnType != typeof(object))
                throw new PoeticParsingException($"Can't bind to {method.Name} since it doesn't return '{nameof(Object)}'.");

            // Creating our delegate, caching it, and returning it to caller.
            _variables[functionName] = 
                (Func<TContext, Arguments, Binder<TContext>, object>)
                Delegate.CreateDelegate(typeof(Func<TContext, Arguments, Binder<TContext>, object>), method);
        }
    }
}
