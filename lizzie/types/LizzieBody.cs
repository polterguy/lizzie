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
using System.Collections;
using System.Collections.Generic;

namespace lizzie.types
{
    public class LizzieBody : LizzieType, IEnumerable
    {
        protected List<LizzieType> _list;

        public LizzieBody()
        {
            _list = new List<LizzieType>();
        }

        public LizzieBody(params LizzieType[] items)
        {
            _list = new List<LizzieType>(items);
        }

        public LizzieBody(IEnumerable<LizzieType> items)
        {
            _list = new List<LizzieType>(items);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public void Add(LizzieType item)
        {
            _list.Add(item);
        }

        public void AddRange(IEnumerable<LizzieType> items)
        {
            _list.AddRange(items);
        }

        public void AddRange(params LizzieType[] items)
        {
            _list.AddRange(items);
        }

        public LizzieType this [int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        #region [ -- Overridden base class methods -- ]

        /*
         * Compiles AST down to a function.
         */
        public override LizzieFunction<TContext> Compile<TContext>()
        {
            var functors = _list.Select(ix => ix.Compile<TContext>()).ToList();
            return new LizzieFunction<TContext>((ctx, binder) => {
                object result = null;
                foreach (var ix in functors) {
                    result = ix(ctx, binder);
                }
                return result;
            });
        }

        public override object Evaluate<TContext>(TContext ctx, Binder<TContext> binder)
        {
            return null;
        }

        public override object Value
        {
            get { return _list; }
        }

        public override string ToString()
        {
            var result = "";
            bool first = true;
            foreach (var ix in _list) {
                if (first)
                    first = false;
                else
                    result += " ";
                result += ix.ToString();
            }
            return result;
        }

        #endregion

        #region [ -- Interface implementations -- ]

        public IEnumerator<LizzieType> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}