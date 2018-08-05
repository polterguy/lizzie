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
using poetic.lambda.collections;

namespace poetic.tests.example_languages.dynamic_bind
{
    /*
     * An instance of this class will be bound to the lambda execution further
     * down in file.
     */
    public class DynamicBinder
    {
        public string Foo
        {
            get;
            set;
        }

        public int Bar
        {
            get;
            set;
        }

        /*
         * Method we dynamically will bind towards function invocation in DSL.
         */
        public void set_foo(Arguments arguments)
        {
            arguments.AssertCount(1);
            Foo = arguments.Get<string> (0);
        }

        /*
         * Method we dynamically will bind towards function invocation in DSL.
         */
        public void set_bar(Arguments arguments)
        {
            arguments.AssertCount(1);
            Bar = arguments.Get<int>(0);
        }

        /*
         * Method we dynamically will bind towards function invocation in DSL.
         */
        public void add(Arguments arguments)
        {
            arguments.AssertCount(2);
            var str_add = arguments.Get<string>(0);
            var int_add = arguments.Get<int>(1);
            Foo += str_add;
            Bar += int_add;
        }
    }
}
