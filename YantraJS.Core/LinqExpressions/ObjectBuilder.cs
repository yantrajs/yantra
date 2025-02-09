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
using YantraJS.Core.LambdaGen;


namespace YantraJS.ExpHelper
{
    public class ObjectBuilder
    {
        // private static Type type = typeof(object);

        //private static MethodInfo _ToString
        //    = typeof(System.Object).GetMethod("ToString", new Type[] { });

        public static Expression ToString(Expression value)
        {
            return value.CallExpression<object, string>(() => (x) => x.ToString(), value);
            //return Expression.Call(value, _ToString);
        }

        //private static MethodInfo _ReferenceEquals
        //    = type.StaticMethod(nameof(Object.ReferenceEquals), typeof(object), typeof(object));

        //public static Expression RefEquals(Expression left, Expression right)
        //{
        //    return Expression.Call(null, _ReferenceEquals, left, right);
        //}
    }
}
