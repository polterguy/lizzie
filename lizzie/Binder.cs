/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System;
using System.Linq;
using System.Reflection;
using lizzie.exceptions;
using System.Collections.Generic;

namespace lizzie
{
    /// <summary>
    /// Binds your context type such that all methods marked with the BindAttribute
    /// becomes available for you as functions in your Lizzie code.
    /// 
    /// NOTICE!
    /// Class is not thread safe, which implies you cannot share instances of it
    /// between different threads. However, you can still cache a "master instance"
    /// of the class, and invoke Clone for each of your threads that needs to use
    /// an instance bound to the same type. Cloning instances of such a master object
    /// has some performance benefits, since creating a new instance of the
    /// class implies some reflection, which is a realatively costy process on
    /// the CLR.
    /// </summary>
    public class Binder<TContext> : ICloneable
    {
        // Statically bound functions.
        readonly Dictionary<string, object> _staticBinder = new Dictionary<string, object>();

        // Stack of dynamically created variables and functions.
        readonly List<Dictionary<string, object>> _stackBinder = new List<Dictionary<string, object>>();

        /// <summary>
        /// Creates a default binder, binding all bound methods in your context type.
        /// </summary>
        public Binder()
        {
            BindTypeMethods();
        }

        /*
         * Private CTOR to allow for cloning instances of class, without having
         * to run through reflection necessary to bind the type.
         */
        Binder(bool initialize)
        {
            if (initialize)
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
            get {

                if (_stackBinder.Count > 0 && _stackBinder[_stackBinder.Count - 1].ContainsKey(symbolName))
                    return _stackBinder[_stackBinder.Count - 1][symbolName];

                // Defaulting to static binder.
                if (_staticBinder.ContainsKey(symbolName))
                    return _staticBinder[symbolName];

                // Oops, no such reference!
                throw new LizzieRuntimeException($"The '{symbolName}' symbol has not been declared.");
            }
            set {

                /*
                 * Checking if we have actually started executing the Lizzie code,
                 * or if we are still in "CLR land".
                 */
                if (_stackBinder.Count == 0) {

                    // Still in "CLR land".
                    _staticBinder[symbolName] = value;

                } else {

                    // Stack has been pushed at least once, and we're in "Lizzie land".
                    _stackBinder[_stackBinder.Count - 1][symbolName] = value;
                }
            }
        }

        /// <summary>
        /// Returns true if the named symbol exists. Notice, the symbol's value might
        /// still be null, even if the symbol exists.
        /// </summary>
        /// <returns><c>true</c>, if symbol exists, <c>false</c> otherwise.</returns>
        /// <param name="symbolName">Symbol name.</param>
        public bool ContainsKey(string symbolName)
        {
            // Prioritizing the stack, to allow for stack value to "override" global values.
            if (_stackBinder.Count > 0 && _stackBinder[_stackBinder.Count - 1].ContainsKey(symbolName))
                return _stackBinder[_stackBinder.Count - 1].ContainsKey(symbolName);

            // Defaulting to static binder.
            return _staticBinder.ContainsKey(symbolName);
        }

        /// <summary>
        /// Returns true if the named symbol exists. Notice, the symbol's value might
        /// still be null, even if the symbol exists.
        /// </summary>
        /// <returns><c>true</c>, if symbol exists, <c>false</c> otherwise.</returns>
        /// <param name="symbolName">Symbol name.</param>
        public bool ContainsDynamicKey(string symbolName)
        {
            if (_stackBinder.Count > 0 && _stackBinder[_stackBinder.Count - 1].ContainsKey(symbolName))
                return _stackBinder[_stackBinder.Count - 1].ContainsKey(symbolName);
            return false;
        }

        /// <summary>
        /// Returns true if the named symbol exists. Notice, the symbol's value might
        /// still be null, even if the symbol exists.
        /// </summary>
        /// <returns><c>true</c>, if symbol exists, <c>false</c> otherwise.</returns>
        /// <param name="symbolName">Symbol name.</param>
        public bool ContainsStaticKey(string symbolName)
        {
            return _staticBinder.ContainsKey(symbolName);
        }

