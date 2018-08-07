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

using System.Collections.Generic;
using NUnit.Framework;
using poetic.lizzie;
using poetic.lambda.parser;

namespace poetic.tests.lizzie_tests
{
    [TestFixture]
    public class TokenizerTest
    {
        [Test]
        public void Tokenize_1()
        {
            const string code = @"foo()
bar[5]";
            var tokenizer = new Tokenizer(code, new LizzieTokenizer());
            var list = new List<string>(tokenizer);
            Assert.AreEqual(7, list.Count);
        }

        [Test]
        public void Tokenize_2()
        {
            const string code = @"  foo   ( )   
bar [    5 ]   

";
            var tokenizer = new Tokenizer(code, new LizzieTokenizer());
            var list = new List<string>(tokenizer);
            Assert.AreEqual(7, list.Count);
        }

        [Test]
        public void Tokenize_3()
        {
            const string code = @"foo(())bar([5],[7])";
            var tokenizer = new Tokenizer(code, new LizzieTokenizer());
            var list = new List<string>(tokenizer);
            Assert.AreEqual(15, list.Count);
        }

        [Test]
        public void Tokenize_4()
        {
            const string code = @"foo ( 57, 77 )
bar ( {  hello_world : howdy  ,
    howdy : [  77  ,  2  ,  57 ] }   )   ";
            var tokenizer = new Tokenizer(code, new LizzieTokenizer());
            var list = new List<string>(tokenizer);
            Assert.AreEqual(24, list.Count);
        }
    }
}
