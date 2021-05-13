using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Extensions;

namespace YantraJS.ExpHelper
{
    public class JSValueExtensionsBuilder
    {
        private readonly static Type type = typeof(JSValueExtensions);

        private static readonly MethodInfo _InstanceOf =
            type.GetMethod(nameof(JSValueExtensions.InstanceOf));
        public static Expression InstanceOf(Expression target, Expression value)
        {
            return Expression.Call(null, _InstanceOf, target, value);
        }

        static readonly MethodInfo _IsIn =
            type.GetMethod(nameof(JSValueExtensions.IsIn));
        public static Expression IsIn(Expression target, Expression value)
        {
            return Expression.Call(null, _IsIn, target, value);
        }

        public static Expression Assign(Expression e, Expression value)
        {
            return Expression.Assign(e, value);
        }

        private readonly static MethodInfo _NullIfTrue =
            type.PublicMethod(nameof(JSValueExtensions.NullIfTrue), typeof(JSValue));

        public static Expression NullIfTrue(Expression exp)
        {
            return Expression.Call(null, _NullIfTrue, exp);
        }

        private readonly static MethodInfo _NullIfFalse =
            type.PublicMethod(nameof(JSValueExtensions.NullIfFalse), typeof(JSValue));

        private readonly static MethodInfo _NullIfUndefined =
            type.StaticMethod<JSValue>(nameof(JSValueExtensions.NullIfUndefined));

        public static Expression NullIfFalse(Expression exp)
        {
            return Expression.Call(null, _NullIfFalse, exp);
        }
        public static Expression NullIfUndefined(Expression exp)
        {
            return Expression.Call(null, _NullIfUndefined, exp);
        }


        static readonly MethodInfo _InvokeMethodKeyString
            = type.InternalMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), KeyStringsBuilder.RefType, ArgumentsBuilder.refType);
        static readonly MethodInfo _InvokeMethodUInt
            = type.InternalMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), typeof(uint), ArgumentsBuilder.refType);
        static readonly MethodInfo _InvokeMethodJSValue
            = type.InternalMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), typeof(JSValue), ArgumentsBuilder.refType);

        public static Expression InvokeMethod(Expression target, Expression method, Expression args)
        {
            if (method.Type == typeof(KeyString))
                return Expression.Call(null, _InvokeMethodKeyString, target, method, args);
            if (method.Type == typeof(uint))
                return Expression.Call(null, _InvokeMethodUInt, target, method, args);
            if (method.Type == typeof(int))
                return Expression.Call(null , _InvokeMethodUInt, target, Expression.Convert(method, typeof(uint)), args);
            return Expression.Call(null, _InvokeMethodJSValue, target, method, args);
        }


    }
}
