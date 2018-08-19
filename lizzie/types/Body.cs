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
            var content = new List<Function<TContext>>();
            var eof = false;
            if (en.MoveNext()) {
                while (true) {
                    var tuple = Symbol<TContext>.Compile(en);
                    if (tuple.Item1 != null)
                        content.Add(tuple.Item1);
                    if (tuple.Item2 || en.Current == "}") {
                        eof = tuple.Item2;
                        break;
                    }
                }

            } else {
                eof = true;
            }

            if (forceClose && en.Current != "}")
                throw new LizzieParsingException("Premature EOF while parsing code.");
            if (!forceClose && !eof && en.Current == "}")
                throw new LizzieParsingException("Unexpected closing brace '}' in code.");
            return new Tuple<List<Function<TContext>>, bool>(content, eof);
        }
    }
}
