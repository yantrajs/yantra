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
    public class JSNumberBuilder
    {
        static Type type = typeof(JSNumber);

        public static Expression NaN =
            Expression.Field(null, type.GetField(nameof(JSNumber.NaN)));

        public static Expression Zero =
            Expression.Field(null, type.GetField(nameof(JSNumber.Zero)));

        public static Expression One =
            Expression.Field(null, type.GetField(nameof(JSNumber.One)));

        public static Expression MinusOne =
            Expression.Field(null, type.GetField(nameof(JSNumber.MinusOne)));

        public static Expression Two =
            Expression.Field(null, type.GetField(nameof(JSNumber.Two)));

        private static FieldInfo _Value =
            type.InternalField(nameof(Core.JSNumber.value));

        public static Expression Value(Expression ex)
        {
            return Expression.Field(ex, _Value);
        }

        private static ConstructorInfo _NewDouble
            = type.Constructor(typeof(double));

        public static Expression New(Expression exp)
        {
            if (exp.Type != typeof(double))
            {
                exp = Expression.Convert(exp, typeof(double));
            }
            return  Expression.TypeAs(Expression.New(_NewDouble, exp), typeof(JSValue));
        }

        private static MethodInfo _AddValue =
                        type.InternalMethod(nameof(Core.JSValue.AddValue), typeof(Core.JSValue));

        public static Expression AddValue(Expression target, Expression value)
        {
            return Expression.Call(target, _AddValue, value);
        }


    }
}
