# YantraJS
Yantra (Machine in Sanskrit) is a Managed JavaScript Engine for .NET (Core and Standard 2) written completely in C#.

Node and V8's tight C++ intgration makes it difficult to write plugins, resulting in unresolved bugs and very complicated source code structure.

C# offers platform independent near native performance, so YantraJS is designed to replace parts of Node that requires high performance.

Multi threaded shared object cache are difficult to achieve in Node, but YantraJS offers smooth connectivity between C# and JavaScript as both objects live under same runtime.

# NuGet
| Name                                               | Package                                                                                                                                                        |
|----------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| YantraJS (With CSX Module Support)                              | [![NuGet](https://img.shields.io/nuget/v/YantraJS.svg?label=NuGet)](https://www.nuget.org/packages/YantraJS)                           |
| YantraJS.Core (Compiler)| [![NuGet](https://img.shields.io/nuget/v/YantraJS.Core.svg?label=NuGet)](https://www.nuget.org/packages/YantraJS.Core) |
| YantraJS.ExpressionCompiler (IL Compiler)           | [![NuGet](https://img.shields.io/nuget/v/YantraJS.ExpressionCompiler.svg?label=NuGet)](https://www.nuget.org/packages/YantraJS.ExpressionCompiler) |
| Yantra JS.ModuleExtensions (Fluent interface for module registration) | [![Nuget](https://img.shields.io/nuget/v/YantraJS.ModuleExtensions?label=NuGet&style=flat-square)](https://www.nuget.org/packages/YantraJS.ModuleExtensions) |

# Features
1. Compiles JavaScript to .Net Assembly 
2. Strict Mode Only JavaScript*
3. Arrow functions
4. Classes
5. Class members
6. Enhanced object literals
7. Template strings and tagged templates
8. Destructuring
9. `let` `const`
10. Map, Set, WeakMap, WeakSet
11. Symbols
12. Subclassable built-ins
13. Binary and Octal literals
14. Module support
15. Null coalesce
16. Optional property chain `identifier?.[]`, `identifier?.(`, `identifier?.identifier`
17. Rest, Default and Spread Parameters
18. Generators, iterators, for..of
19. Async/Await
20. Optional parameters
21. Tail call optimization
22. Many ES5 + ES6 features
23. CommonJS & ES6 Module Support
24. Decimal support, number with `0.2m` prefix has a new literal type called decimal, `typeof 0.2m` is `'decimal'`. Behaviour of decimal is similar to BigInt, you can only use mathematical operators between decimal only. To convert number to decimal you can use `new Decimal(number)`. Math library operates on decimal if input is decimal for methods wherever dotnet supports direct decimal operations.
25. Easily marshal CLR Object to JavaScript and other way around
26. CSX Module support
27. Mixed module system, YantraJS supports `require` and `import`.
28. Explicit resource management with `using` and `await using` keywords.
29. AOT ready (Source Generators have been used for loading Runtime Objects).

`*` Most JavaScript today is available in strict mode, we do not feel any need to support non strict mode as modules are strict by default.

# Mixed modules
Currently YantraJS supports Both CommonJS and ES modules without any extra work, with little trick, module resolution is `node like`. Module loader loads module asynchronously, so `import` will work without any extra effort. However, `require` will run `AsyncPump` to wait till the module is loaded correctly. Unless you do some multithreading, mixed modules will not lead to any deadlocks.

## Module loading order,

* Module names may or may not have extensions.
* File will be loaded if same name exists on disk.
* Engine will try to load `${name}.csx` file, if it exists it will load the module.
* Engine will then try to load `${name}.js` file, if it exits it will load the module.
* Otherwise it will travel to parent directory and search modules in `node_modules` folder in exact same order defined earlier.

# Roadmap
As we often have difficulty in writing cross environment scripts (browser and process such as node), we want to first implement basic common features on both.
1. Next in plan is Network API, Simple `fetch` is available in `YantraContext`, we will focus on adding Stream API.
2. Unified API to access system resources through JavaScript modules that dynamically utilize .NET objects.
3. Add forward compatibility, we will write native modules in such a way that they can execute correctly on node, so new projects written for YantraJS can run correctly on node, but node's existing code will not work on YantraJS. 
4. V8 Protocol Implementation is in progress.


# Documentation

1. [Introduction](https://github.com/yantrajs/yantra/wiki)
   - [Features](https://github.com/yantrajs/yantra/wiki#features)
2. [Expression Compiler](https://github.com/yantrajs/yantra/wiki/Expression-Compiler)
3. [JavaScript Engine](https://github.com/yantrajs/yantra/wiki/JavaScript-Engine-Example)
   - [CommonJS Module Support](https://github.com/yantrajs/yantra/wiki/JavaScript-Engine-Example#jsmodulecontext)
   - [CSX Module Support](https://github.com/yantrajs/yantra/wiki/JavaScript-Engine-Example#yantrajscontext)

# Discussions
We recommend using [Github Discussion](https://github.com/yantrajs/yantra/discussions) on this repository for any question regarding this product.

# Special Thanks
1. We are thankful to authors of Jurassic (we have incorporated number parser, promise and some unit tests from Jurassic.) https://github.com/paulbartrum/jurassic
2. We are thankful to authors of EsprimaDotNet, we initially built prototype over EsprimaDotNet, but we chose to build our parser/scanner from scratch to support token spans. https://github.com/sebastienros/esprima-dotnet
3. We are thankful to author of ILPack (we have incorporated saving IL to Assembly from this library.) https://github.com/Lokad/ILPack
