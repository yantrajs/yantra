using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;

namespace YantraJS.ExpHelper
{
    public class JSBooleanBuilder 
    {
        static Type type = typeof(JSBoolean);

        public static Expression True =
            Expression.Field(null, type.GetField(nameof(JSBoolean.True)));

        public static Expression False =
            Expression.Field(null, type.GetField(nameof(JSBoolean.False)));

        private static FieldInfo _Value =
            type.InternalField(nameof(Core.JSBoolean._value));

        public static Expression Value(Expression target)
        {
            return Expression.Field(target, _Value);
        }

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
