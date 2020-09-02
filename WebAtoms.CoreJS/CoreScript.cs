using Esprima.Ast;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Schema;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.LinqExpressions;
using WebAtoms.CoreJS.Utils;

using Exp = System.Linq.Expressions.Expression;
using ParameterExpression = System.Linq.Expressions.ParameterExpression;

namespace WebAtoms.CoreJS
{

    public class CoreScript: JSAstVisitor<Exp>
    {
        public JSFunctionDelegate Method { get; }

        private LinkedStack<FunctionScope> scope = new LinkedStack<FunctionScope>();

        public LoopScope LoopScope => this.scope.Top.Loop.Top;

        private ParsedScript Code;

        private ParameterExpression FileNameExpression;

        private Dictionary<string, ParameterExpression> keyStrings = new Dictionary<string, ParameterExpression>();

        public Exp KeyOfName(string name)
        {
            ParameterExpression pe;
            if (keyStrings.TryGetValue(name, out pe))
                return pe;
            pe = Exp.Variable(typeof(KeyString), name);
            keyStrings.Add(name, pe);
            return pe;
        }

        private static ConcurrentDictionary<string, JSFunctionDelegate> scripts = new ConcurrentDictionary<string, JSFunctionDelegate>();

        private static JSFunctionDelegate Compile(string code, string location = null)
        {
            return scripts.GetOrAdd(code, (k) =>
            {
                var c = new CoreScript(code, location);
                return c.Method;
            });
        }

        public static JSValue Evaluate(string code, string location = null)
        {
            var fx = Compile(code, location);
            return fx(JSContext.Current, JSArguments.Empty);
        }


        public CoreScript(string code, string location = null)
        {

            location = location ?? "vm";

            FileNameExpression = Exp.Variable(typeof(string), "_fileName");

            this.Code = new ParsedScript(code);
            Esprima.JavaScriptParser parser =
                new Esprima.JavaScriptParser(code, new Esprima.ParserOptions {
                    Range = true,
                    SourceType = SourceType.Script
                });

            // add top level...

            using (var fx = this.scope.Push(new FunctionScope(null)))
            {
                var jScript = parser.ParseScript();
                var script = Visit(jScript);


                var lScope = fx.Scope;
                

                var te = fx.ThisExpression;

                var args = fx.ArgumentsExpression;


                var vList = new List<ParameterExpression>() { 
                    FileNameExpression,
                    lScope
                };

                var sList = new List<Exp>() { 
                    Exp.Assign(FileNameExpression, Exp.Constant(location)),
                    Exp.Assign(lScope, ExpHelper.LexicalScope.NewScope(FileNameExpression,"",1,1))
                };

                var l = Exp.Label(typeof(JSValue));

                foreach(var ks in keyStrings)
                {
                    var v = ks.Value;
                    vList.Add(v);
                    sList.Add(Exp.Assign(v, ExpHelper.KeyStrings.GetOrCreate(Exp.Constant(ks.Key))));
                }

                foreach(var v in fx.Variables)
                {
                    vList.Add(v.Variable);
                    if (v.Init != null)
                    {
                        if (v.Name != null)
                        {
                            sList.Add(Exp.Assign(v.Variable, ExpHelper.JSVariable.New(v.Name)));
                            sList.Add(Exp.Assign(v.Expression, v.Init));
                        } else
                        {
                            sList.Add(Exp.Assign(v.Variable, v.Init));
                        }
                    }
                }
                if (script.Type == typeof(JSVariable))
                    script = JSVariable.ValueExpression(script);
                sList.Add(Exp.Return(l, script));
                sList.Add(Exp.Label(l, Exp.Constant(JSUndefined.Value)));

                script = Exp.Block(vList,
                    Exp.TryFinally(
                        Exp.Block(sList),
                        ExpHelper.IDisposable.Dispose(lScope))
                );

                var lambda = Exp.Lambda<JSFunctionDelegate>(script, te, args);

                this.Method = lambda.Compile();
            }
        }

        protected override Exp VisitProgram(Esprima.Ast.Program program)
        {
            return Exp.Block(program.Body.Select((x) => VisitStatement((Statement)x)));
        }

