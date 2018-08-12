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
using System.Linq;
using System.Collections.Generic;
using poetic.lambda.parser;
using poetic.lambda.exceptions;
using poetic.lambda.collections;

namespace poetic.lizzie
{
    public static class FunctionInvocation<TContext>
    {
        public static Func<TContext, Arguments, Binder<TContext>, object> Create(IEnumerator<string> en)
        {
            /*
             * Some sort of function invocation.
             */
            var name = en.Current;
            if (!en.MoveNext())
                throw new PoeticParsingException($"Unexpected EOF whileparsing function invocation to '{name}'");

            // Making sure we can evaluate our argument late.
            var args = ArgumentsParser<TContext>.Parse(name, en);

            // Returning a function wrapping our function invocation.
            return new Func<TContext, Arguments, Binder<TContext>, object>(delegate(TContext ctx, Arguments arguments, Binder<TContext> binder) {

                // Making sure key exists.
                if (!binder.HasKey(name))
                    throw new PoeticExecutionException($"Function '{name}' does not exist.");

                // Making sure key is a function.
                if (binder[name] is Func<TContext, Arguments, Binder<TContext>, object> func) {

                    // Late binding arguments by evaluating them only when the need is there!
                    var argsIn = new Arguments(args.Select((ix) => ix(ctx, arguments, binder)));
                    return func(ctx, argsIn, binder);
                }

                // Oops! Key was not a function.
                throw new PoeticExecutionException($"'{name}' is not a function.");
            });
        }
    }
}
