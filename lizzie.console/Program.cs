/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System;

namespace lizzie.console
{
    class MainClass
    {
        // This method will be available as a function in your Lizzie code.
        [Bind(Name = "write-line")]
        object ConsoleWriteLine(Binder<MainClass> ctx, Arguments arguments)
        {
            Console.WriteLine(arguments.Get<string>(0));
            return null;
        }

        public static void Main(string[] args)
        {
            // Some inline Lizzie code
            var code = @"
var(@foo, 57)
set(@foo, add(foo, 10))
write-line(foo)
";

            // Creating a tokenizer to tokenize our Lizzie code.
            var tokenizer = new Tokenizer(new LizzieTokenizer());

            // Creating a lambda function from our code.
            var function = Compiler.Compile<MainClass>(tokenizer, code);

            // Creating an instance of our class, which we can bind to our code.
            var context = new MainClass();

            // Creating a binder, and adding some keywords to it.
            var binder = new Binder<MainClass>();
            binder["var"] = Functions<MainClass>.Var;
            binder["set"] = Functions<MainClass>.Set;
            binder["add"] = Functions<MainClass>.Add;

            // Evaluates our Lizzie code.
            function(context, binder);

            // Waiting for user input.
            Console.Read();
        }
    }
}