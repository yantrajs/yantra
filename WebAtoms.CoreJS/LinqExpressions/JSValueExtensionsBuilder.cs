using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSValueExtensionsBuilder
    {
        private readonly static Type type = typeof(Extensions.JSValueExtensions);

        private static MethodInfo _InstanceOf =
            type.GetMethod(nameof(Extensions.JSValueExtensions.InstanceOf));
        public static Expression InstanceOf(Expression target, Expression value)
        {
            return Expression.Call(null, _InstanceOf, target, value);
        }

        private static MethodInfo _IsIn =
            type.GetMethod(nameof(Extensions.JSValueExtensions.IsIn));
        public static Expression IsIn(Expression target, Expression value)
        {
            return Expression.Call(null, _IsIn, target, value);
        }

        public static Expression Assign(Expression e, Expression value)
        {
            return Expression.Assign(e, value);
        }

        private readonly static MethodInfo _NullIfTrue =
            type.StaticMethod<JSValue>(nameof(JSValueExtensions.NullIfTrue));

        public static Expression NullIfTrue(Expression exp)
        {
            return Expression.Call(null, _NullIfTrue, exp);
        }

        private readonly static MethodInfo _NullIfFalse =
            type.StaticMethod<JSValue>(nameof(JSValueExtensions.NullIfFalse));

        public static Expression NullIfFalse(Expression exp)
        {
            return Expression.Call(null, _NullIfFalse, exp);
        }

    }
}
