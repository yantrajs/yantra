# core-js
JavaScript Engine for .NET Standard

# Design

1. Esprima-dot-net parses JavaScript and generates AST.
2. CoreJS module uses generated AST to generate C# source code.
3. C# source code is then compiled to an assembly.
4. C# source code will look exactly same as JavaScript in most cases
5. Debugging in ASP.NET environment will be easy
6. User can evaluate context.current function to find current stack trace in JavaScript environment


# Design choice to generate code

## System.Linq.Expressions
1. Easy generation
2. Closures are little problem - Access closure as a field of current executing function
3. Support on Android and on iOS due to interpretation?
4. Debugging support can be added with Agile Object's Readable Expressions
5. Adding debugging support is easy compared to C# code generator as managing symbols require same efforts.
 ### Complexity
 1. Loop optimization
 2. Tail Call Optimization

https://github.com/agileobjects/ReadableExpressions



## Roslyn (Postponed to future, due to complexity in generating source code for debugging purpose)

1. Use roslyn
2. C# Parser will generate code from syntax tree directly, saving some time
3. Save C# source code
4. Problem will be of indention etc...


## TemplateQuery
1. Use simple text, easy
2. Slow as C# parser will take extra syntax generator step