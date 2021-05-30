using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using YantraJS.Core;
using YantraJS.Core.Clr;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;

namespace YantraJS.LinqExpressions
{
    public static class ClrProxyBuilder
    {

        private static Type type = typeof(ClrProxy);

        private static MethodInfo _marshal =
            type.InternalMethod(nameof(ClrProxy.Marshal), typeof(object));

        private static ConstructorInfo _new =
            type.Constructor(typeof(object), typeof(JSObject));

        public static Expression New(Expression target, Expression prototype)
        {
            return Expression.New(_new, target, prototype).ToJSValue();
        }
        public static Expression Marshal(Expression target)
        {
            return Expression.Call(null, _marshal, Expression.TypeAs(target, typeof(object)));
        }

    }
}
