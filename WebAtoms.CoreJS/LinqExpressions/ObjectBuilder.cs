using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class ObjectBuilder : TypeHelper<System.Object>
    {
        private static MethodInfo _ToString
            = typeof(System.Object).GetMethod("ToString", new Type[] { });

        public static Expression ToString(Expression value)
        {
            return Expression.Call(value, _ToString);
        }

        private static MethodInfo _ReferenceEquals
            = Method<object, object>("ReferenceEquals");

        public static Expression RefEquals(Expression left, Expression right)
        {
            return Expression.Call(null, _ReferenceEquals, left, right);
        }
    }
}
