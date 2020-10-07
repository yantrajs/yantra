using Esprima;
using Esprima.Ast;
using Microsoft.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Core.Generator;
using WebAtoms.CoreJS.ExpHelper;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.LinqExpressions;
using WebAtoms.CoreJS.Parser;
using WebAtoms.CoreJS.Utils;
using static WebAtoms.CoreJS.FunctionScope;
using Exp = System.Linq.Expressions.Expression;
using ParameterExpression = System.Linq.Expressions.ParameterExpression;

namespace WebAtoms.CoreJS
{

    public class CoreScript: JSAstVisitor<Exp>
    {
        public JSFunctionDelegate Method { get; }

        readonly LinkedStack<FunctionScope> scope = new LinkedStack<FunctionScope>();

        public LoopScope LoopScope => this.scope.Top.Loop.Top;

        // private ParsedScript Code;

        readonly string Code;

        readonly ParameterExpression FileNameExpression;

        readonly Dictionary<string, ParameterExpression> keyStrings
            = new Dictionary<string, ParameterExpression>();

        public Exp KeyOfName(string name)
        {
            if (keyStrings.TryGetValue(name, out ParameterExpression pe))
                return pe;
            pe = Exp.Variable(typeof(KeyString), name);
            keyStrings.Add(name, pe);
            return pe;
        }

        static readonly ConcurrentDictionary<string, JSFunctionDelegate> scripts
            = new ConcurrentDictionary<string, JSFunctionDelegate>();

        internal static JSFunctionDelegate Compile(string code, string location = null, IList<string> args = null)
        {
            return scripts.GetOrAdd(code, (k) =>
            {
                var c = new CoreScript(code, location, args);
                return c.Method;
            });
        }

        public static JSValue EvaluateWithTasks(string code, string location = null)
        {
            var fx = Compile(code, location);
            var result = JSUndefined.Value;
            var ctx = JSContext.Current;
            AsyncPump.Run(() => {
                result = fx(new Arguments(ctx));
                return Task.CompletedTask;
            });
            return result;
        }


        public static JSValue Evaluate(string code, string location = null)
        {
            var fx = Compile(code, location);
            var result = JSUndefined.Value;
            var ctx = JSContext.Current;
            result = fx(new Arguments(ctx));
            return result;
        }


        public CoreScript(string code, string location = null, IList<string> argsList = null)
        {
            this.Code = code;
            location = location ?? "vm.js";

            FileNameExpression = Exp.Variable(typeof(string), "_fileName");

            // this.Code = new ParsedScript(code);
            Esprima.JavaScriptParser parser =
                new Esprima.JavaScriptParser(code, new Esprima.ParserOptions {
                    Range = true,
                    Loc = true,
                    SourceType = SourceType.Script
                });

            // add top level...

            using (var fx = this.scope.Push(new FunctionScope((IFunctionDeclaration)null)))
            {
                var jScript = parser.ParseScript();

                var lScope = fx.Scope;


                var te = fx.ThisExpression;

                var args = fx.ArgumentsExpression;

                var argLength = Exp.Parameter(typeof(int));



                var vList = new List<ParameterExpression>() {
                    FileNameExpression,
                    lScope,
                    argLength
                };




                var sList = new List<Exp>() {
                    Exp.Assign(FileNameExpression, Exp.Constant(location)),
                    Exp.Assign(lScope, ExpHelper.LexicalScopeBuilder.NewScope(FileNameExpression,"",1,1)),
                    Exp.Assign(argLength, ArgumentsBuilder.Length(fx.ArgumentsExpression))
                };

                if (argsList != null)
                {
                    int i = 0;
                    foreach (var arg in argsList)
                    {

                        // global arguments are set here for FunctionConstructor

                        fx.CreateVariable(arg,
                            JSVariableBuilder.FromArgument(fx.ArgumentsExpression, i++, arg));
                    }
                }

                var l = fx.ReturnLabel;

                var script = Visit(jScript);

                foreach (var ks in keyStrings)
                {
                    var v = ks.Value;
                    vList.Add(v);
                    sList.Add(Exp.Assign(v, ExpHelper.KeyStringsBuilder.GetOrCreate(Exp.Constant(ks.Key))));
                }

                vList.AddRange(fx.VariableParameters);
                sList.AddRange(fx.InitList);

                sList.Add(Exp.Return(l, script.ToJSValue()));
                sList.Add(Exp.Label(l, Exp.Constant(JSUndefined.Value)));

                script = Exp.Block(vList,
                    Exp.TryFinally(
                        Exp.Block(sList),
                        ExpHelper.IDisposableBuilder.Dispose(lScope))
                );

                var lambda = Exp.Lambda<JSFunctionDelegate>(script, fx.Arguments);

                this.Method = lambda.Compile();

            }
        }

        protected override Exp VisitProgram(Esprima.Ast.Program program)
        {
            return CreateBlock(program.Body);
        }

        protected override Exp VisitCatchClause(Esprima.Ast.CatchClause catchClause)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitFunctionDeclaration(Esprima.Ast.FunctionDeclaration functionDeclaration)
        {
            return CreateFunction(functionDeclaration);
        }

