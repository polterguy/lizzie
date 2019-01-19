/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System.Collections.Generic;
using NUnit.Framework;
using lizzie.exceptions;

namespace lizzie.tests
{
    public class TokenizerTests
    {
        [Test]
        public void FunctionInvocationInteger()
        {
            var code = "a(1)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(4, list.Count);
        }

        [Test]
        public void FunctionInvocationString()
        {
            var code = @"a(""foo"")";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
        }

        [Test]
        public void FunctionInvocationMixed()
        {
            var code = @"a(""foo"", 5, ""bar"")";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(12, list.Count);
        }

        [Test]
        public void FunctionInvocationMixedNested()
        {
            var code = @"a(""foo"", bar(5), ""bar"")";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(15, list.Count);
        }

        [Test]
        public void WeirdSpacing()
        {
            var code = @"  a (   ""\""fo\""o""   ,bar(  5 )     ,""bar""   )   ";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(15, list.Count);
        }

        [Test]
        public void SingleLineComment()
        {
            var code = @"a(""foo"", bar(5)) // , ""bar"")";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(11, list.Count);
        }

        [Test]
        public void MultiLineComment()
        {
            var code = @"
a(""foo"" /* comment */,
     bar(5)/* FOO!! *** */ ) // , ""bar"")
   /* 
 * hello
 */
jo()";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(14, list.Count);
        }

        [Test]
        public void MultiLineCommentThrows()
        {
            var code = @"/*/";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var success = false;
            try {
                var list = new List<string>(tokenizer.Tokenize(code));
            } catch (LizzieTokenizerException) {
                success = true;
            }
            Assert.AreEqual(true, success);
        }

        [Test]
        public void EmptyMultiLineComment()
        {
            var code = @"/**/";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(0, list.Count);
        }
    }
}
