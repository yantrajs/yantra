using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.LinqExpressions;
using WebAtoms.CoreJS.Utils;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

using Exp = System.Linq.Expressions.Expression;

namespace WebAtoms.CoreJS
{
    public class CoreScript: JSAstVisitor<Exp>
    {
        public JSFunctionDelegate Method { get; }

        private LinkedStack<FunctionScope> scope = new LinkedStack<FunctionScope>();

        //public static class TrustedPlatformAssembly
        //{
        //    private static List<MetadataReference> dlls = null;

        //    public static IEnumerable<MetadataReference> Dlls
        //    {
        //        get
        //        {
        //            if (dlls == null)
        //            {
        //                var list = AppDomain.CurrentDomain.GetAssemblies()
        //                    .Where(x => !x.IsDynamic)
        //                    .Select(x => MetadataReference.CreateFromFile(x.Location))
        //                    .ToList();
        //            }
        //            return dlls;
        //        }
        //    }
        //}

        public Exp KeyOfName(string name)
        {
            // do optimization later on..
            return ExpHelper.KeyOf(name);
        }

        //public Exp KeyOfName(Exp name)
        //{
        //    // do optimization later on..
        //    return ExpHelper.KeyOf(name);
        //}


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

//            var codeLiteral = Literal(code).ToFullString();

//            var csCode = $@"using System;
//using System.Collections.Generic;
//using System.Linq;
//using WebAtoms.CoreJS.Core;

//namespace JSGlobal {{

//    public static class JSCode {{
        
//        public readonly static string Code = {codeLiteral};

//        public static JSFunctionDelegate Body = (_this,_) => {{

//#pragma warning disable CS0162 
//            return JSUndefined.Value;
//#pragma warning restore CS0162 
//        }};

//    }}

//}}
//";

//            var tree = ParseSyntaxTree(csCode);

            // lets first generate the code...

            //CSharpCompilation compilation = CSharpCompilation.Create("JSCode", 
            //    new SyntaxTree[] { tree },
            //    TrustedPlatformAssembly.Dlls,
            //    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            //var ms = new MemoryStream();

            //var result = compilation.Emit(ms);

            //var asm = Assembly.Load(ms.ToArray());

            //if(!result.Success)
            //{
            //    throw new InvalidOperationException($"Compilation fixed ... ");
            //}

            //var type = asm.GetType("JSGlobal.JSCode");
            //Method = (JSFunctionDelegate) type.GetField("Body").GetValue(null);
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
            // hoisting is pending ...

