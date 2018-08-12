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
using poetic.lambda.parser;
using poetic.lizzie.keywords;

namespace poetic.lizzie
{
    /// <summary>
    /// Class encapsulating which keywords you want to use.
    /// </summary>
    public class LizzieKeywords<TContext>
    {
        // Dictionary of keywords to actions.
        readonly Dictionary<string, Func<IEnumerator<string>, Action<FunctionStack<TContext>>>> _keywords = 
            new Dictionary<string, Func<IEnumerator<string>, Action<FunctionStack<TContext>>>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lizzie.LizzieKeywords`1"/> class.
        /// </summary>
        /// <param name="populateDefault">If set to <c>true</c> will populate the default keywords, otherwise will not create any keyword dictionary mappings for you at all.</param>
        public LizzieKeywords(bool populateDefault = true)
        {
            if (populateDefault)
                PopulateDefault();
        }

        /// <summary>
        /// Sets or removes the specified named keyword to the specified lambda action.
        /// If you pass in lambda as null, the keyword is removed.
        /// </summary>
        /// <param name="name">Name of keyword.</param>
        /// <param name="lambda">Lambda to associate with keyword.</param>
        public void Set (string name, Func<IEnumerator<string>, Action<FunctionStack<TContext>>> lambda)
        {
            if (lambda == null)
                _keywords.Remove(name);
            else
                _keywords[name] = lambda;
        }

        /// <summary>
        /// Returns true if the specified keyword exists.
        /// </summary>
        /// <returns><c>true</c>, if keyword exists, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        public bool HasKeyword(string name)
        {
            return _keywords.ContainsKey(name);
        }

        /// <summary>
        /// Gets the function for parsing the specified keyword having the specified name.
        /// </summary>
        /// <param name="name">Name of keyword parser to return.</param>
        public Func<IEnumerator<string>, Action<FunctionStack<TContext>>> this[string name]
        {
            get { return _keywords[name]; }
        }

        /*
         * Populates the default keywords.
         */
        void PopulateDefault()
        {
            foreach (var ix in Branching<TContext>.Keywords) {
                _keywords.Add(ix.Item1, ix.Item2);
            }
            foreach (var ix in Variables<TContext>.Keywords) {
                _keywords.Add(ix.Item1, ix.Item2);
            }
        }
    }
}
