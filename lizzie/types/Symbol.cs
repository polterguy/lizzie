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
            if (en.Current == "@") {

                // Literally referencing its name.
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
                function = new Function<TContext>((ctx, binder, arguments) => {
                    var value = binder[symbolName];
                    if (value is Function<TContext> functor) {
                        return functor(ctx, binder, arguments);
                    }
                    return value;
                });
                eof = !en.MoveNext();
                if (!eof && en.Current == "(") {
                    function = Apply(function, en);
                    eof = !en.MoveNext();
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
