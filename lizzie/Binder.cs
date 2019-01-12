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
    /// Binds your context type such that all methods marked with the BindAttribute
    /// becomes available for you as functions in your Lizzie code, in addition
    /// to serving as a 'stack' for your Lizzie code. Class has two levels, statically
    /// bound objects, which once bound cannot be removed in any ways. This includes
    /// bound methods, and other symbolic delegates. In addition it contains stack
    /// bound objects, which are removed once the stacked is 'popped', and a new function
    /// level stack is created once the stack is 'pushed'.
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
    public class Binder<TContext> : BinderBase
    {
        /// <summary>
        /// Creates a default binder, binding all bound methods in your context type.
        /// </summary>
        public Binder()
            : base(typeof(TContext))
        { }

        /*
         * Private CTOR to allow for cloning instances of class, without having
         * to run through reflection necessary to bind the type.
         */
        Binder(bool initialize)
            : base (typeof(TContext), initialize)
        { }

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
        public override BinderBase Clone()
        {
            var clone = new Binder<TContext>(false);
            BinderBase.Clone(this, clone);
            return clone;
        }

        /// <summary>
        /// Protected abstract method expected to return type binder is bound towards.
        /// </summary>
        /// <returns>The binder type.</returns>
        protected override Type GetBinderType()
        {
            return typeof(Binder<TContext>);
        }

        /// <summary>
        /// Protected abstract method expected to return type of functions instance can handle.
        /// </summary>
        /// <returns>The binder function.</returns>
        protected override Type GetBinderFunction()
        {
            return typeof(Function<TContext>);
        }
    }
}
