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
            // Variables.
            binder["var"] = Functions<TContext>.Var;
            binder["set"] = Functions<TContext>.Set;

            // Branching and comparison functions.
            binder["if"] = Functions<TContext>.If;
            binder["eq"] = Functions<TContext>.Eq;
            binder["mt"] = Functions<TContext>.Mt;
            binder["lt"] = Functions<TContext>.Lt;
            binder["mte"] = Functions<TContext>.Mte;
            binder["lte"] = Functions<TContext>.Lte;
            binder["not"] = Functions<TContext>.Not;
            binder["any"] = Functions<TContext>.Any;
            binder["all"] = Functions<TContext>.All;

            // Creates a function.
            binder["function"] = Functions<TContext>.Function;

            // List functions,
            binder["list"] = Functions<TContext>.List;
            binder["count"] = Functions<TContext>.Count;
            binder["get"] = Functions<TContext>.Get;
            binder["add"] = Functions<TContext>.AddList;
            binder["slice"] = Functions<TContext>.Slice;
            binder["each"] = Functions<TContext>.Each;

            // Conversion functions.
            binder["string"] = Functions<TContext>.String;
            binder["number"] = Functions<TContext>.Number;

            // Math functions.
            binder["+"] = Functions<TContext>.Add;
            binder["-"] = Functions<TContext>.Subtract;
            binder["*"] = Functions<TContext>.Multiply;
            binder["/"] = Functions<TContext>.Divide;
            binder["%"] = Functions<TContext>.Modulo;

            // String functions.
            binder["substr"] = Functions<TContext>.Substr;
            binder["length"] = Functions<TContext>.Length;
            binder["replace"] = Functions<TContext>.Replace;

            // Null is simply a constant yielding null.
            binder["null"] = null;
        }

        /*
         * Empty class to help create a Lambda function without needing to
         * bind to some specific type, but rather simply evaluate Lizzie code,
         * without having bound it to anything in particular.
         */
        public class Nothing { };
    }
}
