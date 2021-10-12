#nullable enable
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using System.Linq;

namespace YantraJS.Core.FastParser.Compiler
{

    partial class FastCompiler
    {

        //internal Exp Loop(
        //    IList<ParameterExpression>? parameters,
        //    LabelTarget breakTarget,
        //    LabelTarget continueTarget,
        //    Exp? initialize,
        //    IList<Exp> body,
        //    Exp? postLoop
        //    )
        //{
        //    var statements = pool.AllocateList<Exp>();
        //    var vars = pool.AllocateList<ParameterExpression>();
        //    try
        //    {
        //        if(parameters!=null)
        //            vars.AddRange(parameters);

        //        if (initialize != null)
        //        {
        //            statements.AddExpanded(vars, initialize);
        //        }

        //        statements.Add(Exp.Label(continueTarget));
        //        if (postLoop != null)
        //        {
        //            statements.AddExpanded(vars, postLoop);
        //        }
        //        foreach (var stmt in body)
        //        {
        //            statements.AddExpanded(vars, stmt);
        //        }
        //        statements.Add(Exp.Goto(continueTarget));
        //        statements.Add(Exp.Label(breakTarget));
        //        return Exp.Block(vars, statements);
        //    } finally {
        //        statements.Clear();
        //        vars.Clear();
        //    }
        //}


        protected override Expression VisitForInStatement(AstForInStatement forInStatement, string? label = null)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            // this will create a variable if needed...
            // desugar takes care of let so do not worry
            Exp? identifier = null;
            switch(forInStatement.Init.Type)
            {
                case FastNodeType.Identifier:
                case FastNodeType.VariableDeclaration:
                    identifier = Visit(forInStatement.Init);
                    break;
                default:
                    throw new FastParseException(forInStatement.Start, $"Unexpcted");
            }
            using (var s = scope.Top.Loop.Push(new LoopScope(breakTarget, continueTarget, false, label)))
            {
                var en = Exp.Variable(typeof(IElementEnumerator));

                var pList = en.AsSequence();

                

                var body = VisitStatement(forInStatement.Body);

                var bodyList = Exp.Block(Exp.IfThen(
                        Exp.Not(IElementEnumeratorBuilder.MoveNext(en, identifier)),
                        Exp.Goto(s.Break)),
                    body);

                var right = VisitExpression(forInStatement.Target);
                return Exp.Block(
                    pList,
                    Exp.Assign(en, JSValueBuilder.GetAllKeys(right)),
                    Exp.Loop(bodyList, s.Break, s.Continue)
                    );
            }
        }

        protected override Expression VisitForOfStatement(AstForOfStatement forOfStatement, string? label = null)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            // this will create a variable if needed...
            // desugar takes care of let so do not worry
            Exp? identifier = null;
            switch (forOfStatement.Init.Type)
            {
                case FastNodeType.Identifier:
                case FastNodeType.VariableDeclaration:
                    identifier = Visit(forOfStatement.Init);
                    break;
                default:
                    throw new FastParseException(forOfStatement.Start, $"Unexpcted");
            }
            using (var s = scope.Top.Loop.Push(new LoopScope(breakTarget, continueTarget, false, label)))
            {
                var en = Exp.Variable(typeof(IElementEnumerator));

                var pList = en.AsSequence();

                var body = VisitStatement(forOfStatement.Body);

                var bodyList = Exp.Block(Exp.IfThen(
                        Exp.Not(IElementEnumeratorBuilder.MoveNext(en, identifier)),
                        Exp.Goto(s.Break)),
                    body);

                var right = VisitExpression(forOfStatement.Target);
                var r = Exp.Block(
                    pList,
                    Exp.Assign(en, IElementEnumeratorBuilder.Get(right)),
                    Exp.Loop(bodyList, s.Break, s.Continue)
                    );
                return r;
            }
        }

        protected override Expression VisitForStatement(AstForStatement forStatement, string? label = null)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            // this will create a variable if needed...
            // desugar takes care of let so do not worry
            Exp init = Visit(forStatement.Init);
            var innerBody = new Sequence<Exp>();

            // try {
                var update = Visit(forStatement.Update);
                var test = Visit(forStatement.Test);

                if(test != null)
                {
                    test = Exp.IfThen(Exp.Not(JSValueBuilder.BooleanValue(test)),
                        Exp.Goto(breakTarget));
                    innerBody.Add(test);
                }

                using (var s = scope.Top.Loop.Push(new LoopScope(breakTarget, continueTarget, false, label)))
                {
                    var body = VisitStatement(forStatement.Body);

                    innerBody.Add(body);
                    innerBody.Add(Exp.Label(continueTarget));
                    if (update != null)
                    {
                        innerBody.Add(update);
                    }

                    if (init == null)
                    {
                        var r1 = Exp.Loop(
                            Exp.Block(innerBody),
                            breakTarget);
                        
                        // innerBody.Clear();
                        return r1;
                    }

                    // return Loop(null, breakTarget, continueTarget, init, innerBody, update);

                    var r = Exp.Block(
                        init,
                        Exp.Loop(
                            Exp.Block(innerBody),
                            breakTarget)
                        );
                // innerBody.Clear(); ;
                return r;
                }
            //} finally {
                
            //}
        }

    }
}
