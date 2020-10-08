using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSBooleanBuilder : TypeHelper<Core.JSBoolean>
    {

        public static Expression True =
            Expression.Field(null, Field("True"));

        public static Expression False =
            Expression.Field(null, Field("False"));

        private static FieldInfo _Value =
            InternalField(nameof(Core.JSBoolean._value));

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
