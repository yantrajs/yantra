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
    partial class FastCompiler
    {


        protected override Expression VisitIfStatement(AstIfStatement ifStatement)
        {
            var test = ExpHelper.JSValueBuilder.BooleanValue(VisitExpression(ifStatement.Test));
            var trueCase = VisitStatement(ifStatement.True).ToJSValue();
            if (ifStatement.False != null)
            {
                var elseCase = VisitStatement(ifStatement.False).ToJSValue();
                return Exp.Condition(test, trueCase, elseCase);
            }
            return Exp.Condition(test, trueCase, ExpHelper.JSUndefinedBuilder.Value);
        }
    }
}