        protected override Exp VisitCatchClause(Esprima.Ast.CatchClause catchClause)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitFunctionDeclaration(Esprima.Ast.FunctionDeclaration functionDeclaration)
        {
            return CreateFunction(functionDeclaration);
        }

        private Exp CreateFunction(Esprima.Ast.IFunction functionDeclaration)
        {
            var code = Code.Text(functionDeclaration.Range);

            // get text...

            var previousScope = this.scope.Top;

            // if this is an arrowFunction then override previous thisExperssion
            var previousThis = this.scope.Top.ThisExpression;
            if (!(functionDeclaration is ArrowFunctionExpression))
            {
                previousThis = null;
            }


            using (var cs = scope.Push(new FunctionScope(functionDeclaration)))
            {
                var lexicalScopeVar = cs.Scope;

                var s = cs;
                // use this to create variables...
                var t = s.ThisExpression;
                if (previousThis!=null)
                {
                    s.ThisExpression = previousThis;
                }
                var args = s.ArgumentsExpression;

                var r = s.ReturnLabel;

                var sList = new List<Exp>();

                var vList = new List<ParameterExpression>();

                var pList = functionDeclaration.Params.OfType<Identifier>();
                int i = 0;

                

                var argumentElements = Exp.Variable(typeof(Core.JSValue[]),"args");
                var argumentElementsLength = Exp.Variable(typeof(int), "args.Length");
                vList.Add(argumentElements);
                vList.Add(argumentElementsLength);

                sList.Add(Exp.Assign(argumentElements, ExpHelper.JSArguments.Elements(args)));
                sList.Add(Exp.Assign(argumentElementsLength, 
                    Exp.Condition(
                        Exp.NotEqual(Exp.Constant(null, typeof(Core.JSValue[])),argumentElements),
                            Exp.ArrayLength(argumentElements),
                            Exp.Constant(0, typeof(int)))));

                foreach (var v in pList)
                {
                    var var1 = Exp.Variable(typeof(Core.JSVariable), v.Name);
                    var vf = JSVariable.ValueExpression(var1);

                    vList.Add(var1);
                    sList.Add(Exp.Assign(var1, 
                        ExpHelper.JSVariable.FromArgument(argumentElements, argumentElementsLength, i, v.Name)));

                    s.AddVariable(v.Name, vf);

                    i++;
                }

                Exp lambdaBody = null;
                switch (functionDeclaration.Body)
                {
                    case Statement stmt:
                        lambdaBody = VisitStatement(stmt);
                        break;
                    case Expression exp:
                        lambdaBody = Exp.Return(s.ReturnLabel, VisitExpression(exp));
                        break;
                    default:
                        throw new NotImplementedException();
                }

                // vList.AddRange(s.Variables.Select(x => x.Variable));
                foreach(var v in s.Variables)
                {
                    vList.Add(v.Variable);
                    if (v.Init != null)
                    {
                        if (v.Name == null)
                        {
                            sList.Add(Exp.Assign(v.Variable, v.Init));
                        } else
                        {
                            // create..
                            sList.Add(Exp.Assign(v.Variable, ExpHelper.JSVariable.New(v.Init, v.Name)));
                        }
                    }
                }

                sList.Add(lambdaBody);

                sList.Add(Exp.Label(r, ExpHelper.JSUndefined.Value));

                var block = Exp.Block(vList, sList);


                // adding lexical scope pending...


                var fxName = functionDeclaration.Id?.Name ?? "inline";

                var point = this.Code.Position(functionDeclaration.Range);

                var lexicalScope =
                    Exp.Block(new ParameterExpression[] { lexicalScopeVar },
                    Exp.Assign(lexicalScopeVar, 
                        ExpHelper.LexicalScope.NewScope(
                            FileNameExpression,
                            fxName,
                            point.Line,
                            point.Column
                            )),
                    Exp.TryFinally(
                        block,
                        ExpHelper.IDisposable.Dispose(lexicalScopeVar)));

                var lambda = Exp.Lambda(typeof(JSFunctionDelegate), lexicalScope, t, args);


                // create new JSFunction instance...
                var jfs = ExpHelper.JSFunction.New(lambda, fxName, code);

                if (!(functionDeclaration is Esprima.Ast.FunctionDeclaration))
                {
                    return jfs;
                }

                var jsFVar = Exp.Variable(typeof(JSVariable));
                var jsF = JSVariable.ValueExpression(jsFVar);

                previousScope.AddVariable(fxName, jsF, jsFVar, jfs);

                return jsF;
            }
        }
        protected override Exp VisitStatement(Statement statement)
        {
            var s = this.scope.Top.Scope;
            var r = statement.Range;
            var p = this.Code.Position(r);
            return Exp.Block( 
                ExpHelper.LexicalScope.SetPosition(s, p.Line, p.Column)
                , base.VisitStatement(statement));
        }

