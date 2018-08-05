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
using System.Reflection;
using poetic.lambda.parser;
using poetic.lambda.collections;

namespace poetic.tests.example_languages.dynamic_bind
{
    /*
     * A simple parses example that mutates a simple string input with a
     * parametrised function tokenizer.
     */
    public class DynamicBindParser<T>
    {
        readonly Tokenizer _tokenizer;

        public DynamicBindParser(Tokenizer tokenizer)
        {
            _tokenizer = tokenizer ?? throw new NullReferenceException(nameof(tokenizer));
        }

        private bool IsFunction(string token)
        {
            if (token == "(" || token == ")")
                return false;
            return true;
        }

        private Action<T, Arguments> CreateAction(string methodName, Arguments arguments)
        {
            var method = typeof(T).GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            var methodArgs = method.GetParameters();

            // Sanity checking method.
            if (methodArgs.Length != 1)
                throw new Exception($"Can't bind to {methodName} since it doesn't take exactly one argument");
            if (methodArgs[0].ParameterType != typeof(Arguments))
                throw new Exception($"Can't bind to {methodName} since it doesn't take an {nameof(Arguments)} type of argument");
            if (method.ContainsGenericParameters)
                throw new Exception($"Can't bind to {methodName} since it takes a generic argument.");
            if (method.ReturnType != typeof(void))
                throw new Exception($"Can't bind to {methodName} since it doesn't return void.");

            // Creating our delegate.
            return (Action<T, Arguments>)Delegate.CreateDelegate(typeof(Action<T, Arguments>), method);
        }

        public Actions<T> Parse()
        {
            var retVal = new Actions<T>();
            var enumerator = _tokenizer.GetEnumerator();
            while (enumerator.MoveNext()) {
                if (IsFunction(enumerator.Current)) {
                    var methodName = enumerator.Current;
                    if (!enumerator.MoveNext()) {
                        throw new Exception("Unexpected EOF after function invocation");
                    }
                    if (enumerator.Current != "(") {
                        throw new Exception("No arguments supplied to function invocation");
                    }
                    if (!enumerator.MoveNext()) {
                        throw new Exception("Unexpected EOF after opening paranthesis");
                    }
                    var arguments = new Arguments();
                    while (true) {
                        arguments.Add(enumerator.Current);
                        if (!enumerator.MoveNext())
                            throw new Exception("Unexpected EOF while parsing arguments");
                        if (enumerator.Current == ")") {

                            // Creating our method invocation.
                            var action = CreateAction(methodName, arguments);
                            var wrapper = new Action<T>(delegate (T self) {
                                action(self, arguments);
                            });
                            retVal.Add(wrapper);
                            break;
                        } else if (enumerator.Current == ",") {
                            if (!enumerator.MoveNext())
                                throw new Exception("Unexpected EOF while parsing arguments");
                        }
                    }
                } else {
                    throw new Exception("Unsupported keyword");
                }
            }
            return retVal;
        }
    }
}
