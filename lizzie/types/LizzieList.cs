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
    public class LizzieList : LizzieType
    {
        internal List<LizzieType> _list = new List<LizzieType>();

        public static LizzieList CreateList(IEnumerator<string> en)
        {
            LizzieList result = new LizzieList();
            while (en.MoveNext() && en.Current != ")") {
                result._list.Add(LizzieType.Create(en));
            }
            if (result._list.Count > 0 && result._list[0] is LizzieSymbol) {
                return new LizzieForm(result);
            }
            return result;
        }

        public void Add(LizzieType value)
        {
            _list.Add(value);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public LizzieType this [int index]
        {
            get { return _list[index]; }
        }

        public override object Value
        {
            get { return _list; }
        }

        public override string ToString()
        {
            var result = "(";
            bool first = true;
            foreach (var ix in _list) {
                if (first)
                    first = false;
                else
                    result += " ";
                result += ix.ToString();
            }
            result += ")";
            return result;
        }

        public override Func<TContext, Binder<TContext>, object> Compile<TContext>()
        {
            throw new LizzieExecutionException("You can't execute a list unless it has a symbol as its first value.");
        }

        public override object Evaluate<TContext>(TContext ctx, Binder<TContext> binder)
        {
            throw new LizzieExecutionException("Can't evaluate a list.");
        }
    }
}
