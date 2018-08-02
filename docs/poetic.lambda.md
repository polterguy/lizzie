
# poetic.lambda

This project allows you to declare lists of Action and Func delegates, that you
can either execute sequentially, in parallel (threads), or chained (output from
one is given as input to the next). The idea is that these lists of functions
and delegates provides the foundation for statically types lists of delegates,
which are dynamically created from your own DSL. This is what creates the
_"JIT parsing model"_, which literally translates directly into statically typed
CLR code, allowing you to dynamically _"declare"_ CLR code lists, almost in a
_"LISP"_ type of way, without the weird syntax though - Such that these
delegate lists can be cached, and later retrieved and executed, parametrised
according to some event, and/or method invocation.

This creates a perfect foundation for rule based business logic engines, that
does whatever you want them to do, with whatever syntax you want them to do
it within.

