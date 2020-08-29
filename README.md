# core-js
JavaScript Engine for .NET Standard

# Design

1. Esprima-dot-net parses JavaScript and generates AST.
2. CoreJS module uses generated AST to generate System.Linq.Expression tree.
3. Linq Expressions can be compiled to IL and can also be saved to an assembly for caching.
4. User can evaluate context.current function to find current stack trace in JavaScript environment
5. Majority of code is written as virtual method of JSValue derived object to reduce the amount of code to be generated.

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

https://github.com/agileobjects/ReadableExpressions

## Comparison with JInt and Jurassic
|-------------------------------|---------------------|-----------------|--------------------|
|Feature                        | Yantra              | JInt            | Jurassic           |
|-------------------------------|---------------------|-----------------|--------------------|
|JIT                            | Yes if Available    | No              | Yes                |
|-------------------------------|---------------------|-----------------|--------------------|
|AOT                            | Yes as Interpreted  | Yes             | No                 |
|-------------------------------|---------------------|-----------------|--------------------|
|Platforms                      | All                 | All             | No iOS             |
|-------------------------------|---------------------|-----------------|--------------------|
|Code Generation Type           | Linq Expressions    | Custom          | IL                 |
|-------------------------------|---------------------|-----------------|--------------------|
|Readable Generated Code        | Yes                 | No              | Difficult to Read  |
|-------------------------------|---------------------|-----------------|--------------------|
|Generated Code Context Free    | Yes                 | No              | No                 |
|-------------------------------|---------------------|-----------------|--------------------|
|Context Isolation              | Yes                 | No              | No                 |
|-------------------------------|---------------------|-----------------|--------------------|


