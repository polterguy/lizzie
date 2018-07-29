
# Poetic code you don't need

This is one of those things that doesn't solve anything you can't easily solve yourself.
However, just like sugar in your tea takes away the bitterness of your tea,
this project contains several helper classes that takes away some of the edge from
your C# code, and improves your life as a developer. It is a simple collection
of tools which are the first thing you would normally implement every time you
start a new job. Tiny helper utilities and classes, to simplify your job as a
developer. Poetic is created as a .Net Standard 2.0 project, and should
hence be _"as portable as it gets!"_

The project is not mature by any means of measurement, and should be considered
_super duper ALPHA technology_ at the time of this writing. However, I will fill
in with all those things that are allowing me to more easily do my job, one class
at the time, and consume it myself in my own project - Such that over time it'll
hopefully contain many tiny building blocks, which I believe every software
development department should have access to, without having to implement their
own versions of these tools.

Its idea is to facilitate for creating more _"poetic C# code"_, by giving you
access to classes and utilities, that somehow simplifies the syntax of your
C# code.

## poetic.threading

This library contains the things I normally implement that somehow helps me
doing multi threaded programming. It helps with spawning new threads, synchronising
access to shared resources, etc.

* [Documentation for poetic.threading](docs/poetic.threading.md)

