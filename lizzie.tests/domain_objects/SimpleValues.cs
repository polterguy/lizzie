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

namespace lizzie.tests.domain_objects
{
    public class SimpleValues
    {
        public int ValueInteger { get; set; }
        public string ValueString { get; set; }

        [Bind (Name = "set-value-integer")]
        object SetValue(Binder<SimpleValues> ctx, Arguments arguments)
        {
            ValueInteger = arguments.Get<int>(0);
            return null;
        }

        [Bind(Name = "set-value-string")]
        object SetValueString(Binder<SimpleValues> ctx, Arguments arguments)
        {
            ValueString = arguments.Get<string>(0);
            return null;
        }

        [Bind(Name = "get-constant-integer")]
        object GetConstant(Binder<SimpleValues> ctx, Arguments arguments)
        {
            return 57;
        }

        [Bind(Name = "add-integers")]
        object AddIntegers(Binder<SimpleValues> ctx, Arguments arguments)
        {
            ValueInteger = arguments.Get<int>(0) + arguments.Get<int>(1);
            return null;
        }
    }
}
