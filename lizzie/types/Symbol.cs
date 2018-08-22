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
    /*
     * Class responsible for compiling a symbol.
     */
    static class Symbol<TContext>
    {
        /*
         * Compiles a single symbol, which might be a constant, a symbol reference,
         * a body, or the literal name of a symbol.
         */
        internal static Tuple<Function<TContext>, bool> Compile(IEnumerator<string> en)
        {
            // Checking type of symbol, and acting accordingly.
            if (en.Current == "{")
                return CompileBody(en);
            else if (en.Current == "@")
                return CompileSymbolicReference(en);
            else if (en.Current == "\"")
                return CompileString(en);
            else if (IsNumeric(en.Current))
                return CompileNumber(en);
            else
                return CompileSymbol(en);
        }

        /*
         * Compiles a body down to a function and returns the function to caller.
         */
        static Tuple<Function<TContext>, bool> CompileBody(IEnumerator<string> en)
        {
            // Compiling body, and retrieving functions.
            var tuples = Body<TContext>.Compile(en);
            var functions = tuples.Item1;

            /*
             * Creating a function that evaluates every function sequentially, and
             * returns the result of the last function evaluation to the caller.
             */
            Function<TContext> function = new Function<TContext>((ctx, binder, arguments) => {
                object result = null;
                foreach (var ix in functions) {
                    result = ix(ctx, binder, null);
                }
                return result;
            });

            // Making sure the body becomes evaluated lazy.
            var lazyFunction = new Function<TContext>((ctx2, binder2, arguments2) => {
                return function;
            });
            return new Tuple<Function<TContext>, bool>(lazyFunction, tuples.Item2 || !en.MoveNext());
        }

        /*
         * Compiles a literal reference down to a function and returns the function to caller.
         */
        static Tuple<Function<TContext>, bool> CompileSymbolicReference(IEnumerator<string> en)
        {
            // Sanity checking tokenizer's content, since an '@' must reference an actual symbol.
            if (!en.MoveNext())
                throw new LizzieParsingException("Unexpected EOF after '@'.");

            // Storing symbol's name and sanity checking its name.
            var symbolName = en.Current;

            // Sanity checking symbol name.
            SanityCheckSymbolName(symbolName);

            // Discarding "(" token and checking if we're at EOF.
            var eof = !en.MoveNext();

            // Checking if this is a function invocation.
            if (!eof && en.Current == "(") {

                /*
                 * Notice, since this is a literally referenced function invocation, we
                 * don't want to apply its arguments if the function is being passed around,
                 * but rather return the function as a function, which once evaluated, applies
                 * its arguments. Hence, this becomes a lazy function evaluation, allowing us
                 * to pass in function evaluations that are not evaluated before the receiver
                 * invokes them.
                 */
                var tuple = ApplyArguments(symbolName, en);
                var functor = tuple.Item1;
                return new Tuple<Function<TContext>, bool>(new Function<TContext>((ctx, binder, arguments) => {
                    return functor;
                }), tuple.Item2);

            }  else {

                // Creating a function that evaluates to the constant value of the symbol's name.
                return new Tuple<Function<TContext>, bool>(new Function<TContext>((ctx, binder, arguments) => {
                    return symbolName;
                }), eof);
            }
        }

        /*
         * Compiles a constant string down to a symbol and returns to caller.
         */
        static Tuple<Function<TContext>, bool> CompileString(IEnumerator<string> en)
        {
            // Sanity checking tokenizer's content.
            if (!en.MoveNext())
                throw new LizzieParsingException("Unexpected EOF after '\"'.");

            // Retrieving actual string constant, and sanity checking tokenizer.
            var stringConstant = en.Current;
            if (!en.MoveNext() || en.Current != "\"")
                throw new LizzieParsingException($"Unexpected EOF after to '{stringConstant}'");

            // Returning a function that evaluates to the actual string's constant value.
            var function = new Function<TContext>((ctx, binder, arguments) => {
                return stringConstant;
            });
            return new Tuple<Function<TContext>, bool>(function, !en.MoveNext());
        }

        /*
         * Compiles a constant number down to a symbol and returns to caller.
         */
        static Tuple<Function<TContext>, bool> CompileNumber(IEnumerator<string> en)
        {
            // Holds our actual number, which might be double or long.
            object numericConstant = null;

            // Checking if this is a floating point value.
            if (en.Current.Contains('.')) {

                // Notice, all integer numbers are treated as long.
                numericConstant = double.Parse(en.Current, CultureInfo.InvariantCulture);

            } else {

                // Notice, all floating point numbers are treated as double.
                numericConstant = long.Parse(en.Current, CultureInfo.InvariantCulture);
            }

            // Creates a function that evaluates to the actual constant number.
            var function = new Function<TContext>((ctx, binder, arguments) => {
                return numericConstant;
            });
            return new Tuple<Function<TContext>, bool>(function, !en.MoveNext());
        }

        /*
         * Compiles a symbolic reference down to a function and returns that function to caller.
         */
        static Tuple<Function<TContext>, bool> CompileSymbol(IEnumerator<string> en)
        {
            // Retrieving symbol's name and sanity checking its name.
            var symbolName = en.Current;
            SanityCheckSymbolName(symbolName);

            // Discarding "(" token and checking if we're at EOF.
            var eof = !en.MoveNext();

            // Checking if this is a function invocation.
            if (!eof && en.Current == "(") {

                // Function invocation, making sure we apply arguments,
                return ApplyArguments(symbolName, en);

            } else {

                // Referencing value of symbol.
                return new Tuple<Function<TContext>, bool>(new Function<TContext>((ctx, binder, arguments) => {

                    // Sanity checking that symbol actually exists.
                    if (!binder.ContainsKey(symbolName))
                        throw new LizzieRuntimeException($"The '{symbolName}' symbol does not exist.");
                    return binder[symbolName];
                }), eof);
            }
        }

        /*
         * Applies arguments to a function invoction, such that they're evaluated at runtime.
         */
        static Tuple<Function<TContext>, bool> ApplyArguments(string symbolName, IEnumerator<string> en)
        {
            // Used to hold arguments before they're being applied inside of function evaluation.
            var arguments = new List<Function<TContext>>();

            // Sanity checking tokenizer's content.
            if (!en.MoveNext())
                throw new LizzieParsingException("Unexpected EOF while parsing function invocation.");

            // Looping through all arguments, if there are any.
            while (en.Current != ")")
            {

                // Compiling current argument.
                var tuple = Compile(en);
                arguments.Add(tuple.Item1);
                if (en.Current == ")")
                    break; // And we are done parsing arguments.

                // Sanity checking tokenizer's content, and discarding "," token.
                if (en.Current != ",")
                    throw new LizzieParsingException($"Syntax error in arguments to '{symbolName}', expected ',' separating arguments and found a '{en.Current}'.");
                if (!en.MoveNext())
                    throw new LizzieParsingException("Unexpected EOF while parsing arguments to function invocation.");
            }

            /*
             * Creates a function invocation that evaluates its arguments at runtime.
             */
            return new Tuple<Function<TContext>, bool>(new Function<TContext>((ctx, binder, args) => {

                // Applying arguments.
                var appliedArguments = new Arguments(arguments.Select(ix => ix(ctx, binder, args)));

                // Basic sanity checking.
                if (!binder.ContainsKey(symbolName))
                    throw new LizzieRuntimeException($"The '{symbolName}' symbol does not exist.");

                // Retrieving symbol's value and doing some basic sanity checks.
                var symbol = binder[symbolName];
                if (symbol == null)
                    throw new LizzieRuntimeException($"Symbol '{symbolName}' is null.");
                if (symbol is Function<TContext> functor)
                    return functor(ctx, binder, appliedArguments); // Success!
                throw new LizzieRuntimeException($"Symbol '{symbolName}' is not a function, but a '{symbol.GetType().FullName}'");
            }), !en.MoveNext());
        }

        /*
         * Returns true if this is a numeric value, which might be floating point
         * value, or an integer value.
         */
        static bool IsNumeric(string symbol)
        {
            foreach (var ix in symbol) {
                if ((ix < '0' || ix > '9') && ix != '.')
                    return false;
            }
            return true;
        }

        /*
         * Sanity checks the name of a symbol.
         */
        public static void SanityCheckSymbolName(string symbolName)
        {
            if ("abcdefghijklmnopqrstuvwxyz".IndexOf(char.ToLower(symbolName[0])) == -1)
                throw new LizzieRuntimeException($"A symbol must start with the characters [a-z], symbol '{symbolName}' is not a valid symbol name.");
            foreach (var ix in symbolName) {
                if ("abcdefghijklmnopqrstuvwxyz0123456789*_-".IndexOf(char.ToLower(ix)) == -1)
                    throw new LizzieRuntimeException($"A symbol can only contain the characters [a-z] and [0-9], or '-', '_' and '*', symbol '{symbolName}' is not a valid symbol name.");
            }
            if (symbolName == "*" || symbolName == "_" || symbolName == "*")
                throw new LizzieRuntimeException($"A symbol cannot only contain the character '*', '_' or '-', symbol '{symbolName}' is not a valid symbol name.");
        }
    }
}
