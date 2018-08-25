/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System;

namespace lizzie
{
    /// <summary>
    /// Attribute used to bind CLR methods in your context such that they become
    /// available as functions in your Lizzie code.
    /// </summary>
    [AttributeUsage (AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BindAttribute : Attribute
    {
        /// <summary>
        /// The Lizzie function name for your method.
        ///
        /// If you don't supply a name, the method's name will be used by default.
        /// </summary>
        /// <value>The function name you want to reference your method by in your Lizzie code.</value>
        public string Name { get; set; }
    }
}
