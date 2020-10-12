using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebAtoms.CoreJS.ExpHelper
{
    public static class IDisposableBuilder
    {
        private static MethodInfo _Dispose
            = typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose));

        public static Expression Dispose(Expression exp)
        {
            return Expression.Call(exp, _Dispose);
        }
    }
}
