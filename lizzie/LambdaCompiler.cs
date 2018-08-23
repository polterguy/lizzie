/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System;

namespace lizzie
{
    /// <summary>
    /// Convenience class to more easily compile a lambda function from Lizzie code.
    /// </summary>
    public static class LambdaCompiler
    {
        /// <summary>
        /// Compiles the specified code, binding to the specified context, and
        /// returns a function allowing you to evaluate the specified code.
        /// 
        /// Will bind to all the default 'keywords' in Lizzie found in the
        /// Functions class.
        /// </summary>
        /// <returns>The compiled lambda function.</returns>
        /// <param name="context">Context to bind the evaluation towards.</param>
        /// <param name="code">Lizzie code to compile.</param>
        /// <typeparam name="TContext">The type of context you want to bind towards.</typeparam>
        public static Func<object> Compile<TContext>(TContext context, string code)
        {
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<TContext>(tokenizer, code);
            var binder = new Binder<TContext>();
            BindFunctions(binder);
            return new Func<object>(() => {
                return function(context, binder);
            });
        }

        /// <summary>
        /// Compiling the specified code to a lambda function, without requiring
        /// the caller to bind the evaluation towards a particular type.
        /// 
        /// Will bind to all the default 'keywords' in Lizzie found in the
        /// Functions class.
        /// </summary>
        /// <returns>The compiled lambda function.</returns>
        /// <param name="code">Lizzie code to compile.</param>
        public static Func<object> Compile(string code)
        {
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var binder = new Binder<Nothing>();
            BindFunctions(binder);
            var nothing = new Nothing();
            return new Func<object>(() => {
                return function(nothing, binder);
            });
        }

        /*
         * Binds the specified binder to all default 'keywords' in Lizzie.
         */
        static void BindFunctions<TContext>(Binder<TContext> binder)
        {
            binder["var"] = Functions<TContext>.Var;
            binder["set"] = Functions<TContext>.Set;
            binder["if"] = Functions<TContext>.If;
            binder["eq"] = Functions<TContext>.Eq;
            binder["not"] = Functions<TContext>.Not;
            binder["function"] = Functions<TContext>.Function;
            binder["add"] = Functions<TContext>.Add;
            binder["subtract"] = Functions<TContext>.Subtract;
            binder["multiply"] = Functions<TContext>.Multiply;
            binder["divide"] = Functions<TContext>.Divide;
            binder["modulo"] = Functions<TContext>.Modulo;
        }

        /*
         * Empty class to help create a Lambda function without wanting to
         * bind to some specific type, but rather simply evaluate Lizzie code,
         * without having bound it to anything in particular.
         */
        public class Nothing { };
    }
}
