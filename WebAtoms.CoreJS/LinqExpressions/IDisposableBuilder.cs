using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class IDisposableBuilder : TypeHelper<System.IDisposable>
    {
        private static MethodInfo _Dispose
            = Method("Dispose");

        public static Expression Dispose(Expression exp)
        {
            return Expression.Call(exp, _Dispose);
        }
    }
}
