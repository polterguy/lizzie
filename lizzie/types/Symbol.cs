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
            var eof = false;
            Function<TContext> symbol = null;
            if (en.Current == "@") {

                if (!en.MoveNext())
                    throw new LizzieParsingException("Unexpected EOF while parsing '@'.");

                var tuple = Compile(en);
                var innerSymbol = tuple.Item1;
                symbol = new Function<TContext>((ctx, binder, arguments) => {

                    var symbolValue = Convert.ToString(innerSymbol(ctx, binder, arguments), CultureInfo.InvariantCulture);
                    var function = binder[symbolValue] as Function<TContext>;
                    return function(ctx, binder, arguments);
                });
                if (!eof && en.Current == "(")
                    symbol = Apply(symbol, en);

            } else {

                symbol = Constant(en);
            }

            return new Tuple<Function<TContext>, bool>(symbol, eof || en.Current == "}" || !en.MoveNext());
        }

        static Function<TContext> Constant(IEnumerator<string> en)
        {
            object constantValue = en.Current;
            if (en.Current == "\"") {
                if (!en.MoveNext())
                    throw new LizzieParsingException("Unexpected EOF after '\"'.");
                constantValue = en.Current;
                if (!en.MoveNext() || en.Current != "\"")
                    throw new LizzieParsingException($"Unexpected EOF after to '{constantValue}'");
            } else if (IsNumeric(en.Current)) {
                if (en.Current.Contains('.')) {
                    constantValue = double.Parse(en.Current, CultureInfo.InvariantCulture);
                } else {
                    constantValue = long.Parse(en.Current, CultureInfo.InvariantCulture);
                }
            }
            return new Function<TContext>((ctx, binder, arguments) => {
                return constantValue;
            });
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