        protected override Exp VisitWithStatement(Esprima.Ast.WithStatement withStatement)
        {
            // we will not support with
            throw new NotSupportedException("With statement is not supported");
        }

        protected override Exp VisitWhileStatement(Esprima.Ast.WhileStatement whileStatement)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            using (var s = scope.Top.Loop.Push(new LoopScope(breakTarget, continueTarget)))
            {

                var body = VisitStatement(whileStatement.Body);

                var list = new List<Exp>();

                var test = Exp.Not( ExpHelper.JSValue.BooleanValue(VisitExpression(whileStatement.Test)));

                list.Add(Exp.IfThen(test, Exp.Goto(breakTarget)));
                list.Add(body);

                return Exp.Loop(
                    Exp.Block(list), 
                    breakTarget, 
                    continueTarget);
            }
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
                        var ve = Exp.Variable(typeof(JSVariable), id.Name);
                        var vf = JSVariable.ValueExpression(ve);
                        this.scope.Top.AddVariable(id.Name, vf, ve);
                        // inits.Add(Exp.Assign(ve, Exp.New(typeof(JSVariable))));
                        if (declarator.Init != null)
                        {
                            var init = VisitExpression(declarator.Init);
                            inits.Add(Exp.Assign(ve, ExpHelper.JSVariable.New(init, id.Name) ));
                        } else
                        {
                            inits.Add(Exp.Assign(ve, ExpHelper.JSVariable.New(id.Name)));
                        }
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
                var pe = Exp.Parameter(typeof(Exception));
                var ve = Exp.Variable(typeof(JSVariable));
                var vf = JSVariable.ValueExpression(ve);
                var keyName = KeyOfName(id.Name);
                var catchBlock = new List<Exp>();

                scope.Top.AddVariable(id.Name, vf);

                var initVar = Exp.Condition(
                        Exp.TypeIs(pe, typeof(JSException)),
                        ExpHelper.JSException.Error(Exp.Convert(pe, typeof(JSException))),
                        ExpHelper.JSString.New(ExpHelper.Exception.ToString(pe)));

                catchBlock.Add(Exp.Assign(ve, ExpHelper.JSVariable.New(initVar, id.Name)));
                catchBlock.Add(Exp.Assign(ExpHelper.LexicalScope.Index(keyName), ve));
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
            return ExpHelper.JSException.Throw(VisitExpression(throwStatement.Argument));
        }

