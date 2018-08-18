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
