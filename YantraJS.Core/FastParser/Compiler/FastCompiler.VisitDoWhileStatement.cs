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

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler{

        // In doWhile continue should preced the test
        protected override Expression VisitDoWhileStatement(AstDoWhileStatement doWhileStatement, string label = null)
        {
            var breakTarget = Exp.Label();
            var continueTarget = Exp.Label();
            using (var s = scope.Top.Loop.Push(new LoopScope(breakTarget, continueTarget, false, label)))
            {
                var body = VisitStatement(doWhileStatement.Body);
                var test = Exp.Not(JSValueBuilder.BooleanValue(VisitExpression(doWhileStatement.Test)));
                return Exp.Loop(
                    Exp.Block(body, Exp.Label(continueTarget), Exp.IfThen(test, Exp.Goto(breakTarget))),
                    breakTarget,
                    null);
            }
        }
    }
}
