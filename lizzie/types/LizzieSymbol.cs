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
using lizzie.exceptions;

namespace lizzie.types
{
    public class LizzieSymbol : LizzieConstant
    {
        internal LizzieSymbol(object value)
            : base (value)
        { }

        public static LizzieSymbol CreateSymbol(string value)
        {
            return new LizzieSymbol(value);
        }

        public override Func<TContext, Binder<TContext>, object> Compile<TContext>()
        {
            return new Func<TContext, Binder<TContext>, object>((ix, binder) => {
                return null;
            });
        }

        public override object Evaluate<TContext>(TContext ctx, Binder<TContext> binder)
        {
            throw new LizzieExecutionException("Can't evaluate a symbol.");
        }
    }
}