            using(var cs = scope.Push(new FunctionScope(functionDeclaration)))
            {
                var s = cs.Value;
                // use this to create variables...
                var t = s.ThisExpression;
                var args = s.ArgumentsExpression;

                var r = s.ReturnLabel;

                var lambdaBody = VisitStatement(functionDeclaration.Body.As<Statement>());

                var sList = new List<Exp> ();

                var vList = new List<ParameterExpression>();

                var pList = functionDeclaration.Params.OfType<Identifier>();
                uint i = 0;
                foreach(var v in pList)
                {
                    var var1 = Exp.Variable(typeof(JSVariable));
                    var vf = JSVariable.ValueExpression(var1);

                    vList.Add(var1);

                    sList.Add(Exp.Assign(vf, TypeHelper<JSArray>.Call<uint>( args, "GetAt", Exp.Constant(i))));
                    var vk = KeyOfName(v.Name);
                    // add in scope...
                    sList.Add(ExpHelper.AddToScope(vk, var1));

                    i++;
                }

                vList.AddRange(s.Variables.Select(x => x.Variable));

                sList.Add(lambdaBody);

                sList.Add(Exp.Label(s.ReturnLabel));

                var block = Exp.Block(vList, sList);

                var lambda = Exp.Lambda(block, t, args);

                var fxName = Exp.Constant(functionDeclaration.Id.Name);

                var code = Exp.Constant(functionDeclaration.ToString());
               
                // create new JSFunction instance...
                var jfs = TypeHelper<JSFunction>.New<JSFunctionDelegate, string, string>(lambda, fxName , code);
                return jfs;
            }
            // throw new NotImplementedException();
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
            // lets add variable...
            // forget about const... compiler like typescript should take care of it...
            // let will be implemented in future...
            var inits = new List<Exp>();
            foreach(var declarator in variableDeclaration.Declarations)
            {
                switch(declarator.Id)
                {
                    case Esprima.Ast.Identifier id:
                        var ve = Exp.Variable(typeof(JSVariable));
                        var vf = JSVariable.ValueExpression(ve);
                        this.scope.Top.AddVariable(id.Name, vf, ve);

                        if (declarator.Init != null)
                        {
                            inits.Add(Exp.Assign(vf, VisitExpression(declarator.Init)));
                        } else
                        {
                            inits.Add(Exp.Assign(vf, ExpHelper.Undefined));
                        }
                        // add to scope...
                        var keyName = KeyOfName(id.Name);
                        inits.Add(ExpHelper.AddToScope(keyName, ve));
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            if (inits.Any())
            {
                return Exp.Block(inits);
            }
            return Exp.Block();
        }

        protected override Exp VisitTryStatement(Esprima.Ast.TryStatement tryStatement)
        {
            var block = VisitStatement(tryStatement.Block);
            var cb = tryStatement.Handler;
            if (cb != null)
            {
                var id = cb.Param.As<Identifier>();
                var pe = Exp.Parameter(typeof(JSException));
                var ve = Exp.Variable(typeof(JSVariable));
                var vf = JSVariable.ValueExpression(ve);
                var keyName = KeyOfName(id.Name);
                var catchBlock = new List<Exp>();

                scope.Top.AddVariable(id.Name, vf);

                catchBlock.Add(Exp.Assign(vf, ExpHelper.GetError(pe)));
                catchBlock.Add(ExpHelper.AddToScope(keyName, ve));
                catchBlock.Add(VisitStatement(cb));

                var cbExp = Exp.Catch(pe, Exp.Block(new ParameterExpression[] { ve }, catchBlock ));

                if (tryStatement.Finalizer != null)
                {
                    return Exp.TryCatchFinally(block, VisitStatement(tryStatement.Finalizer), cbExp);
                }

                return Exp.TryCatch(block, cbExp);
            }

            var @finally = tryStatement.Finalizer;
            if (@finally != null)
            {
                return Exp.TryFinally(block, VisitStatement(@finally));
            }

            return Exp.Constant(null);
        }

        protected override Exp VisitThrowStatement(Esprima.Ast.ThrowStatement throwStatement)
        {
            return ExpHelper.Throw(VisitExpression(throwStatement.Argument));
        }

        protected override Exp VisitSwitchStatement(Esprima.Ast.SwitchStatement switchStatement)
        {
            Exp d = null;
            List<System.Linq.Expressions.SwitchCase> cases = new List<System.Linq.Expressions.SwitchCase>();
            foreach(var c in switchStatement.Cases)
            {
                var statements = new List<Exp>();
                // this is probably default...
                foreach(var es in c.Consequent)
                {
                    switch(es)
                    {
                        case Esprima.Ast.Statement stmt:
                            statements.Add(VisitStatement(stmt));
                            break;
                        case Esprima.Ast.Expression exp:
                            statements.Add(VisitExpression(exp));
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }

                Exp block = null;

                if (statements.Any())
                {
                    if (statements.Count == 1)
                    {
                        block = Exp.Block(statements);
                    }
                    else
                    {
                        block = statements[0];
                    }
                }

                if (c.Test == null)
                {
                    d = block;
                } else
                {
                    cases.Add(Exp.SwitchCase(block, VisitExpression(c.Test)));
                }
            }
            return Exp.Switch(VisitExpression(switchStatement.Discriminant), d, null, cases);
        }

        protected override Exp VisitSwitchCase(Esprima.Ast.SwitchCase switchCase)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitReturnStatement(Esprima.Ast.ReturnStatement returnStatement)
        {
            return Exp.Return( this.scope.Top.ReturnLabel, VisitExpression(returnStatement.Argument));
        }

        protected override Exp VisitLabeledStatement(Esprima.Ast.LabeledStatement labeledStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitIfStatement(Esprima.Ast.IfStatement ifStatement)
        {
            var test = VisitExpression(ifStatement.Test);
            var trueCase = VisitStatement(ifStatement.Consequent);
            // process else...
            if (ifStatement.Alternate != null)
            {
                return Exp.Condition(test, trueCase, VisitStatement(ifStatement.Alternate));
            }
            return Exp.Condition(test, trueCase, ExpHelper.Undefined );
        }

        protected override Exp VisitEmptyStatement(Esprima.Ast.EmptyStatement emptyStatement)
        {
            return ExpHelper.Undefined;
        }

        protected override Exp VisitDebuggerStatement(Esprima.Ast.DebuggerStatement debuggerStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitExpressionStatement(Esprima.Ast.ExpressionStatement expressionStatement)
        {
            return VisitExpression(expressionStatement.Expression);
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

        private Exp DoubleValue(Esprima.Ast.Expression exp)
        {
            return ExpHelper.DoubleValue(VisitExpression(exp));
        }

        private Exp BooleanValue(Esprima.Ast.Expression exp)
        {
            return ExpHelper.BooleanValue(VisitExpression(exp));
        }


        protected override Exp VisitUnaryExpression(Esprima.Ast.UnaryExpression unaryExpression)
        {
            var target = unaryExpression.Argument;

            switch (unaryExpression.Operator)
            {
                case UnaryOperator.Plus:
                    return ExpHelper.JSValueFromDouble(Exp.UnaryPlus(DoubleValue(target)));
                case UnaryOperator.Minus:
                    return ExpHelper.JSValueFromDouble(Exp.Negate(DoubleValue(target)));
                case UnaryOperator.BitwiseNot:
                    return ExpHelper.JSValueFromDouble(Exp.Not( Exp.Convert(DoubleValue(target),typeof(int))));
                case UnaryOperator.LogicalNot:
                    return ExpHelper.JSValueFromDouble(Exp.Negate(BooleanValue(target)));
                case UnaryOperator.Delete:
                    // delete expression...
                    var me = target as Esprima.Ast.MemberExpression;
                    Exp pe = null;
                    if (me.Computed)
                    {
                        pe = VisitExpression(me.Property);
                    } else
                    {
                        pe = Exp.Constant(me.Property.As<Identifier>().Name);
                    }
                    return ExpHelper.Delete(VisitExpression(me.Object), pe);
                case UnaryOperator.Void:
                    return ExpHelper.Undefined;
                case UnaryOperator.TypeOf:
                    return ExpHelper.TypeOf(VisitExpression(target));
                case UnaryOperator.Increment:
                    return this.InternalVisitUpdateExpression(unaryExpression);
                case UnaryOperator.Decrement:
                    return this.InternalVisitUpdateExpression(unaryExpression);
            }
            throw new InvalidOperationException();
        }
        protected override Exp VisitUpdateExpression(UpdateExpression updateExpression)
        {
            return InternalVisitUpdateExpression(updateExpression);
        }

        private Exp InternalVisitUpdateExpression(Esprima.Ast.UnaryExpression updateExpression)
        {
            // added support for a++, a--
            if (updateExpression.Prefix) { 
                if (updateExpression.Operator == UnaryOperator.Increment)
                {
                    return ExpHelper.JSValueFromDouble(Exp.AddAssign(DoubleValue(updateExpression.Argument), Exp.Constant(1)));
                }
                return ExpHelper.JSValueFromDouble(Exp.SubtractAssign(DoubleValue(updateExpression.Argument), Exp.Constant(1)));
            }
            var right = VisitExpression(updateExpression.Argument);
            var ve = Exp.Variable(typeof(JSValue));
            if (updateExpression.Operator == UnaryOperator.Increment)
            {
                return Exp.Block(new ParameterExpression[] { ve }, 
                    Exp.Assign(ve,right),
                    Exp.Assign(right, ExpHelper.JSValueFromDouble(Exp.Add(DoubleValue(updateExpression.Argument), Exp.Constant(1)))),
                    ve);
            }
            return Exp.Block(new ParameterExpression[] { ve },
                Exp.Assign(ve, right),
                Exp.Assign(right, ExpHelper.JSValueFromDouble(Exp.Subtract(DoubleValue(updateExpression.Argument), Exp.Constant(1)))),
                ve);
        }

        protected override Exp VisitThisExpression(Esprima.Ast.ThisExpression thisExpression)
        {
            // this can never be null
            // check if the global function thisExpression has been setup or not...
            return this.scope.Top.ThisExpression;
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
            var constructor = VisitExpression(newExpression.Callee);
            var args = newExpression.Arguments.Select(e => VisitExpression((Esprima.Ast.Expression)e));
            var pe = ExpHelper.NewArguments(args);
            return TypeHelper<JSValue>.Call<JSArray>(constructor, "CreateInstance",pe);
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
            switch (literal.TokenType)
            {
                case Esprima.TokenType.BooleanLiteral:
                    return literal.BooleanValue 
                        ? ExpHelper.True
                        : ExpHelper.False;
                case Esprima.TokenType.StringLiteral:
                    return TypeHelper<JSString>.New(literal.StringValue);
                case Esprima.TokenType.RegularExpression:
                    return TypeHelper<JSRegExp>.New(literal.Regex.Pattern, literal.Regex.Flags);
                case Esprima.TokenType.Template:
                    break;
                case Esprima.TokenType.NullLiteral:
                    return TypeHelper<JSNull>.StaticProperty("Value");
                case Esprima.TokenType.NumericLiteral:
                    return TypeHelper<JSNumber>.New(literal.NumericValue);
            }
            throw new NotImplementedException();
        }

        protected override Exp VisitIdentifier(Esprima.Ast.Identifier identifier)
        {
            // if this is null, fetch from global...
            return this.scope.Top[identifier.Name];
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
            var calle = callExpression.Callee;
            var args = callExpression.Arguments.Select((e) => VisitExpression((Esprima.Ast.Expression)e));
            if (calle is Esprima.Ast.MemberExpression me)
            {
                // invoke method...

                // get object...
                var obj = VisitExpression(me.Object);

                var id = me.Property.As<Esprima.Ast.Identifier>();

                var paramArray = ExpHelper.NewArguments(args);

                return TypeHelper<JSValue>.Call<object, JSArray>(obj, "InternalInvoke", Exp.Constant(id.Name), paramArray);

            } else {
                return TypeHelper<JSValue>.Call(VisitExpression(callExpression.Callee), "InvokeFunction", args);
            }
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
            // return Exp.Continue()
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
