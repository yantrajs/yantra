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
namespace YantraJS.ExpHelper
{
    public static class JSArgumentsBuilder
    {
        private static Type type = typeof(JSArguments);
        private static ConstructorInfo _New
            = type.Constructor(new Type[] { typeof(Arguments).MakeByRefType() });

        public static Expression New(Expression args)
        {
            return Expression.New(_New, args);
        }
    }
}
