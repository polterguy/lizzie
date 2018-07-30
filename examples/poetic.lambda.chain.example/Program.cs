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

namespace poetic.lambda.chain.example
{
    class MainClass
    {
        /// <summary>
        /// An example Console application illustrating how to create a Chain
        /// of functors (delegates).
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            /*
             * Creating a Chain of Actions taking no arguments.
             */
            var actions1 = new Actions();
            actions1.Add(() => Console.WriteLine("Delegate 1"));
            actions1.Add(() => Console.WriteLine("Delegate 2"));

            /*
             * Executing each Action in a sequence.
             */
            actions1.Sequence();

            /*
             * Creating a Chain of Actions taking one argument.
             */
            var actions2 = new Actions<string>();
            actions2.Add((input) => Console.WriteLine("Delegate 1 - " + input));
            actions2.Add((input) => Console.WriteLine("Delegate 2 - " + input));

            /*
             * Executing each Action in a sequence passing in "foo" as argument.
             */
            actions2.Sequence("foo");
        }
    }
}
