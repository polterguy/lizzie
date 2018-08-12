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

using System.IO;
using System.Text;
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
        public void Tokenize_01()
        {
            const string code = @"foo()
bar[5]";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(7, list.Count);
        }

        [Test]
        public void Tokenize_02()
        {
            const string code = @"  foo   ( )   
bar [    5 ]   

";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(7, list.Count);
        }

        [Test]
        public void Tokenize_03()
        {
            const string code = @"foo(())bar([5],[7])";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(15, list.Count);
        }

        [Test]
        public void Tokenize_04()
        {
            const string code = @"foo ( 57, 77 )
bar ( {  hello_world : howdy  ,
    howdy : [  77  ,  2  ,  57 ] }   )   ";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(24, list.Count);
        }

        [Test]
        public void Tokenize_05()
        {
            const string code = @"
function foo(input) {
  var x = [1,2,3]
  var y = {first:1,second:2 }
}";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            string result = "";
            foreach (var ix in list) {
                result += " " + ix;
            }
            Assert.AreEqual(29, list.Count);
            Assert.AreEqual(" function foo ( input ) { var x = [ 1 , 2 , 3 ] var y = { first : 1 , second : 2 } }", result);
        }

        [Test]
        public void Tokenize_06()
        {
            const string code = @"
 function foo   ( input     ){
  var x=    [      1   ,  2  ,  3    ]
  var y ={   first  : 1 , second :2}}";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            string result = "";
            foreach (var ix in list) {
                result += " " + ix;
            }
            Assert.AreEqual(29, list.Count);
            Assert.AreEqual(" function foo ( input ) { var x = [ 1 , 2 , 3 ] var y = { first : 1 , second : 2 } }", result);
        }

        [Test]
        public void Tokenize_07()
        {
            const string code = @"function foo(input){var x=[1,2,3]var y={first:1,second:2}}";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            string result = "";
            foreach (var ix in list) {
                result += " " + ix;
            }
            Assert.AreEqual(29, list.Count);
            Assert.AreEqual(" function foo ( input ) { var x = [ 1 , 2 , 3 ] var y = { first : 1 , second : 2 } }", result);
        }

        [Test]
        public void Tokenize_08()
        {
            const string code = @"
foo(""Hello World"")";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual("\"", list[2]);
            Assert.AreEqual("Hello World", list[3]);
            Assert.AreEqual("\"", list[4]);
            string result = "";
            foreach (var ix in list) {
                result += " " + ix;
            }
            Assert.AreEqual(" foo ( \" Hello World \" )", result);
        }

        [Test]
        public void Tokenize_09()
        {
            const string code = @"
foo(""Hello \""World"")";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual("Hello \"World", list[3]);
        }

        [Test]
        public void Tokenize_10()
        {
            const string code = @"
foo(""Hello \r\nWorld"")";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual("Hello \r\nWorld", list[3]);
        }

        [Test]
        public void Tokenize_11()
        {
            const string code = @"
foo(""Hello \nWorld"")";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual("Hello \nWorld", list[3]);
        }

        [Test]
        public void Tokenize_12()
        {
            const string code = @"
foo(""Hello \rWorld"")";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual("Hello \rWorld", list[3]);
        }

        [Test]
        public void Tokenize_13()
        {
            const string code = @"
foo('Hello World')";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual("'", list[2]);
            Assert.AreEqual("Hello World", list[3]);
            Assert.AreEqual("'", list[4]);
            string result = "";
            foreach (var ix in list) {
                result += " " + ix;
            }
            Assert.AreEqual(" foo ( ' Hello World ' )", result);
        }

        [Test]
        public void Tokenize_14()
        {
            const string code = @"
foo('Hello \'World')";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual("Hello 'World", list[3]);
        }

        [Test]
        public void Tokenize_15()
        {
            const string code = @"
foo('Hello \r\nWorld')";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual("'", list[2]);
            Assert.AreEqual("Hello \r\nWorld", list[3]);
            Assert.AreEqual("'", list[4]);
        }

        [Test]
        public void Tokenize_16()
        {
            const string code = @"
foo('Hello \tWorld""""')";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual("'", list[2]);
            Assert.AreEqual("Hello \tWorld\"\"", list[3]);
            Assert.AreEqual("'", list[4]);
        }

        [Test]
        public void Tokenize_17()
        {
            const string code = @"
foo(""Hello World''"")";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual("\"", list[2]);
            Assert.AreEqual("Hello World''", list[3]);
            Assert.AreEqual("\"", list[4]);
        }

        [Test]
        public void Tokenize_18()
        {
            const string code = @"foo.bar(57)";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
        }

        [Test]
        public void Tokenize_19()
        {
            const string code = @"
var foo = 5
foo += 52";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(7, list.Count);
            Assert.AreEqual("+=", list[5]);
        }

        [Test]
        public void Tokenize_20()
        {
            const string code = @"
var foo = 5
foo++";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual("++", list[5]);
        }

        [Test]
        public void Tokenize_21()
        {
            const string code = @"
var foo = 5
++foo";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual("++", list[4]);
        }

        [Test]
        public void Tokenize_22()
        {
            const string code = @"
var foo = 5
++foo // Howdy world!!


";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(6, list.Count);
        }

        [Test]
        public void Tokenize_23()
        {
            const string code = @"
  // SOME COMMENT!!


var/*INLINE IN CODE*/ howdy /* MORE INLINE ! */ foo = /**/5 // Previous is intentionally empty
++/* jo */foo // Howdy world!!
//
// Above is intentionally empty!

/* multiline
 * foo
 * bar
 */
";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(7, list.Count);
        }

        [Test]
        public void Tokenize_24()
        {
            const string code = @"
var foo = 5.0;
foo += 1.57;";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(code));
            Assert.AreEqual(9, list.Count);
            Assert.AreEqual("1.57", list[7]);
        }

        [Test]
        public void Tokenize_25()
        {
            const string code1 = "foo(57)";
            const string code2 = "bar(67)";
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(new string[] { code1, code2 }));
            Assert.AreEqual(8, list.Count);
            Assert.AreEqual("57", list[2]);
            Assert.AreEqual("67", list[6]);
        }

        [Test]
        public void Tokenize_26()
        {
            Stream code1 = new MemoryStream(Encoding.UTF8.GetBytes("foo(57)"));
            Stream code2 = new MemoryStream(Encoding.UTF8.GetBytes("bar(67)"));
            var tokenizer = new lambda.parser.Tokenizer(new lizzie.Tokenizer());
            var list = new List<string>(tokenizer.Tokenize(new Stream[] { code1, code2 }));
            Assert.AreEqual(8, list.Count);
            Assert.AreEqual("57", list[2]);
            Assert.AreEqual("67", list[6]);
        }
    }
}
