using Esprima.Ast;
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

        public static (NamespaceDeclarationSyntax jsNS, 
            ClassDeclarationSyntax jsClass) 
            JSGlobal(BlockSyntax body, string code) {
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

            var lambda = ParenthesizedLambdaExpression(ParameterList(),body);
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


        public static VariableDeclarationSyntax StringVariable(string name, string value = null) {
            var vd = VariableDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)));
            if (value == null)
                vd.AddVariables(VariableDeclarator(name));
            else
                vd.AddVariables(VariableDeclarator(name).WithInitializer(EqualsValueClause(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value)))));
            return vd;
        }

    }

    public struct ScriptLocation
    {
        public string Location { get; set; }

        public int LineNumber { get; set; }

        public int ColumnNumber { get; set; }

    }

    public class CoreScript: JSAstVisitor<string>
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

            //var node = VisitProgram(script) as BlockSyntax;

            //var jsNS = JSSyntaxHelper.JSGlobal(node, code);

            //var unit = SyntaxFactory.CompilationUnit();

            // unit = unit.AddMembers(jsNS.jsNS);

            // var tree = unit.SyntaxTree;

            var codeLiteral = Literal(code).ToFullString();

            var csCode = $@"using System;
using System.Collections.Generic;
using System.Linq;
using WebAtoms.CoreJS.Core;

namespace JSGlobal {{

    public static class JSCode {{
        
        public readonly static string Code = {codeLiteral};