        /// <summary>
        /// Removes the specified key from the stack.
        /// </summary>
        /// <param name="symbolName">Symbol name of item to remove.</param>
        public void RemoveKey(string symbolName)
        {
            _stackBinder[_stackBinder.Count - 1].Remove(symbolName);
        }

        /// <summary>
        /// Creates a new stack and makes it become the current stack.
        /// </summary>
        public void PushStack()
        {
            _stackBinder.Add(new Dictionary<string, object>());
        }

        /// <summary>
        /// Pops the top item off the stack, and makes the previous stack the current stack.
        /// </summary>
        public void PopStack()
        {
            _stackBinder.RemoveAt(_stackBinder.Count - 1);
        }

        /// <summary>
        /// Gets the static item keys.
        /// </summary>
        /// <value>The static item keys for this instance.</value>
        public IEnumerable<string> StaticItems => _staticBinder.Keys;

        /// <summary>
        /// Returns the count of stacks for this instance.
        /// </summary>
        /// <value>The stack count.</value>
        public int StackCount => _stackBinder.Count;

        /*
         * Binds all methods in your TContext type that is marked with the
         * BindAttribute, and make these available for you as symbolic functions
         * in your Lizzie code.
         */
        void BindTypeMethods()
        {
            var methods = typeof(TContext).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
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
            if (method.IsStatic) {

                if (methodArgs.Length != 3)
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't take the right number of arguments");
                if (methodArgs[0].ParameterType != typeof(TContext))
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't take a '{nameof(TContext)}' type of argument as its first argument.");
                if (methodArgs[1].ParameterType != typeof(Binder<TContext>))
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't take a '{nameof(Binder<TContext>)}' type of argument as its second argument.");
                if (methodArgs[2].ParameterType != typeof(Arguments))
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't take an '{nameof(Arguments)}' type of argument as its third argument.");
                if (method.ContainsGenericParameters)
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it requires a generic argument.");
                if (method.ReturnType != typeof(object))
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't return '{nameof(Object)}'.");

            } else {

                if (methodArgs.Length != 2)
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't take the right number of arguments");
                if (methodArgs[0].ParameterType != typeof(Binder<TContext>))
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't take a '{nameof(Binder<TContext>)}' type of argument as its first argument.");
                if (methodArgs[1].ParameterType != typeof(Arguments))
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't take an '{nameof(Arguments)}' type of argument as its second argument.");
                if (method.ContainsGenericParameters)
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it requires a generic argument.");
                if (method.ReturnType != typeof(object))
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't return '{nameof(Object)}'.");
            }

            /*
             * Success, creating our delegate wrapping our method, and adding it to our dictionary with the specified
             * symbolic function name.
             */
            _staticBinder[functionName] = (Function<TContext>)
                Delegate.CreateDelegate(typeof(Function<TContext>), method);
        }

        /// <summary>
        /// Clones this instance.
        /// 
        /// Since invoking the default CTOR has some overhead due to reflection,
        /// caching instances of this class might improve your performance.
        /// However, since instances of the class is not thread safe and cannot
        /// be safely shared among multiple threads, you can use this method
        /// to clone a "master instance" for each of your threads that needs to
        /// bind to the same type. This method is also useful in case you want
        /// to spawn of new threads, where each thread needs access to a thread
        /// safe copy of the stack, at the point of creation.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public Binder<TContext> Clone()
        {
            var clone = new Binder<TContext>(false);
            foreach (var ix in _staticBinder.Keys) {
                clone[ix] = _staticBinder[ix];
            }
            foreach (var ixStack in _stackBinder) {
                var dictionary = new Dictionary<string, object>();
                foreach (var ixKey in ixStack.Keys) {
                    dictionary[ixKey] = ixStack[ixKey];
                }
                clone._stackBinder.Add(dictionary);
            }
            return clone;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
