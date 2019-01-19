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
     * in addition to some "lambda trickery".
     * See the February issue of MSDN Magazine tounderstand class'purpose.
     * 
     * NOTICE!
     * Class is intentionally not made public, although it's actually quite
     * useful as a library tool class, since I'll probably at some point create
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
         * Enters a read lock on your shared resource, and evaluates your lambda.
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
         * Enters a read lock on your shared resource, and evaluates your lambda.
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