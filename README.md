
# Simplified Syntactic C# Sugar

You don't need music in your life, sugar in your tea, or poetry in your code.
However, all of these things slightly improve the quality of your life. Poetry
is a collection of small libraries, that somehow makes your code more poetic and
more dynamic. Poetry makes your CLR code dance.

The project's long term goals is to provide a DSL engine to create and consume
your own Domain Specific Languages, without any compilation, and/or
interpretation occurring, through a _"JIT parsing"_ model, where dynamic script
code is parsed, and translated into statically typed list of delegates, Actions
and Functions. This allows you to write stuff such as for instance.

```
event(incoming_phone as phone)
  if phone.Country equals Countries.Australia
    if localTime.Now.Time is less_than '08:00' or localTime.Now.Time is more_than '17:00'
      phone.transfer (AnsweringMachine.Sales.English)
    else
      phone.transfer (Departments.Sales.English)
```

**FYI** - The above is just an example of an imaginary DSL which allows for
routing phone calls. The idea with the above imaginary language, is that it
subscribes to incoming phones, and if the phone originates from Australia, it
transfer the phones to a specific extension, depending upon what time it is.
Since the above would be a script language, configuring rule engines and parts
of your CLR/.Net applications 100% dynamic, storing snippets of functionality
in your database, or in dynamically parsed files on disc, becomes much simpler,
and hence makes C# and/or the CLR become a _"dynamic execution platform"_,
arguably providing all the benefits from interpreted languages, without having
to throw static typing and C# out the door.

## poetic.lambda

This library allows you to create lists and dictionaries of lambda objects
(delegates), for then to later decide if you want to execute your lambda
objects sequentially, chained or in parallel.

* [Documentation for poetic.lambda](docs/poetic.lambda.md)

## Installation

Poetry is way far from stable, and currently duper duper ALPHA, ALPHA code!
_DON'T USE IT_ except for research and teaching purposes! If you still want
to check it out, clone the repository.

## License

Poetry is licensed under the terms of the MIT license. See the enclosed LICENSE
file for details.

