﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSValueExtensionsBuilder
    {
        private readonly static Type type = typeof(JSValueExtensions);

        private static MethodInfo _InstanceOf =
            type.GetMethod(nameof(JSValueExtensions.InstanceOf));
        public static Expression InstanceOf(Expression target, Expression value)
        {
            return Expression.Call(null, _InstanceOf, target, value);
        }

        private static MethodInfo _IsIn =
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

        private static MethodInfo _InvokeMethodKeyString
            = type.InternalMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), typeof(KeyString), ArgumentsBuilder.refType);
        private static MethodInfo _InvokeMethodUInt
            = type.InternalMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), typeof(uint), ArgumentsBuilder.refType);
        private static MethodInfo _InvokeMethodJSValue
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