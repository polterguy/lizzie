/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

namespace lizzie
{
    public delegate object Function<TContext>(TContext ctx, Binder<TContext> binder, Arguments arguments);
}
