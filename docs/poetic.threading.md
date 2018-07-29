
# poetic.threading

These are the classes and utilities that helps you with your multi threaded
programming. The idea is to create tiny and simple utility classes that simplifies
your code when dealing with anything related to threads in C#.

## poetic.threading.Threads

This class encapsulates the process of creating and starting multiple threads in
C#, and provides a significantly simplified syntax for doing things such as `Join`
invocations, etc. It allows you to encapsulate a bunch of delegates, and start
all of your threads together, optionally waiting for each thread to finish its
execution. Below is some sample code illustrating usage.

```csharp
/*
 * Creating our Threads instance, and adding two delegates to it.
 *
 * Each of these delegates will be executed on a separate thread.
 */
var threads = new Threads();

// Notice, the Threads class is IMMUTABLE!
threads = threads.Add(delegate {
    // Do stuff in different thread here ...
});
threads = threads.Add(delegate {
    // Do stuff in different thread here ...
});

/*
 * Fire and forget execution of all of the above threads.
 */
threads.Start();
```

Notice, since the actual threads are not created before you invoke `Start` or
`Join`, you can reuse the same instance of your `Threads` class multiple times.
Which makes it easy to encapsulate a bunch of jobs you need to do periodically
for some reasons, by storing an instance to your `Threads` class somewhere
in your own code, and simply invoke `Start` or `Join` every time you need to
execute this job for some reasons.

Also realise that this class is immutable, so all operations that somehow modified
its state some way, will not modify its state, but rather instantiate a new instance,
with the modified state, and returning this new instance to caller. This can
be seen in the above code by how we need to assign the return value from `Add`
to the instance as we add new delegates. This is a conscious design choice, which
we have chosen, in order to first make the class itself thread safe, such that
multiple threads can share the same instance. All immutable types are thread safe
by definition. In addition this also makes sure the library is _"F# friendly"_.

### Joining multiple threads

Sometimes you have a bunch of threads doing something, and you want to wait for
all of your threads to finish execution, before proceeding with your main thread.
An example can be illustrated by imagining downloading a bunch of different documents
from the network, where you would want to for efficiency reasons create your network
retrieval logic on a separate thread for each network operation - Yet still not
allow the main thread to continue its work, before all documents have been retrieved.
For these cases you can use the `Join` method, instead of the `Start` method on
your `Threads` instance. Below is an example.

```csharp
/*
 * Creating our Threads instance, and adding two delegates to it.
 */
var threads = new Threads();

// Yet again! IMMUTABLE class!
threads = threads.Add(delegate {
    // Do stuff in different thread here ...
});
threads = threads.Add(delegate {
    // Do stuff in different thread here ...
});

/*
 * Notice! Join will wait for all threads to finish before it gives back control
 * to the main thread!
 */
threads.Join();

// At this point, all above threads are finished doing their stuff!
```

The only difference between the above `Join` example and the first `Start` example,
is that the latter is using `Join` instead of `Start`. At which point execution
will not pass your `Join` invocation, before all threads have finished executing.

Join can optionally be given a milliseconds argument, which becomes the total
amount of time `Join` will spend, before returning control back to the caller.
You can also supply a `TimeSpan` argument as an override, instead of an integer
value of milliseconds. Contrary to the way `Join` works on a single thread, the
milliseconds/time argument passed into `Threads.Join` will be calculated as a
total amount of time, which means you don't need to calculate these numbers yourself,
for each thread's Join invocation.

### Executing delegates on calling thread

If you for some reasons don't want to create a new thread for each of your delegates,
you can choose to sequentially execute each delegate by invoking `Execute` instead
of the above `Start` and `Join` methods. This might be beneficial sometimes during
debugging, in addition to allowing you to create a bunch of execution objects,
for later to decide whether or not you want to execute them on a different thread
or not.

### Sharing data between multiple threads

There is an implementation of the `Threads` class that require you to create it
as a generic instance, passing in an instance of your type, to both `Start`, `Join`
and `Execute`. If you use this type, the instance you execute your delegates with
will all have your shared instance  available to them in your own delegates.
Below is an example.

```csharp
/*
 * In this example we are sharing a string between two threads.
 *
 * NOTICE!
 * There is no synchronisation being done to avoid race conditions while 
 * accessing our shared instance in this example. To synchronise access, you
 * can encapsulate your shared instance inside of a Synchronizer instance.
 * However, since the string type is immutable, this is not necessary for
 * a simple string object shared among multiple threads.
 */
var threads3 = new Threads<string>(
    delegate (string shared) {
        // Do stuff with shared!
    },
    delegate (string shared) {
        // Do stuff with shared!
    });
threads3.Join("Howdy");
```

