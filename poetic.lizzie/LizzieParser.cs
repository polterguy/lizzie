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
using poetic.lambda.exceptions;
using poetic.lambda.collections;

namespace poetic.lizzie
{
    /// <summary>
    /// Parser for Lizzie.
    /// </summary>
    public class LizzieParser<TContext> where TContext : class
    {
        // Binder for instance.
        Binder<TContext> _binder;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lizzie.LizzieParser"/> class.
        /// </summary>
        public LizzieParser()
        {
            _binder = new Binder<TContext>();
        }

        /// <summary>
        /// Parses the tokens in the specified tokenizer and builds up class
        /// such that you can execute it later with a specified context.
        /// </summary>
        /// <param name="tokenizer">Tokenizer.</param>
        public Actions<TContext> Parse(Tokenizer tokenizer)
        {
            // Returned to caller.
            Actions<TContext> actions = new Actions<TContext>();

            // Tying together our delegates.
            var enumerator = tokenizer.GetEnumerator();
            while (enumerator.MoveNext()) {

                // Figuring out where we are and acting accordingly.
                var ix = enumerator.Current;
                switch (ix) {

                    case "function":

                        CreateFunctionDeclaration(enumerator);
                        break;

                    default:

                        actions.Add(CreateRootLevelFunctionInvocation(ix, enumerator));
                        break;
                }
            }
            return actions;
        }

        /*
         * Creates an invocation to a function.
         */
        Action<TContext> CreateRootLevelFunctionInvocation (string name, IEnumerator<string> enumerator)
        {
            // Making sure function exists.
            if (!_binder.HasFunction(name))
                throw new PoeticParsingException($"Function {name} was not found in the current context");

            // Retrieving function delegate.
            var function = _binder[name];

            // Sanity check.
            if (!enumerator.MoveNext())
                throw new PoeticParsingException("Unexpected EOF while parsing function invocation");
            if (enumerator.Current!= "(")
                throw new PoeticParsingException($"Syntax error after function invocation of {name}");

            // Retrieving argument.
            var arguments = new Arguments();
            while (true) {
                if (!enumerator.MoveNext())
                    throw new PoeticParsingException("Unexpected EOF while parsing arguments");
                arguments.Add(enumerator.Current);
                if (!enumerator.MoveNext())
                    throw new PoeticParsingException("Unexpected EOF while parsing arguments");
                if (enumerator.Current == ")") {

                    // Creating our function invocation wrapper.
                    return new Action<TContext>(delegate (TContext self) {
                        function(self, arguments);
                    });
                }
                else if (enumerator.Current == ",")
                {
                    if (!enumerator.MoveNext())
                        throw new Exception("Unexpected EOF while parsing arguments");
                }
            }
        }

        void CreateFunctionDeclaration(IEnumerator<string> enumerator)
        {
            throw new NotImplementedException();
        }

        void SanityCheckFunctionInvocation(string name)
        {
            // Function name must start with [a-zA-Z].
            if ("abcdefghijklmnopqrstuvwxyz".IndexOf(char.ToLower(name[0])) == -1)
                throw new PoeticParsingException($"Function {name} did not start with a-z");
        }
    }
}
