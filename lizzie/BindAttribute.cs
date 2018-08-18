/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System;

namespace lizzie
{
    [AttributeUsage (AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BindAttribute : Attribute
    {
        public string Name
        {
            get;
            set;
        }
    }
}
