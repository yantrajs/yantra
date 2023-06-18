using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;

namespace YantraJS.JSClassGenerator
{
    internal static class SyntaxNodeExtensions
    {

        public static bool IsGeneratorAttributeName(this string name)
        {
            return name.StartsWith("JSClassGenerator")
                || name.StartsWith("JSRegistration")
                || name.StartsWith("JSFunctionG");

        }

        public static bool CouldBeJSClassAsync(
            this SyntaxNode syntaxNode,
            CancellationToken cancellationToken)
        {
            if (syntaxNode is not AttributeSyntax attribute)
                return false;

            var name = ExtractName(attribute.Name);
            if (name == null)
            {
                return false;
            }
            return name.IsGeneratorAttributeName();
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

        public static ITypeSymbol? GetJSClassTypeOrNull(
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

        public static AttributeData? GetAttribute(this ISymbol type)
        {
            var aName = type.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name?.IsGeneratorAttributeName() ?? false);
            return aName;
            //return type.GetAttributes()
            //           .FirstOrDefault(a => (a.AttributeClass?.Name == "JSClassGeneratorAttribute" || a.AttributeClass?.Name == "JSClassGenerator")
            //                    //&& a.AttributeClass.ContainingNamespace is
            //                    // {
            //                    //     Name: "Yantra.Core",
            //                    //     // ContainingNamespace.IsGlobalNamespace: true
            //                    // }
            //                     );
        }


    }
}
