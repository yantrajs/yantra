using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSExceptionBuilder : TypeHelper<Core.JSException>
    {
        private static MethodInfo _Throw =
            InternalMethod<Core.JSValue>(nameof(Core.JSException.Throw));

        public static Expression Throw(Expression value)
        {
            return Expression.Call(null, _Throw, value);
        }

        private static MethodInfo _ThrowNotFunction =
            InternalMethod<Core.JSValue>(nameof(Core.JSException.ThrowNotFunction));

        public static Expression ThrowNotFunction(Expression value)
        {
            return Expression.Call(null, _ThrowNotFunction, value);
        }
        private static PropertyInfo _Error =
            Property("Error");

        public static Expression Error(Expression target)
        {
            return Expression.Property(target, _Error);
        }
    }
}
