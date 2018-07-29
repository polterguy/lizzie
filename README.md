
# You don't need this!

This is one of those things that doesn't solve anything you can't easily solve yourself.
However, just like sugar in your tea takes away the bitterness of your tea,
this project contains several helper classes that takes some of the edge out of
your C# code, and improves your life as a developer. It is a simple collection
of tools which are the first thing you would normally implement every time you
start a new job. Tiny helper utilities and classes, to simplify your job as a
developer. This project is created as a .Net Standard 2.0 project, and should
hence be _"as portable as it gets!"_

The project is not mature by any means of measurement, and should be considered
_super duper ALPHA technology_ at the time of this writing. However, I will fill
it with all those things that are allowing me to more easily do my job, one class
at the time, and consume it myself - Such that over time it'll hopefully contain
tiny building blocks, which I believe every software development department should
have access to, without having to implement their own versions of these tools.

Its idea is to facilitate for creating more _"poetic C# code"_, by giving you
access to classes and utilities, that somehow simplifies the syntax of your
C# code.

## poetic.threading

This library contains the things I normally implement that somehow helps me
doing multi threaded programming. It helps with spawning new threads, synchronising
access to shared resources, etc. Below are the classes implemented in the library.

### poetic.threading.Synchronizer

This class encapsulates a shared instance of a type such that gaining access to
the shared instance becomes impossible without synchronising access to the shared
resource. The idea is to create your shared resource and then pass it into an
instance of the Synchronizer, at which point accessing the shared resource
becomes impossible, without first entering either an `EnterReadLock` or an
`EnterWriteLock` on a `ReaderWriterLockSlim` which is a field in the `Synchronizer`
class. Then instead of passing around your actual shared resource, you pass around
your `Synchronizer` instance to your threads. Below is an example of a type that
might need synchronised access.

```csharp
/*
 * Read from shared instance interface. Implement this interface on your shared type.
 */
interface ISharedRead
{
    string Read();
}

/*
 * Write to shared instance interface. Implement this interface on your shared type.
 */
interface ISharedWrite : ISharedRead
{
    void Write(string value);
}

/*
 * The actual implementation of that type you need synchronised access to.
 */
class Shared : ISharedWrite
{
    string _data = "foo";

    /*
     * A read operation example. Implements ISharedRead from above.
     */
    public string Read()
    {
        return _data;
    }

    /*
     * A read operation example. Implements ISharedWrite from above.
     */
    public void Write(string value)
    {
        _data += value;
    }
}
```

Below is an example of consuming the above type.

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
```


#### Simplified syntax

Sometimes you cannot modify the type you need to share between multiple threads,
and hence you don't have the luxury of creating a _"read interface"_ and a
_"write interface"_ on your type. For such scenarios, you have a short hand version
of the `Synchronizer` class, that simply gives you access to the type inside
of your Read and Write delegates, as the implementing type. Notice, using the
`Synchroniser` this way, for obvious reasons are not as _"safe"_ as implementing
your own interfaces on your own types, since among other things, there is nothing
preventing you or anybody else from actually invoking _"write methods"_ on your
shared instance - However, for those cases where you need this, you can use the
simplified syntax, illustrated below. First some example type, without any
read or write interfaces you can hook onto.

```csharp
class SimpleShared
{
    string _data = "foo";

    /*
     * A read operation example.
     */
    public string Read()
    {
        return _data;
    }

    /*
     * A read operation example.
     */
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
});

// Somewhere inside another thread ...
synchronizerSimple.Write(delegate (SimpleShared shared) {

    // Here you can invoke both "Read" and "Write" methods on your shared instance.
    shared.Write("bar");
});
```
