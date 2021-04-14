using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using Exp = System.Linq.Expressions.Expression;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler{
        protected override Expression VisitDoWhileStatement(AstDoWhileStatement doWhileStatement, string label = null)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            using (var s = scope.Top.Loop.Push(new LoopScope(breakTarget, continueTarget, false, label)))
            {
                var body = VisitStatement(doWhileStatement.Body);
                var test = Exp.Not(JSValueBuilder.BooleanValue(VisitExpression(doWhileStatement.Test)));
                return Exp.Loop(
                    Exp.Block(body, Exp.IfThen(test, Exp.Goto(breakTarget))),
                    breakTarget,
                    continueTarget);
            }
        }
    }
}
