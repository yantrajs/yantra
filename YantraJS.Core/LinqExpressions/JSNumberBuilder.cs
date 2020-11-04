using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;

namespace YantraJS.ExpHelper
{
    public class JSNumberBuilder
    {
        static Type type = typeof(JSNumber);

        public static Expression NaN =
            Expression.Field(null, type.GetField(nameof(JSNumber.NaN)));

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
            return Expression.New(_NewDouble, exp);
        }

        private static MethodInfo _AddValue =
                        type.InternalMethod(nameof(Core.JSValue.AddValue), typeof(Core.JSValue));

        public static Expression AddValue(Expression target, Expression value)
        {
            return Expression.Call(target, _AddValue, value);
        }


    }
}
