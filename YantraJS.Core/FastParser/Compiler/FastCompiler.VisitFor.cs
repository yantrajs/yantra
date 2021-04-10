using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using Exp = System.Linq.Expressions.Expression;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {


        protected override Expression VisitForInStatement(AstForInStatement forInStatement, string label = null)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            // this will create a variable if needed...
            // desugar takes care of let so do not worry
            Exp identifier = null;
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

                var pList = new ParameterExpression[] {
                    en
                };

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

        protected override Expression VisitForOfStatement(AstForOfStatement forOfStatement, string label = null)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            // this will create a variable if needed...
            // desugar takes care of let so do not worry
            Exp identifier = null;
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

                var pList = new ParameterExpression[] {
                    en
                };

                var body = VisitStatement(forOfStatement.Body);

                var bodyList = Exp.Block(Exp.IfThen(
                        Exp.Not(IElementEnumeratorBuilder.MoveNext(en, identifier)),
                        Exp.Goto(s.Break)),
                    body);

                var right = VisitExpression(forOfStatement.Target);
                return Exp.Block(
                    pList,
                    Exp.Assign(en, IElementEnumeratorBuilder.Get(right)),
                    Exp.Loop(bodyList, s.Break, s.Continue)
                    );
            }
        }

        protected override Expression VisitForStatement(AstForStatement forStatement, string label = null)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            // this will create a variable if needed...
            // desugar takes care of let so do not worry
            Exp init = Visit(forStatement.Init);
            var innerBody = pool.AllocateList<Exp>();

            try {
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
                    if(update != null)
                    {
                        innerBody.Add(update);
                    }

                    return Exp.Block(
                        init,
                        Exp.Loop(
                            Exp.Block(innerBody.ToSpan()),
                            breakTarget)
                        );
                }
            } finally {
                innerBody.Clear();
            }
        }

    }
}
