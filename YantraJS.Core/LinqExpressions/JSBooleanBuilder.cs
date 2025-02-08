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
    public class JSBooleanBuilder 
    {
        // static Type type = typeof(JSBoolean);

        public static Expression True =
            NewLambdaExpression.StaticFieldExpression<JSValue>(() => () => JSBoolean.True);
            // Expression.TypeAs( Expression.Field(null, type.GetField(nameof(JSBoolean.True))), typeof(JSValue));

        public static Expression False =
            NewLambdaExpression.StaticFieldExpression<JSValue>(() => () => JSBoolean.False);
            // Expression.TypeAs( Expression.Field(null, type.GetField(nameof(JSBoolean.False))), typeof(JSValue));

        //private static FieldInfo _Value =
        //    type.InternalField(nameof(Core.JSBoolean._value));

        //public static Expression Value(Expression target)
        //{
        //    return Expression.Field(target, _Value);
        //}

        public static Expression NewFromCLRBoolean(Expression target)
        {
            return Expression.Condition(target, JSBooleanBuilder.True, JSBooleanBuilder.False);
        }


        public static Expression Not(Expression value)
        {
            return Expression.Condition(
                JSValueBuilder.BooleanValue(value),
                JSBooleanBuilder.False,
                JSBooleanBuilder.True
                );
        }
    }
}
