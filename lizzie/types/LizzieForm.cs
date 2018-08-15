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
using System.Linq;
using System.Collections.Generic;
using lizzie.exceptions;

namespace lizzie.types
{
    public class LizzieForm : LizzieList
    {
        internal LizzieForm()
        { }

        internal LizzieForm(LizzieList list)
        {
            _list.AddRange(list._list);
        }

        public override Func<TContext, Binder<TContext>, object> Compile<TContext>()
        {
            var name = _list[0].Value.ToString();
            var list = new List<LizzieType>(_list);
            list.RemoveAt(0);
            return new Func<TContext, Binder<TContext>, object>((ctx, binder) => {
                var arguments = new LizzieArguments(list.Select(ix => ix.Evaluate(ctx, binder)));
                var function = binder.GetFunction(name);
                if (function == null) {
                    throw new LizzieExecutionException($"'{name}' function does not exist.");
                }
                return function(ctx, arguments);
            });
        }
    }
}
