using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace WebAtoms.CoreJS
{
    public static class JSSyntaxHelper
    {

        public static (NamespaceDeclarationSyntax jsNS,
            ClassDeclarationSyntax jsClass)
            JSGlobal(BlockSyntax body, string code)
        {
            var ns = NamespaceDeclaration(IdentifierName("JSGlobal"));

            ns = ns.AddUsings(
                UsingDirective(IdentifierName("System")),
                UsingDirective(IdentifierName("System.Linq")),
                UsingDirective(IdentifierName("System.Collections.Generic")),
                UsingDirective(IdentifierName("WebAtoms.CoreJS.Core"))
                );

            var jsc = ClassDeclaration("JSCode");
            jsc = jsc.AddModifiers(
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.StaticKeyword));

            jsc = jsc.AddMembers(Field(StringVariable("Code", code), true, true, true));


            ns = ns.AddMembers(jsc);

            // Generate delegate...

            var lambda = ParenthesizedLambdaExpression(ParameterList(), body);
            var t = Parameter(Identifier("t"));
            var a = Parameter(Identifier("a"));
            lambda = lambda.AddParameterListParameters(t, a);

            var fx = VariableDeclaration(IdentifierName("JSFunctionDelegate"));

            fx = fx.AddVariables(VariableDeclarator("Body").WithInitializer(EqualsValueClause(lambda)));

            jsc = jsc.AddMembers(Field(fx).AddModifiers(
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.StaticKeyword)));

            return (ns, jsc);
        }

        public static FieldDeclarationSyntax Field(
            VariableDeclarationSyntax d,
            bool isPublic = true,
            bool isStatic = false,
            bool isReadOnly = false)
        {
            var fd = FieldDeclaration(d);
            if (isPublic)
                fd.AddModifiers(Token(SyntaxKind.PublicKeyword));
            if (isStatic)
                fd.AddModifiers(Token(SyntaxKind.StaticKeyword));
            if (isReadOnly)
                fd.AddModifiers(Token(SyntaxKind.ReadOnlyKeyword));
            return fd;
        }


        public static VariableDeclarationSyntax StringVariable(string name, string value = null)
        {
            var vd = VariableDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)));
            if (value == null)
                vd.AddVariables(VariableDeclarator(name));
            else
                vd.AddVariables(VariableDeclarator(name).WithInitializer(EqualsValueClause(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value)))));
            return vd;
        }

    }
}
