using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSValueBuilder : TypeHelper<Core.JSValue>
    {
        private static readonly Type type = typeof(JSValue);

        private static PropertyInfo _DoubleValue =
            Property(nameof(Core.JSValue.DoubleValue));
        public static Expression DoubleValue(Expression exp)
        {
            return Expression.Property(exp, _DoubleValue);
        }

        private static PropertyInfo _BooleanValue =
            Property("BooleanValue");
        public static Expression BooleanValue(Expression exp)
        {
            return Expression.Property(exp, _BooleanValue);
        }


        private static MethodInfo _Add =
            Method<Core.JSValue>(nameof(Core.JSValue.AddValue));

        public static Expression Add(Expression target, Expression value)
        {
            return Expression.Call(target, _Add, value);
        }

        private static MethodInfo _TypeOf =
            Method("TypeOf");

        public static Expression TypeOf(Expression target)
        {
            return Expression.Call(target, _TypeOf);
        }

        private static PropertyInfo _IndexKeyString =
                type.IndexProperty(typeof(KeyString));

        private static PropertyInfo _IndexUInt =
                type.IndexProperty(typeof(uint));

        private static PropertyInfo _Index =
                type.IndexProperty(typeof(JSValue));

        private static PropertyInfo _SuperIndexKeyString =
                    type.IndexProperty(typeof(JSObject), typeof(KeyString));

        private static PropertyInfo _SuperIndexUInt =
                    type.IndexProperty(typeof(JSObject), typeof(uint));

        private static PropertyInfo _SuperIndex =
                    type.IndexProperty(typeof(JSObject), typeof(JSValue));

        public static Expression Index(Expression target, Expression super, uint i)
        {
            if (super == null)
            {
                return Index(target, i);
            }
            return Expression.MakeIndex(target, _SuperIndexUInt, new Expression[] { super, Expression.Constant(i) });
        }


        public static Expression Index(Expression target, uint i)
        {

            return Expression.MakeIndex(target, _IndexUInt, new Expression[] { Expression.Constant(i) });
        }


        public static Expression Index(Expression target, Expression super, Expression property)
        {
            if (super == null)
            {
                return Index(target, property);
            }
            if (property.Type == typeof(KeyString))
            {
                return Expression.MakeIndex(target, _SuperIndexKeyString, new Expression[] { super, property });
            }
            if (property.Type == typeof(uint))
            {
                return Expression.MakeIndex(target, _SuperIndexUInt, new Expression[] { super, property });
            }
            if (property.Type == typeof(int))
            {
                return Expression.MakeIndex(target, _SuperIndexUInt, new Expression[] { super, Expression.Convert(property, typeof(uint)) });
            }
            return Expression.MakeIndex(target, _SuperIndex, new Expression[] { super, property });
        }


        public static Expression Index(Expression target, Expression property)
        {
            if (property.Type == typeof(KeyString))
            {
                return Expression.MakeIndex(target, _IndexKeyString, new Expression[] { property });
            }
            if (property.Type == typeof(uint))
            {
                return Expression.MakeIndex(target, _IndexUInt, new Expression[] { property });
            }
            if (property.Type == typeof(int))
            {
                return Expression.MakeIndex(target, _IndexUInt, new Expression[] { Expression.Convert(property, typeof(uint)) });
            }
            return Expression.MakeIndex(target, _Index, new Expression[] { property });
        }

        private static MethodInfo _InvokeMethodKeyString
            = type.MethodStartsWith(nameof(JSValue.InvokeMethod), typeof(KeyString));
        private static MethodInfo _InvokeMethodUInt
            = type.MethodStartsWith(nameof(JSValue.InvokeMethod), typeof(uint));
        private static MethodInfo _InvokeMethodJSValue
            = type.MethodStartsWith(nameof(JSValue.InvokeMethod), typeof(JSValue));

        public static Expression InvokeMethod(Expression target, Expression method, Expression args)
        {
            if (method.Type == typeof(KeyString))
                return Expression.Call(target, _InvokeMethodKeyString, method, args);
            if (method.Type == typeof(uint))
                return Expression.Call(target, _InvokeMethodUInt, method, args);
            if (method.Type == typeof(int))
                return Expression.Call(target, _InvokeMethodUInt, Expression.Convert(method, typeof(uint)), args);
            return Expression.Call(target, _InvokeMethodJSValue, method, args);
        }

        private static MethodInfo _DeleteKeyString
            = Method<KeyString>(nameof(JSValue.Delete));
        private static MethodInfo _DeleteUInt
            = Method<uint>(nameof(JSValue.Delete));
        private static MethodInfo _DeleteJSValue
            = Method<JSValue>(nameof(JSValue.Delete));

        public static Expression Delete(Expression target, Expression method)
        {
            if (method.Type == typeof(KeyString))
                return Expression.Call(target, _DeleteKeyString, method);
            if (method.Type == typeof(uint))
                return Expression.Call(target, _DeleteUInt, method);
            if (method.Type == typeof(int))
                return Expression.Call(target, _DeleteUInt, Expression.Convert(method, typeof(uint)));
            return Expression.Call(target, _DeleteJSValue, method);
        }

        internal static MethodInfo _CreateInstance
            = Method(nameof(JSValue.CreateInstance));

        public static Expression CreateInstance(Expression target, Expression args)
        {
            return Expression.Call(target, _CreateInstance, args);
        }

        internal static MethodInfo StaticEquals
            = InternalStaticMethod<Core.JSValue, Core.JSValue>(nameof(Core.JSValue.StaticEquals));


        private static MethodInfo _Equals
            = Method<Core.JSValue>(nameof(Core.JSValue.Equals));

        public static Expression Equals(Expression target, Expression value)
        {
            return Expression.Call(target, _Equals, value);
        }

        public static Expression NotEquals(Expression target, Expression value)
        {
            return
                ExpHelper.JSBooleanBuilder.NewFromCLRBoolean(
                    Expression.Not(
                    ExpHelper.JSValueBuilder.BooleanValue(Expression.Call(target, _Equals, value))
                ));
        }


        private static MethodInfo _StrictEquals
            = Method<Core.JSValue>(nameof(Core.JSValue.StrictEquals));

        public static Expression StrictEquals(Expression target, Expression value)
        {
            return Expression.Call(target, _StrictEquals, value);
        }

        public static Expression NotStrictEquals(Expression target, Expression value)
        {
            return
                ExpHelper.JSBooleanBuilder.NewFromCLRBoolean(
                Expression.Not(
                ExpHelper.JSValueBuilder.BooleanValue(Expression.Call(target, _StrictEquals, value))));
        }

        private static MethodInfo _Less
            = InternalMethod<Core.JSValue>(nameof(Core.JSValue.Less));

        public static Expression Less(Expression target, Expression value)
        {
            return Expression.Call(target, _Less, value);
        }

        private static MethodInfo _LessOrEqual
            = InternalMethod<Core.JSValue>(nameof(Core.JSValue.LessOrEqual));

        public static Expression LessOrEqual(Expression target, Expression value)
        {
            return Expression.Call(target, _LessOrEqual, value);
        }

        private static MethodInfo _Greater
            = InternalMethod<Core.JSValue>(nameof(Core.JSValue.Greater));
        public static Expression Greater(Expression target, Expression value)
        {
            return Expression.Call(target, _Greater, value);
        }

        private static MethodInfo _GreaterOrEqual
            = InternalMethod<Core.JSValue>(nameof(Core.JSValue.GreaterOrEqual));
        public static Expression GreaterOrEqual(Expression target, Expression value)
        {
            return Expression.Call(target, _GreaterOrEqual, value);
        }


        public static Expression LogicalAnd(Expression target, Expression value)
        {
            return Expression.Coalesce(JSValueExtensionsBuilder.NullIfTrue(target), value);
        }

        public static Expression LogicalOr(Expression target, Expression value)
        {
            return Expression.Coalesce(JSValueExtensionsBuilder.NullIfFalse(target), value);
        }

        private static MethodInfo _GetAllKeys =
            InternalMethod<bool, bool>(nameof(JSValue.GetAllKeys));

        private static MethodInfo _GetEnumerator =
            typeof(IEnumerable<JSValue>).GetMethod(nameof(IEnumerable<JSValue>.GetEnumerator));

        public static Expression GetAllKeys(Expression target)
        {
            return
                Expression.Call(
                    Expression.Call(target, _GetAllKeys, Expression.Constant(false), Expression.Constant(true)),
                    _GetEnumerator);
        }

    }
}
