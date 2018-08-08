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
    /// Class that binds an execution object of type T to a DSL.
    /// </summary>
    public class Binder<TContext>
    {
        // All functions.
        readonly Dictionary<string, Func<TContext, Arguments, object>> _functions;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.parser.Binder`1"/> class.
        /// </summary>
        public Binder()
        {
            _functions = new Dictionary<string, Func<TContext, Arguments, object>>();
            BindTypeMethods();
        }

        /// <summary>
        /// Returns true if function exists, otherwise false.
        /// </summary>
        /// <returns><c>true</c>, if function exists, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        public bool HasFunction(string name)
        {
            return _functions.ContainsKey(name);
        }

        /// <summary>
        /// Gets the function with the specified name.
        /// </summary>
        /// <param name="name">Name of function to retrieve.</param>
        public Func<TContext, Arguments, object> this[string name] => _functions[name];

        /// <summary>
        /// Add the specified function with the given name.
        /// </summary>
        /// <param name="name">Nameof function to add.</param>
        /// <param name="function">Function to add.</param>
        public void Add(string name, Func<TContext, Arguments, object> function)
        {
            _functions[name] = function;
        }

        /*
         * Binds all instance methods found in type.
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
        private Func<TContext, Arguments, object> BindMethod(MethodInfo method, string functionName)
        {
            // Sanity checking function name.
            if (string.IsNullOrEmpty(functionName))
                throw new PoeticBindingException("Can't have functions with empty names");

            // Sanity checking method.
            var methodArgs = method.GetParameters();
            if (methodArgs.Length != 1)
                throw new PoeticParsingException($"Can't bind to {method.Name} since it doesn't take exactly one argument");
            if (methodArgs[0].ParameterType != typeof(Arguments))
                throw new PoeticParsingException($"Can't bind to {method.Name} since it doesn't take an '{nameof(Arguments)}' type of argument");
            if (method.ContainsGenericParameters)
                throw new PoeticParsingException($"Can't bind to {method.Name} since it takes a generic argument.");
            if (method.ReturnType != typeof(object))
                throw new PoeticParsingException($"Can't bind to {method.Name} since it doesn't return '{nameof(Object)}'.");

            // Creating our delegate, caching it, and returning it to caller.
            var retVal = (Func<TContext, Arguments, object>)Delegate.CreateDelegate(typeof(Func<TContext, Arguments, object>), method);
            _functions[functionName] = retVal;
            return retVal;
        }
    }
}
