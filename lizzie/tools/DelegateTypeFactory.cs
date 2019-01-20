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
     * for instance an interface, to allow for binding Lizzie towards the type
     * of your context instance, instead of the generic type argument.
     * 
     * Uses Reflection.Emit to create a delegate type during Lizzie's compilation
     * process. This class is useful when you don't know the most derived type of
     * your context, since the type you're dealing with in your own code is for
     * instance a base class or an interface. Helps with binding Lizzie code in
     * scenarios such as when your context is for instance an interface, and
     * you're using Dependency Injection, etc.
     * 
     * NOTICE!
     * Class is intentionally created as "internal", even though it's probably
     * highly useful as a library helper class, since I want to be able to
     * move it into its own library later at some point, without breaking
     * existing use of Lizzie.
     */
    internal sealed class DelegateTypeFactory
    {
        // Singleton implementation fields.
        static DelegateTypeFactory _instance;

        // Required to avoid race conditions during Singleton instantiation.
        static object _locker = new object();

        // Module builder that allows us to "Emit" code into our AppDomain.
        readonly ModuleBuilder _builder;

        /*
         * Synchronized dictionary containing all delegate types.
         * Serves as a "cache", to avoid creating the same delegate type multiple
         * times.
         */
        Synchronizer<Dictionary<string, TypeInfo>> _delegateTypeSynchronizer = 
            new Synchronizer<Dictionary<string, TypeInfo>>(new Dictionary<string, TypeInfo>());

        // Private CTOR to avoid instantiations of more than one instance (Singleton pattern).
        DelegateTypeFactory()
        {
            // Creates our dynamic assembly, where our delegate types will be emitted.
            var assembly = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("Lizzie.DelegateFactory"),
                AssemblyBuilderAccess.RunAndCollect);
            _builder = assembly.DefineDynamicModule("Lizzie.DelegateFactory");
        }

        /*
         * Returns the Singleton instance.
         */
        public static DelegateTypeFactory Instance
        {
            get {
                if (_instance == null) {

                    // Avoiding race conditions in multithreaded solutions.
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
         * Returns a delegate type for the specified MethodInfo.
         */
        public TypeInfo GetDelegateType(MethodInfo method)
        {
            /*
             * Using the full name of the declaring type as dictionary/"cache"
             * key, in addition to whether or not the method is static or not.
             * 
             * This ensures one delegate type for static and instance methods,
             * for each declaring type.
             */
            var dictionaryKey = method.DeclaringType.FullName + (method.IsStatic ? "_static" : "");

            /*
             * Checking if we have already created a delegate type for the MethodInfo.
             * Making sure we synchronize access to our shared dictionary.
             */
            var delegateType = _delegateTypeSynchronizer.Fetch((dictionary) => {

                // Checking if delegate type has already been cached.
                if (dictionary.ContainsKey(dictionaryKey)) {
                    return dictionary[dictionaryKey];
                }
                return null;
            });

            // Returning previously created delegate type, if one has been created.
            if (delegateType != null)
                return delegateType;

            /*
             * Creating our delegate type for MethodInfo's declaring type, and
             * store it in our dictionary, to avoid creating multiple delegate
             * types for the same MethodInfo.
             * Making sure we synchronize access to our shared dictionary.
             */
            _delegateTypeSynchronizer.Write((dictionary) => {

                /*
                 * In case a context switch occurs between our Read lambda and our Write lambda
                 * we need to "double check" if some other thread was able to create
                 * our delegate type also inside of this lambda.
                 */
                if (!dictionary.ContainsKey(dictionaryKey)) {

                    // Creates our delegate type, and caches it in dictionary.
                    delegateType = CreateDelegateType(method, dictionaryKey);
                    dictionary[dictionaryKey] = delegateType;
                }
            });
            return delegateType;
        }

        #region [ -- Private helper methods -- ]

        /*
         * Dynamically creates and emits our delegate type into our ModuleBuilder.
         */
        TypeInfo CreateDelegateType(MethodInfo method, string delegateTypeName)
        {
            // Defines our type and constructor to our type.
            var typeBuilder = _builder.DefineType(
                delegateTypeName,
                TypeAttributes.Sealed | TypeAttributes.NotPublic,
                typeof(MulticastDelegate));
            var constructor = typeBuilder.DefineConstructor(
                MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
                CallingConventions.Standard,
                new[] { typeof(object), typeof(IntPtr) });
            constructor.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            /*
             * Retrieves arguments from method, and make sure our delegate type
             * can handle the same set of arguments.
             * 
             * NOTICE!
             * To allow for "late binding" delegate towards its this pointer for
             * instance methods, we need to check if method is static, and if not,
             * we allow for implicitly passing in the "this pointer" as its first
             * argument.
             */
            var parameters = method.GetParameters();
            var parameterTypes = parameters.Select(p => p.ParameterType).ToList();
            if (!method.IsStatic)
                parameterTypes.Insert(0, method.DeclaringType);
            var invokeMethod = typeBuilder.DefineMethod(
                "Invoke",
                MethodAttributes.HideBySig | MethodAttributes.Private,
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

        #endregion
    }
}