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

using lizzie.types;

namespace lizzie
{
    public abstract class LizzieType
    {
        public static LizzieType Create(IEnumerator<string> en)
        {
            switch (en.Current)
            {
                case "\"":
                    return LizzieString.Create(en);
                case "(":
                    return LizzieList.Create(en);
                case "'":
                    if (!en.MoveNext())
                        throw new LizzieParsingException("Unexpected EOF after quote character (').");
                    var inner = Create(en);
                    var list = new LizzieForm();
                    list.Add(new LizzieSymbol("quote"));
                    list.Add(inner);
                    return list;
                default:
                    if (IsSymbol(en.Current))
                        return LizzieSymbol.Create(en);
                    else
                        return LizzieNumber.Create(en);
            }
        }

        static bool IsSymbol(string name)
        {
            foreach (var ix in name)
            {
                if ("0123456789.".IndexOf(ix) == -1)
                    return true;
            }
            return false;
        }

        #region [ -- Abstract methods -- ]

        public abstract object Value { get; }

        public abstract LizzieFunction<TContext> Compile<TContext>() where TContext : class;

        public abstract object Evaluate<TContext>(TContext ctx, Binder<TContext> binder) where TContext : class;

        #endregion

        #region [ -- Overriden base classmethods -- ]

        public override string ToString()
        {
            return Value?.ToString() ?? "null";
        }

        #endregion
    }
}
