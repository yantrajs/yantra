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
using System.Linq;

namespace YantraJS.LinqExpressions
{
    public static class ClrProxyBuilder
    {

        static ClrProxyBuilder()
        {
            var d = new Dictionary<Type, MethodInfo>(10);
            foreach(var m in type.GetMethods())
            {
                if (m.Name != nameof(ClrProxy.Marshal))
                    continue;
                d[m.GetParameters()[0].ParameterType] = m;
            }
            _marshal = d;
        }

        private static Type type = typeof(ClrProxy);

        private static Dictionary<Type, MethodInfo> _marshal;

        private static ConstructorInfo _new =
            type.Constructor(typeof(object), typeof(JSObject));

        public static Expression New(Expression target, Expression prototype)
        {
            return Expression.New(_new, target, prototype).ToJSValue();
        }
        public static Expression Marshal(Expression target)
        {
            if (typeof(JSValue).IsAssignableFrom(target.Type))
                return target;
            if (_marshal.TryGetValue(target.Type, out var m))
            {
                return Expression.Call(null, m, target);
            }
            return Expression.Call(null, _marshal[typeof(object)], target);
        }

    }
}
