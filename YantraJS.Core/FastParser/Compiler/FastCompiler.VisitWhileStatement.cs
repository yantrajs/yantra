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
        protected override Expression VisitWhileStatement(AstWhileStatement whileStatement, string label = null)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            using (var s = scope.Top.Loop.Push(new LoopScope(breakTarget, continueTarget, false, label)))
            {

                var body = Visit(whileStatement.Body);

                var list = new SparseList<Exp>();

                var test = Exp.Not(ExpHelper.JSValueBuilder.BooleanValue(Visit(whileStatement.Test)));

                list.Add(Exp.IfThen(test, Exp.Goto(breakTarget)));
                list.Add(body);

                return Exp.Loop(
                    Exp.Block(list),
                    breakTarget,
                    continueTarget);
            }
        }

    }
}
