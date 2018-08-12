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

using poetic.lambda.parser;
using poetic.lambda.collections;

namespace poetic.tests.lizzie_tests.contexts
{
    /*
     * A context with several functions, allowing us to chain invocations.
     */
    public class MultipleFunctions
    {
        public int Value { get; set; }

        [Function(Name = "get_1")]
        public object Get_1(Arguments args)
        {
            return Value;
        }

        [Function(Name = "get_2")]
        public object Get_2(Arguments args)
        {
            return Value * 2;
        }

        [Function(Name = "set_1")]
        public object Set_1(Arguments args)
        {
            Value = args.Get<int>(0);
            return null;
        }

        [Function(Name = "set_2")]
        public object Set_2(Arguments args)
        {
            Value = args.Get<int>(0) + args.Get<int>(1);
            return null;
        }

        [Function(Name = "set_3")]
        public object Set_3(Arguments args)
        {
            Value = args.Get<int>(0) + args.Get<int>(1) + args.Get<int>(2) + args.Get<int>(3);
            return null;
        }

        [Function(Name = "increment")]
        public object Increment(Arguments args)
        {
            Value = args.Get<int>(0);
            return Value + 1;
        }
    }
}
