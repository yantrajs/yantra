# core-js
JavaScript Engine for .NET Standard

# Design

1. Scanner and Parser are written in C# from scratch.
2. YantraJS.Core module uses generated AST to generate ILExpression tree. ILExpression tree is easy to visualize and one node represents one type of generated IL code.
3. ILExpressions can be compiled to IL and can also be saved to an assembly for caching.
4. User can evaluate context.current function to find current stack trace in JavaScript environment
5. Majority of code is written as virtual method of JSValue derived object to reduce the amount of code to be generated.
6. AOT platforms will not perform JIT Inlining, due to this, every method is implemented as simple Static method to avoid unnecessary one more method call after casting.
7. Generators are implemented as simple jumps, they run in the same thread.
8. Async/Await will be generated over generators.

# No support for primitive types
This decision was taken based on following observation
1. Every method call will require reference type in order to support JavaScript's advanced types.
2. Generating code based on primitive type will anyway create a reference as every value will be sent in as `object`.
3. So having JSDouble value holding a value as double is of exactly same cost as using double and passing it on as object.
4. Supporting privimite type will also make code generation very complex as too much of condition checks will make further enhancement difficult.

# Roadmap
1. Add V8 Debugging protocol
2. Save IL and reload IL instead of recompiling everytime. Blocker: CLR Assembly has limit on number of strings, many large javascript files easily overcome this limit. And an entire assembly needs to be loaded only to execute one function. We are investigating a new way to save generated IL code in many files so only function needed to run code will be loaded.
3. Async Generator Support
4. Null coalesce operators
5. Task support in JavaScript
6. Few node in-built modules, fs, process, net etc...
7. Align ES conformance, many of the methods are not 100% compatible with ES6 standard.

# Low Priority Roadmap
1. Support Date for larger years than -9999 to 9999 years. This will require rewriting JSDate with some date type other than DateTime that supports larger range.

# So how to generate faster code?
No we do not want to generate faster code, everything runs in CLR, CLR supports primitive types and you can always write high performance C# code and call it from JavaScript.

We can import CSX modules, which can be compiled natively to give higher performance. CSX Modules have higher priority, so if you are creating a package that can run on node as well as yantra, you can ship CSX to give higher performance over same javascript on node.

# Design choice to generate code

1. AST is transformed into ILExpression tree, which is simple form of IL to be generated.

 ### Complexity
 1. Tail Call Optimization is done by JIT so we do not do any optimization.

 ## No System.Text.Json

 We are not going to use System.Text.Json because it is not available on .NET Frameworks. This library is designed to run on older platforms as well.

 ## Performance Points

 1. Do not use Switch pattern matching for types, 
 https://sharplab.io/#v2:EYLgtghgzgLgpgJwDQxNMSAmIDUAfAAQCYBGAWACgCBmAAmNoGFaBvS2j29zm+gFloBZABQBKWgF4AfEJLCAdgFcANstEBubhy306BAYLkAlAPYmYtCOLYVOd2lADuASxgBjABbCrrHfc5u0HC0AIK0APokIH7+9owm8lAmynAAdADqCK5wwpGpAGYaMbGcwAhwEADWmrYlAUG0AEIRRNG1dXEJSSkZWfC5RAVF7R2l5VU1JQC+xUxdyWmZ2d6pAGLDdjMjOrz6QkTCpuaWojYlAG4QCLTAkrTCIaIQk7HxiQu9y8BDLxxbdrsDNRDmYLFYzq95j0lv0IGsNpwtltKLwIMBYAgIG4LAwjhYbDs6GiMViLM55BZVqxaABzOAwdS0JGUFFEUIgWh43ztXjkiz5O4kGqE2gmc6ILKYYJ82hU6S0fI1ZFUNmNDlciG6WgygUSWhEX5cEa8MUS5xS7UU2WSGSKyhTIA==
 Based on above example, it seems every `is` operator is expensive as it makes first call to check if it is assignable from the given type. 
 It is not a simple call, it is series of calls to check if current type is same or current type is derived from assignable type.

 2. Virtual properies are useful. As casting also requires a `call` instruction.
 3. Struct initializations can be improved by using `ref Put()` instead of Assignment or Set with `in` operator
 https://sharplab.io/#v2:C4LghgzgtgPgAgJgIwFgBQcDMACR2DC2A3utmbjnACzYCyAFAJTGnlsBuYATtmNgLzYAdgFMA7tgCCTANys25MADoAyiOD1REgFIqAClwD2ABxFdgAT3pIANAkaM5aBeXnYAvm7dZcNWgiYWZxdsTh4+QS0pWTc2ZQBxdUDI8WxdAxMzS3oqGwBWRzdPYLJvSgQpIJCfSRUAHnSjU3MLAD5eJxDsWPIAbTgkJQAlAFchYABLKBElfEMoYwmAGzM1LnYJgGMRCCVadQALQwATAEkFpfoB4bHJ6dn5xZWuNY3t3f3gI7OLgHljSaGIS7SQAc1BXB2EAm7BEpyESwmQiRoMYAF0ehRsJCAGZpfRNLIWbCJDTMEglLpwADs2JEeOUtE6LmKXX6g1G4ymMzmC2WqzMbx2e0OJ3Oxku105dx5j35L0FW2Fn2+4qW/0BwKUYIhUJhcIRSJR6MxWOo2DUGiR+IyzUsoTASxGInJpsUewEDqdImZLKKXkpPggwC4I02wCk9QAKu0KVScFG6L6PAG2D5sMHQ+GbYSWlUXD4kRGAGqO53JtM4IsW4BgYAjCAVvpS27ch5856vJUfUU/CVXDmt+68p4C9bdkVfMV/AETIEg8GQiDQ2HwxHIoSojGUys5zItejV9g2bDViCuncuUvez3sJsuFS1+sQT2N02s8jFdxAA==


## Comparison with JInt and Jurassic
|Feature                        | Yantra              | JInt            | Jurassic           |
|-------------------------------|---------------------|-----------------|--------------------|
|JIT                            | Yes if Available    | No              | Yes                |
|AOT                            | Yes as Interpreted  | Yes             | No                 |
|Platforms                      | All                 | All             | No iOS             |
|Code Generation Type           | Linq Expressions    | Custom          | IL                 |
|Readable Generated Code        | Yes                 | No              | Difficult to Read  |
|Generated Code Context Free    | Yes                 | No              | No                 |
|Context Isolation              | Yes                 | No              | No                 |


## Unit Testing

1. `assert()`, This function will test if given value is true (that is not empty, not zero, not null and not undefined).
2. `assert.strictEqual(left, right [,error])` checks if left is strictly equal to right, displays error if it does not match.
3. `assert.notStrictEqual(left, right [,error])` checks if left is not strictly equal to right, displays error if it matches.
4. `assert.throws(fn [, error] [, message])` executes given function and expects an error.
5. `assert.matches(left, regex, [, error])` checks if left matches right regex, displays error if it does not match.
6. `assert.doesNotMatch(left, regex, [, error])` checks if left matches right regex, displays error if it does not match.


# Pending

|   |CREATE|READ|UPDATE|DELETE|
|---|:---:|:---:|:---:|:---:|
|`Object.freeze`| ✗ | ✓ | ✗ | ✗ |
|`Object.seal`| ✗ | ✓ | ✓ | ✗ |
|`Object.preventExtensions`| ✗ | ✓ | ✓ | ✓ |


# Package Installer

https://gist.github.com/alistairjevans/4de1dccfb7288e0460b7b04f9a700a04

https://gist.github.com/mholo65/ad5776c36559410f45d5dcd0181a5c64
