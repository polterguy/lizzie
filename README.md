
# Lizze a dynamic scripting language for.Net

Lizzie is a dynamic scripting language for .Net based upon the design pattern
called _"Symbolic Delegates"_. This allows you to execute dynamically created
scripts, that does neither compile nor are interpreted, but instead translates
directly down to delegates. Below is some example code.

```javascript
var(@foo, 57)
set(@foo, add(@foo, 10))
foo
```

The above code will return 67 when evaluated.