In the above example, all your thread delegates will be given the value of
_"Howdy"_.


## poetic.threading.Synchronizer

This class encapsulates a shared instance of a type such that gaining access to
the shared instance becomes impossible without synchronising access to the shared
resource. The idea is to create your shared resource and then pass it into an
instance of the Synchronizer, at which point accessing the shared resource
becomes impossible, without first entering either a _"ReadLock"_ or a
_"WriteLock"_ on a `ReaderWriterLockSlim`, which is a field in the `Synchronizer`
class. Then instead of passing around your actual shared resource, you pass around
your `Synchronizer` instance to your threads.

Since the `Synchronizer` expects the type to implement some sort of _"read only"_
interface, in addition to a _"write and read"_ interface, this gives you a guarantee
of that you can never invoke a _"write method"_ on your shared resource, inside
a _"read only"_ delegate. Below is an example of usage. First some type that needs
synchronised access.

```csharp
// Read from shared instance interface.
interface ISharedRead
{
    string Read();
}

// Write to shared instance interface.
interface ISharedWrite : ISharedRead
{
    void Write(string value);
}

// The actual implementation of that type you need synchronised access to.
class Shared : ISharedWrite
{
    string _data = "foo";

    // A read operation example. Implements ISharedRead from above.
    public string Read()
    {
        return _data;
    }

    // A read operation example. Implements ISharedWrite from above.
    public void Write(string value)
    {
        _data += value;
    }
}
```

Below is an example of consuming the above type, making sure we always have
synchronised access to the shared instance.

```csharp
/*
 * Creating our Synchronizer, that ensures synchronised access to a
 * single "Shared" instance.
 */
var synchronizer = new Synchronizer<Shared, ISharedRead, ISharedWrite>(new Shared());

/*
 * Creating a couple of threads, all accessing the same shared instance,
 * making sure we synchronize access to our shared resource, using our
 * above Synchronizer instance.
 */
var thread1 = new Thread(new ThreadStart(delegate {

    /*
     * This is where the magic of our Synchronizer instance occurs, since
     * this will enter a "read lock" on our ReaderWriterLockSlim instance,
     * which is a field in our Synchronizer class.
     * 
     * Hence, after we enter our "Read" delegate below, no "Write" invocations
     * will be allowed to start, in any other threads, before our "Read"
     * delegate has finished executing.
     */
    synchronizer.Read(delegate (ISharedRead shared) {
        foo_from_thread_1 = shared.Read();
    });
}));

/*
 * Creating another thread, consuming our shared resource, but this
 * time in write mode.
 */
var thread2 = new Thread(new ThreadStart(delegate {

    /*
     * Entering a Write lock, such that we can modify our shared instance.
     * This delegate will never be executed if there are other delegates,
     * in other threads, either trying to read from the shared resource,
     * or write to it.
     *
     * Hence, we have "synchronised access" to our above Shared instance.
     */
    synchronizer.Write(delegate (ISharedWrite shared) {
        shared.Write(" bar");
    });
}));

/*
 * Starting both of our threads.
 */
thread1.Start();
thread2.Start();
```


### poetic.threading.Synchronizer - Simplified syntax

Sometimes you cannot modify the type you need to share between multiple threads,
and hence you don't have the luxury of creating a _"read interface"_ and a
_"write interface"_ on your type. For such scenarios, you have a short hand version
of the `Synchronizer` class, that simply gives you access to the type inside
of your Read and Write delegates, as the implementing type. Notice, using the
`Synchroniser` this way, for obvious reasons are not as _"safe"_ as implementing
your own interfaces on your own types, since among other things, there is nothing
preventing you or anybody else from actually invoking _"write methods"_ on your
shared instance, inside a `Read` delegate - However, for those cases where you
cannot modify the shared type, you can use the simplified syntax, illustrated
below. First an example type, without any read or write interfaces you can hook onto.

```csharp
class SimpleShared
{
    string _data = "foo";

    // A read operation example.
    public string Read()
    {
        return _data;
    }

    // A write operation example.
    public void Write(string value)
    {
        _data += value;
    }
}
```

Then an example of synchronising access to the above type.

```csharp
var synchronizerSimple = new Synchronizer<Shared>(new SimpleShared());

// Somewhere inside another thread ...
synchronizerSimple.Read(delegate (SimpleShared shared) {

    // NOTICE!! Make sure you only invoke "Read" methods here!
    var foo = shared.Read();

    // The following code would be a severe logical error in your application,
    // waiting to explode in your face!
    // shared.Write("bar");
});

// Somewhere inside another thread ...
synchronizerSimple.Write(delegate (SimpleShared shared) {

    // Here you can invoke both "Read" and "Write" methods on your shared instance.
    shared.Write("bar");
});
```
