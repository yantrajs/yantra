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
            var marshal = nameof(ClrProxy.Marshal);
            foreach(var m in type.GetMethods())
            {
                if (m.Name != marshal)
                    continue;
                d[m.GetParameters()[0].ParameterType] = m;
            }
            _marshal = d;
            var from = nameof(ClrProxy.From);
            d = new Dictionary<Type, MethodInfo>(10);
            foreach (var m in type.GetMethods())
            {
                if (m.Name != from)
                    continue;
                if (m.GetParameters().Length != 1)
                    continue;
                d[m.GetParameters()[0].ParameterType] = m;
            }
            _from = d;
        }

        private static Type type = typeof(ClrProxy);

        private static Dictionary<Type, MethodInfo> _marshal;

        private static Dictionary<Type, MethodInfo> _from;

        public static Expression Marshal(Expression target)
        {
            if (typeof(JSValue).IsAssignableFrom(target.Type))
                return target;
            if (_marshal.TryGetValue(target.Type, out var m))
            {
                return Expression.Call(null, m, target);
            }
            if (target.Type.IsValueType)
            {
                return Expression.Call(null, _marshal[typeof(object)], Expression.Box( target));
            }
            return Expression.Call(null, _marshal[typeof(object)], target);
        }

        public static Expression From(Expression target)
        {
            if (typeof(JSValue).IsAssignableFrom(target.Type))
                return target;
            var targetType = target.Type;
            if (_from.TryGetValue(targetType, out var m))
            {
                return Expression.Call(null, m, target);
            }
            if (targetType.IsValueType)
            {
                return Expression.Call(null, _from[typeof(object)], Expression.Box(target));
            }
            foreach(var pair in _from)
            {
                if (pair.Key.IsAssignableFrom(targetType))
                {
                    return Expression.Call(null, pair.Value, target);
                }
            }
            return Expression.Call(null, _from[typeof(object)], target);
        }
    }
}
