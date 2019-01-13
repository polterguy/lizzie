/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace lizzie
{
    /*
     * Internal class used to create delegate types when binding "deeply", towards
     * for instance an interface, to allow for binding Lizzie towards an inherited
     * type.
     * 
     * Uses Reflection.Emit to create a delegate type during Lizzie compilation process.
     */
    internal class DelegateTypeFactory
    {
        readonly ModuleBuilder _builder;

        public DelegateTypeFactory()
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Lizzie.DelegateFactory"), AssemblyBuilderAccess.RunAndCollect);
            _builder = assembly.DefineDynamicModule("Lizzie.DelegateFactory");
        }

        /*
         * Dynamically creates and emits our delegate type into our ModuleBuilder.
         */
        public Type CreateDelegateType(MethodInfo method)
        {
            // Creates a unique type name to emit.
            string baseName = string.Format("{0}.{1}", method.DeclaringType.Name, method.Name);
            string uniqueTypeName = CreateUniqueTypeName(baseName);

            // Defines our type and constructor to our type.
            var typeBuilder = _builder.DefineType(uniqueTypeName, TypeAttributes.Sealed | TypeAttributes.Public, typeof(MulticastDelegate));
            var constructor = typeBuilder.DefineConstructor(
                MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
                CallingConventions.Standard, new[] { typeof(object), typeof(IntPtr) });
            constructor.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            /*
             * Retrieves arguments from method, and make sure our delegate types can handle the same set of arguments.
             * 
             * NOTICE!
             * To allow for "late binding" delegate towards its this pointer for instance methods,
             * we need to check if method is static, and if not, we allow for implicitly passing
             * in the "this pointer" as its first argument.
             */
            var parameters = method.GetParameters();
            var parameterTypes = parameters.Select(p => p.ParameterType).ToList();
            if (!method.IsStatic)
                parameterTypes.Insert(0, method.DeclaringType);
            var invokeMethod = typeBuilder.DefineMethod("Invoke",
                                                        MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public,
                                                        method.IsStatic ? CallingConventions.Standard : CallingConventions.HasThis,
                                                        method.ReturnType,
                                                        parameterTypes.ToArray());
            invokeMethod.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            // Defining arguments to our method.
            for (int i = 0; i < parameters.Length; i++) {
                var parameter = parameters[i];
                invokeMethod.DefineParameter(i + 1, ParameterAttributes.None, parameter.Name);
            }

            // Returns our delegate type to caller.
            return typeBuilder.CreateTypeInfo();
        }

        /*
         * Creates a unique type name to use during Reflection.Emit.
         */
        private string CreateUniqueTypeName(string nameBase)
        {
            int number = 2;
            string name = nameBase;
            while (_builder.GetType(name) != null)
                name = nameBase + number++;
            return name;
        }
    }
}