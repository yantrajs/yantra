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
        protected override Expression VisitConditionalExpression(AstConditionalExpression conditionalExpression)
        {
            Exp EvaluateTest(AstExpression exp)
            {
                if (exp.Type == FastNodeType.UnaryExpression)
                {
                    var u = exp as AstUnaryExpression;
                    if (u.Operator == UnaryOperator.Negate)
                    {
                        var eu = VisitExpression(u.Argument);
                        var e1 = JSValueBuilder.BooleanValue(eu);
                        var e2 = Exp.Not(e1);
                        return e2;
                    }
                }
                return JSValueBuilder.BooleanValue(VisitExpression(exp));
            }
            var test = EvaluateTest(conditionalExpression.Test);
            var @true = VisitExpression(conditionalExpression.True);
            var @false = VisitExpression(conditionalExpression.False);
            return Exp.Condition(
                test,
                @true,
                @false, typeof(JSValue));
        }
    }
}
