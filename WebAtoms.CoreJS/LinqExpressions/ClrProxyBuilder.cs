using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using WebAtoms.CoreJS.Core.Clr;

namespace WebAtoms.CoreJS.LinqExpressions
{
    public static class ClrProxyBuilder
    {

        private static Type type = typeof(ClrProxy);

        private static MethodInfo _marshal =
            type.InternalMethod(nameof(ClrProxy.Marshal), typeof(object));
        public static Expression Marshal(Expression target)
        {
            return Expression.Call(null, _marshal, Expression.TypeAs(target, typeof(object)));
        }

    }
}
