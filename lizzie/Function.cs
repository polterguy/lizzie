/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

namespace lizzie
{
    /// <summary>
    /// Delegate for all function invocations evaluated by Lizzie in its lambda delegate.
    /// </summary>
    public delegate object Function<TContext>(TContext ctx, Binder<TContext> binder, Arguments arguments);

    /// <summary>
    /// Delegate for a lambda object created by Lizzie.
    /// </summary>
    public delegate object Lambda<TContext>(TContext ctx, Binder<TContext> binder);
}
