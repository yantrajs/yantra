using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading;

namespace YantraJS.JSClassGenerator
{

    [Generator]
    public class JSClassGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var jsClasses = context.SyntaxProvider
                .CreateSyntaxProvider(
                    SyntaxNodeExtensions.CouldBeJSClassAsync,
                    SyntaxNodeExtensions.GetJSClassTypeOrNull)
                .Where(x => x is not null)
                .Collect();

            context.RegisterSourceOutput(jsClasses, GenerateCode);
        }


        private static void GenerateCode(
            SourceProductionContext context,
            ImmutableArray<ITypeSymbol> enumerations)
        {
            if (enumerations.IsDefaultOrEmpty)
                return;

            var types = new List<(ITypeSymbol type, AttributeData attribute)>();
            ITypeSymbol names = null!;
            

            

            // first lets build all classes

            // then build names...

            foreach (var type in enumerations)
            {
                var a = type.GetAttribute();
                if (a?.AttributeClass?.Name == null)
                {
                    continue;
                }
                if (a.AttributeClass.Name.StartsWith("JSRegistration"))
                {
                    names = type;
                    continue;
                }
                types.Add((type,a));
            }

            var allNames = new List<string>();
            foreach (var name in names.GetMembers().Where(x => x.Kind == SymbolKind.Field))
            {
                allNames.Add(name.Name);
            }

            var gc = new JSGeneratorContext(types);


            foreach(var type in  gc.AssemblyTypes)
            {
                var code = "";
                code = ClassGenerator.GenerateClass(type, gc);
                var typeNamespace = type.Type.ContainingNamespace.IsGlobalNamespace
                    ? null
                        : $"{type.Type.ContainingNamespace}.";

                context.AddSource($"{typeNamespace}{type.Type.Name}.g.cs", code);
            }

            var c = RegistrationGenerator.GenerateNames(names, gc);
            var cNS = names.ContainingNamespace.IsGlobalNamespace
                ? null
                    : $"{names.ContainingNamespace}.";

            context.AddSource($"{cNS}{names.Name}.g.cs", c);
        }

    }
}
