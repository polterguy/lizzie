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
using System.Threading;

namespace poetic.lambda.threads.example
{
    /// <summary>
    /// An dummy example of a class where instances of the class needs to be shared
    /// between multiple threads, and hence access to the instance needs to be
    /// synchronized.
    /// </summary>
    class Shared
    {
        string _data = "initial";

        /// <summary>
        /// Returns the state of the instance.
        /// </summary>
        /// <returns>The read.</returns>
        public string Read()
        {
            return _data;
        }

        /// <summary>
        /// Modifies the state of the instance.
        /// </summary>
        /// <param name="value">Value.</param>
        public void Write(string value)
        {
            _data += value;
        }
    }

    class MainClass
    {
        /// <summary>
        /// An example of how to use the Threads class to spawn of multiple threads,
        /// with some slightly simplified syntax.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
        }
    }
}
