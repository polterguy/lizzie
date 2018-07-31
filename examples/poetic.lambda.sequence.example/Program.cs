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

namespace poetic.lambda.sequence.example
{
    class MainClass
    {
        /// <summary>
        /// An example Console application illustrating how to create a Chain
        /// of functors (delegates).
        /// </summary>
        public static void Main()
        {
            /*
             * Creating a Sequence of Actions taking no arguments.
             */
            var sequence1 = new Sequence();
            sequence1.Add(() => Console.WriteLine("Delegate 1"));
            sequence1.Add(() => Console.WriteLine("Delegate 2"));

            /*
             * Executing all actions in a sequence.
             */
            sequence1.Sequential();

            /*
             * Creating a Sequence of Actions taking one argument, and braiding.
             */
            var sequence2 = new Sequence<string>();
            sequence2.Add((input) => Console.WriteLine("Delegate 1 - Arg; " + input));
            sequence2.Add((input) => Console.WriteLine("Delegate 2 - Arg; " + input));

            /*
             * Executing all actions in a sequence.
             */
            sequence2.Braid(new string[] { "1", "2" });

            // Waiting for user input.
            Console.Read();
        }
    }
}
