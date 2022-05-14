# YantraJS
Yantra (Machine in Sanskrit) is a Managed JavaScript Engine for .NET Standard written completely in C#.

# NuGet
| Name                                               | Package                                                                                                                                                        |
|----------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| YantraJS (With CSX Module Support)                              | [![NuGet](https://img.shields.io/nuget/v/YantraJS.svg?label=NuGet)](https://www.nuget.org/packages/YantraJS)                           |
| YantraJS.Core (Compiler)| [![NuGet](https://img.shields.io/nuget/v/YantraJS.Core.svg?label=NuGet)](https://www.nuget.org/packages/YantraJS.Core) |
| YantraJS.ExpressionCompiler (IL Compiler)           | [![NuGet](https://img.shields.io/nuget/v/YantraJS.ExpressionCompiler.svg?label=NuGet)](https://www.nuget.org/packages/YantraJS.ExpressionCompiler) |

# Features
1. Compiles JavaScript to .Net Assembly 
2. Strict Mode Only JavaScript*
3. Arrow functions
4. Classes
5. Enhanced object literals
6. Template strings and tagged templates
7. Destructuring
8. `let` `const`
9. Map, Set, WeakMap, WeakSet
10. Symbols
11. Subclassable built-ins
12. Binary and Octal literals
13. Module support
14. Null coalesce
15. Optional property chain `identifier?.[]`, `identifier?.(`, `identifier?.identifier`
16. Rest, Default and Spread Parameters
17. Generators, iterators, for..of
18. Async/Await
19. Optional parameters
20. Tail call optimization
21. Many ES5 + ES6 features
22. CommonJS Module Support
23. Easily marshal CLR Object to JavaScript and other way around
24. CSX Module support
25. Mixed module system, YantraJS supports `require` and `import`.

`*` Most JavaScript today is available in strict mode, we do not feel any need to support non strict mode as modules are strict by default.

# Mixed modules
Currently YantraJS supports Both CommonJS and ES modules without any extra work, with little trick, module resolution is `node like`, it does not take `.js` extension into account. We are trying to make a workaround and we will update the product soon. Module loader loads module asynchronously, so `import` will work without any extra effort. However, `require` will run `AsyncPump` to wait till the module is loaded correctly, this may lead to some deadlocks.

## Mixed Modules Roadmap
1. Detect the loading order, first check if the same named file exists, if yes, load it, allow auto appending extension in the configuration, so module loader can load corresponding files accordingly.
2. Create correct algorithm to resolve module name to support loading of CSX module in the mix. YantraJS supports loading module independent of the implementation. We can support other languages like python, php in the YantraJS. This is only possible to load modules without extension.

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

