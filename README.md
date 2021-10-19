# YantraJS
Yantra (Machine in Sanskrit) is a Managed JavaScript Engine for .NET Standard written completely in C#.

# NuGet
| Name                                               | Package                                                                                                                                                        |
|----------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| YantraJS (With CSX Module Support)                              | [![NuGet](https://img.shields.io/nuget/v/YantraJS.svg?label=NuGet)](https://www.nuget.org/packages/YantraJS)                           |
| YantraJS.Core (Compiler)| [![NuGet](https://img.shields.io/nuget/v/YantraJS.Core.svg?label=NuGet)](https://www.nuget.org/packages/YantraJS.Core) |
| YantraJS.ExpressionCompiler (IL Compiler)           | [![NuGet](https://img.shields.io/nuget/v/YantraJS.ExpressionCompiler.svg?label=NuGet)](https://www.nuget.org/packages/YantraJS.ExpressionCompiler) |
| WebAtoms.YantraJS                 | [![NuGet](https://img.shields.io/nuget/v/WebAtoms.YantraJS.svg?label=NuGet)](https://www.nuget.org/packages/WebAtoms.YantraJS) |


# Multi License
1. Source code under this repository (except the git sub module for test262) is released under GNU LGPLv3 license. In order to build/distribute anything from this source code test262 should be omitted unless you obtain and comply license granted by owners of test262 library.
2. Binaries on NuGet are released under same license.
3. For different license, please contact us at support at neurospeech dot com.
4. We will change license as this project grows and there is enough contribution from community.
5. We are looking for sponsors to make this project available under MIT.

# Licenses

There are three kind of licenses available for `YantraJS.Core`, `YantraJS` and `YantraJS.ExpressionCompiler`. These licenses are available `per Application`.

1. [LGPL License](https://github.com/yantrajs/yantra/wiki/License#lgpl-license)
2. [YantraJS Standard License](https://github.com/yantrajs/yantra/wiki/License#yantrajs-standard-license)
3. [YantraJS Enteprise License](https://github.com/yantrajs/yantra/wiki/License#yantrajs-enterprise-license)

To purchase license, please visit [YantraJS Web site](https://yantrajs.com)

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

