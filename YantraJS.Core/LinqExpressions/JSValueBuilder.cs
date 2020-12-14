using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using YantraJS.Core;

namespace YantraJS.ExpHelper
{
    public class JSValueBuilder
    {
        private static readonly Type type = typeof(JSValue);

        private static PropertyInfo _DoubleValue =
            type.Property(nameof(Core.JSValue.DoubleValue));

        private static PropertyInfo _lengthProperty
            = type.Property(nameof(Core.JSValue.Length));

        public static Expression Length(Expression target)
        {
            return Expression.Property(target, _lengthProperty);
        }

        public static Expression DoubleValue(Expression exp)
        {
            return Expression.Property(exp, _DoubleValue);
        }

        private static PropertyInfo _BooleanValue =
            type.Property(nameof(JSValue.BooleanValue));
        public static Expression BooleanValue(Expression exp)
        {
            return Expression.Property(exp, _BooleanValue);
        }


        private static MethodInfo _Add =
            type.InternalMethod(nameof(Core.JSValue.AddValue), typeof(JSValue));

        public static Expression Add(Expression target, Expression value)
        {
            return Expression.Call(target, _Add, value);
        }

        private static MethodInfo _TypeOf =
            type.GetMethod(nameof(JSValue.TypeOf));

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

        private static MethodInfo _DeleteKeyString
            = type.InternalMethod(nameof(JSValue.Delete), typeof(KeyString));
        private static MethodInfo _DeleteUInt
            = type.InternalMethod(nameof(JSValue.Delete), typeof(uint));
        private static MethodInfo _DeleteJSValue
            = type.InternalMethod(nameof(JSValue.Delete), typeof(JSValue));

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
            = type.GetMethod(nameof(JSValue.CreateInstance));

        public static Expression CreateInstance(Expression target, Expression args)
        {
            return Expression.Call(target, _CreateInstance, args);
        }

        internal static MethodInfo StaticEquals
            = type.InternalMethod(nameof(Core.JSValue.StaticEquals), typeof(JSValue), typeof(JSValue));


        private static MethodInfo _Equals
            = type.InternalMethod(nameof(Core.JSValue.Equals), typeof(JSValue));

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
            = type.InternalMethod(nameof(Core.JSValue.StrictEquals), typeof(JSValue));

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
            = type.InternalMethod(nameof(Core.JSValue.Less), typeof(JSValue));

        public static Expression Less(Expression target, Expression value)
        {
            return Expression.Call(target, _Less, value);
        }

        private static MethodInfo _LessOrEqual
            = type.InternalMethod(nameof(Core.JSValue.LessOrEqual), typeof(JSValue));

        public static Expression LessOrEqual(Expression target, Expression value)
        {
            return Expression.Call(target, _LessOrEqual, value);
        }

        private static MethodInfo _Greater
            = type.InternalMethod(nameof(Core.JSValue.Greater), typeof(JSValue));
        public static Expression Greater(Expression target, Expression value)
        {
            return Expression.Call(target, _Greater, value);
        }

        private static MethodInfo _GreaterOrEqual
            = type.InternalMethod(nameof(Core.JSValue.GreaterOrEqual), typeof(JSValue));
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
            type.InternalMethod(nameof(JSValue.GetAllKeys), typeof(bool), typeof(bool));

        private static MethodInfo _GetEnumerator =
            typeof(IEnumerable<JSValue>).GetMethod(nameof(IEnumerable<JSValue>.GetEnumerator));

        public static Expression GetAllKeys(Expression target)
        {
            return
                    // Expression.Call(
                    Expression.Call(target, _GetAllKeys, Expression.Constant(true), Expression.Constant(false))
                    // ,
                    //_GetEnumerator);
                    ;
        }

        private static MethodInfo _ConvertTo =
            type.InternalMethod(nameof(JSValue.TryConvertTo), typeof(Type), typeof(object).MakeByRefType());

        private static MethodInfo _ForceConvert =
            type.InternalMethod(nameof(JSValue.ForceConvert), typeof(Type));

        private static MethodInfo _Convert =
    type.InternalMethod(nameof(JSValue.Convert), typeof(Type), typeof(object));

        public static Expression Convert(Expression jsValue, Type type, Expression defaultValue)
        {
            return Expression.Convert(Expression.Call(jsValue, _Convert, Expression.Constant(type), defaultValue), type);
        }


        public static Expression ForceConvert(Expression jsValue, Type type)
        {
            return Expression.Convert( Expression.Call(jsValue, _ForceConvert, Expression.Constant(type)), type);
        }

        public static Expression ConvertTo(Expression jsValue, Expression type, Expression outVar)
        {
            return Expression.Call(jsValue, _ConvertTo, type, outVar);
        }

        public static Expression ConvertTo(Expression jsValue, Type type, Expression outVar)
        {
            return ConvertTo(jsValue, Expression.Constant(type), outVar);
        }

        public static Expression Coalesce(
            Expression jsValue, 
            Type type, 
            Expression outVar,
            string memberName,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return Expression.Condition(
                ConvertTo(jsValue, Expression.Constant(type), outVar), 
                // true
                outVar,
                // false
                JSExceptionBuilder.Throw(
                    $"{type.Name}.prototype.{memberName} called with object not of type {type}", 
                    type,
                    function,
                    filePath,
                    line));
        }

    }
}
