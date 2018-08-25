/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System.Collections.Generic;
using NUnit.Framework;
using lizzie.tests.context_types;

namespace lizzie.tests
{
    public class EvalTests
    {
        [Test]
        public void EvaluateInlineCode()
        {
            var lambda = LambdaCompiler.Compile(@"eval(""+(57,10,10)"")");
            var result = lambda();
            Assert.AreEqual(77, result);
        }
    }
}
