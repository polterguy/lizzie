/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System;
using System.Reflection;
using lizzie.exceptions;
using System.Collections.Generic;

namespace lizzie
{
    /// <summary>
    /// Base class for binders.
    /// </summary>
    public abstract class BinderBase
    {
        /// <summary>
        /// Creates a default binder, binding all bound methods in your context type.
        /// </summary>
        public BinderBase(Type type)
        {
            MaxStackSize = -1;
            BindTypeMethods(type);
        }

        /// <summary>
        /// Protected CTOR to allow for cloning instances of class, without having
        /// to run through reflection necessary to bind the type.
        /// </summary>
        /// <param name="type">Type to bind towards.</param>
        /// <param name="initialize">If set to <c>true</c> will initialize context with bound Lizzie functions.</param>
        protected BinderBase(Type type, bool initialize)
        {
            MaxStackSize = -1;
            if (initialize)
                BindTypeMethods(type);
        }

        /// <summary>
        /// Statically bound variables/functions, and root level variables.
        /// </summary>
        protected Dictionary<string, object> StaticBinder { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Stack of dynamically created variables and functions.
        /// </summary>
        protected List<Dictionary<string, object>> StackBinder { get; } = new List<Dictionary<string, object>>();

        /// <summary>
        /// Gets or sets the maximum size of the stack.
        /// 
        /// This becomes the maximum number of functions you can invoke recursively,
        /// and is intended to avoid exhausting your CLR stack by entering into a
        /// never ending recursive function invocation tree.
        /// 
        /// The default value is -1, implying no check.
        /// For security reasons you might want to set this to some arbitrary number,
        /// such as 50 or 100 to avoid malicious code eating up your CLR stack and
        /// causing a stack overflow in your CLR.
        /// </summary>
        /// <value>The maximum size of your stack, or rather your maximum number of
        /// stacks (function invocations).</value>
        public int MaxStackSize { get; set; }

        /// <summary>
        /// Gets the static item keys.
        /// </summary>
        /// <value>The static item keys for this instance.</value>
        public IEnumerable<string> StaticItems => StaticBinder.Keys;

        /// <summary>
        /// Returns the count of stacks for this instance.
        /// </summary>
        /// <value>The stack count.</value>
        public int StackCount => StackBinder.Count;

        /// <summary>
        /// Gets or sets the value with the given key. You can set the content
        /// to either a constant or a Lizzie function, at which point you can
        /// retrieve the object by referencing it symbolically in your Lizzie code.
        /// 
        /// Will prioritize retrieving or setting the stack symbol before any global values.
        /// </summary>
        /// <param name="symbolName">Name or symbol for your value.</param>
        public object this[string symbolName]
        {
            get
            {

                /*
                 * Checking if we have a stack level object matching the specified symbol.
                 *
                 * We do this to allow for locally declared symbols to "hide" global symbols.
                 * Locally declared symbols are symbols declared inside of functions, or after the
                 * stack has been "pushed" at least once or more.
                 */
                if (StackBinder.Count > 0 && StackBinder[StackBinder.Count - 1].ContainsKey(symbolName))
                    return StackBinder[StackBinder.Count - 1][symbolName];

                // Defaulting to looking in "static" binder.
                if (StaticBinder.ContainsKey(symbolName))
                    return StaticBinder[symbolName];

                // Oops, no such symbol!
                throw new LizzieRuntimeException($"The '{symbolName}' symbol has not been declared.");
            }
            set
            {

                /*
                 * Checking if we should set the static (global) symbol, which we
                 * do if the stack has not (yet) been pushed at least once, or the
                 * symbol already exists at the global scope.
                 */
                if (StackBinder.Count == 0 || StaticBinder.ContainsKey(symbolName))
                {

                    /*
                     * Stack has not (yet) been pushed, or symbol exists in global scope.
                     */
                    StaticBinder[symbolName] = value;

                }
                else
                {

                    // Symbol not found on stack, hence setting global symbol's value.
                    StackBinder[StackBinder.Count - 1][symbolName] = value;
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
            // Checking if our stack contains symbol.
            if (StackBinder.Count > 0 && StackBinder[StackBinder.Count - 1].ContainsKey(symbolName))
                return true;

            // Defaulting to static binder.
            return StaticBinder.ContainsKey(symbolName);
        }

        /// <summary>
        /// Returns true if the named symbol exists. Notice, the symbol's value might
        /// still be null, even if the symbol exists.
        /// </summary>
        /// <returns><c>true</c>, if symbol exists, <c>false</c> otherwise.</returns>
        /// <param name="symbolName">Symbol name.</param>
        public bool ContainsDynamicKey(string symbolName)
        {
            if (StackBinder.Count > 0)
                return StackBinder[StackBinder.Count - 1].ContainsKey(symbolName);
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
            return StaticBinder.ContainsKey(symbolName);
        }

        /// <summary>
        /// Removes the specified key from the stack. You can only remove
        /// elements if you have 'pushed' the stack at least once, which by default
        /// in Lizzie only occurs from within a function.
        /// </summary>
        /// <param name="symbolName">Symbol name of item to remove.</param>
        public void RemoveKey(string symbolName)
        {
            if (StackBinder.Count > 0)
                StackBinder[StackBinder.Count - 1].Remove(symbolName);
            StaticBinder.Remove(symbolName);
        }

        /// <summary>
        /// Creates a new stack and makes it become the current stack.
        /// </summary>
        public void PushStack()
        {
            if (StackBinder.Count == MaxStackSize)
                throw new LizzieRuntimeException("Your maximum stack size has been exceeded");
            StackBinder.Add(new Dictionary<string, object>());
        }

        /// <summary>
        /// Pops the top item off the stack, and makes the previous stack the current stack.
        /// </summary>
        public void PopStack()
        {
            StackBinder.RemoveAt(StackBinder.Count - 1);
        }

        #region [ -- Abstract methods you'll need to override on your own type implementing class -- ]

        /// <summary>
        /// Protected abstract method expected to return type binder is bound towards.
        /// </summary>
        /// <returns>The binder type.</returns>
        protected abstract Type GetBinderType();

        /// <summary>
        /// Protected abstract method expected to return type of functions instance can handle.
        /// </summary>
        /// <returns>The binder function.</returns>
        protected abstract Type GetBinderFunction();

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public abstract BinderBase Clone();

        #endregion

        /// <summary>
        /// Helper method to implement cloning of instance on your own binder type.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <param name="destination">Destination.</param>
        protected static void Clone(BinderBase source, BinderBase destination)
        {
            destination.MaxStackSize = source.MaxStackSize;
            foreach (var ix in source.StaticBinder.Keys)
            {
                destination[ix] = source.StaticBinder[ix];
            }
            foreach (var ixStack in source.StackBinder)
            {
                var dictionary = new Dictionary<string, object>();
                foreach (var ixKey in ixStack.Keys)
                {
                    dictionary[ixKey] = ixStack[ixKey];
                }
                destination.StackBinder.Add(dictionary);
            }
        }

        #region [ -- Private helper methods -- ]

        /*
         * Binds all methods in your TContext type that is marked with the
         * BindAttribute, and make these available for you as symbolic functions
         * in your Lizzie code.
         */
        void BindTypeMethods(Type type)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var ix in methods)
            {

                var attribute = ix.GetCustomAttribute<BindAttribute>();
                if (attribute != null)
                {

                    BindMethod(type, ix, attribute.Name ?? ix.Name);
                }
            }
        }

        /*
         * Binds a single method.
         */
        void BindMethod(Type type, MethodInfo method, string functionName)
        {
            // Sanity checking function name.
            if (string.IsNullOrEmpty(functionName))
                throw new LizzieBindingException("Can't bind to functions unless you choose a non-empty function name.");

            // Sanity checking method.
            var methodArgs = method.GetParameters();
            if (method.IsStatic)
            {

                if (methodArgs.Length != 3)
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't take the right number of arguments");
                if (methodArgs[0].ParameterType != type)
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't take a '{nameof(type)}' type of argument as its first argument.");
                if (methodArgs[1].ParameterType != GetBinderType())
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't take a '{GetBinderType()}' type of argument as its second argument.");
                if (methodArgs[2].ParameterType != typeof(Arguments))
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't take an '{nameof(Arguments)}' type of argument as its third argument.");
                if (method.ContainsGenericParameters)
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it requires a generic argument.");
                if (method.ReturnType != typeof(object))
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't return '{nameof(Object)}'.");

            }
            else
            {

                if (methodArgs.Length != 2)
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't take the right number of arguments");
                if (methodArgs[0].ParameterType != GetBinderType())
                    throw new LizzieBindingException($"Can't bind to {method.Name} since it doesn't take a '{GetBinderType().Name}' type of argument as its first argument.");
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
            StaticBinder[functionName] = Delegate.CreateDelegate(GetBinderFunction(), method);
        }

        #endregion
    }
}
