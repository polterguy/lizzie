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

                        // Creating our function.
                        var function = Function<TContext>.Create(enumerator);
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
                throw new PoeticParsingException("Unexpected EOF while parsing function invocation expecting opening paranthesis");
            if (enumerator.Current != "(")
                throw new PoeticParsingException($"Syntax error after function invocation of {name}");
            if (!enumerator.MoveNext())
                throw new PoeticParsingException("Unexpected EOF while parsing function invocation expecting closing paranthesis or arguments");

            // Retrieving argument.
            var arguments = new Arguments();
            while (true) {
                if (enumerator.Current == ",") {
                    if (!enumerator.MoveNext())
                        throw new PoeticParsingException("Unexpected EOF while parsing function invocation expecting argument after comma");
                }
                if (enumerator.Current == ")") {

                    // Creating our function invocation wrapper.
                    return new Action<TContext>(delegate (TContext self) {
                        function(self, arguments);
                    });

                } else {
                    arguments.Add(enumerator.Current);
                }
                if (!enumerator.MoveNext())
                    throw new PoeticParsingException("Unexpected EOF while parsing arguments");
            }
        }

        /*void CreateFunction(IEnumerator<string> enumerator)
        {
            // Retrieving function name.
            if (!enumerator.MoveNext())
                throw new PoeticParsingException("Unexpected EOF while parsing function declaration");

            // Storing name of function, and sanity checking name.
            var name = enumerator.Current;
            SanityCheckFunctionName(name);

            // Skipping opening paranthesis.
            if (!enumerator.MoveNext())
                throw new PoeticParsingException("Unexpected EOF while expecting function declaration opening paranthesis");

            // Parsing arguments to function.
            var arguments = new List<string>();
            while (true) {
                if (!enumerator.MoveNext())
                    throw new PoeticParsingException("Unexpected EOF while expecting function declaration argument");
                var arg = enumerator.Current;
                if (arg == ")")
                    break;
                SanityCheckVariableName(arg);
                arguments.Add(arg);
                if (!enumerator.MoveNext())
                    throw new PoeticParsingException("Unexpected EOF while expecting function closing paranthesis");
                if (enumerator.Current == ")")
                    break; // End of function declaration arguments.
                else if (enumerator.Current != ",")
                    throw new PoeticParsingException("Unexpected token in function declaration, when expecting ',' or ')'");
            }

            // Parsing function body.
            if (!enumerator.MoveNext())
                throw new PoeticParsingException("Unexpected EOF while expecting function body");
            if (enumerator.Current != "{")
                throw new PoeticParsingException("No opening brace found in function declaration");

            // Now creating our lambda object for our function.
            var lambda = CreateScopeLambda(enumerator);
            var function = new Func<TContext, Arguments, object>(
            delegate (TContext context, Arguments funcArgs) {
                lambda.Execute(context);
                return null;
            });

            // Creating binding between function name and Func function created above.
            _binder.Add(name, function);
        }

        Actions<TContext> CreateScopeLambda(IEnumerator<string> enumerator)
        {
            // Return value.
            Actions<TContext> lambda = new Actions<TContext>();

            // At this point we expect to have already seen the first opening brace.
            int braceCount = 1;
            while (braceCount > 0) {
                if (!enumerator.MoveNext())
                    throw new PoeticParsingException("Unexpected EOF while parsing function body");
                if (enumerator.Current == "}") {
                    braceCount -= 1;
                } else if (enumerator.Current == "{") {

                    // Inner lambda scope.
                    var innerLambda = CreateScopeLambda(enumerator);
                    lambda.Add(new Action<TContext>(delegate (TContext context) {
                        innerLambda.Execute(context);
                    }));

                } else {

                    /*
                     * Besides from function invocations at the root level, this
                     * is the only place where we can find actual 'code'.
                     * /
                    lambda.Add(ParseLambdaToken(enumerator));
                }
            }
            return lambda;
        }

        Action<TContext> ParseLambdaToken (IEnumerator<string> enumerator)
        {
            var token = enumerator.Current;
            if (IsKeyword(token)) {

                // TODO: Implement
                // TODO: Also check Stack object if this is a variable reference
                throw new NotImplementedException();

            } else {

                // This is a function invocation.
                if (!enumerator.MoveNext())
                    throw new PoeticParsingException("Unexpected EOF while looking for opening paranthesis for function invocation.");
                return CreateFunctionInvocation(token, enumerator);
            }
        }

        Action<TContext> CreateFunctionInvocation(string name, IEnumerator<string> enumerator)
        {
            // Sanity check.
            if (enumerator.Current != "(")
                throw new PoeticParsingException($"Syntax error after function invocation of {name}");

            // Making sure function exists.
            if (!_binder.HasFunction(name))
                throw new PoeticParsingException($"Function {name} was not found in the current context");

            // Retrieving function delegate.
            var function = _binder[name];

            // Retrieving argument.
            var arguments = new Arguments();
            while (true)
            {
                if (!enumerator.MoveNext())
                    throw new PoeticParsingException("Unexpected EOF while parsing arguments");
                arguments.Add(enumerator.Current);
                if (!enumerator.MoveNext())
                    throw new PoeticParsingException("Unexpected EOF while parsing arguments");
                if (enumerator.Current == ")") {

                    // Creating our function invocation wrapper.
                    return new Action<TContext>(delegate (TContext ctx) {
                        function(ctx, arguments);
                    });
                } else if (enumerator.Current == ",") {
                    if (!enumerator.MoveNext())
                        throw new Exception("Unexpected EOF while parsing arguments");
                }
            }
        }

        bool IsKeyword(string token)
        {
            switch (token) {
                case "var":
                case "if":
                case "else":
                case "else-if":
                case "while":
                    return true;
                default:
                    return false;
            }
        }

        void SanityCheckVariableName(string name)
        {
            // Variable name must obey by the same rules as a function name.
            SanityCheckFunctionName(name);
        }

        void SanityCheckFunctionName(string name)
        {
            // Function name must start with [a-zA-Z].
            if ("abcdefghijklmnopqrstuvwxyz".IndexOf(char.ToLower(name[0])) == -1)
                throw new PoeticParsingException($"Function {name} did not start with a-z");
        }*/
    }
}
