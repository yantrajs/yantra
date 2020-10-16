using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.ExpHelper
{
    public static class JSExceptionBuilder
    {
        private static Type type = typeof(JSException);

        private static ConstructorInfo _new =
            type.Constructor(typeof(string), typeof(string), typeof(string), typeof(int));

        private static MethodInfo _Throw =
            type.InternalMethod(nameof(Core.JSException.Throw), typeof(Core.JSValue));

        public static Expression Throw(Expression value)
        {
            return Expression.Call(null, _Throw, value);
        }

        private static MethodInfo _ThrowNotFunction =
            type.InternalMethod(nameof(Core.JSException.ThrowNotFunction), typeof(Core.JSValue));

        public static Expression ThrowNotFunction(Expression value)
        {
            return Expression.Call(null, _ThrowNotFunction, value);
        }
        private static PropertyInfo _Error =
            type.Property(nameof(JSException.Error));

        public static Expression Error(Expression target)
        {
            return Expression.Property(target, _Error);
        }

        public static Expression New(string message, 
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return Expression.New(_new,
                Expression.Constant(message),
                Expression.Constant(function),
                Expression.Constant(filePath),
                Expression.Constant(line));
        }
    }
}
