using Esprima;
using Esprima.Ast;
// using FastExpressionCompiler;
using Microsoft.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YantraJS.Core;
using YantraJS.Core.CodeGen;
using YantraJS.Core.Generator;
using YantraJS.Core.LinqExpressions;
using YantraJS.Core.LinqExpressions.Logical;
using YantraJS.Emit;
using YantraJS.ExpHelper;
using YantraJS.Extensions;
using YantraJS.LinqExpressions;
using YantraJS.Parser;
using YantraJS.Utils;
using static YantraJS.FunctionScope;
using Exp = System.Linq.Expressions.Expression;
using ParameterExpression = System.Linq.Expressions.ParameterExpression;

namespace YantraJS
{

    public class CoreScript: JSAstVisitor<Exp>
    {
        public System.Linq.Expressions.Expression<JSFunctionDelegate> Method { get; }

        readonly LinkedStack<FunctionScope> scope = new LinkedStack<FunctionScope>();

        public LoopScope LoopScope => this.scope.Top.Loop.Top;

        public Exp FileNameExpression => ScriptInfoBuilder.FileName(this.scope.Top.ScriptInfo);
        public Exp CodeStringExpression => ScriptInfoBuilder.Code(this.scope.Top.ScriptInfo);

        // private ParsedScript Code;

        // readonly string Code;

        private StringArray _keyStrings = new StringArray();


        public Exp KeyOfName(string name)
        {
            // search for variable...
            var fx = typeof(KeyStrings).GetField(name);
            if (fx != null)
            {
                return Exp.Field(null, fx);
            }

            var i = _keyStrings.GetOrAdd(name);
            return ScriptInfoBuilder.KeyString(this.scope.Top.ScriptInfo,(int)i);
        }

