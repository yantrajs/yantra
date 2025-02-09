using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using YantraJS.Core.LambdaGen;

namespace YantraJS.ExpHelper
{
    public class JSRegExpBuilder
    {
        // private static ConstructorInfo _New = typeof(JSRegExp).Constructor(typeof(string), typeof(string));

        public static Expression New(Expression exp, Expression exp2)
        {
            return
                Expression.TypeAs(
                        NewLambdaExpression.NewExpression<JSRegExp>(() => () => new JSRegExp("","")
                        , exp
                        , exp2)
                    , typeof(JSValue));
            // return Expression.TypeAs( Expression.New(_New, exp, exp2), typeof(JSValue));
        }

    }
}