        public static JSFunctionDelegate Body = (_this,_) => {{

#pragma warning disable CS0162 
            return JSUndefined.Value;
#pragma warning restore CS0162 
        }};

    }}

}}
";

            var tree = ParseSyntaxTree(csCode);

            // lets first generate the code...

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
            Method = (JSFunctionDelegate) type.GetField("Body").GetValue(null);
        }

        protected override string VisitProgram(Esprima.Ast.Program program)
        {
            return $@"{{ {
                string.Join(";\r\n",  
                    program.Body.Select((x) => VisitStatement((Esprima.Ast.Statement)x))) } }}";
        }

        protected override string VisitCatchClause(Esprima.Ast.CatchClause catchClause)
        {
            var id = catchClause.Param.As<Esprima.Ast.Identifier>().Name;
            id = Identifier(id).ToFullString();
            //var identifier = Identifier(id.Name);
            //var block = (BlockSyntax)VisitBlockStatement(catchClause.Body);
            //var c = CatchClause(CatchDeclaration(ParseTypeName("JSException"), identifier), null, block);
            //return c;
            return $"catch (JSException {id}){{ {VisitBlockStatement(catchClause.Body)} }}";
        }

        protected override string VisitFunctionDeclaration(Esprima.Ast.FunctionDeclaration functionDeclaration)
        {
            /**
             * var @namedFunction = new JSFunction((t, a) =&gt; {
             *    var a1 = a[0];
             *    var a2 = a[1]; 
             *    
             *    // enter stack...
             *    
             *    // init default...
             *    
             *    // write body ...
             *    
             *    // exit stack...
             *    
             * }, "@namedFunction", "Source code");
             */
            throw new NotImplementedException();
        }

        protected override string VisitWithStatement(Esprima.Ast.WithStatement withStatement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitWhileStatement(Esprima.Ast.WhileStatement whileStatement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitVariableDeclaration(Esprima.Ast.VariableDeclaration variableDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override string VisitTryStatement(Esprima.Ast.TryStatement tryStatement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitThrowStatement(Esprima.Ast.ThrowStatement throwStatement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitSwitchStatement(Esprima.Ast.SwitchStatement switchStatement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitSwitchCase(Esprima.Ast.SwitchCase switchCase)
        {
            throw new NotImplementedException();
        }

        protected override string VisitReturnStatement(Esprima.Ast.ReturnStatement returnStatement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitLabeledStatement(Esprima.Ast.LabeledStatement labeledStatement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitIfStatement(Esprima.Ast.IfStatement ifStatement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitEmptyStatement(Esprima.Ast.EmptyStatement emptyStatement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitDebuggerStatement(Esprima.Ast.DebuggerStatement debuggerStatement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitExpressionStatement(Esprima.Ast.ExpressionStatement expressionStatement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitForStatement(Esprima.Ast.ForStatement forStatement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitForInStatement(Esprima.Ast.ForInStatement forInStatement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitDoWhileStatement(Esprima.Ast.DoWhileStatement doWhileStatement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitArrowFunctionExpression(Esprima.Ast.ArrowFunctionExpression arrowFunctionExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitUnaryExpression(Esprima.Ast.UnaryExpression unaryExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitUpdateExpression(Esprima.Ast.UpdateExpression updateExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitThisExpression(Esprima.Ast.ThisExpression thisExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitSequenceExpression(Esprima.Ast.SequenceExpression sequenceExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitObjectExpression(Esprima.Ast.ObjectExpression objectExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitNewExpression(Esprima.Ast.NewExpression newExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitMemberExpression(Esprima.Ast.MemberExpression memberExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitLogicalExpression(Esprima.Ast.BinaryExpression binaryExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitLiteral(Esprima.Ast.Literal literal)
        {
            throw new NotImplementedException();
        }

        protected override string VisitIdentifier(Esprima.Ast.Identifier identifier)
        {
            throw new NotImplementedException();
        }

        protected override string VisitFunctionExpression(Esprima.Ast.IFunction function)
        {
            throw new NotImplementedException();
        }

        protected override string VisitClassExpression(Esprima.Ast.ClassExpression classExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitExportDefaultDeclaration(Esprima.Ast.ExportDefaultDeclaration exportDefaultDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override string VisitExportAllDeclaration(Esprima.Ast.ExportAllDeclaration exportAllDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override string VisitExportNamedDeclaration(Esprima.Ast.ExportNamedDeclaration exportNamedDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override string VisitExportSpecifier(Esprima.Ast.ExportSpecifier exportSpecifier)
        {
            throw new NotImplementedException();
        }

        protected override string VisitImport(Esprima.Ast.Import import)
        {
            throw new NotImplementedException();
        }

        protected override string VisitImportDeclaration(Esprima.Ast.ImportDeclaration importDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override string VisitImportNamespaceSpecifier(Esprima.Ast.ImportNamespaceSpecifier importNamespaceSpecifier)
        {
            throw new NotImplementedException();
        }

        protected override string VisitImportDefaultSpecifier(Esprima.Ast.ImportDefaultSpecifier importDefaultSpecifier)
        {
            throw new NotImplementedException();
        }

        protected override string VisitImportSpecifier(Esprima.Ast.ImportSpecifier importSpecifier)
        {
            throw new NotImplementedException();
        }

        protected override string VisitMethodDefinition(Esprima.Ast.MethodDefinition methodDefinitions)
        {
            throw new NotImplementedException();
        }

        protected override string VisitForOfStatement(Esprima.Ast.ForOfStatement forOfStatement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitClassDeclaration(Esprima.Ast.ClassDeclaration classDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override string VisitClassBody(Esprima.Ast.ClassBody classBody)
        {
            throw new NotImplementedException();
        }

        protected override string VisitYieldExpression(Esprima.Ast.YieldExpression yieldExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitTaggedTemplateExpression(Esprima.Ast.TaggedTemplateExpression taggedTemplateExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitSuper(Esprima.Ast.Super super)
        {
            throw new NotImplementedException();
        }

        protected override string VisitMetaProperty(Esprima.Ast.MetaProperty metaProperty)
        {
            throw new NotImplementedException();
        }

        protected override string VisitArrowParameterPlaceHolder(Esprima.Ast.ArrowParameterPlaceHolder arrowParameterPlaceHolder)
        {
            throw new NotImplementedException();
        }

        protected override string VisitObjectPattern(Esprima.Ast.ObjectPattern objectPattern)
        {
            throw new NotImplementedException();
        }

        protected override string VisitSpreadElement(Esprima.Ast.SpreadElement spreadElement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitAssignmentPattern(Esprima.Ast.AssignmentPattern assignmentPattern)
        {
            throw new NotImplementedException();
        }

        protected override string VisitArrayPattern(Esprima.Ast.ArrayPattern arrayPattern)
        {
            throw new NotImplementedException();
        }

        protected override string VisitVariableDeclarator(Esprima.Ast.VariableDeclarator variableDeclarator)
        {
            throw new NotImplementedException();
        }

        protected override string VisitTemplateLiteral(Esprima.Ast.TemplateLiteral templateLiteral)
        {
            throw new NotImplementedException();
        }

        protected override string VisitTemplateElement(Esprima.Ast.TemplateElement templateElement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitRestElement(Esprima.Ast.RestElement restElement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitProperty(Esprima.Ast.Property property)
        {
            throw new NotImplementedException();
        }

        protected override string VisitAwaitExpression(Esprima.Ast.AwaitExpression awaitExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitConditionalExpression(Esprima.Ast.ConditionalExpression conditionalExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitCallExpression(Esprima.Ast.CallExpression callExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitBinaryExpression(Esprima.Ast.BinaryExpression binaryExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitArrayExpression(Esprima.Ast.ArrayExpression arrayExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitAssignmentExpression(Esprima.Ast.AssignmentExpression assignmentExpression)
        {
            throw new NotImplementedException();
        }

        protected override string VisitContinueStatement(Esprima.Ast.ContinueStatement continueStatement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitBreakStatement(Esprima.Ast.BreakStatement breakStatement)
        {
            throw new NotImplementedException();
        }

        protected override string VisitBlockStatement(Esprima.Ast.BlockStatement blockStatement)
        {
            throw new NotImplementedException();
        }
    }
}
