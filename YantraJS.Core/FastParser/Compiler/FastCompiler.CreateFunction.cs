using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.Generator;
using YantraJS.Core.LinqExpressions;
using YantraJS.Core.LinqExpressions.Generators;
using YantraJS.ExpHelper;
using Exp = System.Linq.Expressions.Expression;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {

        private Exp CreateFunction(
            AstFunctionExpression functionDeclaration,
            Exp super = null,
            bool createClass = false,
            string className = null
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

            var parentScriptInfo = this.scope.Top.ScriptInfo;

            var nodeCode = node.Code;

            var code = StringSpanBuilder.New(
                ScriptInfoBuilder.Code(parentScriptInfo),
                nodeCode.Offset ,
                nodeCode.Length);
            var sList = pool.AllocateList<Exp>();
            var bodyInits = pool.AllocateList<Exp>();
            var vList = pool.AllocateList<ParameterExpression>();

            try
            {
                using (var cs = scope.Push(new FastFunctionScope(functionDeclaration, previousThis, super)))
                {
                    var lexicalScopeVar = cs.Context;


                    FastFunctionScope.VariableScope jsFVarScope = null;

                    if (functionName != null)
                    {
                        jsFVarScope = previousScope.GetVariable(functionName);

                    }

                    var s = cs;
                    // use this to create variables...
                    // var t = s.ThisExpression;
                    var args = s.ArgumentsExpression;
                    var stackItem = s.StackItem;
                    var r = s.ReturnLabel;


                    Exp fxName;
                    Exp localFxName;
                    if (functionName != null)
                    {
                        var id = functionDeclaration.Id;
                        fxName = StringSpanBuilder.New(parentScriptInfo,
                            id.Name.Offset,
                            id.Name.Length);
                        localFxName = StringSpanBuilder.New(
                            ScriptInfoBuilder.Code(s.ScriptInfo),
                            id.Name.Offset,
                            id.Name.Length);
                    }
                    else
                    {
                        fxName = StringSpanBuilder.Empty;
                        localFxName = StringSpanBuilder.Empty;
                    }

                    var point = node.Start.Start; // this.Code.Position(functionDeclaration.Range);

                    var fn = ScriptInfoBuilder.FileName(s.ScriptInfo);

                    JSContextStackBuilder.Push(sList, s.Context, stackItem, fn, localFxName, point.Line, point.Column);

                    // var pList = functionDeclaration.Params.OfType<Identifier>();

                    var argumentElements = args;

                    var pe = functionDeclaration.Params.GetEnumerator();
                    while (pe.MoveNext(out var v, out var i))
                    {
                        if (v.Identifier.Type == FastNodeType.SpreadElement && v.Identifier is AstSpreadElement spe)
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



                    var lexicalScope =
                        Exp.Block(new ParameterExpression[] { lexicalScopeVar, stackItem },
                        Exp.Assign(lexicalScopeVar,
                            JSContextBuilder.Current),
                        Exp.TryFinally(
                            block
                             , JSContextStackBuilder.Pop(stackItem, lexicalScopeVar))
                        );

                    Exp closureArray = Exp.Constant(null, typeof(JSVariable[]));
                    if (cs.ClosureList != null)
                    {
                        closureArray = Exp.NewArrayInit(typeof(JSVariable), cs.ClosureList.Select(x => x.Variable));
                    }

                    Exp scriptInfo = parentScriptInfo;

                    functionName = (functionName ?? "inline") + "_" + point;

                    Exp ToDelegate(System.Linq.Expressions.LambdaExpression e1)
                    {
                        if (super != null)
                            return e1;
                        int index = _innerFunctions.Count;
                        _innerFunctions.Add(e1.Compile());
                        return ScriptInfoBuilder.Function(scriptInfo, index, e1.Type);
                    }

                    System.Linq.Expressions.LambdaExpression lambda;
                    Exp jsf;
                    if (functionDeclaration.Generator)
                    {
                        lambda = Exp.Lambda(typeof(JSGeneratorDelegate),
                            YieldRewriter.Rewrite(block, r, cs.Generator, lexicalScopeVar),
                            functionName, new ParameterExpression[] {
                            cs.ScriptInfo, cs.Closures, cs.Generator, stackItem, cs.Arguments
                            });
                        // rewrite lambda...

                        // lambda.Compile();

                        jsf = JSGeneratorFunctionBuilder.New(parentScriptInfo, closureArray, ToDelegate(lambda), fxName, code);

                    }
                    else if (functionDeclaration.Async)
                    {
                        lambda = Exp.Lambda(typeof(JSAsyncDelegate), lexicalScope, functionName, new ParameterExpression[] {
                        cs.ScriptInfo, cs.Closures, cs.Awaiter, cs.Arguments
                    });
                        jsf = JSAsyncFunctionBuilder.New(parentScriptInfo, closureArray, ToDelegate(lambda), fxName, code);
                    }
                    else
                    {
                        lambda = Exp.Lambda(typeof(JSClosureFunctionDelegate), lexicalScope, functionName, new ParameterExpression[] {
                        cs.ScriptInfo, cs.Closures, cs.Arguments });
                        if (createClass)
                        {
                            jsf = JSClassBuilder.New(parentScriptInfo, closureArray, ToDelegate(lambda), super, className ?? "Unnamed");
                        }
                        else
                        {
                            jsf = JSClosureFunctionBuilder.New(parentScriptInfo, closureArray, ToDelegate(lambda), fxName, code, functionDeclaration.Params.Count);
                        }
                    }

                    if (jsFVarScope != null)
                    {
                        jsFVarScope.SetPostInit(jsf);
                        return jsFVarScope.Expression;
                    }
                    return jsf;
                }
            } finally
            {
                bodyInits.Clear();
                sList.Clear();
                vList.Clear();
            }
        }

    }
}
