using Esprima.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Utils;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace WebAtoms.CoreJS
{

    public static class JSSyntaxHelper
    {
        public static NamespaceDeclarationSyntax JSGlobal =
            NamespaceDeclaration(IdentifierName("JSGlobal"));

        public static FieldDeclarationSyntax Field(VariableDeclarationSyntax d, bool isPublic = true, bool isStatic = false, bool isReadOnly = false)
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

        public static VariableDeclarationSyntax StringVariable(string name, string value = null) {
            var vd = VariableDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)));
            if (value == null)
                vd.AddVariables(VariableDeclarator(name));
            else
                vd.AddVariables(VariableDeclarator(name).WithInitializer(EqualsValueClause(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value)))));
            return vd;
        }

        public static ClassDeclarationSyntax JSClass(string code)
        {
            var cd = ClassDeclaration("JSCode");
            cd.Members.Add(Field(StringVariable("code", code), true, true, true));
            return cd;
        }
    }

    public struct ScriptLocation
    {
        public string Location { get; set; }

        public int LineNumber { get; set; }

        public int ColumnNumber { get; set; }

    }

    public class CoreScript: JSAstVisitor<SyntaxNode>
    {
        public JSFunctionDelegate Method { get; }

        public static class TrustedPlatformAssembly
        {
            private static List<MetadataReference> dlls = null;

            public static IEnumerable<MetadataReference> Dlls
            {
                get
                {
                    if (dlls == null)
                    {
                        var list = AppDomain.CurrentDomain.GetAssemblies()
                            .Where(x => !x.IsDynamic)
                            .Select(x => MetadataReference.CreateFromFile(x.Location))
                            .ToList();
                    }
                    return dlls;
                }
            }
        }

        public CoreScript(string code, string location = null)
        {

            Esprima.JavaScriptParser parser =
                new Esprima.JavaScriptParser(code, new Esprima.ParserOptions {
                Loc = true,
                SourceType = SourceType.Script
                });

            var script = parser.ParseScript();

            var node = VisitProgram(script);

            var unit = SyntaxFactory.CompilationUnit()
                .WithMembers(
                SingletonList<MemberDeclarationSyntax>(
                    NamespaceDeclaration(IdentifierName("JSGlobal"))
                    .WithMembers(SingletonList<MemberDeclarationSyntax>(
                        ClassDeclaration("JSCode")
                            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                            .WithMembers(
                            )
                        ))
                    )
                );

            var jsNS = NamespaceDeclaration(
                IdentifierName("JSGlobal"));

            var jsClass = ClassDeclaration("JSCode");
            VariableDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword))
            SyntaxFactory.FieldDeclaration(code);

            jsNS.AddMembers(jsClass);

            unit = unit.AddMembers(jsNS);

            var tree = unit.SyntaxTree;



            CSharpCompilation compilation = CSharpCompilation.Create("JSCode", 
                new SyntaxTree[] { tree },
                TrustedPlatformAssembly.Dlls,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var ms = new MemoryStream();

            var result = compilation.Emit(ms);

            var asm = Assembly.Load(ms.ToArray());

            if(!result.Success)
            {
                throw new InvalidOperationException($"Compilation fixed ... ");
            }

            var type = asm.GetType("JSGlobal.JSCode");
            Method = (JSFunctionDelegate) type.GetField("globalMethod").GetValue(null);
        }

        protected override SyntaxNode VisitProgram(Esprima.Ast.Program program)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitCatchClause(Esprima.Ast.CatchClause catchClause)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitFunctionDeclaration(Esprima.Ast.FunctionDeclaration functionDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitWithStatement(Esprima.Ast.WithStatement withStatement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitWhileStatement(Esprima.Ast.WhileStatement whileStatement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitVariableDeclaration(Esprima.Ast.VariableDeclaration variableDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitTryStatement(Esprima.Ast.TryStatement tryStatement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitThrowStatement(Esprima.Ast.ThrowStatement throwStatement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitSwitchStatement(Esprima.Ast.SwitchStatement switchStatement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitSwitchCase(Esprima.Ast.SwitchCase switchCase)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitReturnStatement(Esprima.Ast.ReturnStatement returnStatement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitLabeledStatement(Esprima.Ast.LabeledStatement labeledStatement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitIfStatement(Esprima.Ast.IfStatement ifStatement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitEmptyStatement(Esprima.Ast.EmptyStatement emptyStatement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitDebuggerStatement(Esprima.Ast.DebuggerStatement debuggerStatement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitExpressionStatement(Esprima.Ast.ExpressionStatement expressionStatement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitForStatement(Esprima.Ast.ForStatement forStatement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitForInStatement(Esprima.Ast.ForInStatement forInStatement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitDoWhileStatement(Esprima.Ast.DoWhileStatement doWhileStatement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitArrowFunctionExpression(Esprima.Ast.ArrowFunctionExpression arrowFunctionExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitUnaryExpression(Esprima.Ast.UnaryExpression unaryExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitUpdateExpression(Esprima.Ast.UpdateExpression updateExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitThisExpression(Esprima.Ast.ThisExpression thisExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitSequenceExpression(Esprima.Ast.SequenceExpression sequenceExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitObjectExpression(Esprima.Ast.ObjectExpression objectExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitNewExpression(Esprima.Ast.NewExpression newExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitMemberExpression(Esprima.Ast.MemberExpression memberExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitLogicalExpression(Esprima.Ast.BinaryExpression binaryExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitLiteral(Esprima.Ast.Literal literal)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitIdentifier(Esprima.Ast.Identifier identifier)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitFunctionExpression(Esprima.Ast.IFunction function)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitClassExpression(Esprima.Ast.ClassExpression classExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitExportDefaultDeclaration(Esprima.Ast.ExportDefaultDeclaration exportDefaultDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitExportAllDeclaration(Esprima.Ast.ExportAllDeclaration exportAllDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitExportNamedDeclaration(Esprima.Ast.ExportNamedDeclaration exportNamedDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitExportSpecifier(Esprima.Ast.ExportSpecifier exportSpecifier)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitImport(Esprima.Ast.Import import)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitImportDeclaration(Esprima.Ast.ImportDeclaration importDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitImportNamespaceSpecifier(Esprima.Ast.ImportNamespaceSpecifier importNamespaceSpecifier)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitImportDefaultSpecifier(Esprima.Ast.ImportDefaultSpecifier importDefaultSpecifier)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitImportSpecifier(Esprima.Ast.ImportSpecifier importSpecifier)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitMethodDefinition(Esprima.Ast.MethodDefinition methodDefinitions)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitForOfStatement(Esprima.Ast.ForOfStatement forOfStatement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitClassDeclaration(Esprima.Ast.ClassDeclaration classDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitClassBody(Esprima.Ast.ClassBody classBody)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitYieldExpression(Esprima.Ast.YieldExpression yieldExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitTaggedTemplateExpression(Esprima.Ast.TaggedTemplateExpression taggedTemplateExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitSuper(Esprima.Ast.Super super)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitMetaProperty(Esprima.Ast.MetaProperty metaProperty)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitArrowParameterPlaceHolder(Esprima.Ast.ArrowParameterPlaceHolder arrowParameterPlaceHolder)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitObjectPattern(Esprima.Ast.ObjectPattern objectPattern)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitSpreadElement(Esprima.Ast.SpreadElement spreadElement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitAssignmentPattern(Esprima.Ast.AssignmentPattern assignmentPattern)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitArrayPattern(Esprima.Ast.ArrayPattern arrayPattern)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitVariableDeclarator(Esprima.Ast.VariableDeclarator variableDeclarator)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitTemplateLiteral(Esprima.Ast.TemplateLiteral templateLiteral)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitTemplateElement(Esprima.Ast.TemplateElement templateElement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitRestElement(Esprima.Ast.RestElement restElement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitProperty(Esprima.Ast.Property property)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitAwaitExpression(Esprima.Ast.AwaitExpression awaitExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitConditionalExpression(Esprima.Ast.ConditionalExpression conditionalExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitCallExpression(Esprima.Ast.CallExpression callExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitBinaryExpression(Esprima.Ast.BinaryExpression binaryExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitArrayExpression(Esprima.Ast.ArrayExpression arrayExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitAssignmentExpression(Esprima.Ast.AssignmentExpression assignmentExpression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitContinueStatement(Esprima.Ast.ContinueStatement continueStatement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitBreakStatement(Esprima.Ast.BreakStatement breakStatement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitBlockStatement(Esprima.Ast.BlockStatement blockStatement)
        {
            throw new NotImplementedException();
        }
    }
}
