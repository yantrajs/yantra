using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSNumberBuilder : TypeHelper<Core.JSNumber>
    {


        public static Expression NaN =
            Expression.Field(null, Field("NaN"));

        private static FieldInfo _Value =
            InternalField(nameof(Core.JSNumber.value));

        public static Expression Value(Expression ex)
        {
            return Expression.Field(ex, _Value);
        }

        private static ConstructorInfo _NewDouble
            = Constructor<double>();

        public static Expression New(Expression exp)
        {
            if (exp.Type != typeof(double))
            {
                exp = Expression.Convert(exp, typeof(double));
            }
            return Expression.New(_NewDouble, exp);
        }

        private static MethodInfo _AddValue =
                        Method<Core.JSValue>(nameof(Core.JSValue.AddValue));

        public static Expression AddValue(Expression target, Expression value)
        {
            return Expression.Call(target, _AddValue, value);
        }


    }
}
