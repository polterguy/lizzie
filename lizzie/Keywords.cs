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
using lizzie.collections;
using lizzie.parser;

namespace poetic.lizzie
{
    public class Keywords<TContext>
    {
        // Dictionary of keywords to actions.
        readonly Dictionary<string, Func<IEnumerator<string>, Func<TContext, Arguments, Binder<TContext>, object>>> _keywords = 
            new Dictionary<string, Func<IEnumerator<string>, Func<TContext, Arguments, Binder<TContext>, object>>>();

        public Keywords(bool populateDefault = true)
        {
            if (populateDefault)
                PopulateDefault();
        }

        public void Set (string name, Func<IEnumerator<string>, Func<TContext, Arguments, Binder<TContext>, object>> lambda = null)
        {
            if (lambda == null)
                _keywords.Remove(name);
            else
                _keywords[name] = lambda;
        }

        public bool HasKeyword(string name)
        {
            return _keywords.ContainsKey(name);
        }

        public Func<IEnumerator<string>, Func<TContext, Arguments, Binder<TContext>, object>> this[string name]
        {
            get { return _keywords[name]; }
        }

        /*
         * Populates the default keywords.
         */
        void PopulateDefault()
        {
            foreach (var ix in Return<TContext>.Keywords) {
                Set(ix.Item1, ix.Item2);
            }
        }
    }
}
