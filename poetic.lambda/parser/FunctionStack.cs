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

using System.Collections.Generic;

namespace poetic.lambda.parser
{
    /// <summary>
    /// Class encapsulating an execution stack.
    /// </summary>
    public class FunctionStack<TContext>
    {
        // The actual content of your stack.
        readonly Dictionary<string, object> _stack = new Dictionary<string, object>();

        // The binder for this instance.
        readonly Binder<TContext> _binder;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.parser.Stack`1"/> class.
        /// </summary>
        /// <param name="binder">Binder to associate this stack with.</param>
        public FunctionStack(Binder<TContext> binder, TContext context)
        {
            _binder = binder;
            Context = context;
        }

        /// <summary>
        /// Gets or sets the return value for this instance.
        /// </summary>
        /// <value>The return value.</value>
        public object Return
        {
            get;
            set;
        }

        /// <summary>
        /// The context instance this FunctionStack is associated with.
        /// </summary>
        /// <value>The context of the current stack.</value>
        public TContext Context
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the specified stack value.
        /// </summary>
        /// <param name="name">Name of stack item.</param>
        public object this[string name]
        {
            get {
                // Prioritizing values from the stack.
                if (_stack.ContainsKey(name))
                    return _stack[name];

                // Defaulting to binder's value.
                return _binder[name];
            }
            set => _stack[name] = value;
        }
    }
}
