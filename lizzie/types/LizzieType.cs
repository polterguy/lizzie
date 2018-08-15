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
using System.Collections.Generic;
using lizzie.exceptions;

namespace lizzie.types
{
    public abstract class LizzieType
    {
        public static LizzieType Create(IEnumerator<string> en)
        {
            switch (en.Current) {
                case "(":
                    return LizzieList.CreateList(en);
                case "'":
                    if (!en.MoveNext())
                        throw new LizzieParsingException("Unexpected EOF after single quote.");
                    var inner = LizzieType.Create(en);
                    var list = new LizzieForm();
                    list.Add(new LizzieSymbol("quote"));
                    list.Add(inner);
                    return list;
                default:
                    return LizzieConstant.CreateConstant(en);
            }
        }

        public abstract object Value
        {
            get;
        }

        public abstract Func<TContext, Binder<TContext>, object> Compile<TContext>();

        public abstract object Evaluate<TContext>(TContext ctx, Binder<TContext> binder);
    }
}