        private Exp CreateClass(Identifier id, Expression super, ClassBody body)
        {

            // need to save super..
            // create a super variable...
            Exp superExp = null;
            if (super != null)
            {
                superExp = VisitExpression(super);
            }

            foreach(var property in body.Body)
            {
                switch (property.Kind)
                {
                    case PropertyKind.None:
                        break;
                    case PropertyKind.Data:
                        break;
                    case PropertyKind.Get:
                        break;
                    case PropertyKind.Set:
                        break;
                    case PropertyKind.Init:
                        break;
                    case PropertyKind.Constructor:
                        break;
                    case PropertyKind.Method:
                        break;
                }
            }

            throw new NotImplementedException();
        }

        private Exp CreateFunction(
            Esprima.Ast.IFunction functionDeclaration
            )
        {
            var code = Code.Substring(functionDeclaration.Range.Start, 
                functionDeclaration.Range.End - functionDeclaration.Range.Start);

            // get text...

            var previousScope = this.scope.Top;

            // if this is an arrowFunction then override previous thisExperssion

            var previousThis = this.scope.Top.ThisExpression;
            if (!(functionDeclaration is ArrowFunctionExpression))
            {
                previousThis = null;
            }

            var functionName  = functionDeclaration.Id?.Name;


            using (var cs = scope.Push(new FunctionScope(functionDeclaration, previousThis)))
            {
                var lexicalScopeVar = cs.Scope;


                FunctionScope.VariableScope jsFVarScope = null;

                if (functionName != null)
                {
                    jsFVarScope = previousScope.GetVariable(functionName);

                }

                var s = cs;
                // use this to create variables...
                var t = s.ThisExpression;
                var args = s.ArgumentsExpression;

                var r = s.ReturnLabel;

                var sList = new List<Exp>();

                var vList = new List<ParameterExpression>();

                var pList = functionDeclaration.Params.OfType<Identifier>();
                int i = 0;

                

                var argumentElements = args;
                // var argumentElementsLength = Exp.Variable(typeof(int), "args.Length");
                // vList.Add(argumentElementsLength);

                //sList.Add(Exp.Assign(argumentElementsLength, 
                //    Exp.Condition(
                //        Exp.NotEqual(Exp.Constant(null, typeof(Core.JSValue[])),argumentElements),
                //            Exp.ArrayLength(argumentElements),
                //            Exp.Constant(0, typeof(int)))));

                foreach (var v in pList)
                {
                    var v1 = s.CreateVariable(v.Name, 
                        ExpHelper.JSVariableBuilder.FromArgument(argumentElements, i, v.Name));
                    i++;
                }

                if(functionDeclaration.HoistingScope != null)
                {
                    foreach(var fh in functionDeclaration.HoistingScope.FunctionDeclarations)
                    {
                        var name = fh.Id.Name;
                        if (string.IsNullOrEmpty(name))
                        {
                            continue;
                        }
                        s.CreateVariable(name, JSVariableBuilder.New(name));
                    }
                    foreach(var vh in functionDeclaration.HoistingScope.VariableDeclarations)
                    {
                        foreach(var vd in vh.Declarations)
                        {
                            s.CreateVariable(vd.Id.As<Identifier>().Name,
                                vd.Init == null
                                ? null
                                : VisitExpression(vd.Init), vh.Kind != VariableDeclarationKind.Var);
                        }
                    }
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

                vList.AddRange(s.VariableParameters);
                sList.AddRange(s.InitList);

                sList.Add(lambdaBody);

                sList.Add(Exp.Label(r, ExpHelper.JSUndefinedBuilder.Value));

                var block = Exp.Block(vList, sList);


                // adding lexical scope pending...


                var fxName = functionDeclaration.Id?.Name ?? "inline";

                var point = functionDeclaration.Location.Start; // this.Code.Position(functionDeclaration.Range);

                var lexicalScope =
                    Exp.Block(new ParameterExpression[] { lexicalScopeVar },
                    Exp.Assign(lexicalScopeVar, 
                        ExpHelper.LexicalScopeBuilder.NewScope(
                            FileNameExpression,
                            fxName,
                            point.Line,
                            point.Column
                            )),
                    Exp.TryFinally(
                        block,
                        ExpHelper.IDisposableBuilder.Dispose(lexicalScopeVar)));

                var lambda = functionDeclaration.Generator 
                    ? Exp.Lambda(typeof(JSGeneratorDelegate), lexicalScope, cs.Generator, cs.Arguments)
                    : Exp.Lambda(typeof(JSFunctionDelegate), lexicalScope, cs.Arguments);


                // create new JSFunction instance...
                var jfs = functionDeclaration.Generator 
                    ? JSGeneratorFunctionBuilder.New(lambda, fxName, code)
                    : JSFunctionBuilder.New(lambda, fxName, code, functionDeclaration.Params.Count);

                if (!(functionDeclaration is Esprima.Ast.FunctionDeclaration))
                {
                    if (jsFVarScope != null)
                    {
                        jsFVarScope.SetInit(jfs);
                        return jsFVarScope.Expression;
                    }
                    return jfs;
                }
                jsFVarScope.SetInit(jfs);
                return jsFVarScope.Expression;
            }
        }

        private Exp DebugExpression<T, TR>(T ast, Func<TR> exp)
            where T: INode
            where TR: Exp
        {
            // return exp();
            var s = this.scope.Top.Scope;
            var p = ast.Location.Start;
            try
            {
                return Exp.Block(
                    LexicalScopeBuilder.SetPosition(s, p.Line, p.Column),
                    exp());
            }
            catch (Exception ex) when (!(ex is CompilerException))
            {
                throw new CompilerException($"Failed to parse at {p.Line},{p.Column} {ex}", ex);
            }
        }

        protected override Exp VisitStatement(Statement statement)
        {
            return DebugExpression(statement, () => base.VisitStatement(statement));
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

                var test = Exp.Not( ExpHelper.JSValueBuilder.BooleanValue(VisitExpression(whileStatement.Test)));

                list.Add(Exp.IfThen(test, Exp.Goto(breakTarget)));
                list.Add(body);

                return Exp.Loop(
                    Exp.Block(list), 
                    breakTarget, 
                    continueTarget);
            }
        }

