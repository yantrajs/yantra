using Esprima.Ast;
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

using Exp = System.Linq.Expressions.Expression;

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

    /// <summary>
    /// Convert variable to named to variable reference when a variable or
    /// parameter is captured.
    /// 
    /// 1. First store everything as ParameterExpression
    /// 2. On capture, convert ParameterExpression to FieldExpression of ParameterExpression
    /// 3. Change all references.. (complicated)
    /// 
    /// Is it possible to create a event listener to add change of variable...?
    /// </summary>
    public class LexicalScope
    {
        private BinaryUInt32Map<ParameterExpression> map = new BinaryUInt32Map<ParameterExpression>();
        readonly LexicalScope parent;

        public LexicalScope(LexicalScope parent)
        {
            this.parent = parent;
        }
        public ParameterExpression Push(Type type, string name)
        {
            var pe = Exp.Parameter(type, name);
            KeyString k = name;
            map[k.Key] = pe;
            return pe;
        }

        public Exp Search(string name)
        {
            KeyString k = name;
            if (map.TryGetValue(k.Key, out var pe))
                return pe;
            if(parent != null)
            {
                return parent.Search(name);
            }
            // need to call JSContext.Current[name];
            return null;
        }


    }

    public class CoreScript: JSAstVisitor<Exp>
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

        private LinkedStack<LexicalScope> lexicalScope;

        public CoreScript(string code, string location = null)
        {
            lexicalScope = new LinkedStack<LexicalScope>(() => new LexicalScope(this.lexicalScope.Top?.Value));

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

        protected override Exp VisitProgram(Esprima.Ast.Program program)
        {
            return Exp.Block(program.Body.Select((x) => VisitStatement((Statement)x)));
        }

        protected override Exp VisitCatchClause(Esprima.Ast.CatchClause catchClause)
        {
            //var id = catchClause.Param.As<Esprima.Ast.Identifier>().Name;
            //id = Identifier(id).ToFullString();
            //var pe = Exp.Parameter(typeof(JSException));
            //var body = this.VisitBlockStatement(catchClause.Body);
            //return Exp.Catch(pe, body);
            throw new NotImplementedException();
        }

        protected override Exp VisitFunctionDeclaration(Esprima.Ast.FunctionDeclaration functionDeclaration)
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

        protected override Exp VisitWithStatement(Esprima.Ast.WithStatement withStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitWhileStatement(Esprima.Ast.WhileStatement whileStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitVariableDeclaration(Esprima.Ast.VariableDeclaration variableDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitTryStatement(Esprima.Ast.TryStatement tryStatement)
        {
            var block = VisitStatement(tryStatement.Block);
            var cb = tryStatement.Handler;
            if (cb != null)
            {
                using (var scope = lexicalScope.PushNew()) {
                    var pe = scope.Value.Push(typeof(JSException), cb.Param.As<Identifier>().Name);
                    var cbExp = Exp.Catch(pe, VisitStatement(cb));
                    return Exp.TryCatch(block, cbExp);
                }
            }

            return Exp.Constant(null);
        }

        protected override Exp VisitThrowStatement(Esprima.Ast.ThrowStatement throwStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitSwitchStatement(Esprima.Ast.SwitchStatement switchStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitSwitchCase(Esprima.Ast.SwitchCase switchCase)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitReturnStatement(Esprima.Ast.ReturnStatement returnStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitLabeledStatement(Esprima.Ast.LabeledStatement labeledStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitIfStatement(Esprima.Ast.IfStatement ifStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitEmptyStatement(Esprima.Ast.EmptyStatement emptyStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitDebuggerStatement(Esprima.Ast.DebuggerStatement debuggerStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitExpressionStatement(Esprima.Ast.ExpressionStatement expressionStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitForStatement(Esprima.Ast.ForStatement forStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitForInStatement(Esprima.Ast.ForInStatement forInStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitDoWhileStatement(Esprima.Ast.DoWhileStatement doWhileStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitArrowFunctionExpression(Esprima.Ast.ArrowFunctionExpression arrowFunctionExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitUnaryExpression(Esprima.Ast.UnaryExpression unaryExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitUpdateExpression(Esprima.Ast.UpdateExpression updateExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitThisExpression(Esprima.Ast.ThisExpression thisExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitSequenceExpression(Esprima.Ast.SequenceExpression sequenceExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitObjectExpression(Esprima.Ast.ObjectExpression objectExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitNewExpression(Esprima.Ast.NewExpression newExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitMemberExpression(Esprima.Ast.MemberExpression memberExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitLogicalExpression(Esprima.Ast.BinaryExpression binaryExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitLiteral(Esprima.Ast.Literal literal)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitIdentifier(Esprima.Ast.Identifier identifier)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitFunctionExpression(Esprima.Ast.IFunction function)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitClassExpression(Esprima.Ast.ClassExpression classExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitExportDefaultDeclaration(Esprima.Ast.ExportDefaultDeclaration exportDefaultDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitExportAllDeclaration(Esprima.Ast.ExportAllDeclaration exportAllDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitExportNamedDeclaration(Esprima.Ast.ExportNamedDeclaration exportNamedDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitExportSpecifier(Esprima.Ast.ExportSpecifier exportSpecifier)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitImport(Esprima.Ast.Import import)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitImportDeclaration(Esprima.Ast.ImportDeclaration importDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitImportNamespaceSpecifier(Esprima.Ast.ImportNamespaceSpecifier importNamespaceSpecifier)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitImportDefaultSpecifier(Esprima.Ast.ImportDefaultSpecifier importDefaultSpecifier)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitImportSpecifier(Esprima.Ast.ImportSpecifier importSpecifier)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitMethodDefinition(Esprima.Ast.MethodDefinition methodDefinitions)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitForOfStatement(Esprima.Ast.ForOfStatement forOfStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitClassDeclaration(Esprima.Ast.ClassDeclaration classDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitClassBody(Esprima.Ast.ClassBody classBody)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitYieldExpression(Esprima.Ast.YieldExpression yieldExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitTaggedTemplateExpression(Esprima.Ast.TaggedTemplateExpression taggedTemplateExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitSuper(Esprima.Ast.Super super)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitMetaProperty(Esprima.Ast.MetaProperty metaProperty)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitArrowParameterPlaceHolder(Esprima.Ast.ArrowParameterPlaceHolder arrowParameterPlaceHolder)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitObjectPattern(Esprima.Ast.ObjectPattern objectPattern)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitSpreadElement(Esprima.Ast.SpreadElement spreadElement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitAssignmentPattern(Esprima.Ast.AssignmentPattern assignmentPattern)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitArrayPattern(Esprima.Ast.ArrayPattern arrayPattern)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitVariableDeclarator(Esprima.Ast.VariableDeclarator variableDeclarator)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitTemplateLiteral(Esprima.Ast.TemplateLiteral templateLiteral)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitTemplateElement(Esprima.Ast.TemplateElement templateElement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitRestElement(Esprima.Ast.RestElement restElement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitProperty(Esprima.Ast.Property property)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitAwaitExpression(Esprima.Ast.AwaitExpression awaitExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitConditionalExpression(Esprima.Ast.ConditionalExpression conditionalExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitCallExpression(Esprima.Ast.CallExpression callExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitBinaryExpression(Esprima.Ast.BinaryExpression binaryExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitArrayExpression(Esprima.Ast.ArrayExpression arrayExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitAssignmentExpression(Esprima.Ast.AssignmentExpression assignmentExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitContinueStatement(Esprima.Ast.ContinueStatement continueStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitBreakStatement(Esprima.Ast.BreakStatement breakStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitBlockStatement(Esprima.Ast.BlockStatement blockStatement)
        {
            throw new NotImplementedException();
        }
    }
}
