/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using NUnit.Framework;
using lizzie.tests.context_types;
using lizzie.exceptions;

namespace lizzie.tests
{
    public class VariableTests
    {
        [Test]
        public void VariableAssignedToIntegerValue()
        {
            var code = @"
var(@foo, 57)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["var"] = Functions<Nothing>.Var;
            var result = function(ctx, binder);
            Assert.AreEqual(57, result);
        }

        [Test]
        public void VariableAssignedToFloatingPointValue()
        {
            var code = @"
var(@foo, 57.67)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["var"] = Functions<Nothing>.Var;
            var result = function(ctx, binder);
            Assert.AreEqual(57.67, result);
        }

        [Test]
        public void VariableAssignedToStringLiteralValue()
        {
            var code = @"
var(@foo, ""bar"")
foo";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["var"] = Functions<Nothing>.Var;
            var result = function(ctx, binder);
            Assert.AreEqual("bar", result);
        }

        [Test]
        public void VariableDeReferenced()
        {
            var code = @"
var(@foo, 57)
foo";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["var"] = Functions<Nothing>.Var;
            var result = function(ctx, binder);
            Assert.AreEqual(57, result);
        }

        [Test]
        public void NonExistentVariableThrows()
        {
            var code = "foo";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            var success = false;
            try {
                function(ctx, binder);
            } catch (LizzieRuntimeException) {
                success = true;
            }
            Assert.IsTrue(success);
        }

        [Test]
        public void VariableDeReferencedInsideBody()
        {
            var code = @"{
  var(@foo, 57)
  foo
}";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["var"] = Functions<Nothing>.Var;
            var result = function(ctx, binder);
            Assert.AreEqual(57, result);
        }

        [Test]
        public void VariableDeReferencedOutsideOfScopeThrows()
        {
            var code = @"{
  var(@foo, 57)
}
foo";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["var"] = Functions<Nothing>.Var;
            var success = false;
            try {
                function(ctx, binder);
            } catch (LizzieRuntimeException) {
                success = true;
            }
            Assert.IsTrue(success);
        }

        [Test]
        public void VariableDeclaredTwiceThrows()
        {
            var code = @"
var(@foo, 57)
var(@foo, 57)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["var"] = Functions<Nothing>.Var;
            var success = false;
            try {
                function(ctx, binder);
            } catch (LizzieRuntimeException) {
                success = true;
            }
            Assert.IsTrue(success);
        }

        [Test]
        public void VariableDeclarationWithoutInitialAssignment()
        {
            var code = @"
var(@foo)
foo";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["var"] = Functions<Nothing>.Var;
            var result = function(ctx, binder);
            Assert.IsNull(result);
        }

        [Test]
        public void VariableReAssignment()
        {
            var code = @"
var(@foo, 57)
set(@foo, 67)
foo";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["var"] = Functions<Nothing>.Var;
            binder["set"] = Functions<Nothing>.Set;
            var result = function(ctx, binder);
            Assert.AreEqual(67, result);
        }

        [Test]
        public void VariableReAssignedToNull()
        {
            var code = @"
var(@foo, 57)
set(@foo)
foo";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["var"] = Functions<Nothing>.Var;
            binder["set"] = Functions<Nothing>.Set;
            var result = function(ctx, binder);
            Assert.IsNull(result);
        }

        [Test]
        public void ReAssigningStaticallyCompiledValueThrows()
        {
            var code = @"
set(@foo)";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["var"] = Functions<Nothing>.Var;
            binder["set"] = Functions<Nothing>.Set;
            binder["foo"] = 57;
            var success = false;
            try {
                function(ctx, binder);
            } catch (LizzieRuntimeException) {
                success = true;
            }
            Assert.IsTrue(success);
        }

        [Test]
        public void VariableChangedFromDoubleToString()
        {
            var code = @"
var(@foo, 57.67)
set(@foo, ""bar"")
foo";
            var tokenizer = new Tokenizer(new LizzieTokenizer());
            var function = Compiler.Compile<Nothing>(tokenizer, code);
            var ctx = new Nothing();
            var binder = new Binder<Nothing>();
            binder["var"] = Functions<Nothing>.Var;
            binder["set"] = Functions<Nothing>.Set;
            var result = function(ctx, binder);
            Assert.AreEqual("bar", result);
        }
    }
}
