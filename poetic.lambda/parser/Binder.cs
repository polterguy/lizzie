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
        // Contains cache of all functions.
        Dictionary<string, Func<Arguments, object>> _functions = new Dictionary<string, Func<Arguments, object>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.parser.Binder`1"/> class.
        /// </summary>
        public Binder()
        {
            Bind();
        }

        /*
         * Binds all methods found in type.
         */
        private void Bind()
        {
            // Finding all relevant methods in type.
            var methods = typeof(TContext).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            foreach (var ix in methods) {

                // Checking if this is a mapped method.
                var attribute = ix.GetCustomAttribute<FunctionAttribute>();
                if (attribute != null) {

                    /*
                     * Binding method, defaulting its function name to method
                     * name if no explicit name was given in its attribute.
                     */
                    Bind(ix, attribute.Name ?? ix.Name);
                }
            }
        }

        /*
         * Private helper that binds one MethodInfo to a function name.
         */
        private Func<Arguments, object> Bind(MethodInfo method, string functionName)
        {
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
            var retVal = (Func<Arguments, object>)Delegate.CreateDelegate(typeof(Func<Arguments, object>), method);
            _functions[functionName] = retVal;
            return retVal;
        }
    }
}
