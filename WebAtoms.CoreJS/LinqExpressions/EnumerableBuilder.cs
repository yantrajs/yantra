using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.LinqExpressions
{
    internal static class EnumerableBuilder
    {

        private static MethodInfo _MoveNext =
            typeof(IEnumerator).GetMethod(nameof(IEnumerator.MoveNext));

        private static PropertyInfo _Current =
            typeof(IEnumerator<JSValue>).GetProperty(nameof(IEnumerator<JSValue>.Current));

        public static Expression MoveNext(Expression target)
        {
            return Expression.Call(target, _MoveNext);
        }

        public static Expression Current(Expression target)
        {
            return Expression.Property(target, _Current);
        }

    }
}
