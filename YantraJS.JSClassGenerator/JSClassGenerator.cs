using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
                .CreateSyntaxProvider(CouldBeJSClassAsync, GetJSClassTypeOrNull)
                .Where(x => x is not null)
                .Collect();

            context.RegisterSourceOutput(jsClasses, GenerateCode);
        }

        private static bool CouldBeJSClassAsync(
            SyntaxNode syntaxNode,
            CancellationToken cancellationToken)
        {
            if (syntaxNode is not AttributeSyntax attribute)
                return false;

            var name = ExtractName(attribute.Name);
            if (name == null)
            {
                return false;
            }
            return name.StartsWith("JSNameG") || name.StartsWith("JSClassG");
        }

        private static string? ExtractName(NameSyntax? name)
        {
            return name switch
            {
                SimpleNameSyntax ins => ins.Identifier.Text,
                QualifiedNameSyntax qns => qns.Right.Identifier.Text,
                _ => null
            };
        }

        private static ITypeSymbol? GetJSClassTypeOrNull(
            GeneratorSyntaxContext context,
            CancellationToken cancellationToken)
        {
            var attributeSyntax = (AttributeSyntax)context.Node;

            // "attribute.Parent" is "AttributeListSyntax"
            // "attribute.Parent.Parent" is a C# fragment the attributes are applied to
            if (attributeSyntax.Parent?.Parent is not ClassDeclarationSyntax classDeclaration)
                return null;

            var type = context.SemanticModel.GetDeclaredSymbol(classDeclaration) as ITypeSymbol;

            return type is null || !IsEnumeration(type) ? null : type;
        }

        private static bool IsEnumeration(ISymbol type)
        {
            return GetAttribute(type) != null;
        }

        private static AttributeData? GetAttribute(ISymbol type)
        {
            var aName = type.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name != null);
            var name = aName?.AttributeClass?.Name;
            if (name == null)
                return null;
            if (name.StartsWith("JSClassGenerator") || name.StartsWith("JSName"))
                return aName;
            return null;
            //return type.GetAttributes()
            //           .FirstOrDefault(a => (a.AttributeClass?.Name == "JSClassGeneratorAttribute" || a.AttributeClass?.Name == "JSClassGenerator")
            //                    //&& a.AttributeClass.ContainingNamespace is
            //                    // {
            //                    //     Name: "Yantra.Core",
            //                    //     // ContainingNamespace.IsGlobalNamespace: true
            //                    // }
            //                     );
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
                var a = GetAttribute(type);
                if (a?.AttributeClass?.Name == null)
                {
                    continue;
                }
                if (a.AttributeClass.Name.StartsWith("JSName"))
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


            foreach(var (type,a) in  types)
            {
                var code = GenerateCode(type, a, allNames);
                var typeNamespace = type.ContainingNamespace.IsGlobalNamespace
                    ? null
                        : $"{type.ContainingNamespace}.";

                context.AddSource($"{typeNamespace}{type.Name}.g.cs", code);
            }

            var c = GenerateNames(names, allNames);
            var cNS = names.ContainingNamespace.IsGlobalNamespace
                ? null
                    : $"{names.ContainingNamespace}.";

            context.AddSource($"{cNS}{names.Name}.g.cs", c);
        }

        private static  string GenerateNames(ITypeSymbol type, List<string> names)
        {
            var sb = new StringBuilder();

            sb = sb.AppendLine("using System.Collections.Generic;")
                .AppendLine("using System.Runtime.CompilerServices;")
                .AppendLine("using System.Text;")
                .AppendLine("using YantraJS.Core.Runtime;")
                .AppendLine("using YantraJS.Extensions;");

            var ns = type.ContainingNamespace.ToString();
            if (ns != "YantraJS.Core")
            {
                sb = sb.AppendLine("using YantraJS.Core;");
            }

            sb = sb.AppendLine($"namespace {ns} {{ ");

            sb = sb.AppendLine($"partial class {type.Name} {{");

            foreach(var name in names)
            {
                if (type.GetMembers(name).Any(x => x.Name == name))
                    continue;
                sb.AppendLine($"public static readonly KeyString {name};");
            }

            sb.AppendLine($"static {type.Name}() {{");
            foreach(var name in names)
            {
                sb.AppendLine($"{type.Name}.{name} = \"{name}\";");
            }
            sb.AppendLine("}");

            sb.AppendLine("}");
            sb.AppendLine("}");


            return sb.ToString();
        }

        private static string GenerateCode(ITypeSymbol type, AttributeData ad, List<string> names)
        {
            // get attribute info...

            var sb = new StringBuilder();

            sb = sb.AppendLine("using System.Collections.Generic;")
                .AppendLine("using System.Runtime.CompilerServices;")
                .AppendLine("using System.Text;")
                .AppendLine("using YantraJS.Core.Runtime;")
                .AppendLine("using YantraJS.Extensions;");

            var ns = type.ContainingNamespace.ToString();
            if (ns != "YantraJS.Core")
            {
                sb = sb.AppendLine("using YantraJS.Core;");
            }

            sb = sb.AppendLine($"namespace {ns} {{ ");

            sb = sb.AppendLine($"partial class {type.Name} {{");

            var className = type.Name;

            if(ad.ConstructorArguments.Length > 0)
            {
                className = ad.ConstructorArguments[0].Value!.ToString();
            }

            sb = sb.AppendLine($"internal {type.Name}(): base(JSContext.Current[Names.{className}][KeyStrings.prototype] as JSObject) {{}}");

            // generate each marked method...
            sb.AppendLine("public static void RegisterClass(JSContext context) {");

            sb.AppendLine($@"
                var @class = new JSFunction((in Arguments a) => new {type.Name}(in a), ""{className}"");
                context[Names.{className}] = @class;
            ");

            sb.AppendLine("}");

            sb = sb.AppendLine("}");
            sb = sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
