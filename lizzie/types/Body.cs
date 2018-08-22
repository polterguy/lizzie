/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System;
using System.Collections.Generic;
using lizzie.exceptions;

namespace lizzie.types
{
    /*
     * Class responsible for compiling a "body" segment.
     */
    static class Body<TContext>
    {
        /*
         * Compiles a body segment, which might be the root level symbolic
         * content of a Lizzie lambda object, or the stuff between '{' and '}'.
         */
        internal static Tuple<List<Function<TContext>>, bool> Compile(IEnumerator<string> en, bool forceClose = true)
        {
            // Creating a list of functions and returning these to caller.
            var content = new List<Function<TContext>>();
            var eof = !en.MoveNext();
            while (!eof && en.Current != "}") {

                // Compiling currently tokenized symbol.
                var tuple = Symbol<TContext>.Compile(en);

                // Adding function invocation to list of functions.
                content.Add(tuple.Item1);

                // Checking if we're done compiling body.
                eof = tuple.Item2;
                if (eof || en.Current == "}")
                    break; // Even if we're not at EOF, we might be at '}', ending the current body.
            }

            // Sanity checking tokenizer's content, before returning functions to caller.
            if (forceClose && en.Current != "}")
                throw new LizzieParsingException("Premature EOF while parsing code.");
            if (!forceClose && !eof && en.Current == "}")
                throw new LizzieParsingException("Unexpected closing brace '}' in code.");
            return new Tuple<List<Function<TContext>>, bool>(content, eof);
        }
    }
}
