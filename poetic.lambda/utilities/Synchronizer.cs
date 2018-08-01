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
using System;
using System.Threading;

namespace poetic.lambda.utilities
{
    /// <summary>
    /// Allows you to encapsulate an instance of a type that needs to be shared
    /// between multiple threads, such that access to the instance is easily
    /// synchronised.
    /// </summary>
    public class Synchronizer<TImpl, TIRead, TIWrite>
        where TImpl : TIWrite, TIRead
    {
        // Our actual locker, that will synchronise access to our _shared instance.
        ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        // Our actual shared resource.
        TImpl _shared;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:threadsynchronization.Synchronizer`1"/> class.
        /// </summary>
        /// <param name="shared">Instance that needs synchronised access in multiple threads.</param>
        public Synchronizer(TImpl shared)
        {
            _shared = shared;
        }

        /// <summary>
        /// Enters a read lock giving the caller access to the shared instance in
        /// "read only" mode.
        /// </summary>
        /// <param name="functor">Functor.</param>
        public void Read(Action<TIRead> functor)
        {
            _lock.EnterReadLock();
            try {
                functor(_shared);
            } finally {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Enters a write lock giving the caller access to the shared resource
        /// in "read and write" mode.
        /// </summary>
        /// <param name="functor">Functor.</param>
        public void Write(Action<TIWrite> functor)
        {
            _lock.EnterWriteLock();
            try {
                functor(_shared);
            } finally {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Enters a write lock giving the caller access to the shared resource
        /// in "read and write" mode, for then to reassign the shared object to
        /// the value returned from the Func.
        /// </summary>
        /// <param name="functor">Functor.</param>
        public void Assign(Func<TIWrite, TImpl> functor)
        {
            _lock.EnterWriteLock();
            try {
                _shared = functor(_shared);
            } finally {
                _lock.ExitWriteLock();
            }
        }
    }

    /// <summary>
    /// Simplified syntax where you cannot modify the shared type and implement
    /// your own read and write interfaces.
    /// 
    /// Notice, when using this class you are on your own in regards to making sure
    /// you never actually modify the shared instance inside a "read only" delegate.
    /// </summary>
    public class Synchronizer<TImpl> : Synchronizer<TImpl, TImpl, TImpl>
    {
        public Synchronizer(TImpl shared)
            : base (shared)
        { }
    }
}
