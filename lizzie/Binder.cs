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
using lizzie.exceptions;

namespace lizzie
{
    public class Binder<TContext>
    {
        readonly Dictionary<string, object> _functions = new Dictionary<string, object>();

        public Binder()
        {
            BindTypeMethods();
        }

        public object this[string name]
        {
            get {
                if (!_functions.ContainsKey(name)) {
                    return null;
                }
                return _functions[name];
            }
            set => _functions[name] = value;
        }

        public T Get<T>(string name) where T : class
        {
            var result = this[name];
            if (result is T)
                return result as T;
            return default(T);
        }

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

        void BindMethod(MethodInfo method, string functionName)
        {
            if (string.IsNullOrEmpty(functionName))
                throw new LizzieBindingException("Can't have functions with empty names");

            var methodArgs = method.GetParameters();

            if (methodArgs.Length != 2)
                throw new LizzieParsingException($"Can't bind to {method.Name} since it doesn't take exactly two arguments");
            if (methodArgs[0].ParameterType != typeof(Binder<TContext>))
                throw new LizzieParsingException($"Can't bind to {method.Name} since it doesn't take an '{nameof(Arguments)}' type of argument as its first argument.");
            if (methodArgs[1].ParameterType != typeof(Arguments))
                throw new LizzieParsingException($"Can't bind to {method.Name} since it doesn't take an '{nameof(Arguments)}' type of argument as its first argument.");
            if (method.ContainsGenericParameters)
                throw new LizzieParsingException($"Can't bind to {method.Name} since it requires a generic argument.");
            if (method.ReturnType != typeof(object))
                throw new LizzieParsingException($"Can't bind to {method.Name} since it doesn't return '{nameof(Object)}'.");

            _functions[functionName] = (Function<TContext>)
                Delegate.CreateDelegate(typeof(Function<TContext>), method);
        }
    }
}
