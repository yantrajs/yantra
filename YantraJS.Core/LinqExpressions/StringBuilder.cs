using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
namespace YantraJS.ExpHelper
{
    public class StringBuilder
    {
        private static Type type = typeof(string);

        private static MethodInfo _Compare =
            type.StaticMethod(nameof(String.Compare), typeof(string), typeof(string));

        public static Expression Compare(Expression left, Expression right)
        {
            return Expression.Call(null, _Compare, left, right);
        }

        private static MethodInfo _Concat =
            type.StaticMethod(nameof(String.Concat), typeof(string), typeof(string));

        public static Expression Concat(Expression left, Expression right)
        {
            return Expression.Call(null, _Concat, left, right);
        }

    }
}
