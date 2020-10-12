using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class StringBuilder
    {
        private static Type type = typeof(string);

        private static MethodInfo _Compare =
            type.StaticMethod(nameof(String.Compare), typeof(string), typeof(string));

        public static Expression Compare(Expression left, Expression right)
        {
            return Expression.Call(null, _Compare, left, right);
        }

        private static MethodInfo _Concat =
            type.StaticMethod(nameof(String.Concat), typeof(string), typeof(string));

        public static Expression Concat(Expression left, Expression right)
        {
            return Expression.Call(null, _Concat, left, right);
        }

    }
}
