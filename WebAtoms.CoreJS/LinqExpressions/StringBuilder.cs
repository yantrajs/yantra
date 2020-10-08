using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class StringBuilder : TypeHelper<System.String>
    {
        private static MethodInfo _Compare =
            StaticMethod<string, string>("Compare");

        public static Expression Compare(Expression left, Expression right)
        {
            return Expression.Call(null, _Compare, left, right);
        }

        private static MethodInfo _Equals =
            StaticMethod<string, string>("Equals");

        public static Expression Equals(Expression left, Expression right)
        {
            return Expression.Call(null, _Equals, left, right);
        }

        private static MethodInfo _Concat =
            StaticMethod<string, string>("Concat");

        public static Expression Concat(Expression left, Expression right)
        {
            return Expression.Call(null, _Concat, left, right);
        }

    }
}
