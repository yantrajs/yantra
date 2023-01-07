using System;
using System.Linq;
using YantraJS.Core.Generator;
using YantraJS.Core.LinqExpressions;
using YantraJS.Core.LinqExpressions.GeneratorsV2;
using YantraJS.ExpHelper;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using YantraJS.Expressions;
using System.Reflection;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {

        private Exp CreateFunction(
            AstFunctionExpression functionDeclaration,
            Exp super = null,
            bool createClass = false,
            string className = null,
            IFastEnumerable<AstClassProperty> memberInits = null
            )
        {
            var node = functionDeclaration;

            // get text...

            var previousScope = this.scope.Top;

            // if this is an arrowFunction then override previous thisExperssion

            var previousThis = this.scope.Top.ThisExpression;
            if (!(functionDeclaration.IsArrowFunction))
            {
                previousThis = null;
            }

            var functionName = functionDeclaration.Id?.Name.Value;

            // var parentScriptInfo = this.scope.Top.ScriptInfo;

            var nodeCode = node.Code;

            var code = StringSpanBuilder.New(
                ScriptInfoBuilder.Code(scriptInfo),
                nodeCode.Offset ,
                nodeCode.Length);
            var sList = new Sequence<Exp>();
            var bodyInits = new Sequence<Exp>();
            var vList = new Sequence<ParameterExpression>();

            //try
            //{
            // using (var cs = scope.Push(new FastFunctionScope(pool, functionDeclaration, previousThis, super)))
            var current = this.scope.Top.RootScope;
            var cs = scope.Push(new FastFunctionScope(
                pool,
                functionDeclaration,
                previousThis,
                super,
                memberInits: memberInits,
                previous: functionDeclaration.IsArrowFunction ? current: null));
                {
                    var lexicalScopeVar = cs.Context;

                    vList.Add(cs.Context);
                    vList.Add(cs.StackItem);
                    sList.Add(Exp.Assign(cs.Context, JSContextBuilder.Current));

                    FastFunctionScope.VariableScope jsFVarScope = null;

                    if (functionName != null)
                    {
                        jsFVarScope = previousScope.GetVariable(functionName);

                    }

                    var s = cs;
                    // use this to create variables...
                    // var t = s.ThisExpression;
                    var args = s.ArgumentsExpression;
                    var stackItem = cs.StackItem;
                    var r = s.ReturnLabel;


                    Exp fxName;
                    Exp localFxName;
                    int nameOffset;
                    int nameLength;
                    if (functionName != null)
                    {
                        var id = functionDeclaration.Id;
                        fxName = StringSpanBuilder.New(
                            ScriptInfoBuilder.Code(scriptInfo),
                            id.Name.Offset,
                            id.Name.Length);
                        localFxName = StringSpanBuilder.New(
                            ScriptInfoBuilder.Code(scriptInfo),
                            id.Name.Offset,
                            id.Name.Length);
                        nameOffset = id.Name.Offset;
                        nameLength = id.Name.Length;
                    }
                    else
                    {
                        fxName = StringSpanBuilder.Empty;
                        localFxName = StringSpanBuilder.Empty;
                        nameOffset = 0;
                        nameLength = 0;
                    }

                    var point = node.Start.Start; // this.Code.Position(functionDeclaration.Range);

                    // var fn = ScriptInfoBuilder.FileName(s.ScriptInfo);

                    // JSContextStackBuilder.Push(sList, s.Context, stackItem, fn, localFxName, point.Line, point.Column);
                    sList.Add(Exp.Assign(stackItem, CallStackItemBuilder.New(cs.Context, scriptInfo,
                        nameOffset,
                        nameLength,
                        point.Line,
                        point.Column
                        )));

                    // var pList = functionDeclaration.Params.OfType<Identifier>();

                    var argumentElements = args;

                    var pe = functionDeclaration.Params.GetFastEnumerator();
                    while (pe.MoveNext(out var v, out var i))
                    {
                        if (v.Identifier.IsSpreadElement(out var spe))
                        {
                            CreateAssignment(bodyInits, spe.Argument,
                                ArgumentsBuilder.RestFrom(argumentElements, (uint)i)
                                , true,
                                true);

                            continue;
                        }
                        CreateAssignment(bodyInits, v.Identifier,
                            ExpHelper.JSVariableBuilder.FromArgumentOptional(argumentElements, i, VisitExpression(v.Init)),
                            true,
                            true);
                    }

                    Exp lambdaBody = VisitStatement(functionDeclaration.Body);

                    vList.AddRange(s.VariableParameters);
                    sList.AddRange(s.InitList);

                    sList.AddRange(bodyInits);

                if (s.MemberInits != null)
                {
                    InitMembers(sList, s);
                }
                sList.Add(lambdaBody);
                    // sList.Add(JSContextStackBuilder.Pop(stackItem));
                    if (createClass)
                    {
                        sList.Add(Exp.Return(r, Exp.Coalesce(
                            s.ThisExpression,
                            JSExceptionBuilder.Throw("this cannot be null")
                            )));
                    }
                    sList.Add(Exp.Label(r, ExpHelper.JSUndefinedBuilder.Value));
                    // sList.Add();

                    var block = Exp.Block(vList, sList);


                    // adding lexical scope pending...



                    //var lexicalScope =
                    //    Exp.Block(new ParameterExpression[] { lexicalScopeVar, stackItem },
                    //    Exp.Assign(lexicalScopeVar,
                    //        JSContextBuilder.Current),
                    //        block);

                    //Exp closureArray = Exp.Constant(null, typeof(JSVariable[]));
                    //if (cs.ClosureList != null)
                    //{
                    //    closureArray = Exp.NewArrayInit(typeof(JSVariable), cs.ClosureList.Select(x => x.Variable));
                    //}

                    // Exp scriptInfo = parentScriptInfo;

                    functionName = functionName ?? "inline";

                    Exp ToDelegate(LambdaExpression e1)
                    {
                        return e1;
                        //if (super != null)
                        //    return e1;
                        //int index = _innerFunctions.Count;
                        //_innerFunctions.Add(e1.Compile());
                        //return ScriptInfoBuilder.Function(scriptInfo, index, e1.Type);
                    }

                    var scriptFunctionName = new FunctionName(functionName, this.location, point.Line, point.Column);

                    LambdaExpression lambda;
                    Exp jsf;
                    if (functionDeclaration.Generator)
                    {
                        //lambda = Exp.Lambda(typeof(JSGeneratorDelegate),
                        //    YieldRewriter.Rewrite(block, r, cs.Generator, lexicalScopeVar),
                        //    functionName, new ParameterExpression[] {
                        //    cs.ScriptInfo, cs.Closures, cs.Generator, stackItem, cs.Arguments
                        //    });
                        // rewrite lambda...

                        // lambda.Compile();

                        lambda = GeneratorRewriter.Rewrite(in scriptFunctionName, block, cs.ReturnLabel, cs.Generator, 
                            replaceArgs: cs.Arguments,
                            replaceStackItem: cs.StackItem,
                            replaceContext: cs.Context, 
                            replaceScriptInfo: scriptInfo);

                        jsf = JSGeneratorFunctionBuilderV2.New(lambda, fxName, code);

                        // jsf = JSGeneratorFunctionBuilder.New(parentScriptInfo, closureArray, ToDelegate(lambda), fxName, code);

                    }
                    else if (functionDeclaration.Async)
                    {

                        lambda = GeneratorRewriter.Rewrite(in scriptFunctionName, block, cs.ReturnLabel, cs.Generator,
                            replaceArgs: cs.Arguments,
                            replaceStackItem: cs.StackItem,
                            replaceContext: cs.Context,
                            replaceScriptInfo: scriptInfo);

                        jsf = JSAsyncFunctionBuilder.Create(
                            JSGeneratorFunctionBuilderV2.New(lambda, fxName, code));

                        //lambda = Exp.Lambda(typeof(JSAsyncDelegate), block, in scriptFunctionName, new ParameterExpression[] {
                        //    cs.ScriptInfo, cs.Closures, cs.Awaiter, cs.Arguments
                        //});
                        //jsf = JSAsyncFunctionBuilder.New(parentScriptInfo, closureArray, ToDelegate(lambda), fxName, code);
                    }
                    else
                    {
                        lambda = Exp.Lambda(typeof(JSFunctionDelegate), block, in scriptFunctionName, new ParameterExpression[] { cs.Arguments });
                        //if (createClass)
                        //{
                        //    jsf = JSClassBuilder.New(parentScriptInfo, closureArray, ToDelegate(lambda), super, className ?? "Unnamed");
                        //}
                        //else
                        {
                            jsf = JSFunctionBuilder.New(ToDelegate(lambda), fxName, code, functionDeclaration.Params.Count);
                        }
                    }

                //bodyInits.Clear();
                //sList.Clear();
                //vList.Clear();
                cs.Dispose();

                    if (jsFVarScope != null)
                    {
                        jsFVarScope.SetPostInit(jsf);
                        return jsFVarScope.Expression;
                    }
                    return jsf;
                }
            //} finally
            //{
            //    bodyInits.Clear();
            //    sList.Clear();
            //    vList.Clear();
            //}
        }

        private void InitMembers(Sequence<Expression> sList, FastFunctionScope s)
        {
            var @this = s.ThisExpression;
            var en = s.MemberInits.GetFastEnumerator();
            while(en.MoveNext(out var member))
            {
                var name = GetName(member);
                var value = member.Init == null ? JSUndefinedBuilder.Value : Visit(member.Init);
                var init = JSObjectBuilder.AddValue(name, value, JSPropertyAttributes.ConfigurableValue);
                sList.Add(Exp.Call(@this, init.Member as MethodInfo, init.Arguments));
            }
        }
    }
}
