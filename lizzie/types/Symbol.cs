/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using lizzie.exceptions;

namespace lizzie.types
{
    static class Symbol<TContext>
    {
        internal static Tuple<Function<TContext>, bool> Compile(IEnumerator<string> en)
        {
            Function<TContext> function = null;
            var eof = false;
            if (en.Current == "{") {

                // Body.
                var tuples = Body<TContext>.Compile(en);
                var functions = tuples.Item1;
                function = new Function<TContext>((ctx, binder, arguments) => {
                    object result = null;
                    foreach (var ix in functions) {
                        result = ix(ctx, binder, null);
                    }
                    return result;
                });
                eof = tuples.Item2 || !en.MoveNext();

            } else if (en.Current == "@") {

                // Literally referencing symbol's name.
                if (!en.MoveNext())
                    throw new LizzieParsingException("Unexpected EOF after '@'.");
                var stringConstant = en.Current;
                function = new Function<TContext>((ctx, binder, arguments) => {
                    return stringConstant;
                });
                eof = !en.MoveNext();

            } else if (en.Current == "\"") {

                // String literal constant.
                if (!en.MoveNext())
                    throw new LizzieParsingException("Unexpected EOF after '\"'.");
                var stringConstant = en.Current;
                if (!en.MoveNext() || en.Current != "\"")
                    throw new LizzieParsingException($"Unexpected EOF after to '{stringConstant}'");
                function = new Function<TContext>((ctx, binder, arguments) => {
                    return stringConstant;
                });
                eof = !en.MoveNext();

            } else if (IsNumeric(en.Current)) {

                // Numeric constant.
                object numericConstant = null;

                // Checking if this is a floating point value.
                if (en.Current.Contains('.')) {
                    numericConstant = double.Parse(en.Current, CultureInfo.InvariantCulture);
                } else {
                    numericConstant = long.Parse(en.Current, CultureInfo.InvariantCulture);
                }
                function = new Function<TContext>((ctx, binder, arguments) => {
                    return numericConstant;
                });
                eof = !en.MoveNext();

            } else {

                // Symbolically referencing values in our binder.
                var symbolName = en.Current;
                eof = !en.MoveNext();

                // Checking if this is a function invocation.
                if (!eof && en.Current == "(") {

                    // Function invocation.
                    function = new Function<TContext>((ctx, binder, arguments) => {
                        return (binder[symbolName] as Function<TContext>)(ctx, binder, arguments);
                    });
                    function = Apply(function, en);
                    eof = !en.MoveNext();

                } else {

                    // Referencing value.
                    function = new Function<TContext>((ctx, binder, arguments) => {
                        return binder[symbolName];
                    });
                }
            }
            return new Tuple<Function<TContext>, bool>(function, eof);
        }

        static bool IsNumeric(string symbol)
        {
            foreach (var ix in symbol) {
                if ((ix < '0' || ix > '9') && ix != '.')
                    return false;
            }
            return true;
        }

        internal static Function<TContext> Apply(Function<TContext> function, IEnumerator<string> en)
        {
            var arguments = new List<Function<TContext>>();
            if (!en.MoveNext())
                throw new LizzieParsingException("Unexpected EOF while parsing function invocation.");
            if (en.Current != ")") {
                while (true) {
                    var tuple = Compile(en);
                    if (tuple.Item1 != null)
                        arguments.Add(tuple.Item1);
                    if (en.Current == ")")
                        break;
                    if (!en.MoveNext())
                        throw new LizzieParsingException("Unexpected EOF while parsing arguments to function invocation.");
                }
            }
            return new Function<TContext>((ctx, binder, args) => {
                var applied = new Arguments(arguments.Select(ix => ix(ctx, binder, args)));
                return function(ctx, binder, applied);
            });
        }
    }
}
