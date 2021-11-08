# YantraJS
Yantra (Machine in Sanskrit) is a Managed JavaScript Engine for .NET Standard written completely in C#.

# NuGet
| Name                                               | Package                                                                                                                                                        |
|----------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| YantraJS (With CSX Module Support)                              | [![NuGet](https://img.shields.io/nuget/v/YantraJS.svg?label=NuGet)](https://www.nuget.org/packages/YantraJS)                           |
| YantraJS.Core (Compiler)| [![NuGet](https://img.shields.io/nuget/v/YantraJS.Core.svg?label=NuGet)](https://www.nuget.org/packages/YantraJS.Core) |
| YantraJS.ExpressionCompiler (IL Compiler)           | [![NuGet](https://img.shields.io/nuget/v/YantraJS.ExpressionCompiler.svg?label=NuGet)](https://www.nuget.org/packages/YantraJS.ExpressionCompiler) |
| WebAtoms.YantraJS                 | [![NuGet](https://img.shields.io/nuget/v/WebAtoms.YantraJS.svg?label=NuGet)](https://www.nuget.org/packages/WebAtoms.YantraJS) |

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

