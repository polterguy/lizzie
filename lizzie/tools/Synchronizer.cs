/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System;
using System.Threading;

namespace lizzie.tools
{
    /*
     * Synchronizes access to a shared resource using ReaderWriterLockerSlim,
     * in addition to some "lambda trickery". The idea is to ensure you have
     * synchronized access to some shared resource, never allowing access to it,
     * from outside of a read or write lock. Kind of like the "lock" statement does,
     * only using a ReaderWriterLockerSlim instead of a lock. This has the advantage
     * of that multiple threads might access your shared resource in read only mode,
     * while only one thread can modify it with a write operation.
     * 
     * See the February issue of MSDN Magazine in 2019 to understand the purpose
     * of this class.
     * 
     * NOTICE!
     * Class is intentionally not made public, although it's actually quite
     * useful as a library class, since I'll probably at some point create
     * a "utility NuGet library" where I wrap it at some point in the future,
     * and I don't want to create incompatible changes as I do that.
     */
    internal class Synchronizer<TImpl, TIRead, TIWrite> where TImpl : TIWrite, TIRead
    {
        ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        TImpl _shared;

        // CTOR
        public Synchronizer(TImpl shared)
        {
            _shared = shared;
        }

        /*
         * Enters a read lock on your shared resource, and evaluates your lambda,
         * ensuring only read lambdas can access the shared resource at the same
         * time.
         */
        public void Read(Action<TIRead> functor)
        {
            _lock.EnterReadLock();
            try {
                functor(_shared);
            } finally {
                _lock.ExitReadLock();
            }
        }

        /*
         * Enters a read lock on your shared resource, and evaluates your lambda,
         * returning a TResult to caller, ensuring only read lambdas can access
         * the shared resource at the same time.
         * 
         * NOTICE!
         * Method cannot be used to return the synchronized object itself,
         * but rather to return something your synchronized instance "contains",
         * to avoid allowing access to the synchronized instance outside of a
         * Read/Write lambda.
         */
        public TResult Fetch<TResult>(Func<TIRead, TResult> functor)
        {
            _lock.EnterReadLock();
            try {
                var result = functor(_shared);
                if ((object)result == (object)_shared)
                    throw new ApplicationException("You cannot use the Synchronizer.Fetch method to return your synchronized instance.");
                return result;
            } finally {
                _lock.ExitReadLock();
            }
        }

        /*
         * Enters a write lock on your shared resource, and evaluates your lambda,
         * making sure no other read or write lambdas can access your shared resource
         * at the same time.
         */
        public void Write(Action<TIWrite> functor)
        {
            _lock.EnterWriteLock();
            try {
                functor(_shared);
            } finally {
                _lock.ExitWriteLock();
            }
        }
    }

    /*
     * Simplicity wrapper/helper class,to avoid having to repeat the same type
     * argument multiple times, when you don't have read and write interfaces
     * on your type.
     */
    internal class Synchronizer<TImpl> : Synchronizer<TImpl, TImpl, TImpl>
    {
        public Synchronizer(TImpl shared)
            : base(shared)
        { }
    }
}