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
using poetic.threading;

namespace poetic.threading.synchronizer.example
{
    /// <summary>
    /// Read interface implemented by our Shared class.
    /// </summary>
    interface ISharedRead
    {
        string Read();
    }

    /// <summary>
    /// Write interface implemented by our Shared class.
    /// 
    /// Notice, since a "Write" operation should have access to everything that
    /// a "Read" operation should have access to, we inherit our write interface
    /// from our read interface.
    /// </summary>
    interface ISharedWrite : ISharedRead
    {
        void Write(string value);
    }

    /// <summary>
    /// An dummy example of a class where instances of the class needs to be shared
    /// between multiple threads, and hence access to the instance needs to be
    /// synchronized.
    /// </summary>
    class Shared : ISharedWrite
    {
        string _data = "foo";

        /// <summary>
        /// Returns the state of the instance.
        /// </summary>
        /// <returns>The read.</returns>
        public string Read()
        {
            return _data;
        }

        /// <summary>
        /// Modifies the state of the instance.
        /// </summary>
        /// <param name="value">Value.</param>
        public void Write(string value)
        {
            _data += value;
        }
    }

    /// <summary>
    /// An example of how to use the Synchronizer class from poetic.threading to
    /// easily synchronize access to some shared resource across multiple threads.
    /// </summary>
    class MainClass
    {
        public static void Main(string[] args)
        {
            /*
             * Creating our Synchronizer, that ensures synchronised access to a
             * single "Shared" instance.
             * 
             * Notice, instead of keeping a reference to our "Shared" instance,
             * we completely hide our Shared instance, and encapsulate it as a
             * field, inside of our "Synchronizer" instance, which gives us a
             * guarantee of that we're never able to access our "Shared" object,
             * unless we first enter either a "Read" or a "Write" lock first,
             * providing us with a guarantee of that all access to our shared
             * resource is always synchronized.
             * 
             * Notice, if you wanted to you could create a simpler Synchronizer
             * instance by using the one which only requires one generic argument,
             * at which point both your Read and Write delegate would be given
             * the shared instance directly, at which point determining what is
             * a "Read operation" and what is a "Write operation" would have to
             * be determined by you manually. If you do, you wouldn't have to
             * implement any read interface or write interface on your shared
             * resource, at the cost of possibly executing a write operation
             * inside a Read delegate.
             */
            var synchronizer = new Synchronizer<Shared, ISharedRead, ISharedWrite>(new Shared());

            /*
             * Creating a couple of threads, all accessing the same shared instance,
             * making sure we synchronize access to our shared resource, using our
             * above Synchronizer instance.
             */
            string data1 = "";
            var thread1 = new Thread(new ThreadStart(delegate {

                /*
                 * This is where the magic of our Synchronizer instance occurs, since
                 * this will enter a "read lock" of our ReaderWriterLockSlim instance,
                 * which is a field in our Synchronizer class.
                 * 
                 * Hence, after we enter our "Read" delegate below, no "Write" invocations
                 * will be allowed to start, in any other threads, before our "Read"
                 * delegate has finished executing.
                 */
                synchronizer.Read(delegate (ISharedRead shared) {
                    data1 = shared.Read();
                });
            }));

            /*
             * Creating another thread, consuming our shared resource, but this
             * time in write mode.
             */
            var thread2 = new Thread(new ThreadStart(delegate {

                /*
                 * Entering a Write lock, such that we can modify our shared instance.
                 */
                synchronizer.Write(delegate (ISharedWrite shared) {
                    shared.Write(" bar");
                });
            }));

            /*
             * Creating another thread, consuming our shared resource, but this
             * time in write mode.
             */
            string data3 = "";
            var thread3 = new Thread(new ThreadStart(delegate {

                /*
                 * Entering a Read lock such that we can read values from our
                 * shared instance.
                 */
                synchronizer.Read(delegate (ISharedRead shared) {
                    data3 = shared.Read();
                });
            }));

            /*
             * Starting all of our threads
             */
            thread1.Start();
            thread2.Start();
            thread3.Start();

            /*
             * Waiting for all threads to finish.
             */
            thread1.Join();
            thread2.Join();
            thread3.Join();

            /*
             * Spitting out results from threads on Console.
             * 
             * Notice, this might spit out different values each time you execute
             * your program depending upon which thread is able to enter a "lock" first.
             */
            Console.WriteLine(string.Format("Thread 1 value '{0}'", data1));
            Console.WriteLine(string.Format("Thread 3 value '{0}'", data3));

            /*
             * Example of simplified syntax, that doesn't require modifying the
             * shared type.
             * 
             * Although not as safe as the above syntax, requiring 3 different
             * generic arguments, this syntax allows you to wrap instances of
             * types where you do not have the luxury of being able tomodify
             * the type of instance you are actually sharing.
             */
            var synchronizerSimple = new Synchronizer<Shared>(new Shared());

            // Somewhere inside another thread ...
            synchronizerSimple.Read(delegate (Shared shared) {

                // NOTICE!! Make sure you only invoke "Read" methods here!
                var foo = shared.Read();
            });

            // Somewhere inside another thread ...
            synchronizerSimple.Write(delegate (Shared shared) {

                // Here you can invoke both "Read" and "Write" methods on your shared instance.
                shared.Write("bar");
            });

            /*
             * Making sure we keep the console around until user types a key, such
             * that he can see the results of his execution.
             */
            Console.Read();
        }
    }
}
