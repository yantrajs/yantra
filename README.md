# core-js
JavaScript Engine for .NET Standard

# Design

1. Esprima-dot-net parses JavaScript and generates AST.
2. CoreJS module uses generated AST to generate System.Linq.Expression tree.
3. Linq Expressions can be compiled to IL and can also be saved to an assembly for caching.
4. User can evaluate context.current function to find current stack trace in JavaScript environment
5. Majority of code is written as virtual method of JSValue derived object to reduce the amount of code to be generated.
6. AOT platforms will not perform JIT Inlining, due to this, every method is implemented as simple Static method to avoid unnecessary one more method call after casting.
7. Generators are implemented as Suspended Threads, as generating complex state machine at runtime would be very time consuming and would require too much of code analysis.
8. Async/Await will be generated over generators.

# No support for primitive types
This decision was taken based on following observation
1. Every method call will require reference type in order to support JavaScript's advanced types.
2. Generating code based on primitive type will anyway create a reference as every value will be sent in as `object`.
3. So having JSDouble value holding a filed as double is of exactly same cost as using double and passing it on as object.
4. Supporting privimite type will also make code generation very complex as too much of condition checks will make further enhancement difficult.

# So how to generate faster code?
No we do not want to generate faster code, everything runs in CLR, CLR supports primitive types and you can always write high performance C# code and call it from JavaScript.

On platforms where Roslyn is available, you can easily generate code high performance code writtin in C#.

# Design choice to generate code

## System.Linq.Expressions
1. Easy generation
2. Closures are handled by Linq Compiler
3. Support on Android and on iOS with interpretation, testing pending...
4. Debugging is easy as you can read generated expressions
5. Adding debugging support is easy compared to C# code generator as managing symbols require same efforts.
 ### Complexity
 1. Tail Call Optimization

 ## No System.Text.Json

 We are not going to use System.Text.Json because it is not available on .NET Frameworks. This library is designed to run on older platforms as well.

 ## Performance Points

 1. Do not use Switch pattern matching for types, 
 https://sharplab.io/#v2:EYLgtghgzgLgpgJwDQxNMSAmIDUAfAAQCYBGAWACgCBmAAmNoGFaBvS2j29zm+gFloBZABQBKWgF4AfEJLCAdgFcANstEBubhy306BAYLkAlAPYmYtCOLYVOd2lADuASxgBjABbCrrHfc5u0HC0AIK0APokIH7+9owm8lAmynAAdADqCK5wwpGpAGYaMbGcwAhwEADWmrYlAUG0AEIRRNG1dXEJSSkZWfC5RAVF7R2l5VU1JQC+xUxdyWmZ2d6pAGLDdjMjOrz6QkTCpuaWojYlAG4QCLTAkrTCIaIQk7HxiQu9y8BDLxxbdrsDNRDmYLFYzq95j0lv0IGsNpwtltKLwIMBYAgIG4LAwjhYbDs6GiMViLM55BZVqxaABzOAwdS0JGUFFEUIgWh43ztXjkiz5O4kGqE2gmc6ILKYYJ82hU6S0fI1ZFUNmNDlciG6WgygUSWhEX5cEa8MUS5xS7UU2WSGSKyhTIA==
 Based on above example, it seems every `is` operator is expensive as it makes first call to check if it is assignable from the given type. 
 It is not a simple call, it is series of calls to check if current type is same or current type is derived from assignable type.

 2. Virtual properies are useful. As casting also requires a `call` instruction.

https://github.com/agileobjects/ReadableExpressions

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