        protected override Exp VisitSwitchStatement(Esprima.Ast.SwitchStatement switchStatement)
        {
            Exp d = null;
            List<System.Linq.Expressions.SwitchCase> cases = new List<System.Linq.Expressions.SwitchCase>();
            var @continue = this.scope.Top.Loop?.Top?.Continue;
            var @break = Exp.Label();
            var ls = new LoopScope(@break ,@continue, true);
            using (var bt = this.scope.Top.Loop.Push(ls))
            {
                foreach (var c in switchStatement.Cases)
                {
                    var statements = new List<Exp>();
                    // this is probably default...
                    foreach (var es in c.Consequent)
                    {
                        switch (es)
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
                            block = statements[0];
                        }
                        else
                        {
                            block = Exp.Block(statements);
                        }
                    }

                    if (c.Test == null)
                    {
                        d = block;
                    }
                    else
                    {
                        cases.Add(Exp.SwitchCase(block, VisitExpression(c.Test)));
                    }
                }
            }
            var r = Exp.Block(
                Exp.Switch(VisitExpression(switchStatement.Discriminant), d , ExpHelper.JSValue.StaticEquals, cases),
                Exp.Label(@break));
            return r;
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
            var test =  ExpHelper.JSValue.BooleanValue(VisitExpression(ifStatement.Test));
            var trueCase = VisitStatement(ifStatement.Consequent);
            // process else...
            if (ifStatement.Alternate != null)
            {
                return Exp.Condition(test, trueCase, VisitStatement(ifStatement.Alternate));
            }
            return Exp.Condition(test, trueCase, ExpHelper.JSUndefined.Value );
        }

        protected override Exp VisitEmptyStatement(Esprima.Ast.EmptyStatement emptyStatement)
        {
            return ExpHelper.JSUndefined.Value;
        }

        protected override Exp VisitDebuggerStatement(Esprima.Ast.DebuggerStatement debuggerStatement)
        {
            return ExpHelper.JSDebugger.RaiseBreak();
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
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            using (var s = scope.Top.Loop.Push(new LoopScope(breakTarget, continueTarget)))
            {

                var body = VisitStatement(doWhileStatement.Body);

                var list = new List<Exp>();

                var test = Exp.Not(VisitExpression(doWhileStatement.Test));

                list.Add(body);
                list.Add(Exp.IfThen(test, Exp.Goto(breakTarget)));

                return Exp.Loop(
                    Exp.Block(list),
                    breakTarget,
                    continueTarget);
            }
        }

        protected override Exp VisitArrowFunctionExpression(Esprima.Ast.ArrowFunctionExpression arrowFunctionExpression)
        {
            return CreateFunction(arrowFunctionExpression);
        }

        private Exp DoubleValue(Esprima.Ast.Expression exp)
        {
            return ExpHelper.JSValue.DoubleValue(VisitExpression(exp));
        }

        private Exp BooleanValue(Esprima.Ast.Expression exp)
        {
            return ExpHelper.JSValue.BooleanValue(VisitExpression(exp));
        }


        protected override Exp VisitUnaryExpression(Esprima.Ast.UnaryExpression unaryExpression)
        {
            var target = unaryExpression.Argument;

            switch (unaryExpression.Operator)
            {
                case UnaryOperator.Plus:
                    return ExpHelper.JSNumber.New(Exp.UnaryPlus(DoubleValue(target)));
                case UnaryOperator.Minus:
                    return ExpHelper.JSNumber.New(Exp.Negate(DoubleValue(target)));
                case UnaryOperator.BitwiseNot:
                    return ExpHelper.JSNumber.New(Exp.Not( Exp.Convert(DoubleValue(target),typeof(int))));
                case UnaryOperator.LogicalNot:
                    return Exp.Condition(BooleanValue(target), ExpHelper.JSContext.False, ExpHelper.JSContext.True );
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
                    return ExpHelper.JSValue.Delete(VisitExpression(me.Object), pe);
                case UnaryOperator.Void:
                    return ExpHelper.JSUndefined.Value;
                case UnaryOperator.TypeOf:
                    return ExpHelper.JSValue.TypeOf(VisitExpression(target));
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
                    return ExpHelper.JSNumber.New(Exp.AddAssign(DoubleValue(updateExpression.Argument), Exp.Constant(1)));
                }
                return ExpHelper.JSNumber.New(Exp.SubtractAssign(DoubleValue(updateExpression.Argument), Exp.Constant(1)));
            }
            var right = VisitExpression(updateExpression.Argument);
            var ve = Exp.Variable(typeof(JSValue));
            if (updateExpression.Operator == UnaryOperator.Increment)
            {
                return Exp.Block(new ParameterExpression[] { ve }, 
                    Exp.Assign(ve,right),
                    Exp.Assign(right, ExpHelper.JSNumber.New(Exp.Add(DoubleValue(updateExpression.Argument), Exp.Constant((double)1)))),
                    ve);
            }
            return Exp.Block(new ParameterExpression[] { ve },
                Exp.Assign(ve, right),
                Exp.Assign(right, ExpHelper.JSNumber.New(Exp.Subtract(DoubleValue(updateExpression.Argument), Exp.Constant((double)1)))),
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

        class ExpressionHolder
        {
            public Exp Value;
            public Exp Getter;
            public Exp Setter;
        }

        protected override Exp VisitObjectExpression(Esprima.Ast.ObjectExpression objectExpression)
        {
            var keys = new List<(Exp, ExpressionHolder)>();
            var properties = new Dictionary<string, ExpressionHolder>();
            foreach(Property p in objectExpression.Properties)
            {
                Exp key = null;
                Exp value = null;
                switch (p.Key)
                {
                    case Identifier id
                        when !p.Computed:
                        key = KeyOfName(id.Name);
                        if (p.Shorthand)
                        {
                            value = this.scope.Top[id.Name];
                        } else
                        {
                            value = VisitExpression((Expression)p.Value);
                        }
                        if (p.Kind == PropertyKind.Get || p.Kind == PropertyKind.Set)
                        {
                            ExpressionHolder m = null;
                            if(!properties.TryGetValue(id.Name, out m))
                            {
                                m = new ExpressionHolder { };
                                properties[id.Name] = m;
                                keys.Add((key, m));
                            }
                            if (p.Kind == PropertyKind.Get)
                            {
                                m.Getter = value;
                            } else
                            {
                                m.Setter = value; 
                            }
                            m.Value = ExpHelper.JSProperty.Property(key, m.Getter,m.Setter);
                            continue;
                        }
                        else
                        {
                            value = ExpHelper.JSProperty.Value(key, value);
                        }
                        break;
                    default:
                        throw new NotSupportedException();
                }
                keys.Add((key, new ExpressionHolder { Value = value }));
            }

            return ExpHelper.JSObject.New(keys.Select((x) => (x.Item1, x.Item2.Value)));
        }

        protected override Exp VisitNewExpression(Esprima.Ast.NewExpression newExpression)
        {
            var constructor = VisitExpression(newExpression.Callee);
            var args = newExpression.Arguments.Select(e => VisitExpression((Esprima.Ast.Expression)e));
            var pe = ExpHelper.JSArguments.New(args);
            return ExpHelper.JSValue.CreateInstance(constructor, pe);
        }

        protected override Exp VisitMemberExpression(Esprima.Ast.MemberExpression memberExpression)
        {
            switch (memberExpression.Property)
            {
                case Identifier id:
                    if (!memberExpression.Computed)
                    {
                        return ExpHelper.JSValue.KeyStringIndex(
                            VisitExpression(memberExpression.Object),
                            KeyOfName(id.Name));
                    }
                    return ExpHelper.JSValue.Index(
                        VisitExpression(memberExpression.Object),
                        KeyOfName(id.Name));
                case Literal l
                    when l.TokenType == Esprima.TokenType.BooleanLiteral:
                    return ExpHelper.JSValue.Index(
                        VisitExpression(memberExpression.Object),
                        l.BooleanValue ? (uint)0 : (uint)1);
                case Literal l
                    when l.TokenType == Esprima.TokenType.StringLiteral:
                    return ExpHelper.JSValue.KeyStringIndex(
                        VisitExpression(memberExpression.Object),
                        KeyOfName(l.StringValue));
                case Literal l
                    when l.TokenType == Esprima.TokenType.NumericLiteral:
                    return ExpHelper.JSValue.Index(
                        VisitExpression(memberExpression.Object),
                        (uint)l.NumericValue);

            }
            throw new NotImplementedException();
        }

        protected override Exp VisitLogicalExpression(Esprima.Ast.BinaryExpression binaryExpression)
        {
            var left = VisitExpression(binaryExpression.Left);
            var right = VisitExpression(binaryExpression.Right);
            return BinaryOperation.Operation(left, right, binaryExpression.Operator);
        }

        protected override Exp VisitLiteral(Esprima.Ast.Literal literal)
        {
            (Exp exp,string name) GetLiteral()
            {
                switch (literal.TokenType)
                {
                    case Esprima.TokenType.BooleanLiteral:
                        return literal.BooleanValue
                            ? (ExpHelper.JSContext.True, "true")
                            : (ExpHelper.JSContext.False, "false");
                    case Esprima.TokenType.StringLiteral:
                        return (ExpHelper.JSString.New(Exp.Constant(literal.StringValue)), literal.StringValue.Left(5));
                    case Esprima.TokenType.RegularExpression:
                        return (ExpHelper.JSRegExp.New(
                            Exp.Constant(literal.Regex.Pattern),
                            Exp.Constant(literal.Regex.Flags)), (literal.Regex.Pattern + literal.Regex.Flags).Left(10));
                    case Esprima.TokenType.Template:
                        break;
                    case Esprima.TokenType.NullLiteral:
                        return (ExpHelper.JSNull.Value, "null");
                    case Esprima.TokenType.NumericLiteral:
                        return (ExpHelper.JSNumber.New(Exp.Constant(literal.NumericValue)), literal.NumericValue.ToString());
                }
                throw new NotImplementedException();
            }
            var (exp, name) = GetLiteral();
            var pe = Exp.Variable(typeof(JSValue), name);
            this.scope.Top.AddVariable(null, pe, pe, exp);
            return pe;
            
        }

        protected override Exp VisitIdentifier(Esprima.Ast.Identifier identifier)
        {
            // if this is null, fetch from global...
            var local = this.scope.Top[identifier.Name];
            if (local != null)
                return local;
            return ExpHelper.JSContext.Index(KeyOfName(identifier.Name));
        }

        protected override Exp VisitFunctionExpression(Esprima.Ast.IFunction function)
        {
            var a = CreateFunction(function);
            var pe = Exp.Parameter(typeof(JSValue));
            this.scope.Top.AddVariable(null, pe, pe, a);
            return pe;
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
            var test = VisitExpression(conditionalExpression.Test);
            var @true = VisitExpression(conditionalExpression.Consequent);
            var @false = VisitExpression(conditionalExpression.Alternate);
            return Exp.Condition(
                ExpHelper.JSValue.BooleanValue(test),
                @true,
                @false);
        }

        protected override Exp VisitCallExpression(Esprima.Ast.CallExpression callExpression)
        {
            var calle = callExpression.Callee;
            var args = callExpression.Arguments.Select((e) => VisitExpression((Esprima.Ast.Expression)e)).ToList();
            
            var paramArray = args.Any()
                ? ExpHelper.JSArguments.New(args)
                : ExpHelper.JSArguments.Empty();
            if (calle is Esprima.Ast.MemberExpression me)
            {
                // invoke method...

                // get object...
                var obj = VisitExpression(me.Object);

                var id = me.Property.As<Esprima.Ast.Identifier>();


                var name = KeyOfName(id.Name);

                return ExpHelper.JSValue.InvokeMethod(obj, name, paramArray);

            } else {
                var a = ExpHelper.JSNull.Value;
                return ExpHelper.JSValue.InvokeFunction(
                    VisitExpression(callExpression.Callee), a, paramArray);
            }
        }

        protected override Exp VisitBinaryExpression(Esprima.Ast.BinaryExpression binaryExpression)
        {
            var left = VisitExpression(binaryExpression.Left);
            var right = VisitExpression(binaryExpression.Right);
            switch(binaryExpression.Operator)
            {
                case BinaryOperator.Plus:
                    return ExpHelper.JSValue.Add(left, right);
            }
            var a = BinaryOperation.Operation(left, right, binaryExpression.Operator);
            return a;
        }

        protected override Exp VisitArrayExpression(Esprima.Ast.ArrayExpression arrayExpression)
        {
            return ExpHelper.JSArray.New(arrayExpression.Elements.Select(x => VisitExpression((Esprima.Ast.Expression)x)));
        }

        protected override Exp VisitAssignmentExpression(Esprima.Ast.AssignmentExpression assignmentExpression)
        {
            // simple identifier based assignments or 
            // array index based assignments...

            var left = VisitExpression((Esprima.Ast.Expression)assignmentExpression.Left);
            var right = VisitExpression((Esprima.Ast.Expression)assignmentExpression.Right);


            var a = BinaryOperation.Assign(left, right, assignmentExpression.Operator);
            return a;
        }

        protected override Exp VisitContinueStatement(Esprima.Ast.ContinueStatement continueStatement)
        {
            // return Exp.Continue()
            return Exp.Continue(this.scope.Top.Loop.Top.Continue);
        }

        protected override Exp VisitBreakStatement(Esprima.Ast.BreakStatement breakStatement)
        {
            var ls = this.LoopScope;
            if (ls.IsSwitch)
                return Exp.Goto(ls.Break);
            return Exp.Break(ls.Break);
        }

        protected override Exp VisitBlockStatement(Esprima.Ast.BlockStatement blockStatement)
        {
            return Exp.Block(blockStatement.Body.Select(a => VisitStatement((Esprima.Ast.Statement)a)));
        }
    }
}
