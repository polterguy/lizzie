/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using NUnit.Framework;
using lizzie.tests.context_types;

namespace lizzie.tests
{
    public class Binder
    {
        [Test]
        public void CloneWithoutStack()
        {
            var original = new Binder<SimpleValues>();
            var clone = original.Clone();
            foreach (var ix in original.StaticItems) {
                Assert.AreEqual(clone[ix], original[ix]);
            }
            foreach (var ix in clone.StaticItems) {
                Assert.AreEqual(clone[ix], original[ix]);
            }
            Assert.AreEqual(0, clone.StackCount);
        }

        [Test]
        public void CloneWithPushedStack()
        {
            var original = new Binder<SimpleValues>();
            original.PushStack();
            original["foo"] = 57;
            original.PushStack();
            original["bar"] = 77;
            var clone = original.Clone();
            foreach (var ix in original.StaticItems) {
                Assert.AreEqual(clone[ix], original[ix]);
            }
            foreach (var ix in clone.StaticItems) {
                Assert.AreEqual(clone[ix], original[ix]);
            }
            Assert.AreEqual(2, clone.StackCount);
            Assert.AreEqual(77, clone["bar"]);
            clone.PopStack();
            Assert.AreEqual(57, clone["foo"]);
        }

        [Test]
        public void StaticFunctions()
        {
            var lambda = LambdaCompiler.Compile(new SimpleValues(), "get-static()");
            var result = lambda();
            Assert.AreEqual(7, result);
        }

        class SimpleValueExtended : SimpleValues
        {
            [Bind(Name = "extended-function")]
            protected object ExtendedFunction(Binder<SimpleValues> ctx, Arguments arguments)
            {
                return arguments.Get<int>(0) + 57;
            }
        }

        class SimpleValueDoubleExtended : SimpleValueExtended
        {
            [Bind(Name = "extended-function-2")]
            object ExtendedFunction2(Binder<SimpleValues> ctx, Arguments arguments)
            {
                return arguments.Get<int>(0) + 3;
            }
        }

        [Test]
        public void ExtendedClassShallowBind()
        {
            SimpleValues simple = new SimpleValueExtended();
            var lambda = LambdaCompiler.Compile(simple, "extended-function(20)");
            var error = false;
            try {
                var result = lambda();
            } catch {
                error = true;
            }
            Assert.AreEqual(true, error);
        }

        [Test]
        public void ExtendedClassDeepBind()
        {
            SimpleValues simple = new SimpleValueExtended();
            var lambda = LambdaCompiler.Compile(simple, "extended-function(20)", true);
            var result = lambda();
            Assert.AreEqual(77, result);
        }

        [Test]
        public void ExtendedClassDeepBindInvokeInheritedPublicMethod()
        {
            SimpleValues simple = new SimpleValueExtended();
            var lambda = LambdaCompiler.Compile(simple, "+(get-constant-integer-2(), extended-function(3))", true);
            var result = lambda();
            Assert.AreEqual(117, result);
        }

        [Test]
        public void ExtendedClassDeepBindInvokeInheritedProtectedMethod()
        {
            SimpleValues simple = new SimpleValueExtended();
            var lambda = LambdaCompiler.Compile(simple, "+(get-constant-integer(), extended-function(3))", true);
            var result = lambda();
            Assert.AreEqual(117, result);
        }

        [Test]
        public void ExtendedClassDeepBindInvokeInheritedStaticMethod()
        {
            SimpleValues simple = new SimpleValueExtended();
            var lambda = LambdaCompiler.Compile(simple, "+(get-static(), extended-function(20))", true);
            var result = lambda();
            Assert.AreEqual(84, result);
        }

        [Test]
        public void DoubleExtendedClassDeepBindInvokeInheritedStaticMethod()
        {
            SimpleValues simple = new SimpleValueDoubleExtended();
            var lambda = LambdaCompiler.Compile(simple, "+(get-static(), extended-function(20), extended-function-2(3))", true);
            var result = lambda();
            Assert.AreEqual(90, result);
        }

        class BaseClass
        {
            [Bind(Name = "foo")]
            protected virtual object Foo(Binder<BaseClass> ctx, Arguments arguments)
            {
                return 57;
            }
        }

        class SuperClass : BaseClass
        {
            [Bind(Name = "foo")]
            protected override object Foo(Binder<BaseClass> ctx, Arguments arguments)
            {
                return 77;
            }
        }

        [Test]
        public void VirtualInheritedDeeplyBound()
        {
            BaseClass simple = new SuperClass();
            var lambda = LambdaCompiler.Compile(simple, "foo()", true);
            var result = lambda();
            Assert.AreEqual(77, result);
        }

        [Test]
        public void DeeplyBound()
        {
            SimpleValues simple = new SimpleValueDoubleExtended();
            var binder = new Binder<SimpleValues>(simple);
            Assert.AreEqual(true, binder.DeeplyBound);
        }

        [Test]
        public void ShallowBound()
        {
            SimpleValues simple = new SimpleValues();
            var binder = new Binder<SimpleValues>();
            Assert.AreEqual(false, binder.DeeplyBound);
        }

        [Test]
        public void MaxStackSizeNoThrow()
        {
            var nothing = new LambdaCompiler.Nothing();
            var binder = new Binder<LambdaCompiler.Nothing> {
                MaxStackSize = 21
            };
            LambdaCompiler.BindFunctions(binder);
            var lambda = LambdaCompiler.Compile(nothing, binder, @"
var(@recursions, 0)
var(@my-func, function({
  set(@recursions,+(recursions,1))
  if(lt(recursions,20),{
    my-func()
  })
}))
my-func()
");
            // Should NOT throw!
            lambda();

            // Verifying instance is NOT deeply bound!
            Assert.AreEqual(false, binder.DeeplyBound);
        }

        [Test]
        public void MaxStackSizeThrows()
        {
            var nothing = new LambdaCompiler.Nothing();
            var binder = new Binder<LambdaCompiler.Nothing> {
                MaxStackSize = 19
            };
            LambdaCompiler.BindFunctions(binder);
            var lambda = LambdaCompiler.Compile(nothing, binder, @"
var(@recursions, 0)
var(@my-func, function({
  set(@recursions,+(recursions,1))
  if(lt(recursions,20),{
    my-func()
  })
}))
my-func()
");
            // SHOULD throw!
            var success = false;
            try {
                lambda();
            } catch {
                success = true;
            }
            Assert.AreEqual(true, success);
        }
    }
}
