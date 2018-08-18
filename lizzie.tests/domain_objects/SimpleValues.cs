/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

namespace lizzie.tests.domain_objects
{
    public class SimpleValues
    {
        public int ValueInteger { get; set; }
        public string ValueString { get; set; }

        [Bind(Name = "set-value-integer")]
        object SetValueInteger(Binder<SimpleValues> ctx, Arguments arguments)
        {
            ValueInteger = arguments.Get<int>(0);
            return null;
        }

        [Bind(Name = "get-value-integer")]
        object GetValueInteger(Binder<SimpleValues> ctx, Arguments arguments)
        {
            return ValueInteger;
        }

        [Bind(Name = "set-value-string")]
        object SetValueString(Binder<SimpleValues> ctx, Arguments arguments)
        {
            ValueString = arguments.Get<string>(0);
            return null;
        }

        [Bind(Name = "get-constant-integer")]
        object GetConstantInteger(Binder<SimpleValues> ctx, Arguments arguments)
        {
            return 57;
        }

        [Bind(Name = "add-integers")]
        object AddIntegers(Binder<SimpleValues> ctx, Arguments arguments)
        {
            ValueInteger = arguments.Get<int>(0) + arguments.Get<int>(1);
            return null;
        }
    }
}
