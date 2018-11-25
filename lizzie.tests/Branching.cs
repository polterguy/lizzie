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
    public class Branching
    {
        [Test]
        public void IfVariableHasValueTrue()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo, 1)
if(foo, {
  57
})
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void IfVariableHasValueFalse()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo)
if(foo, {
  57
})
");
            var result = lambda();
            Assert.IsNull(result);
        }

        [Test]
        public void LazyIfConditionYieldsTrue()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo, function({
  67
}))
if(@foo(), {
  57
})
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void DeclaredArgumentPassedIn()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo,function({
  input
}, @input))
if(@foo(""howdy""), {
  57
})
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void ElseYieldsTrue()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo)
if(foo, {
  67
}, {
  57
})
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void ElseYieldsFalse()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo, 1)
if(foo, {
  67
}, {
  57
})
");
            var result = lambda();
            Assert.AreEqual(67, result);
        }

        [Test]
        public void IfEqualsTrue()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo, 7)
if(eq(foo, 7), {
  57
}, {
  67
})
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void IfEqualsFalse()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo, 5)
if(eq(foo, 7), {
  57
}, {
  67
})
");
            var result = lambda();
            Assert.AreEqual(67, result);
        }

        [Test]
        public void IfNotEqualsTrue()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo, 7)
if(not(eq(foo, 7)), {
  57
}, {
  67
})
");
            var result = lambda();
            Assert.AreEqual(67, result);
        }

        [Test]
        public void IfNotEqualsFalse()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo, 5)
if(not(eq(foo, 7)), {
  57
}, {
  67
})
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void IfMoreThanTrue()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo, 7)
if(mt(foo, 5), {
  57
}, {
  67
})
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void IfMoreThanFalse()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo, 5)
if(mt(foo, 7), {
  57
}, {
  67
})
");
            var result = lambda();
            Assert.AreEqual(67, result);
        }

        [Test]
        public void IfLessThanTrue()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo, 7)
if(lt(foo, 9), {
  57
}, {
  67
})
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void IfLessThanFalse()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo, 5)
if(lt(foo, 3), {
  57
}, {
  67
})
");
            var result = lambda();
            Assert.AreEqual(67, result);
        }

        [Test]
        public void IfMoreThanEqualsTrue()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo, 7)
if(mte(foo, 7), {
  57
}, {
  67
})
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void IfMoreThanEqualsFalse()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo, 5)
if(mte(foo, 7), {
  57
}, {
  67
})
");
            var result = lambda();
            Assert.AreEqual(67, result);
        }

        [Test]
        public void IfLessThanEqualsTrue()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo, 7)
if(lte(foo, 9), {
  57
}, {
  67
})
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void IfLessThanEqualsFalse()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo, 5)
if(lte(foo, 3), {
  57
}, {
  67
})
");
            var result = lambda();
            Assert.AreEqual(67, result);
        }

        [Test]
        public void IfAnyTrue()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo1)
var(@foo2, 1)
var(@foo3)
if(any(@foo1, @foo2, @foo3), {
  57
}, {
  67
})
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void IfAnyTrueFunction()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo1)
var(@foo2, function({
  'foo'
}))
var(@foo3)
if(any(@foo1, @foo2(), @foo3), {
  57
}, {
  67
})
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void IfAnyFalse()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo1)
var(@foo2)
var(@foo3)
if(any(@foo1, @foo2, @foo3), {
  57
}, {
  67
})
");
            var result = lambda();
            Assert.AreEqual(67, result);
        }

        [Test]
        public void IfAllTrue()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo1, 1)
var(@foo2, 2)
var(@foo3, 3)
if(all(@foo1, @foo2, @foo3), {
  57
}, {
  67
})
");
            var result = lambda();
            Assert.AreEqual(57, result);
        }

        [Test]
        public void IfAllFalse()
        {
            var lambda = LambdaCompiler.Compile(@"
var(@foo1, 1)
var(@foo2, 2)
var(@foo3)
if(all(@foo1, @foo2, @foo3), {
  57
}, {
  67
})
");
            var result = lambda();
            Assert.AreEqual(67, result);
        }
    }
}
