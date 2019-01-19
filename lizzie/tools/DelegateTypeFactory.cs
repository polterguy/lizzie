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
using System.Collections.Generic;

namespace lizzie.tools
{
    /*
     * Internal class used to create delegate types when binding "deeply", towards
     * for instance an interface, to allow for binding Lizzie towards an inherited
     * type.
     * 
     * Uses Reflection.Emit to create a delegate type during Lizzie compilation process.
     */
    internal sealed class DelegateTypeFactory
    {
        // Singleton implementation fields.
        static DelegateTypeFactory _instance;
        static object _locker = new object();

        // Fields.
        readonly ModuleBuilder _builder;
        Synchronizer<Dictionary<string, Type>> _delegateTypesSynchronizer = new Synchronizer<Dictionary<string, Type>>(new Dictionary<string, Type>());

        // Private CTOR to avoid instantiations of more than one single instance (Singleton pattern).
        DelegateTypeFactory()
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Lizzie.DelegateFactory"), AssemblyBuilderAccess.RunAndCollect);
            _builder = assembly.DefineDynamicModule("Lizzie.DelegateFactory");
        }

        /*
         * Returns the Singleton instance.
         */
        public static DelegateTypeFactory Instance
        {
            get {
                if (_instance == null) {
                    lock (_locker) {

                        // Double check, in case of context switch after checking for null.
                        if (_instance == null) {
                            _instance = new DelegateTypeFactory();
                        }
                    }
                }
                return _instance;
            }
        }

        /*
         * Returns a delegate type for the specified method.
         */
        public Type GetDelegateType(MethodInfo method)
        {
            /*
             * Using full name of type as dictionary key.
             * 
             * NOTICE!
             * Since each declaring type needs a unique delegate type, we
             * use the declaring type's full name as our dictionary lookup, to
             * avoid creating the same delegate type more than once.
             */
            var dictionaryKey = method.DeclaringType.FullName;

            /*
             * Checking if we have already created a delegate type for MethodInfo.
             * Making sure we synchronize access to our shared dictionary.
             */
            Type delegateType = null;
            _delegateTypesSynchronizer.Read((dictionary) => {

                // Checking if delegate type has already been cached.
                if (dictionary.ContainsKey(dictionaryKey)) {
                    delegateType = dictionary[dictionaryKey];
                }
            });

            // Checking if we already have a delegate type wrapping declaring type's methods.
            if (delegateType != null)
                return delegateType;

            /*
             * Creating our delegate type for MethodInfo's declaring type, and stores it in our
             * dictionary, to avoid creating multiple delegate types for the same MethodInfo.
             * Making sure we synchronize access to our shared dictionary.
             */
            _delegateTypesSynchronizer.Write((dictionary) => {

                /*
                 * In case a context switch occurs between our Read lambda and our Write lambda
                 * we need to "double check" if some other thread was able to create
                 * our delegate type also inside of this lambda.
                 */
                if (!dictionary.ContainsKey(dictionaryKey)) {

                    // Creates our delegate type, and caches it in dictionary.
                    delegateType = CreateDelegateType(method);
                    dictionary[dictionaryKey] = delegateType;
                }
            });
            return delegateType;
        }

        #region [ -- Private helper methods -- ]

        /*
         * Dynamically creates and emits our delegate type into our ModuleBuilder.
         */
        private Type CreateDelegateType(MethodInfo method)
        {
            // Creates a unique type name to emit.
            string baseName = string.Format("{0}.{1}", method.DeclaringType.Name, method.Name);
            string uniqueTypeName = CreateUniqueTypeName(baseName);

            // Defines our type and constructor to our type.
            var typeBuilder = _builder.DefineType(
                uniqueTypeName,
                TypeAttributes.Sealed | TypeAttributes.Public,
                typeof(MulticastDelegate));
            var constructor = typeBuilder.DefineConstructor(
                MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
                CallingConventions.Standard,
                new[] { typeof(object), typeof(IntPtr) });
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
            var invokeMethod = typeBuilder.DefineMethod(
                "Invoke",
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

        #endregion
    }
}