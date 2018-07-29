
# poetic.threading

These are the classes and utilities that helps you with your multi threaded
programming. The idea is to create tiny and simple utility classes that simplifies
your code when dealing with anything related to threads in C#.

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
synchronised access to the instance.

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
