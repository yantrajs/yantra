using System;
using System.Collections.Generic;
using YantraJS.Core.CodeGen;
using YantraJS.Core.LinqExpressions;
using YantraJS.Core.LinqExpressions.GeneratorsV2;
using YantraJS.Emit;
using YantraJS.ExpHelper;
using YantraJS.Expressions;
using YantraJS.Utils;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

namespace YantraJS.Core.FastParser.Compiler
{
    public partial class FastCompiler : AstMapVisitor<YExpression>
    {

        private readonly FastPool pool;

        readonly LinkedStack<FastFunctionScope> scope = new LinkedStack<FastFunctionScope>();
        private readonly string location;

        public LoopScope LoopScope => this.scope.Top.Loop.Top;

        private StringArray _keyStrings = new StringArray();

        // private FastList<object> _innerFunctions;

        private YParameterExpression scriptInfo;

        public YExpression<JSFunctionDelegate> Method { get; }

        public FastCompiler(
            in StringSpan code,
            string location = null,
            IList<string> argsList = null,
            ICodeCache codeCache = null) {
            this.pool = new FastPool();

            location = location ?? "vm.js";
            this.location = location;

            // FileNameExpression = Exp.Variable(typeof(string), "_fileName");
            // CodeStringExpression = Exp.Variable(typeof(string), "code");


            // this.Code = new ParsedScript(code);


            // _innerFunctions = pool.AllocateList<object>();

            // add top level...

            var parserPool = new FastPool();
            var parser = new FastParser(new FastTokenStream(parserPool, code));
            var jScript = parser.ParseProgram();
            parserPool.Dispose();

            using (var fx = this.scope.Push(new FastFunctionScope(pool, (AstFunctionExpression)null, isAsync: jScript.IsAsync))) {


                // System.Console.WriteLine($"Parsing done...");

                var lScope = fx.Context;

                if (argsList != null && jScript.HoistingScope != null) {
                    var list = new Sequence<StringSpan>(jScript.HoistingScope.Count);
                    //try
                    //{
                        var e = jScript.HoistingScope.GetFastEnumerator();
                        while (e.MoveNext(out var a))
                        {
                            if (argsList.Contains(a.Value))
                                continue;
                            list.Add(a);
                        }
                        jScript.HoistingScope = list;
                    // list.Clear();
                    //} finally
                    //    {
                    //        list.Clear();
                    //    }
                }

                this.scriptInfo = Exp.Parameter(typeof(ScriptInfo));


                var args = fx.ArgumentsExpression;

                var te = ArgumentsBuilder.This(args);

                var stackItem = fx.StackItem;

                var vList = new Sequence<ParameterExpression>() {
                    scriptInfo,
                    lScope,
                    stackItem
                };





                if (argsList != null) {
                    int i = 0;
                    foreach (var arg in argsList) {

                        // global arguments are set here for FunctionConstructor

                        fx.CreateVariable(arg,
                            JSVariableBuilder.FromArgument(fx.ArgumentsExpression, i++, arg));
                    }
                }

                var l = fx.ReturnLabel;

                var script = Visit(jScript);

                var sList = new Sequence<Exp>() {
                    Exp.Assign(scriptInfo, ScriptInfoBuilder.New(location,code.Value)),
                    Exp.Assign(lScope, JSContextBuilder.Current)
                };

                //sList.Add(Exp.Assign(ScriptInfoBuilder.Functions(scriptInfo),
                //    Exp.Constant(_innerFunctions.ToArray())));

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
                foreach (var v in fx.Variables) {
                    if (v.Variable != null && v.Variable.Type == typeof(JSVariable)) {
                        if (argsList?.Contains(v.Name) ?? false)
                            continue;
                        if (v.Name == "this")
                            continue;
                        sList.Add(JSContextBuilder.Register(lScope, v.Variable));
                    }
                }


                sList.Add(Exp.Return(l, script.ToJSValue()));
                sList.Add(Exp.Label(l, JSUndefinedBuilder.Value));

                script = Exp.Block(vList, Exp.TryFinally(Exp.Block(sList), JSContextStackBuilder.Pop(stackItem, lScope)));
                if (jScript.IsAsync)
                {
                    var g = GeneratorRewriter.Rewrite("vm", script, fx.ReturnLabel, fx.Generator,
                        replaceArgs: fx.Arguments,
                        replaceStackItem: fx.StackItem,
                        replaceContext: fx.Context,
                        replaceScriptInfo: scriptInfo);

                    var jsf = JSAsyncFunctionBuilder.Create(
                        JSGeneratorFunctionBuilderV2.New(g, StringSpanBuilder.New("vm"), StringSpanBuilder.New(code.Value)));

                    var np = Expression.Parameter(ArgumentsBuilder.refType, "a");
                    jsf = JSFunctionBuilder.InvokeFunction(jsf, np);
                    // scr
                    this.Method = Exp.Lambda<JSFunctionDelegate>("vm", jsf, new ParameterExpression[] { np });
                    return;
                }


                var lambda = Exp.Lambda<JSFunctionDelegate>("body", script, fx.Arguments);

                // System.Console.WriteLine($"Code Generation done...");

                this.Method = lambda;
            }
        }

        private Expression VisitExpression(AstExpression exp) => Visit(exp);

        private Expression VisitStatement(AstStatement exp) => Visit(exp);

        protected override Expression VisitClassStatement(AstClassExpression classStatement)
        {
            return CreateClass(classStatement.Identifier, classStatement.Base, classStatement);
        }

        protected override Expression VisitContinueStatement(AstContinueStatement continueStatement)
        {
            string name = continueStatement.Label?.Name.Value;
            if (name != null)
            {
                var target = this.LoopScope.Get(name);
                if (target == null)
                    throw JSContext.Current.NewSyntaxError($"No label found for {name}");
                return Exp.Continue(target.Break);
            }
            return Exp.Continue(this.scope.Top.Loop.Top.Continue);
        }

        protected override Expression VisitDebuggerStatement(AstDebuggerStatement debuggerStatement)
        {
            return ExpHelper.JSDebuggerBuilder.RaiseBreak();
        }



        protected override Expression VisitEmptyExpression(AstEmptyExpression emptyExpression)
        {
            return Exp.Empty;
        }

        protected override Expression VisitExpressionStatement(AstExpressionStatement expressionStatement)
        {
            return Visit(expressionStatement.Expression);
        }

        protected override Expression VisitFunctionExpression(AstFunctionExpression functionExpression)
        {
            return CreateFunction(functionExpression);
        }





        protected override Expression VisitSpreadElement(AstSpreadElement spreadElement)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitThrowStatement(AstThrowStatement throwStatement)
        {
            return ExpHelper.JSExceptionBuilder.Throw(VisitExpression(throwStatement.Argument));
        }

        protected override Expression VisitYieldExpression(AstYieldExpression yieldExpression)
        {
            var target = VisitExpression(yieldExpression.Argument);
            return YExpression.Yield(target, yieldExpression.Delegate);

        }
    }
}