        private List<VariableScope> CreateVariableDeclaration(Esprima.Ast.VariableDeclaration variableDeclaration)
        {
            // lets add variable...
            // forget about const... compiler like typescript should take care of it...
            // let will be implemented in future...
            var inits = new List<VariableScope>();
            bool newScope = variableDeclaration.Kind == VariableDeclarationKind.Let
                || variableDeclaration.Kind == VariableDeclarationKind.Const;
            foreach (var declarator in variableDeclaration.Declarations)
            {
                
                switch (declarator.Id)
                {
                    case Esprima.Ast.Identifier id:
                        var ve = this.scope.Top.CreateVariable(id.Name, declarator.Init != null
                            ? ExpHelper.JSVariableBuilder.New(VisitExpression(declarator.Init), id.Name)
                            : null, newScope);
                        inits.Add(ve);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            return inits;
        }

        private Exp CreateMemberExpression(Exp target, Expression property, bool computed)
        {
            switch (property)
            {
                case Identifier id:
                    if (!computed)
                    {
                        return ExpHelper.JSValueBuilder.Index(
                            target,
                            KeyOfName(id.Name));
                    }
                    return ExpHelper.JSValueBuilder.Index(
                        target,
                        VisitIdentifier(id));
                case Literal l
                    when l.TokenType == Esprima.TokenType.BooleanLiteral:
                    return ExpHelper.JSValueBuilder.Index(
                        target,
                        l.BooleanValue ? (uint)0 : (uint)1);
                case Literal l
                    when l.TokenType == Esprima.TokenType.StringLiteral:
                    return ExpHelper.JSValueBuilder.Index(
                        target,
                        KeyOfName(l.StringValue));
                case Literal l
                    when l.TokenType == Esprima.TokenType.NumericLiteral
                        && l.NumericValue >= 0 && (l.NumericValue % 1 == 0):
                    return ExpHelper.JSValueBuilder.Index(
                        target,
                        (uint)l.NumericValue);
                case StaticMemberExpression se:
                    return JSValueBuilder.Index(target, VisitExpression(se.Property));

            }
            if (computed)
            {
                return JSValueBuilder.Index(target, VisitExpression(property));
            }

            throw new NotImplementedException();
        }

        private Exp CreateAssignment(IArrayPatternElement pattern, Exp init, bool createVariable = false) {
            Exp target;
            List<Exp> inits;
            switch (pattern)
            {
                case Identifier id:
                    if (createVariable)
                    {
                        var v = this.scope.Top.CreateVariable(id.Name);
                        target = v.Expression;
                    } else
                    {
                        target = this.VisitIdentifier(id);
                    }
                    return Exp.Assign(target, init);
                case ObjectPattern objectPattern:
                    inits = new List<Exp>();
                    foreach(var prop in objectPattern.Properties)
                    {
                        Exp start = null;
                        switch (prop)
                        {
                            case Property property:
                                switch (property.Key)
                                {
                                    case Identifier id:
                                        start = CreateMemberExpression(init, id, property.Computed);
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                                switch(property.Value)
                                {
                                    case Identifier vid:
                                        inits.Add(CreateAssignment(vid, start, true));
                                        break;
                                    case IArrayPatternElement vp:
                                        inits.Add(CreateAssignment(vp, start, true));
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                } 
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    return Exp.Block(inits);
                case ArrayPattern arrayPattern:
                    inits = new List<Exp>();
                    int index = -1;
                    foreach(var element in arrayPattern.Elements)
                    {
                        index++;
                        Exp start = null;
                        switch (element)
                        {
                            case Identifier id:
                                start = JSValueBuilder.Index(init, (uint)index);
                                inits.Add(CreateAssignment(id, start));
                                break;
                        }
                    }
                    return Exp.Block(inits);
            }
            throw new NotImplementedException();
        }


        protected override Exp VisitVariableDeclaration(Esprima.Ast.VariableDeclaration variableDeclaration)
        {
            // lets add variable...
            // forget about const... compiler like typescript should take care of it...
            // let will be implemented in future...
            var inits = new List<Exp>();
            bool newScope = variableDeclaration.Kind == VariableDeclarationKind.Let
                || variableDeclaration.Kind == VariableDeclarationKind.Const;
            VariableScope temp;

            foreach (var declarator in variableDeclaration.Declarations)
            {
                switch(declarator.Id)
                {
                    case Esprima.Ast.Identifier id:
                        // variable might exist in current scope
                        // do not create and just set a value here...
                        var ve = this.scope.Top.CreateVariable(id.Name, null, newScope);
                        if (declarator.Init != null)
                        {
                            var init = ExpHelper.JSVariableBuilder.New(VisitExpression(declarator.Init), id.Name);
                            inits.Add(Exp.Assign(ve.Variable, init));
                        } else
                        {
                            inits.Add(Exp.Assign(ve.Variable, JSVariableBuilder.New(id.Name)));
                        }
                        break;
                    case Esprima.Ast.ObjectPattern objectPattern:
                        // it will always have an init...
                        // put init in temp...
                        temp = this.scope.Top.GetTempVariable();
                        inits.Add(Exp.Assign(temp.Variable, VisitExpression(declarator.Init)));
                        inits.Add(CreateAssignment(objectPattern, temp.Expression, true));
                        break;
                    case Esprima.Ast.ArrayPattern arrayPattern:
                        // it will always have an init...
                        // put init in temp...
                        temp = this.scope.Top.GetTempVariable();
                        inits.Add(Exp.Assign(temp.Variable, VisitExpression(declarator.Init)));
                        inits.Add(CreateAssignment(arrayPattern, temp.Expression, true));
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
                var pe = scope.Top.CreateException(id.Name);
                var v = scope.Top.CreateVariable(id.Name);
                
                var catchBlock = Exp.Block(new ParameterExpression[] { v.Variable},
                    Exp.Assign(v.Variable, ExpHelper.JSVariableBuilder.NewFromException(pe.Variable, id.Name)),
                    VisitBlockStatement(cb.Body));
                var cbExp = Exp.Catch(pe.Variable, catchBlock.ToJSValue());


                if (tryStatement.Finalizer != null)
                {
                    return Exp.TryCatchFinally(block.ToJSValue(), VisitStatement(tryStatement.Finalizer).ToJSValue(), cbExp);
                }

                return Exp.TryCatch(block.ToJSValue(), cbExp);
            }

            var @finally = tryStatement.Finalizer;
            if (@finally != null)
            {
                return Exp.TryFinally(block.ToJSValue(), VisitStatement(@finally).ToJSValue());
            }

            return JSUndefinedBuilder.Value;
        }

        protected override Exp VisitThrowStatement(Esprima.Ast.ThrowStatement throwStatement)
        {
            return ExpHelper.JSExceptionBuilder.Throw(VisitExpression(throwStatement.Argument));
        }

        class SwitchInfo
        {
            public List<Exp> Tests = new List<Exp>();
            public Exp Body;
        }

        protected override Exp VisitSwitchStatement(Esprima.Ast.SwitchStatement switchStatement)
        {
            Exp d = null;
            var @continue = this.scope.Top.Loop?.Top?.Continue;
            var @break = Exp.Label();
            var ls = new LoopScope(@break ,@continue, true);
            List<SwitchInfo> cases = new List<SwitchInfo>();
            using (var bt = this.scope.Top.Loop.Push(ls))
            {
                SwitchInfo lastCase = new SwitchInfo();
                foreach (var c in switchStatement.Cases)
                {
                    List<Exp> body = new List<Exp>();
                    foreach (var es in c.Consequent)
                    {
                        switch (es)
                        {
                            case Esprima.Ast.Statement stmt:
                                body.Add(VisitStatement(stmt));
                                break;
                            case Esprima.Ast.Expression exp:
                                body.Add(VisitExpression(exp));
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
                    }

                    if (c.Test == null)
                    {
                        d = Exp.Block(body);
                        lastCase = new SwitchInfo();
                        continue;
                    }

                    var test = VisitExpression(c.Test);
                    lastCase.Tests.Add(test);

                    if (body.Count > 0)
                    {
                        cases.Add(lastCase);
                        lastCase.Body = Exp.Block(body);
                        lastCase = new SwitchInfo();
                    }
                }
            }
            var r = Exp.Block(
                Exp.Switch(
                    VisitExpression(switchStatement.Discriminant), 
                    d.ToJSValue() ?? JSUndefinedBuilder.Value , 
                    ExpHelper.JSValueBuilder.StaticEquals, 
                    cases.Select(x => Exp.SwitchCase(x.Body.ToJSValue(), x.Tests) ).ToList()),
                Exp.Label(@break));
            return r;
        }

        protected override Exp VisitSwitchCase(Esprima.Ast.SwitchCase switchCase)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitReturnStatement(Esprima.Ast.ReturnStatement returnStatement)
        {
            return Exp.Return( this.scope.Top.ReturnLabel, 
                returnStatement.Argument != null 
                ? VisitExpression(returnStatement.Argument)
                : JSUndefinedBuilder.Value);
        }

        protected override Exp VisitLabeledStatement(Esprima.Ast.LabeledStatement labeledStatement)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitIfStatement(Esprima.Ast.IfStatement ifStatement)
        {
            var test =  ExpHelper.JSValueBuilder.BooleanValue(VisitExpression(ifStatement.Test));
            var trueCase = VisitStatement(ifStatement.Consequent);
            // process else...
            if (!typeof(JSValue).IsAssignableFrom(trueCase.Type))
            {
                trueCase = Exp.Block(trueCase, JSUndefinedBuilder.Value);
            }
            if (ifStatement.Alternate != null)
            {
                var elseCase = VisitStatement(ifStatement.Alternate);
                if (!typeof(JSValue).IsAssignableFrom(elseCase.Type))
                {
                    elseCase = Exp.Block(elseCase, JSUndefinedBuilder.Value);
                }
                return Exp.Condition(test, trueCase, elseCase);
            }
            return Exp.Condition(test, trueCase, ExpHelper.JSUndefinedBuilder.Value);
        }

        protected override Exp VisitEmptyStatement(Esprima.Ast.EmptyStatement emptyStatement)
        {
            return ExpHelper.JSUndefinedBuilder.Value;
        }

        protected override Exp VisitDebuggerStatement(Esprima.Ast.DebuggerStatement debuggerStatement)
        {
            return ExpHelper.JSDebuggerBuilder.RaiseBreak();
        }

        protected override Exp VisitExpressionStatement(Esprima.Ast.ExpressionStatement expressionStatement)
        {
            return VisitExpression(expressionStatement.Expression);
        }

        protected override Exp VisitExpression(Expression expression)
        {
            var p = expression.Location.Start ;
            try
            {
                return base.VisitExpression(expression);
            }catch (Exception ex) when (!(ex is CompilerException))
            {
                throw new CompilerException($"Failed to parse at {p.Line},{p.Column}\r\n{ex}", ex);
            }
        }

        protected override Exp VisitForStatement(Esprima.Ast.ForStatement forStatement)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            using (var s = scope.Top.Loop.Push(new LoopScope(breakTarget, continueTarget)))
            {
                return NewLexicalScope(new FunctionScope(this.scope.Top), forStatement, () =>
                {
                    var init = JSUndefinedBuilder.Value;
                    if (forStatement.Init != null)
                    {
                        switch (forStatement.Init)
                        {
                            case Expression exp:
                                init = VisitExpression(exp);
                                break;
                            case Statement stmt:
                                init = VisitStatement(stmt);
                                break;
                            default:
                                throw new NotSupportedException();
                        }
                    }
                    var list = new List<Exp>();
                    var body = VisitStatement(forStatement.Body);
                    var update = forStatement.Update == null ? null : VisitExpression(forStatement.Update);
                    if (forStatement.Test != null)
                    {
                        var test = Exp.Not(ExpHelper.JSValueBuilder.BooleanValue(VisitExpression(forStatement.Test)));

                        list.Add(Exp.IfThen(test, Exp.Goto(breakTarget)));
                    }
                    list.Add(body);
                    if (update != null)
                    {
                        list.Add(update);
                    }
                    return Exp.Block(init, Exp.Loop(
                        Exp.Block(list),
                        breakTarget,
                        continueTarget));
                });
            }
        }

        protected override Exp VisitForInStatement(Esprima.Ast.ForInStatement forInStatement)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            using (var s = scope.Top.Loop.Push(new LoopScope(breakTarget, continueTarget)))
            {
                return NewLexicalScope(new FunctionScope(this.scope.Top), forInStatement, () => {

                    var en = Exp.Variable(typeof(IEnumerator<JSValue>));


                    var pList = new List<ParameterExpression>() {
                        en
                    };

                    var body = VisitStatement(forInStatement.Body);

                    Exp identifier = null;

                    var list = new List<Exp>();
                    switch (forInStatement.Left)
                    {
                        case Identifier i:
                            identifier = VisitIdentifier(i);
                            break;
                        case VariableDeclaration vd:
                            var vdList = CreateVariableDeclaration(vd);
                            identifier = vdList.First().Expression;
                            break;
                    }

                    var sList = new List<Exp>
                    {
                        Exp.IfThen(Exp.Not(EnumerableBuilder.MoveNext(en)), Exp.Goto(s.Break)),
                        Exp.Assign(identifier, EnumerableBuilder.Current(en)),
                        // sList.Add(Exp.Assign(identifier, EnumerableBuilder.Current(en)));
                        body
                    };

                    var bodyList = Exp.Block(sList);

                    var right = VisitExpression(forInStatement.Right);
                    return Exp.Block(
                        pList,
                        Exp.Assign(en, JSValueBuilder.GetAllKeys(right)),
                        Exp.Loop(bodyList, s.Break, s.Continue)
                        );
                });
            }
        }

        protected override Exp VisitDoWhileStatement(Esprima.Ast.DoWhileStatement doWhileStatement)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            using (var s = scope.Top.Loop.Push(new LoopScope(breakTarget, continueTarget)))
            {

                var body = VisitStatement(doWhileStatement.Body);

                var list = new List<Exp>();

                var test = Exp.Not( JSValueBuilder.BooleanValue( VisitExpression(doWhileStatement.Test)));

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
            return ExpHelper.JSValueBuilder.DoubleValue(VisitExpression(exp));
        }

        private Exp BooleanValue(Esprima.Ast.Expression exp)
        {
            return ExpHelper.JSValueBuilder.BooleanValue(VisitExpression(exp));
        }


        protected override Exp VisitUnaryExpression(Esprima.Ast.UnaryExpression unaryExpression)
        {
            var target = unaryExpression.Argument;

            switch (unaryExpression.Operator)
            {
                case UnaryOperator.Plus:
                    return ExpHelper.JSNumberBuilder.New(Exp.UnaryPlus(DoubleValue(target)));
                case UnaryOperator.Minus:
                    switch(target)
                    {
                        case Literal l when l.TokenType == TokenType.NumericLiteral:
                            return JSNumberBuilder.New(Exp.Constant(-l.NumericValue));
                    }
                    return ExpHelper.JSNumberBuilder.New(Exp.Negate(DoubleValue(target)));
                case UnaryOperator.BitwiseNot:
                    return ExpHelper.JSNumberBuilder.New(Exp.Not( Exp.Convert(DoubleValue(target),typeof(int))));
                case UnaryOperator.LogicalNot:
                    return Exp.Condition(BooleanValue(target), JSBooleanBuilder.False, JSBooleanBuilder.True );
                case UnaryOperator.Delete:
                    // delete expression...
                    var me = target as Esprima.Ast.MemberExpression;
                    var targetObj = VisitExpression(me.Object);
                    if (me.Computed)
                    {
                        Exp pe = VisitExpression(me.Property);
                        return JSValueBuilder.Delete(targetObj, pe);
                    } else
                    {
                        switch (me.Property)
                        {
                            case Literal l when l.TokenType == TokenType.NumericLiteral:
                                return JSValueBuilder.Delete(targetObj, Exp.Constant((uint)l.NumericValue));
                            case Literal l1 when l1.TokenType == TokenType.StringLiteral:
                                return JSValueBuilder.Delete(targetObj, KeyOfName(l1.StringValue));
                            case Identifier id:
                                return JSValueBuilder.Delete(targetObj, KeyOfName(id.Name));
                        }
                    }
                    break;
                case UnaryOperator.Void:
                    return ExpHelper.JSUndefinedBuilder.Value;
                case UnaryOperator.TypeOf:
                    return ExpHelper.JSValueBuilder.TypeOf(VisitExpression(target));
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
            var right = VisitExpression(updateExpression.Argument);
            var ve = Exp.Variable(typeof(JSValue));
            if (updateExpression.Prefix) { 
                if (updateExpression.Operator == UnaryOperator.Increment)
                {
                    return Exp.Block(new ParameterExpression[] { ve },
                        JSValueExtensionsBuilder.Assign(right, ExpHelper.JSNumberBuilder.New(Exp.Add(DoubleValue(updateExpression.Argument), Exp.Constant((double)1)))),
                        JSValueExtensionsBuilder.Assign(ve, right));
                }
                return Exp.Block(new ParameterExpression[] { ve },
                    JSValueExtensionsBuilder.Assign(right, ExpHelper.JSNumberBuilder.New(Exp.Subtract(DoubleValue(updateExpression.Argument), Exp.Constant((double)1)))),
                    JSValueExtensionsBuilder.Assign(ve, right));
            }
            if (updateExpression.Operator == UnaryOperator.Increment)
            {
                return Exp.Block(new ParameterExpression[] { ve },
                    JSValueExtensionsBuilder.Assign(ve,right),
                    JSValueExtensionsBuilder.Assign(right , ExpHelper.JSNumberBuilder.New(Exp.Add(DoubleValue(updateExpression.Argument), Exp.Constant((double)1)))),
                    ve);
            }
            return Exp.Block(new ParameterExpression[] { ve },
                JSValueExtensionsBuilder.Assign(ve, right),
                JSValueExtensionsBuilder.Assign(right, ExpHelper.JSNumberBuilder.New(Exp.Subtract(DoubleValue(updateExpression.Argument), Exp.Constant((double)1)))),
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
            var list = sequenceExpression.Expressions.Select(x => VisitExpression(x)).ToList();
            return Exp.Block(list);
        }


        protected override Exp VisitObjectExpression(Esprima.Ast.ObjectExpression objectExpression)
        {
            var keys = new List<ExpressionHolder>();
            var properties = new Dictionary<string, ExpressionHolder>();
            foreach(Property p in objectExpression.Properties)
            {
                Exp key = null;
                Exp value = null;
                string name = null;
                switch (p.Key)
                {
                    case Identifier id
                        when !p.Computed:
                        key = KeyOfName(id.Name);
                        name = id.Name;
                        break;
                    case Literal l when l.TokenType == TokenType.StringLiteral:
                        key = KeyOfName(l.StringValue);
                        name = l.StringValue;
                        break;
                    case Literal l when l.TokenType == TokenType.NumericLiteral:
                        key = Exp.Constant((uint)l.NumericValue);
                        break;
                    default:
                        throw new NotSupportedException();
                }
                if (p.Shorthand)
                {
                    value = this.scope.Top[name];
                }
                else
                {
                    value = VisitExpression((Expression)p.Value);
                }
                if (p.Kind == PropertyKind.Get || p.Kind == PropertyKind.Set)
                {
                    if (!properties.TryGetValue(name, out var m))
                    {
                        m = new ExpressionHolder {
                            Key = key,
                            Getter = Exp.Constant(null, typeof(JSFunction)),
                            Setter = Exp.Constant(null, typeof(JSFunction))
                        };
                        properties[name] = m;
                        keys.Add(m);
                    }
                    if (p.Kind == PropertyKind.Get)
                    {
                        m.Getter = value;
                    }
                    else
                    {
                        m.Setter = value;
                    }
                    // m.Value = ExpHelper.JSPropertyBuilder.Property(key, m.Getter, m.Setter);
                    continue;
                }
                else
                {
                    // value = ExpHelper.JSPropertyBuilder.Value(key, value);
                    keys.Add(new ExpressionHolder { 
                        Key = key,
                        Value = value
                    });
                }
                // keys.Add(new ExpressionHolder { Value = value });
            }

            return ExpHelper.JSObjectBuilder.New(keys);
        }

        protected override Exp VisitNewExpression(Esprima.Ast.NewExpression newExpression)
        {
            var constructor = VisitExpression(newExpression.Callee);
            var args = newExpression.Arguments.Select(e => VisitExpression((Esprima.Ast.Expression)e)).ToList();
            var pe = ArgumentsBuilder.New( JSUndefinedBuilder.Value, args);
            return ExpHelper.JSValueBuilder.CreateInstance(constructor, pe);
        }

        protected override Exp VisitMemberExpression(Esprima.Ast.MemberExpression memberExpression)
        {
            switch (memberExpression.Property)
            {
                case Identifier id:
                    if (!memberExpression.Computed)
                    {
                        return ExpHelper.JSValueBuilder.Index(
                            VisitExpression(memberExpression.Object),
                            KeyOfName(id.Name));
                    }
                    return ExpHelper.JSValueBuilder.Index(
                        VisitExpression(memberExpression.Object),
                        VisitIdentifier(id));
                case Literal l
                    when l.TokenType == Esprima.TokenType.BooleanLiteral:
                    return ExpHelper.JSValueBuilder.Index(
                        VisitExpression(memberExpression.Object),
                        l.BooleanValue ? (uint)0 : (uint)1);
                case Literal l
                    when l.TokenType == Esprima.TokenType.StringLiteral:
                    return ExpHelper.JSValueBuilder.Index(
                        VisitExpression(memberExpression.Object),
                        KeyOfName(l.StringValue));
                case Literal l
                    when l.TokenType == Esprima.TokenType.NumericLiteral 
                        && l.NumericValue >= 0 && (l.NumericValue % 1 == 0):
                    return ExpHelper.JSValueBuilder.Index(
                        VisitExpression(memberExpression.Object),
                        (uint)l.NumericValue);
                case StaticMemberExpression se:
                    return JSValueBuilder.Index( VisitExpression(memberExpression.Object),VisitExpression(se.Property));

            }
            if (memberExpression.Computed)
            {
                return JSValueBuilder.Index(VisitExpression(memberExpression.Object), VisitExpression(memberExpression.Property));
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
                            ? (ExpHelper.JSBooleanBuilder.True, "true")
                            : (ExpHelper.JSBooleanBuilder.False, "false");
                    case Esprima.TokenType.StringLiteral:
                        return (ExpHelper.JSStringBuilder.New(Exp.Constant(literal.StringValue)), literal.StringValue.Left(5));
                    case Esprima.TokenType.RegularExpression:
                        return (ExpHelper.JSRegExpBuilder.New(
                            Exp.Constant(literal.Regex.Pattern),
                            Exp.Constant(literal.Regex.Flags)), (literal.Regex.Pattern + literal.Regex.Flags).Left(10));
                    case Esprima.TokenType.Template:
                        break;
                    case Esprima.TokenType.NullLiteral:
                        return (ExpHelper.JSNullBuilder.Value, "null");
                    case Esprima.TokenType.NumericLiteral:
                        return (ExpHelper.JSNumberBuilder.New(Exp.Constant(literal.NumericValue)), literal.NumericValue.ToString());
                }
                throw new NotImplementedException();
            }
            // var (exp, name) = GetLiteral();
            // var pe = Exp.Variable(typeof(JSValue), name);
            // this.scope.Top.AddVariable(null, pe, pe, exp);
            // return pe;
            return GetLiteral().exp;
            
        }

        protected override Exp VisitIdentifier(Esprima.Ast.Identifier identifier)
        {
            // if this is null, fetch from global...
            if (identifier.Name == "arguments")
            {
                var functionScope = this.scope.Top.TopScope;
                var vs = functionScope.CreateVariable("arguments",
                    JSArgumentsBuilder.New(functionScope.ArgumentsExpression));
                return vs.Expression;
            }

            var local = this.scope.Top[identifier.Name];
            if (local != null)
                return local;
            return ExpHelper.JSContextBuilder.Index(KeyOfName(identifier.Name));
        }

        protected override Exp VisitFunctionExpression(Esprima.Ast.IFunction function)
        {
            var a = CreateFunction(function);
            // var pe = Exp.Parameter(typeof(JSValue));
            // this.scope.Top.CreateVariable(null, pe, pe, a);
            return a;
        }

        protected override Exp VisitClassExpression(Esprima.Ast.ClassExpression classExpression)
        {
            return CreateClass(classExpression.Id, classExpression.SuperClass, classExpression.Body);
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
            return CreateClass(classDeclaration.Id, classDeclaration.SuperClass, classDeclaration.Body);
        }

        protected override Exp VisitClassBody(Esprima.Ast.ClassBody classBody)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitYieldExpression(Esprima.Ast.YieldExpression yieldExpression)
        {
            if (yieldExpression.Delegate)
            {
                return JSGeneratorBuilder.Delegate(this.scope.Top.Generator, VisitExpression(yieldExpression.Argument));
            }
            return JSGeneratorBuilder.Yield(this.scope.Top.Generator, VisitExpression(yieldExpression.Argument));
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
            switch (assignmentPattern.Left)
            {
                case ObjectPattern objectPattern:
                    return CreateAssignment(objectPattern, VisitExpression(assignmentPattern.Right as Expression ));
                case ArrayPattern arrayPattern:
                    return CreateAssignment(arrayPattern, VisitExpression(assignmentPattern.Right as Expression));
            }
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
            var quasis = new List<string>();
            foreach(var quasi in templateLiteral.Quasis)
            {
                quasis.Add(quasi.Value.Raw);
            }
            return JSTemplateStringBuilder.New(quasis, templateLiteral.Expressions.Select(x => VisitExpression(x)));
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
                ExpHelper.JSValueBuilder.BooleanValue(test),
                @true,
                @false, typeof(JSValue));
        }

        protected override Exp VisitCallExpression(Esprima.Ast.CallExpression callExpression)
        {
            var calle = callExpression.Callee;
            var args = callExpression.Arguments.Select((e) => VisitExpression((Esprima.Ast.Expression)e)).ToList();
            
            if (calle is Esprima.Ast.MemberExpression me)
            {
                // invoke method...

                // get object...
                var obj = VisitExpression(me.Object);

                Exp name;

                switch(me.Property)
                {
                    case Identifier id:
                        name = KeyOfName(id.Name);
                        break;
                    case Literal l when l.TokenType == TokenType.StringLiteral:
                        name = KeyOfName(l.StringValue);
                        break;
                    case Literal l1 when l1.TokenType == TokenType.NumericLiteral:
                        name = Exp.Constant((uint)l1.NumericValue);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                // var id = me.Property.As<Esprima.Ast.Identifier>();


                // var name = KeyOfName(id.Name);
                var paramArray = args.Any()
                    ? ArgumentsBuilder.New(obj, args)
                    : ArgumentsBuilder.Empty();

                return JSValueBuilder.InvokeMethod(obj, name, paramArray);

            } else {
                var paramArray = args.Any()
                    ? ArgumentsBuilder.New(JSUndefinedBuilder.Value, args)
                    : ArgumentsBuilder.Empty();
                var callee = VisitExpression(callExpression.Callee);
                return DebugExpression( callExpression, () => JSFunctionBuilder.InvokeFunction(callee, paramArray));
            }
        }

        protected override Exp VisitBinaryExpression(Esprima.Ast.BinaryExpression binaryExpression)
        {
            var left = VisitExpression(binaryExpression.Left);
            var right = VisitExpression(binaryExpression.Right);
            switch(binaryExpression.Operator)
            {
                case BinaryOperator.Plus:
                    return ExpHelper.JSValueBuilder.Add(left, right);
            }
            var a = BinaryOperation.Operation(left, right, binaryExpression.Operator);
            return a;
        }

        protected override Exp VisitArrayExpression(Esprima.Ast.ArrayExpression arrayExpression)
        {
            List<Exp> list = new List<Exp>();
            foreach(var e in arrayExpression.Elements)
            {
                switch(e)
                {
                    case Expression exp:
                        list.Add(VisitExpression(exp));
                        break;
                    case null:
                        list.Add(Exp.Constant(null, typeof(JSValue)));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return ExpHelper.JSArrayBuilder.New(list);
        }

        protected override Exp VisitAssignmentExpression(Esprima.Ast.AssignmentExpression assignmentExpression)
        {
            // simple identifier based assignments or 
            // array index based assignments...

            var left = VisitExpression((Esprima.Ast.Expression)assignmentExpression.Left);
            var right = VisitExpression(assignmentExpression.Right);


            var a = BinaryOperation.Assign(left, right, assignmentExpression.Operator);
            return a;
        }

        protected override Exp VisitContinueStatement(Esprima.Ast.ContinueStatement continueStatement)
        {
            return Exp.Continue(this.scope.Top.Loop.Top.Continue);
        }

        protected override Exp VisitBreakStatement(Esprima.Ast.BreakStatement breakStatement)
        {
            var ls = this.LoopScope;
            if (ls.IsSwitch)
                return Exp.Goto(ls.Break);
            return Exp.Break(ls.Break);
        }

        private Exp NewLexicalScope(FunctionScope fnScope, Node exp, Func<Exp> factory)
        {
            using(var scope = this.scope.Push(fnScope))
            {
                var position = exp.Location.Start;

                // collect variables...
                var vList = new List<ParameterExpression>() { scope.Scope };

                var visited = factory();

                vList.AddRange(scope.VariableParameters);

                return Exp.Block(vList, Exp.TryFinally(
                    Exp.Block(
                        Exp.Assign(scope.Scope, 
                            ExpHelper.LexicalScopeBuilder.NewScope(
                                FileNameExpression, scope.Function?.Id?.Name ?? "", position.Line, position.Column)),
                        visited
                        ).ToJSValue()
                    , IDisposableBuilder.Dispose(scope.Scope)));
            }
        }

        private Exp VisitStatements(in NodeList<IStatementListItem> body)
        {
            return Exp.Block(body.Select(x => VisitStatement((Statement)x)));
        }

        protected override Exp VisitBlockStatement(Esprima.Ast.BlockStatement blockStatement)
        {
            return this.NewLexicalScope(new FunctionScope(this.scope.Top), 
                blockStatement , () => VisitStatements(blockStatement.Body));
        }

        private Exp CreateBlock(in NodeList<IStatementListItem> body) {
            var items = new List<IStatementListItem>();
            foreach (var stmt in body.ToList())
            {
                if (stmt is FunctionDeclaration fx && !string.IsNullOrEmpty(fx.Id?.Name))
                {
                    var name = fx.Id.Name;
                    this.scope.Top.CreateVariable(name);
                }
                items.Add(stmt);
            }

            var visitedList = body.Select(a => VisitStatement((Statement)a)).ToList();

            if (visitedList.Any())
            {
                return Exp.Block(visitedList);
            }
            return JSUndefinedBuilder.Value;
        }
    }

    public class ExpressionHolder
    {
        public Exp Key;
        public Exp Value;
        public Exp Getter;
        public Exp Setter;
    }

}