        internal static JSFunctionDelegate Compile(in StringSpan code, string location = null, IList<string> args = null, ICodeCache codeCache = null)
        {
            codeCache = codeCache ?? DictionaryCodeCache.Current;
            var script = code;
            var jsc = new JSCode(location, code, args, () => {
                var cc = new CoreScript(script, location, args, codeCache);
                return cc.Method;
            });
            return codeCache.GetOrCreate(in jsc);
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


        public static JSValue Evaluate(string code, string location = null, ICodeCache codeCache = null)
        {
            var fx = Compile(code, location, null, codeCache);
            var result = JSUndefined.Value;
            var ctx = JSContext.Current;
            result = fx(new Arguments(ctx));
            return result;
        }

        public CoreScript(in StringSpan code, string location = null, IList<string> argsList = null, ICodeCache codeCache = null)
        {
            // this.Code = code;
            location = location ?? "vm.js";

            // FileNameExpression = Exp.Variable(typeof(string), "_fileName");
            // CodeStringExpression = Exp.Variable(typeof(string), "code");


            // this.Code = new ParsedScript(code);
            Esprima.JavaScriptParser parser =
                new Esprima.JavaScriptParser(code.Value, new Esprima.ParserOptions {
                    Range = true,
                    Loc = true,
                    // SourceType = SourceType.Script
                });

            // add top level...

            using (var fx = this.scope.Push(new FunctionScope((IFunction)null)))
            {

                var jScript = parser.ParseScript();

                var lScope = fx.Context;

                ScopeAnalyzer scopeAnalyzer = new ScopeAnalyzer();
                scopeAnalyzer.Visit(jScript);

                if(jScript.HoistingScope != null && argsList != null)
                {
                    foreach (var a in argsList)
                    {
                        jScript.HoistingScope.Remove(a);
                    }
                }

                var scriptInfo = fx.ScriptInfo;

                var te = fx.ThisExpression;

                var args = fx.ArgumentsExpression;

                var stackItem = fx.StackItem;

                var vList = new List<ParameterExpression>() {
                    scriptInfo,
                    lScope,
                    stackItem
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

                var sList = new List<Exp>() {
                    Exp.Assign(scriptInfo, ScriptInfoBuilder.New(location,code.Value)),
                    Exp.Assign(lScope, JSContextBuilder.Current)
                };

                JSContextStackBuilder.Push(sList, lScope, stackItem, Exp.Constant(location), StringSpanBuilder.Empty, 0, 0);

                sList.Add(ScriptInfoBuilder.Build(scriptInfo, _keyStrings));

                // ref var keyStrings = ref _keyStrings;
                //foreach (var ks in keyStrings.AllValues())
                //{
                //    var v = ks.Value;
                //    vList.Add(v);
                //    sList.Add(Exp.Assign(v, ExpHelper.KeyStringsBuilder.GetOrCreate(Exp.Constant(ks.Key))));
                //}

                vList.AddRange(fx.VariableParameters);
                sList.AddRange(fx.InitList);
                // register globals..
                foreach (var v in fx.Variables)
                {
                    if (v.Variable != null && v.Variable.Type == typeof(JSVariable))
                    {
                        if (argsList?.Contains(v.Name) ?? false)
                            continue;
                        sList.Add(JSContextBuilder.Register(lScope, v.Variable));
                    }
                }


                sList.Add(Exp.Return(l, script.ToJSValue()));
                sList.Add(Exp.Label(l, JSUndefinedBuilder.Value));

                //script = Exp.Block(vList,
                //    Exp.TryFinally(
                //        Exp.Block(sList),
                //        ExpHelper.IDisposableBuilder.Dispose(lScope))
                //);
                //var catchExp = Exp.Parameter(typeof(Exception));
                //vList.Add(catchExp);

                //var catchWithFilter = Exp.Catch(
                //    catchExp,
                //    Exp.Throw(JSExceptionBuilder.From(catchExp), typeof(JSValue)),
                //    Exp.Not(Exp.TypeIs(catchExp, typeof(JSException))));
                //script = Exp.Block(vList,
                //    Exp.TryCatchFinally(
                //        Exp.Block(sList),
                //        JSContextStackBuilder.Pop(stackItem),
                //        catchWithFilter)

                // sList.Add(JSContextStackBuilder.Pop(stackItem));

                script = Exp.Block(vList, sList);
                

                var lambda = Exp.Lambda<JSFunctionDelegate>(script, fx.Arguments);

                this.Method = lambda;
            }
        }

        protected override Exp VisitProgram(Esprima.Ast.Program program)
        {
            if (program.HoistingScope != null)
            {
                var top = this.scope.Top;
                foreach(var v in program.HoistingScope)
                {
                    // fetch global if exists..
                    var g = JSValueBuilder.Index(top.Context, KeyOfName(v));

                    // these are global variables...
                    var vs = this.scope.Top.CreateVariable(v);
                    vs.Expression = JSVariableBuilder.Property(vs.Variable);
                    vs.SetInit(JSVariableBuilder.New(g, v));
                }
            }
            return CreateBlock(in program.Body);
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
            Exp superExp;
            if (super != null)
            {
                superExp = VisitExpression(super);
            } else
            {
                superExp = JSContextBuilder.Object;
            }

            Exp constructor = null;
            Dictionary<string, ExpressionHolder> cache = new Dictionary<string, ExpressionHolder>();
            List<ExpressionHolder> members = new List<ExpressionHolder>();
            ExpressionHolder expHolder;

            var superVar = Exp.Parameter(typeof(JSFunction));
            var superPrototypeVar = Exp.Parameter(typeof(JSObject));

            List<Exp> stmts = new List<Exp>
            {
                Exp.Assign(superVar, Exp.TypeAs(superExp, typeof(JSFunction))),
                Exp.Assign(superPrototypeVar, JSFunctionBuilder.Prototype(superVar))
            };

            Exp retValue = null;

            foreach (var property in body.Body)
            {
                var name = property.Key.As<Identifier>()?.Name;
                var method = property as MethodDefinition;
                switch (property.Kind)
                {
                    case PropertyKind.Get:
                        if(!cache.TryGetValue(name, out expHolder))
                        {
                            expHolder = new ExpressionHolder() { 
                                Key = KeyOfName(name)
                            };
                            cache[name] = expHolder;
                            members.Add(expHolder);
                            expHolder.Static = method.Static;
                        }
                        expHolder.Getter = CreateFunction(property.Value as IFunction, superPrototypeVar);
                        break;
                    case PropertyKind.Set:
                        if (!cache.TryGetValue(name, out expHolder))
                        {
                            expHolder = new ExpressionHolder() {
                                Key = KeyOfName(name)
                            };
                            cache[name] = expHolder;
                            members.Add(expHolder);
                            expHolder.Static = method.Static;
                        }
                        expHolder.Setter = CreateFunction(property.Value as IFunction, superPrototypeVar);
                        break;
                    case PropertyKind.Constructor:
                        retValue = CreateFunction(property.Value as IFunction, superVar, true, id?.Name);
                        break;
                    case PropertyKind.Method:
                        members.Add(new ExpressionHolder()
                        {
                            Key = KeyOfName(name),
                            Value = CreateFunction(property.Value as IFunction, superPrototypeVar),
                            Static = method.Static
                        });
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            retValue = retValue ?? JSClassBuilder.New( this.scope.Top.ScriptInfo, null, constructor, superVar, id?.Name ?? "Unnamed");
            foreach(var exp in members)
            {
                if(exp.Value != null)
                {
                    retValue = exp.Static
                        ? JSClassBuilder.AddStaticValue(retValue, exp.Key, exp.Value)
                        : JSClassBuilder.AddValue(retValue, exp.Key, exp.Value);
                    continue;
                }
                retValue = exp.Static 
                    ? JSClassBuilder.AddStaticProperty(retValue, exp.Key, exp.Getter, exp.Setter)
                    : JSClassBuilder.AddProperty(retValue, exp.Key, exp.Getter, exp.Setter);
            }
            // stmts.Add(retValue);

            if (id?.Name != null)
            {
                var v = this.scope.Top.CreateVariable(id.Name);
                stmts.Add(Exp.Assign(v.Expression, retValue));
            }else
            {
                stmts.Add(retValue);
            }

            return Exp.Block(new ParameterExpression[] { superVar, superPrototypeVar }, stmts);
        }

        private Exp CreateFunction(
            Esprima.Ast.IFunction functionDeclaration,
            Exp super = null,
            bool createClass = false,
            string className = null
            )
        {
            var node = functionDeclaration as Node;
            var codeExp = CodeStringExpression;
            var code = StringSpanBuilder.New(codeExp, node.Range.Start, 
                node.Range.End - node.Range.Start);

            // get text...

            var previousScope = this.scope.Top;

            // if this is an arrowFunction then override previous thisExperssion

            var previousThis = this.scope.Top.ThisExpression;
            if (!(functionDeclaration is ArrowFunctionExpression))
            {
                previousThis = null;
            }

            var functionName = functionDeclaration.Id?.Name;

            var parentScriptInfo = this.scope.Top.ScriptInfo;


            using (var cs = scope.Push(new FunctionScope(functionDeclaration, previousThis, super)))
            {
                var lexicalScopeVar = cs.Context;


                FunctionScope.VariableScope jsFVarScope = null;

                if (functionName != null)
                {
                    jsFVarScope = previousScope.GetVariable(functionName);

                }

                var s = cs;
                // use this to create variables...
                var t = s.ThisExpression;
                var args = s.ArgumentsExpression;
                var stackItem = s.StackItem;

                var r = s.ReturnLabel;

                var sList = new List<Exp>();

                Exp fxName;
                if (functionName != null)
                {
                    var id = functionDeclaration.Id;
                    fxName = StringSpanBuilder.New(codeExp, id.Range.Start, id.Range.End - id.Range.Start);
                }
                else
                {
                    fxName = StringSpanBuilder.Empty;
                }

                var functionStatement = functionDeclaration as Node;

                var point = functionStatement.Location.Start; // this.Code.Position(functionDeclaration.Range);

                JSContextStackBuilder.Push(sList, s.Context, stackItem, FileNameExpression, fxName, point.Line, point.Column);

                var vList = new List<ParameterExpression>();

                // var pList = functionDeclaration.Params.OfType<Identifier>();
                int i = 0;

                var argumentElements = args;

                List<Exp> bodyInits = new List<Exp>();

                foreach (var v in functionDeclaration.Params)
                {
                    switch (v.Type)
                    {
                        case Nodes.Identifier:
                            Identifier id = v as Identifier;
                            s.CreateVariable(id.Name,
                                ExpHelper.JSVariableBuilder.FromArgument(argumentElements, i, id.Name));
                            break;
                        case Nodes.AssignmentPattern:
                            AssignmentPattern ap = v as AssignmentPattern;
                            var inits = CreateAssignment(
                                ap.Left,
                                ExpHelper.JSVariableBuilder.FromArgumentOptional(argumentElements, i, VisitExpression(ap.Right)), 
                                true, 
                                true);
                            bodyInits.Add(inits);
                            break;
                        case Nodes.RestElement:
                            var re = v as RestElement;
                            id = re.Argument as Identifier;
                            inits = CreateAssignment(re.Argument,
                                ArgumentsBuilder.RestFrom(argumentElements, (uint)i)
                                , true,
                                true);
                            bodyInits.Add(inits);
                            break;
                        //case ArrayPattern aap:
                        //    bodyInits.Add(CreateAssignment(v, argumentElements, true, true));
                        //    break;
                        default:
                            bodyInits.Add(CreateAssignment(v, ArgumentsBuilder.GetAt(argumentElements, i), true, true));
                            break;
                        //case AssignmentPattern asp:
                        //    break;
                        //case ObjectPattern op:
                        //    break;
                        //case ArrayPattern ap:
                        //    break;
                    }
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

                vList.AddRange(s.VariableParameters);
                sList.AddRange(s.InitList);

                sList.AddRange(bodyInits);

                sList.Add(lambdaBody);
                // sList.Add(JSContextStackBuilder.Pop(stackItem));
                sList.Add(Exp.Label(r, ExpHelper.JSUndefinedBuilder.Value));
                // sList.Add();

                var block = Exp.Block(vList, sList);


                // adding lexical scope pending...



                var lexicalScope =
                    Exp.Block(new ParameterExpression[] { lexicalScopeVar, stackItem },
                    Exp.Assign(lexicalScopeVar,
                        JSContextBuilder.Current),
                    //Exp.Assign(stackItem, 
                    //    JSContextBuilder.Push(
                    //        lexicalScopeVar,
                    //        FileNameExpression,
                    //        fxName,
                    //        point.Line,
                    //        point.Column
                    //        )),
                    Exp.TryFinally(
                        block
                         ,JSContextStackBuilder.Pop(stackItem, lexicalScopeVar))
                    );

                Exp closureArray = Exp.Constant(null, typeof(JSVariable[]));
                if (cs.ClosureList != null)
                {
                    closureArray = Exp.NewArrayInit(typeof(JSVariable), cs.ClosureList.Select(x => x.Variable));
                }

                Exp scriptInfo = parentScriptInfo;

                System.Linq.Expressions.LambdaExpression lambda;
                Exp jsf;
                if (functionDeclaration.Generator)
                {
                    lambda = Exp.Lambda(typeof(JSGeneratorDelegate), lexicalScope, cs.ScriptInfo, cs.Closures,  cs.Generator, cs.Arguments);
                    jsf = JSGeneratorFunctionBuilder.New(parentScriptInfo, closureArray, lambda, fxName, code);
                } else if (functionDeclaration.Async)
                {
                    lambda = Exp.Lambda(typeof(JSAsyncDelegate), lexicalScope, cs.ScriptInfo, cs.Closures, cs.Awaiter, cs.Arguments);
                    jsf = JSAsyncFunctionBuilder.New(parentScriptInfo, closureArray, lambda, fxName, code);
                } else
                {
                    lambda = Exp.Lambda(typeof(JSClosureFunctionDelegate), lexicalScope, cs.ScriptInfo, cs.Closures, cs.Arguments);
                    if (createClass)
                    {
                        jsf = JSClassBuilder.New(parentScriptInfo, closureArray, lambda, super, className ?? "Unnamed");
                    } else
                    {
                        jsf = JSClosureFunctionBuilder.New(parentScriptInfo, closureArray, lambda, fxName, code, functionDeclaration.Params.Count);
                    }
                }

                //// create new JSFunction instance...
                //var jfs = functionDeclaration.Generator 
                //    ? JSGeneratorFunctionBuilder.New(lambda, fxName, code)
                //    : ( createClass 
                //        ? JSClassBuilder.New(lambda, super, className ?? "Unnamed")
                //        : JSFunctionBuilder.New(lambda, fxName, code, functionDeclaration.Params.Count));

                if (!(functionDeclaration is Esprima.Ast.FunctionDeclaration))
                {
                    if (jsFVarScope != null)
                    {
                        jsFVarScope.SetPostInit(jsf);
                        return jsFVarScope.Expression;
                    }
                    return jsf;
                }
                if (jsFVarScope != null)
                {
                    jsFVarScope.SetPostInit(jsf);
                    return jsFVarScope.Expression;
                }
                return jsf;
            }
        }

        public override Exp DebugNode(Node node, Exp result)
        {
            var topStack = this.scope.Top;
            var si = topStack.StackItem;
            var p = node.Location.Start;
            return JSContextStackBuilder.Update(si, p.Line, p.Column,
                    result);
        }

        private Exp DebugExpression<T, TR>(T ast, Func<TR> exp)
            where T: Node
            where TR: Exp
        {
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    return exp();
            //}
            var topStack = this.scope.Top;
            var s = topStack.Context;
            var si = topStack.StackItem;
            var p = ast.Location.Start;
            try
            {
                return JSContextStackBuilder.Update(si, p.Line, p.Column,
                    exp());
            }
            catch (Exception ex) when (!(ex is CompilerException))
            {
                throw new CompilerException($"Failed to parse at {p.Line},{p.Column} {ex}", ex);
            }
        }

        //protected override Exp VisitStatement(Statement statement)
        //{
        //    return DebugExpression(statement, () => base.VisitStatement(statement));
        //}

        protected override Exp VisitWithStatement(Esprima.Ast.WithStatement withStatement)
        {
            // we will not support with
            throw new NotSupportedException("With statement is not supported");
        }

        protected override Exp VisitWhileStatement(Esprima.Ast.WhileStatement whileStatement, string label = null)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            using (var s = scope.Top.Loop.Push(new LoopScope(breakTarget, continueTarget, false, label)))
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

        #region Not Used
        //private List<VariableScope> CreateVariableDeclaration(Esprima.Ast.VariableDeclaration variableDeclaration)
        //{
        //    // lets add variable...
        //    // forget about const... compiler like typescript should take care of it...
        //    // let will be implemented in future...
        //    var inits = new List<VariableScope>();
        //    bool newScope = variableDeclaration.Kind == VariableDeclarationKind.Let
        //        || variableDeclaration.Kind == VariableDeclarationKind.Const;
        //    foreach (var declarator in variableDeclaration.Declarations)
        //    {

        //        switch (declarator.Id)
        //        {
        //            case Esprima.Ast.Identifier id:
        //                var ve = this.scope.Top.CreateVariable(id.Name, declarator.Init != null
        //                    ? ExpHelper.JSVariableBuilder.New(VisitExpression(declarator.Init), id.Name)
        //                    : null, newScope);
        //                inits.Add(ve);
        //                break;
        //            default:
        //                throw new NotSupportedException();
        //        }
        //    }
        //    return inits;
        //}
        #endregion

        private Exp CreateMemberExpression(Exp target, Expression property, bool computed)
        {
            switch (property.Type)
            {
                case Nodes.Identifier:
                    Identifier id = property as Identifier;
                    if (!computed)
                    {
                        return ExpHelper.JSValueBuilder.Index(
                            target,
                            KeyOfName(id.Name));
                    }
                    return ExpHelper.JSValueBuilder.Index(
                        target,
                        VisitIdentifier(id));
                case Nodes.Literal:
                    Literal l = property as Literal;
                    switch (l.TokenType)
                    {
                        case Esprima.TokenType.BooleanLiteral:
                            return ExpHelper.JSValueBuilder.Index(
                                target,
                                l.BooleanValue ? (uint)0 : (uint)1);

                        case Esprima.TokenType.StringLiteral:
                            return ExpHelper.JSValueBuilder.Index(
                                target,
                                    KeyOfName(l.StringValue));
                        case Esprima.TokenType.NumericLiteral:
                            if (l.NumericValue >= 0 && (l.NumericValue % 1 == 0))
                                return ExpHelper.JSValueBuilder.Index(
                                    target,
                                    (uint)l.NumericValue);
                            break;
                    }
                    break;
                case Nodes.MemberExpression:
                    StaticMemberExpression se = property as StaticMemberExpression;
                    return JSValueBuilder.Index(target, VisitExpression(se.Property));

            }
            if (computed)
            {
                return JSValueBuilder.Index(target, VisitExpression(property));
            }

            throw new NotImplementedException();
        }

        private Exp CreateAssignment(
            Expression pattern, 
            Exp init, 
            bool createVariable = false,
            bool newScope = false) {
            Exp target;
            List<Exp> inits;
            switch (pattern)
            {
                case Identifier id:
                    inits = new List<Exp>();
                    if (createVariable)
                    {
                        var v = this.scope.Top.CreateVariable(id.Name, null, newScope);
                        inits.Add(Exp.Assign(v.Variable, JSVariableBuilder.New(id.Name)));
                        target = v.Expression;
                    } else
                    {
                        target = this.VisitIdentifier(id);
                    }
                    inits.Add(Exp.Assign(target, init));
                    return Exp.Block(inits);
                case ObjectPattern objectPattern:
                    inits = new List<Exp>();
                    foreach(var prop in objectPattern.Properties)
                    {
                        Exp start = null;
                        switch (prop)
                        {
                            case Property property:
                                switch (property.Key.Type)
                                {
                                    case Nodes.Identifier:
                                        Identifier id = property.Key as Identifier;
                                        start = CreateMemberExpression(init, id, property.Computed);
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                                switch(property.Value.Type)
                                {
                                    case Nodes.Identifier:
                                        Identifier vid = property.Value as Identifier;
                                        inits.Add(CreateAssignment(vid, start, true, newScope));
                                        break;
                                    case Nodes.ArrayPattern:
                                    case Nodes.ObjectPattern:
                                        BindingPattern vp = property.Value as BindingPattern;
                                        inits.Add(CreateAssignment(vp, start, true, newScope));
                                        break;
                                    // TODO
                                    case Nodes.AssignmentPattern:
                                        AssignmentPattern ap = property.Value as AssignmentPattern;
                                        inits.Add(CreateAssignment(ap.Left, 
                                            Exp.Coalesce(
                                                JSValueExtensionsBuilder.NullIfUndefined(start),
                                                VisitExpression(ap.Right))
                                        ));
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
                    using (var enVar = this.scope.Top.GetTempVariable(typeof(IElementEnumerator)))
                    {
                        var en = enVar.Expression;
                        using (var item = this.scope.Top.GetTempVariable())
                        {
                            inits.Add(Exp.Assign(en, IElementEnumeratorBuilder.Get(init)));
                            foreach (var element in arrayPattern.Elements)
                            {
                                switch (element)
                                {
                                    case Identifier id:
                                        // inits.Add(CreateAssignment(id, start));
                                        if (createVariable)
                                        {
                                            var v = this.scope.Top.CreateVariable(id.Name, null, newScope);
                                            inits.Add(Exp.Assign(v.Variable, JSVariableBuilder.New(id.Name)));
                                        }
                                        var assignee = VisitIdentifier(id);
                                        inits.Add(IElementEnumeratorBuilder.AssignMoveNext(assignee, en, 
                                            item.Expression));
                                        break;
                                    case RestElement spe:
                                        // loop...
                                        if (createVariable && spe.Argument is Identifier id2) {
                                            var v = this.scope.Top.CreateVariable(id2.Name, null, newScope);
                                            inits.Add(Exp.Assign(v.Variable, JSVariableBuilder.New(id2.Name)));
                                        } 
                                        
                                        var spid = VisitExpression(spe.Argument as Expression);
                                        
                                        using (var arrayVar = this.scope.Top.GetTempVariable(typeof(JSArray)))
                                        {
                                            inits.Add(Exp.Assign(arrayVar.Expression, JSArrayBuilder.New()));
                                            var @break = Exp.Label();
                                            var add = JSArrayBuilder.Add(arrayVar.Expression, item.Expression);
                                            var @breakStmt = Exp.Goto(@break);
                                            var loop = Exp.Loop(Exp.Block(
                                                Exp.IfThenElse(
                                                    IElementEnumeratorBuilder.MoveNext(en, item.Expression),
                                                    add,
                                                    breakStmt)
                                                ), @break);
                                            inits.Add(loop);
                                            inits.Add(Exp.Assign(spid, arrayVar.Expression));
                                        }
                                        break;
                                    case BindingPattern ape:
                                        // nested array ...
                                        // nested object ...
                                        var check = IElementEnumeratorBuilder.MoveNext(en, item.Expression);
                                        inits.Add(check);
                                        inits.Add(CreateAssignment(ape, item.Expression, true, newScope));
                                        break;
                                    default:
                                        inits.Add(IElementEnumeratorBuilder.MoveNext(en, item.Expression));
                                        break;
                                }
                            }
                        }
                    }
                    return Exp.Block(inits);
            }
            throw new NotImplementedException();
        }

        protected override Exp VisitVariableDeclaration(VariableDeclaration variableDeclaration)
        {
            var v = new ScopedVariableDeclaration(variableDeclaration);
            foreach (var vd in v.Declarators)
            {
                if (vd.Declarator.Init != null)
                {
                    vd.Init = VisitExpression(vd.Declarator.Init);
                }
            }
            var list = VisitVariableDeclaration(v);
            return Exp.Block(list);
        }

        // for loop will require pulling variables on the top
        // for updae and test...

        // Run variable declaration twice !!? Try it..
        protected List<Exp> VisitVariableDeclaration(
            ScopedVariableDeclaration variableDeclaration)
        {
            // lets add variable...
            // forget about const... compiler like typescript should take care of it...
            // let will be implemented in future...
            var inits = new List<Exp>();
            bool newScope = variableDeclaration.NewScope;

            foreach (var sDeclarator in variableDeclaration.Declarators)
            {
                var declarator = sDeclarator.Declarator;
                Exp dInit = null;
                if (sDeclarator.Init != null)
                {
                    dInit = sDeclarator.Init;
                }
                switch(declarator.Id.Type)
                {
                    case Nodes.Identifier:
                        Esprima.Ast.Identifier id = declarator.Id as Identifier;
                        // variable might exist in current scope
                        // do not create and just set a value here...
                        var ve = this.scope.Top.CreateVariable(id.Name, null, newScope);
                        if (dInit != null)
                        {
                            // var init = ExpHelper.JSVariableBuilder.New(dInit, id.Name);
                            inits.Add(Exp.Assign(ve.Variable, Exp.Coalesce(ve.Variable, JSVariableBuilder.New(id.Name))));
                            inits.Add(Exp.Assign(ve.Expression, dInit));
                        } else
                        {
                            inits.Add(Exp.Assign(ve.Variable, Exp.Coalesce(ve.Variable, JSVariableBuilder.New(id.Name))));
                        }
                        break;
                    case Nodes.ObjectPattern:
                        Esprima.Ast.ObjectPattern objectPattern = declarator.Id as ObjectPattern;
                        // it will always have an init...
                        // put init in temp...
                        using (var temp = this.scope.Top.GetTempVariable())
                        {
                            inits.Add(Exp.Assign(temp.Variable, dInit));
                            inits.Add(CreateAssignment(objectPattern, temp.Expression, true, newScope));
                        }
                        break;
                    case Nodes.ArrayPattern:
                        Esprima.Ast.ArrayPattern arrayPattern = declarator.Id as ArrayPattern;
                        // it will always have an init...
                        // put init in temp...
                        using (var temp = this.scope.Top.GetTempVariable())
                        {
                            inits.Add(Exp.Assign(temp.Variable, dInit));
                            inits.Add(CreateAssignment(arrayPattern, temp.Expression, true, newScope));
                        }
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            return inits;
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
            public List<Exp> Body;
            public readonly System.Linq.Expressions.LabelTarget Label = Exp.Label("case-start");
        }

        protected override Exp VisitSwitchStatement(Esprima.Ast.SwitchStatement switchStatement)
        {

            bool allStrings = true;
            bool allNumbers = true;
            bool allIntegers = true;


            List<Exp> defBody = null;
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
                            //case Esprima.Ast.Expression exp:
                            //    body.Add(VisitExpression(exp));
                            //    break;
                            default:
                                throw new InvalidOperationException();
                        }
                    }

                    if (c.Test == null)
                    {
                        defBody = body;
                        lastCase = new SwitchInfo();
                        continue;
                    }

                    Exp test = null;
                    switch ((c.Test.Type, c.Test))
                    {
                        case (Nodes.Literal, Literal literal):

                            switch (literal.TokenType)
                            {
                                case TokenType.StringLiteral:
                                    allNumbers = false;
                                    allStrings = false;
                                    test = Exp.Constant(literal.StringValue);
                                    break;
                                case TokenType.NumericLiteral:
                                    var n = literal.NumericValue;
                                    if ((n % 1) != 0)
                                    {
                                        allIntegers = false;
                                    }
                                    test = Exp.Constant(literal.NumericValue);
                                    break;
                                case TokenType.BooleanLiteral:
                                    allNumbers = false;
                                    allStrings = false;
                                    allIntegers = false;
                                    test = literal.BooleanValue ? JSBooleanBuilder.True : JSBooleanBuilder.False;
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }

                            break;
                        default:
                            test = VisitExpression(c.Test);
                            allNumbers = false;
                            allStrings = false;
                            allIntegers = false;
                            break;
                    }
                    lastCase.Tests.Add(test);

                    if (body.Count > 0)
                    {
                        cases.Add(lastCase);
                        body.Insert(0, Exp.Label(lastCase.Label));
                        lastCase.Body = body;
                        lastCase = new SwitchInfo();
                    }
                }

                System.Reflection.MethodInfo equalsMethod = null;

                SwitchInfo last = null;
                foreach (var @case in cases)
                {
                    // if last one is not break statement... make it fall through...
                    if (last != null)
                    {
                        last.Body.Add(Exp.Goto(@case.Label));
                    }
                    last = @case;

                    if (allNumbers)
                    {
                        if (allIntegers)
                        {
                            @case.Tests = @case.Tests.ConvertToInteger();
                        }
                        else
                        {
                            // convert every case to double..
                            @case.Tests = @case.Tests.ConvertToNumber();
                        }
                    }
                    else
                    {
                        if (allStrings)
                        {
                            // force everything to string if it isn't
                            @case.Tests = @case.Tests.ConvertToString();
                        }
                        else
                        {
                            @case.Tests = @case.Tests.ConvertToJSValue();
                            equalsMethod = ExpHelper.JSValueBuilder.StaticEquals;
                        }
                    }


                }

                var testTarget = VisitExpression(switchStatement.Discriminant);
                if (allNumbers)
                {
                    if (allIntegers)
                    {
                        testTarget = Exp.Convert(JSValueBuilder.DoubleValue(testTarget), typeof(int));
                    }
                    else
                    {
                        testTarget = JSValueBuilder.DoubleValue(testTarget);
                    }
                }
                else
                {
                    if (allStrings)
                    {
                        testTarget = ObjectBuilder.ToString(testTarget);
                    }
                    else
                    {

                    }
                }

                Exp d = null;
                var lastLine = switchStatement.Location.Start.Line;
                if (defBody != null)
                {
                    var defLabel = Exp.Label($"default-start-{lastLine}");
                    if (last != null)
                    {
                        last.Body.Add(Exp.Goto(defLabel));
                    }
                    defBody.Insert(0, Exp.Label(defLabel));
                    d = Exp.Block(defBody);
                }

                var r = Exp.Block(
                    Exp.Switch(
                        testTarget,
                        d.ToJSValue() ?? JSUndefinedBuilder.Value,
                        equalsMethod,
                        cases.Select(x => Exp.SwitchCase(Exp.Block(x.Body).ToJSValue(), x.Tests)).ToList()),
                    Exp.Label(@break));
                return r;
            }
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
            switch(labeledStatement.Body.Type)
            {
                case Nodes.ForStatement:
                    return VisitForStatement(labeledStatement.Body as ForStatement, labeledStatement.Label.Name);
                case Nodes.ForOfStatement:
                    return VisitForOfStatement(labeledStatement.Body as ForOfStatement, labeledStatement.Label.Name);
                case Nodes.ForInStatement:
                    return VisitForInStatement(labeledStatement.Body as ForInStatement, labeledStatement.Label.Name);
                case Nodes.WhileStatement:
                    return VisitWhileStatement(labeledStatement.Body as WhileStatement, labeledStatement.Label.Name);
                case Nodes.DoWhileStatement:
                    return VisitDoWhileStatement(labeledStatement.Body as DoWhileStatement, labeledStatement.Label.Name);
                default:
                    throw JSContext.Current.NewSyntaxError($"Label can only be used for loops");
            }
            throw new NotImplementedException();
        }

        protected override Exp VisitIfStatement(Esprima.Ast.IfStatement ifStatement)
        {
            var test =  ExpHelper.JSValueBuilder.BooleanValue(VisitExpression(ifStatement.Test));
            var trueCase = VisitStatement(ifStatement.Consequent).ToJSValue();
            if (ifStatement.Alternate != null)
            {
                var elseCase = VisitStatement(ifStatement.Alternate).ToJSValue();
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

        //protected override Exp VisitExpression(Expression expression)
        //{
        //    var p = expression.Location.Start ;
        //    try
        //    {
        //        return base.VisitExpression(expression);
        //    }catch (Exception ex) when (!(ex is CompilerException))
        //    {
        //        throw new CompilerException($"Failed to parse at {p.Line},{p.Column}\r\n{ex}", ex);
        //    }
        //}

        protected override Exp VisitForStatement(Esprima.Ast.ForStatement forStatement, string label)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            ScopedVariableDeclaration varDec = null;
            var paramList = new List<ParameterExpression>();
            var blockList = new List<Exp>();
            var init = JSUndefinedBuilder.Value;

            using (var s = scope.Top.Loop.Push(new LoopScope(breakTarget, continueTarget, false, label)))
            {
                var forInit = forStatement.Init;
                if (forInit != null)
                {
                    switch(forInit)
                    {
                        case VariableDeclaration dec:
                            varDec = new ScopedVariableDeclaration(dec) {
                                Copy = true
                            };
                            this.scope.Top.PushToNewScope = varDec;

                            foreach(var vd in varDec.Declarators)
                            {
                                if(vd.Declarator.Init != null)
                                {
                                    vd.Init = VisitExpression(vd.Declarator.Init);
                                }
                            }
                            break;
                        case Expression exp:
                            init = VisitExpression(exp);
                            blockList.Add(init);
                            break;
                        case Statement stmt:
                            init = VisitStatement(stmt);
                            blockList.Add(init);
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }

                return NewLexicalScope(new FunctionScope(this.scope.Top), forStatement, () =>
                {


                    // we need one more copy of same variables if they were declared with "let"
                    if (varDec !=null && varDec.NewScope)
                    {

                        

                        var scopedDeclarator = new List<ScopedVariableDeclarator>();
                        foreach (var v in this.scope.Top.Variables) {
                            scopedDeclarator.Add(new ScopedVariableDeclarator() {
                                Declarator = new VariableDeclarator(new Identifier(v.Name), null),
                                Init = v.Expression
                            });
                        }
                        if (scopedDeclarator.Count > 0)
                        {
                            varDec = new ScopedVariableDeclaration(scopedDeclarator);
                            this.scope.Top.PushToNewScope = varDec;
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
                    list.Add(Exp.Label(continueTarget));
                    if (update != null)
                    {
                        list.Add(update);
                    }
                    blockList.Add(Exp.Loop(Exp.Block(list), breakTarget));
                    return new List<Exp> { Exp.Block(paramList, blockList) };
                });
            }
        }

        protected override Exp VisitForInStatement(Esprima.Ast.ForInStatement forInStatement, string label = null)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            Exp identifier = null;
            ScopedVariableDeclaration varDec;
            switch (forInStatement.Left)
            {
                case Identifier id:
                    identifier = VisitIdentifier(id);
                    break;
                case VariableDeclaration vd:
                    // should only be one initializer...
                    identifier = Exp.Variable(typeof(JSValue));
                    varDec = new ScopedVariableDeclaration(vd, identifier);
                    this.scope.Top.PushToNewScope = varDec;
                    break;
            }
            using (var s = scope.Top.Loop.Push(new LoopScope(breakTarget, continueTarget, false, label)))
            {
                var en = Exp.Variable(typeof(IElementEnumerator));

                var pList = new List<ParameterExpression>() {
                    en
                };

                if (identifier is ParameterExpression pe)
                    pList.Add(pe);

                var body = VisitStatement(forInStatement.Body);

                var sList = new List<Exp>
                {
                    Exp.IfThen(Exp.Not(IElementEnumeratorBuilder.MoveNext(en, identifier)), Exp.Goto(s.Break)),
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
            }
        }

        protected override Exp VisitDoWhileStatement(Esprima.Ast.DoWhileStatement doWhileStatement, string label = null)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            using (var s = scope.Top.Loop.Push(new LoopScope(breakTarget, continueTarget, false, label)))
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
                    if(target.Type == Nodes.Literal)
                    {
                        Literal l = unaryExpression.Argument as Literal;
                        if (l.TokenType == TokenType.NumericLiteral)
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
                        var mep = me.Property;
                        switch (mep.Type)
                        {
                            case Nodes.Literal:
                                Literal l = mep as Literal;
                                if (l.TokenType == TokenType.NumericLiteral)
                                    return JSValueBuilder.Delete(targetObj, Exp.Constant((uint)l.NumericValue));
                                if(l.TokenType == TokenType.StringLiteral)
                                    return JSValueBuilder.Delete(targetObj, KeyOfName(l.StringValue));
                                break;
                            case Nodes.Identifier:
                                Identifier id = mep as Identifier;
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
            var list = Visit(in sequenceExpression.Expressions);
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
                var pKey = p.Key;
                switch (pKey.Type)
                {
                    case Nodes.Identifier:
                        Identifier id = pKey as Identifier;
                        if (!p.Computed)
                        {
                            key = KeyOfName(id.Name);
                            name = id.Name;
                        }
                        else {
                            key = this.scope.Top.GetVariable(id.Name).Expression;
                            name = id.Name;
                        }
                        break;
                    case Nodes.Literal:
                        Literal l = pKey as Literal;
                        if (l.TokenType == TokenType.StringLiteral)
                        {
                            key = KeyOfName(l.StringValue);
                            name = l.StringValue;
                        }
                        else if (l.TokenType == TokenType.NumericLiteral)
                        {
                            key = Exp.Constant((uint)l.NumericValue);
                        }
                        else
                            throw new NotSupportedException();
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
            var args = Visit(in newExpression.Arguments);
            var pe = ArgumentsBuilder.New(JSUndefinedBuilder.Value, args);
            return ExpHelper.JSValueBuilder.CreateInstance(constructor, pe);
        }

        protected override Exp VisitMemberExpression(Esprima.Ast.MemberExpression memberExpression)
        {
            var isSuper = memberExpression.Object is Super;
            var target = isSuper
                ? this.scope.Top.ThisExpression
                : VisitExpression(memberExpression.Object);
            var super = isSuper ? this.scope.Top.Super : null;
            var mp = memberExpression.Property;
            switch (mp.Type)
            {
                case Nodes.Identifier:
                    Identifier id = mp as Identifier;
                    if (!memberExpression.Computed)
                    {
                        return ExpHelper.JSValueBuilder.Index(
                            target,
                            super,
                            KeyOfName(id.Name));
                    }
                    return ExpHelper.JSValueBuilder.Index(
                        target,
                        super,
                        VisitIdentifier(id));
                case Nodes.Literal:
                    Literal l = mp as Literal;
                    if(l.TokenType == Esprima.TokenType.BooleanLiteral)
                        return ExpHelper.JSValueBuilder.Index(
                            target,
                            super,
                            l.BooleanValue ? (uint)0 : (uint)1);
                    if(l.TokenType == Esprima.TokenType.StringLiteral)
                        return ExpHelper.JSValueBuilder.Index(
                            target,
                            super,
                            KeyOfName(l.StringValue));
                    if(l.TokenType == Esprima.TokenType.NumericLiteral 
                        && l.NumericValue >= 0 && (l.NumericValue % 1 == 0))
                        return ExpHelper.JSValueBuilder.Index(
                            target,
                            super,
                            (uint)l.NumericValue);
                    break;
                case Nodes.MemberExpression:
                    MemberExpression se = mp as MemberExpression;
                    return JSValueBuilder.Index( target,super, VisitExpression(se.Property));

            }
            if (memberExpression.Computed)
            {
                return JSValueBuilder.Index(target, super, VisitExpression(memberExpression.Property));
            }
            throw new NotImplementedException();
        }

        protected override Exp VisitLogicalExpression(Esprima.Ast.BinaryExpression binaryExpression)
        {
            Exp right;
            Exp left;
            // if any of it is literal...
            //if(binaryExpression.Left.Type == Nodes.Literal)
            //{
            //    var literal = binaryExpression.Left as Literal;
            //    right = VisitExpression(binaryExpression.Right);
            //    switch(literal.TokenType)
            //    {
            //        case TokenType.BooleanLiteral:
            //            return LogicalBoolean.Build(
            //                literal.BooleanValue,
            //                binaryExpression.Operator,
            //                right);
            //        case TokenType.NumericLiteral:
            //            return LogicalNumeric.Build(
            //                literal.NumericValue,
            //                binaryExpression.Operator,
            //                right);
            //        case TokenType.StringLiteral:
            //            return LogicalString.Build(
            //                literal.StringValue,
            //                binaryExpression.Operator,
            //                right);
            //        case TokenType.NullLiteral:
            //            return LogicalNull.Build(
            //                binaryExpression.Operator,
            //                right);
            //    }
            //} else if (binaryExpression.Right.Type == Nodes.Literal)
            //{
            //    var literal = binaryExpression.Right as Literal;
            //    right = VisitExpression(binaryExpression.Left);
            //    switch (literal.TokenType)
            //    {
            //        case TokenType.BooleanLiteral:
            //            return LogicalBoolean.Build(
            //                literal.BooleanValue,
            //                binaryExpression.Operator,
            //                right);
            //        case TokenType.NumericLiteral:
            //            return LogicalNumeric.Build(
            //                literal.NumericValue,
            //                binaryExpression.Operator,
            //                right);
            //        case TokenType.StringLiteral:
            //            return LogicalString.Build(
            //                literal.StringValue,
            //                binaryExpression.Operator,
            //                right);
            //        case TokenType.NullLiteral:
            //            return LogicalNull.Build(
            //                binaryExpression.Operator,
            //                right);
            //    }

            //}


            left = VisitExpression(binaryExpression.Left);
            right = VisitExpression(binaryExpression.Right);
            return BinaryOperation.Operation(left, right, binaryExpression.Operator);
        }

        protected override Exp VisitLiteral(Esprima.Ast.Literal literal)
        {

            (Exp exp, string name) GetLiteral()
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

            if (identifier.Name == "undefined")
            {
                return JSUndefinedBuilder.Value;
            }

            var var = this.scope.Top.GetVariable(identifier.Name, true);
            if (var != null)
                return var.Expression;

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
            var exports = this.scope.Top.GetVariable("exports");
            var defExports = JSValueBuilder.Index(exports.Expression, KeyOfName("default"));
            return Exp.Assign(defExports, VisitStatement(exportDefaultDeclaration.Declaration as Statement));
        }

        protected override Exp VisitExportAllDeclaration(Esprima.Ast.ExportAllDeclaration exportAllDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override Exp VisitExportNamedDeclaration(Esprima.Ast.ExportNamedDeclaration exportNamedDeclaration)
        {
            var exports = this.scope.Top.GetVariable("exports");
            Exp left;
            var top = this.scope.Top;
            var ed = exportNamedDeclaration.Declaration;
            switch (ed.Type)
            {
                case Nodes.VariableDeclaration:
                    VariableDeclaration vd = ed as VariableDeclaration;
                    var sdd = new ScopedVariableDeclaration(vd);
                    var list = this.VisitVariableDeclaration(sdd);
                    foreach(var id in IdentifierExtractor.Names(vd))
                    {
                        left = JSValueBuilder.Index(exports.Expression, KeyOfName(id));
                        var right = top.GetVariable(id);
                        list.Add(Exp.Assign(left, right.Expression));
                    }
                    return Exp.Block(list);
                case Nodes.ClassDeclaration:
                    ClassDeclaration cd = ed as ClassDeclaration;
                    if (cd.Id != null)
                    {
                        left = JSValueBuilder.Index(exports.Expression, KeyOfName(cd.Id.Name));
                        return Exp.Assign(left, VisitClassDeclaration(cd));
                    }
                    break;
                case Nodes.FunctionDeclaration:
                    FunctionDeclaration fd = ed as FunctionDeclaration;
                    if (fd.Id != null)
                    {
                        left = JSValueBuilder.Index(exports.Expression, KeyOfName(fd.Id.Name));
                        return Exp.Assign(left, VisitFunctionDeclaration(fd));
                    }
                    break;

            }
            throw new NotSupportedException();
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
            // get require... on temp variable...
            var tempRequire = Exp.Parameter(typeof(JSValue));
            var require = this.scope.Top.GetVariable("require");
            var source = VisitExpression(importDeclaration.Source);
            var args = ArgumentsBuilder.New(JSUndefinedBuilder.Value, source);
            Exp prop;
            VariableScope imported;
            List<Exp> stmts = new List<Exp>() {
                Exp.Assign(tempRequire, JSFunctionBuilder.InvokeFunction(require.Expression, args) )
            };

            foreach (var d in importDeclaration.Specifiers)
            {
                switch (d.Type) {
                    case Nodes.ImportDefaultSpecifier:
                        ImportDefaultSpecifier ids = d as ImportDefaultSpecifier;
                        imported = this.scope.Top.CreateVariable(ids.Local.Name);
                        prop = JSValueBuilder.Index(tempRequire, KeyOfName("default"));
                        stmts.Add(Exp.Assign(imported.Expression, prop));
                        break;
                    case Nodes.ImportNamespaceSpecifier:
                        ImportNamespaceSpecifier ins = d as ImportNamespaceSpecifier;
                        imported = this.scope.Top.CreateVariable(ins.Local.Name);
                        stmts.Add(Exp.Assign(imported.Expression, tempRequire ));
                        break;
                    case Nodes.ImportSpecifier:
                        ImportSpecifier iss = d as ImportSpecifier;
                        imported = this.scope.Top.CreateVariable(iss.Local.Name);
                        prop = JSValueBuilder.Index(tempRequire, KeyOfName(iss.Imported.Name));
                        stmts.Add(Exp.Assign(imported.Expression, prop));
                        break;
                }            
            }
            return Exp.Block(
                new ParameterExpression[] { tempRequire }, 
                stmts);
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

        protected override Exp VisitForOfStatement(Esprima.Ast.ForOfStatement forOfStatement, string label = null)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            // ParameterExpression iterator = null;
            Exp identifier = null;
            ScopedVariableDeclaration varDec;
            var left = forOfStatement.Left;
            switch (left.Type)
            {
                case Nodes.VariableDeclaration:
                    VariableDeclaration vd = left as VariableDeclaration;
                    identifier = Exp.Variable(typeof(JSValue));
                    varDec = new ScopedVariableDeclaration(vd, identifier);
                    this.scope.Top.PushToNewScope = varDec;
                    break;
                case Nodes.Identifier:
                    Identifier id = left as Identifier;
                    identifier = VisitIdentifier(id);
                    break;
            }
            using (var s = scope.Top.Loop.Push(new LoopScope(breakTarget, continueTarget, false, label)))
            {                

                var en = Exp.Variable(typeof(IElementEnumerator));

                var pList = new List<ParameterExpression>() {
                    en,
                    identifier as ParameterExpression
                };

                var body = VisitStatement(forOfStatement.Body);

                var sList = new List<Exp>
                {
                    Exp.IfThen(
                        Exp.Not(
                            IElementEnumeratorBuilder.MoveNext(en, identifier)), 
                        Exp.Goto(s.Break)),
                    body
                };

                var bodyList = Exp.Block(sList);

                var right = VisitExpression(forOfStatement.Right);
                return Exp.Block(
                    pList,
                    Exp.Assign(en, IElementEnumeratorBuilder.Get(right)),
                    Exp.Loop(bodyList, s.Break, s.Continue)
                    );
            }
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
            var tag = taggedTemplateExpression.Tag;
            var args = new List<Expression>();
            var strings = new List<Expression>();
            foreach (var a in taggedTemplateExpression.Quasi.Quasis) {
                strings.Add(new Literal(a.Value.Raw));
            }
            args.Add(new ArrayExpression(new NodeList<Expression>(strings)));
            args.AddRange(taggedTemplateExpression.Quasi.Expressions);

            return VisitCallExpression(new CallExpression(tag, new NodeList<Expression>(args)), taggedTemplateExpression);
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
            var left = assignmentPattern.Left;
            switch (left.Type)
            {
                case Nodes.ObjectPattern:
                    ObjectPattern objectPattern = left as ObjectPattern;
                    return CreateAssignment(objectPattern, VisitExpression(assignmentPattern.Right as Expression ));
                case Nodes.ArrayPattern:
                    ArrayPattern arrayPattern = left as ArrayPattern;
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
            return JSTemplateStringBuilder.New(quasis, Visit(in templateLiteral.Expressions));
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
            return JSAwaiterBuilder.Await(this.scope.Top.Awaiter, VisitExpression(awaitExpression.Argument));
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

        protected override Exp VisitCallExpression(Esprima.Ast.CallExpression callExpression){
            return VisitCallExpression(callExpression, callExpression);
        }

        protected Exp VisitCallExpression(Esprima.Ast.CallExpression callExpression, Expression node)
        {
            var calle = callExpression.Callee;
            var args = new List<Exp>(callExpression.Arguments.Count);
            bool hasSpread = false;
            foreach (var a in callExpression.Arguments) {
                var ae = a as Expression;
                if (ae.Type != Nodes.SpreadElement) {
                    args.Add(VisitExpression(ae));
                    continue;
                }
                // spread....
                var sa = a as SpreadElement;
                args.Add(new ClrSpreadExpression(VisitExpression(sa.Argument)));
                hasSpread = true;
            }
            
            if (calle is Esprima.Ast.MemberExpression me)
            {
                // invoke method...


                Exp name;

                switch(me.Property.Type)
                {
                    case Nodes.Identifier:
                        var id = me.Property as Identifier;
                        name = me.Computed ? VisitExpression(id) : KeyOfName(id.Name);
                        // name = KeyOfName(id.Name);
                        break;
                    case Nodes.Literal:
                        var l = me.Property as Literal;
                        if (l.TokenType == TokenType.StringLiteral)
                            name = KeyOfName(l.StringValue);
                        else if (l.TokenType == TokenType.NumericLiteral)
                            name = Exp.Constant((uint)l.NumericValue);
                        else 
                            throw new NotImplementedException();
                        break;
                    case Nodes.MemberExpression:
                        name = VisitMemberExpression(me.Property as MemberExpression);
                        break;
                    default:
                        throw new NotImplementedException($"{me.Property}");
                }

                // var id = me.Property.As<Esprima.Ast.Identifier>();
                bool isSuper = me.Object is Super;
                var super = isSuper ? this.scope.Top.Super : null;
                var target = isSuper
                    ? this.scope.Top.ThisExpression
                    : VisitExpression(me.Object);

                // var name = KeyOfName(id.Name);
                var paramArray = args.Any()
                    ? ArgumentsBuilder.New(isSuper ? target : JSUndefinedBuilder.Value , args, hasSpread)
                    : ArgumentsBuilder.Empty();

                if(isSuper)
                {
                    var superMethod = JSValueBuilder.Index(super, name);
                    return JSFunctionBuilder.InvokeFunction(superMethod, paramArray);
                }

                return DebugNode( node, JSValueExtensionsBuilder.InvokeMethod(target, name, paramArray));

            } else {

                bool isSuper = callExpression.Callee is Super;

                if (isSuper)
                {
                    var paramArray1 = ArgumentsBuilder.New(this.scope.Top.ThisExpression, args, hasSpread);
                    var super = this.scope.Top.Super;
                    return JSFunctionBuilder.InvokeSuperConstructor(this.scope.Top.ThisExpression, super, paramArray1);
                }

                var paramArray = args.Any()
                    ? ArgumentsBuilder.New(JSUndefinedBuilder.Value, args, hasSpread)
                    : ArgumentsBuilder.Empty();
                var callee = VisitExpression(callExpression.Callee);
                return DebugNode( node, JSFunctionBuilder.InvokeFunction(callee, paramArray));
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
                if (e == null)
                {
                    list.Add(Exp.Constant(null, typeof(JSValue)));
                    continue;
                }
                list.Add(VisitExpression(e));
            }
            return ExpHelper.JSArrayBuilder.New(list);
        }

        protected override Exp VisitAssignmentExpression(Esprima.Ast.AssignmentExpression assignmentExpression)
        {
            // simple identifier based assignments or 
            // array index based assignments...

            var leftExp = assignmentExpression.Left as Esprima.Ast.Expression;

            switch(leftExp.Type)
            {
                case Nodes.ArrayPattern:
                case Nodes.ObjectPattern:
                    return CreateAssignment(leftExp, VisitExpression(assignmentExpression.Right));
            }

            var left = VisitExpression((Esprima.Ast.Expression)assignmentExpression.Left);
            var right = VisitExpression(assignmentExpression.Right);


            var a = BinaryOperation.Assign(left, right, assignmentExpression.Operator);
            return a;
        }

        protected override Exp VisitContinueStatement(Esprima.Ast.ContinueStatement continueStatement)
        {
            string name = continueStatement.Label?.Name;
            if (name != null)
            {
                var target = this.LoopScope.Get(name);
                if (target == null)
                    throw JSContext.Current.NewSyntaxError($"No label found for {name}");
                return Exp.Continue(target.Break);
            }
            return Exp.Continue(this.scope.Top.Loop.Top.Continue);
        }

        protected override Exp VisitBreakStatement(Esprima.Ast.BreakStatement breakStatement)
        {
            var ls = this.LoopScope;

            string name = breakStatement.Label?.Name;
            if(name != null)
            {
                var target = this.LoopScope.Get(name);
                if (target == null)
                    throw JSContext.Current.NewSyntaxError($"No label found for {name}");
                return Exp.Break(target.Break);
            }

            if (ls.IsSwitch)
                return Exp.Goto(ls.Break);

            return Exp.Break(ls.Break);
        }

        private Exp NewLexicalScope(
            FunctionScope fnScope, 
            Node exp, 
            Func<List<Exp>> factory)
        {
            var top = this.scope.Top;
            var varToPush = top.PushToNewScope;

            using(var scope = this.scope.Push(fnScope))
            {
                var position = exp.Location.Start;

                // collect variables...
                var vList = new List<ParameterExpression>() {};

                var stmtList = new List<Exp>() {
                    //Exp.Assign(scope.Scope,
                    //        ExpHelper.LexicalScopeBuilder.NewScope(
                    //            FileNameExpression, scope.Function?.Id?.Name ?? "", position.Line, position.Column))
                };

                // bool hasVarDeclarations = scope.IsFunctionScope || scope.Variables.Any();


                List<Exp> pushedInits = new List<Exp>();

                if (varToPush != null)
                {
                    var list = VisitVariableDeclaration(varToPush);
                    pushedInits.AddRange(list);
                    top.PushToNewScope = null;
                    // hasVarDeclarations = true;
                }

                var visited = factory();

                stmtList.AddRange(scope.InitList);
                vList.AddRange(scope.VariableParameters);
                stmtList.AddRange(pushedInits);
                stmtList.AddRange(visited);

                return Exp.Block(vList,stmtList).ToJSValue();
            }

        }

        protected override Exp VisitBlockStatement(Esprima.Ast.BlockStatement blockStatement)
        {
            return this.NewLexicalScope(new FunctionScope(this.scope.Top), 
                blockStatement , () =>
                {

                    if (blockStatement.HoistingScope != null)
                    {
                        foreach (var v in blockStatement.HoistingScope)
                        {
                            this.scope.Top.CreateVariable(v, JSVariableBuilder.New(v));
                        }
                    }
                    return VisitBody(blockStatement);
                });
        }

        private List<Exp> VisitBody(BlockStatement stmt)
        {
            return Visit(in stmt.Body);
        }

        private Exp CreateBlock(in NodeList<Statement> body) {
            var visitedList = Visit(in body);

            if (visitedList.Any())
            {
                return Exp.Block(visitedList);
            }
            return JSUndefinedBuilder.Value;
        }
    }

    public class ExpressionHolder
    {
        public bool Static;
        public Exp Key;
        public Exp Value;
        public Exp Getter;
        public Exp Setter;
    }

}
