/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System.Collections.Generic;
using lizzie.exceptions;

namespace lizzie.types
{
    static class Body<TContext>
    {
        internal static List<Function<TContext>> Compile(IEnumerator<string> en, bool forceClose = true)
        {
            // Iterating through each root level function invocation in code.
            var content = new List<Function<TContext>>();
            while (true) {

                // Expecting symbol name to be current value of enumerator.
                var symbol = en.Current;

                // Moving to "(" of function invocation and sanity checking code.
                if (!en.MoveNext())
                    throw new LizzieParsingException($"Unexpected EOF while parsing '{symbol}'");

                // Appending function invocation to return value.
                content.Add(FunctionInvocation<TContext>.Compile(symbol, en));

                // Checking if we're at end of body.
                if (en.Current == "}" || !en.MoveNext())
                    break;
            }

            // Sanity checking code before we return list of root level function invocations to caller.
            if (forceClose) {

                if (en.Current != "}" || !en.MoveNext())
                    throw new LizzieParsingException($"Unexpected and premature EOF while parsing code.");

            } else {

                if (en.Current == "}")
                    throw new LizzieParsingException("Syntax error, too many closing braces ('}') in code.");
            }

            // Returning list of functions to caller.
            return content;
        }
    }
}
