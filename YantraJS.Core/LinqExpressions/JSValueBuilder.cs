using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using YantraJS.Core;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;


namespace YantraJS.ExpHelper
{
    public class JSValueBuilder
    {
        private static readonly Type type = typeof(JSValue);

        private static PropertyInfo _DoubleValue =
            type.Property(nameof(Core.JSValue.DoubleValue));

        private static PropertyInfo _IntValue =
            type.Property(nameof(Core.JSValue.IntValue));


        private static PropertyInfo _IsNullOrUndefined
            = type.Property(nameof(Core.JSValue.IsNullOrUndefined));

        private static MethodInfo _ToKey
            = type.InternalMethod(nameof(Core.JSValue.ToKey), typeof(bool));

        private static MethodInfo _Power
            = type.PublicMethod(nameof(Core.JSValue.Power), typeof(JSValue));

        private static MethodInfo _ValueOf
            = type.PublicMethod(nameof(Core.JSValue.ValueOf));

        public static Expression ToKey(Expression exp)
        {
            return Expression.Call(exp, _ToKey, Expression.Constant(true));
        }

        public static Expression IsNullOrUndefined(Expression target)
        {
            if (target.Type == typeof(JSVariable))
                target = JSVariable.ValueExpression(target);
            return Expression.Property(target, _IsNullOrUndefined);
        }

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

        public static Expression IntValue(Expression exp)
        {
            return Expression.Property(exp, _IntValue);
        }

        public static Expression Power(Expression left,Expression right) {
            return Expression.Call(left, _Power, right);
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
                    type.PublicIndex(typeof(JSObject), typeof(KeyString));

        private static PropertyInfo _SuperIndexUInt =
                    type.PublicIndex(typeof(JSObject), typeof(uint));

        private static PropertyInfo _SuperIndex =
                    type.PublicIndex(typeof(JSObject), typeof(JSValue));

        private static MethodInfo _PropertyOrUndefinedKeyString =
            type.PublicMethod(nameof(JSValue.PropertyOrUndefined), KeyStringsBuilder.RefType);
        private static MethodInfo _PropertyOrUndefinedUInt =
            type.PublicMethod(nameof(JSValue.PropertyOrUndefined), typeof(uint));
        private static MethodInfo _PropertyOrUndefined =
            type.PublicMethod(nameof(JSValue.PropertyOrUndefined), typeof(JSValue));
        private static MethodInfo _SuperPropertyOrUndefinedKeyString =
            type.PublicMethod(nameof(JSValue.PropertyOrUndefined), typeof(JSObject), KeyStringsBuilder.RefType);
        private static MethodInfo _SuperPropertyOrUndefinedUInt =
            type.PublicMethod(nameof(JSValue.PropertyOrUndefined), typeof(JSObject), typeof(uint));
        private static MethodInfo _SuperPropertyOrUndefined =
            type.PublicMethod(nameof(JSValue.PropertyOrUndefined), typeof(JSObject), typeof(JSValue));

        public static Expression Index(Expression target, Expression super, uint i, bool coalesce = false)
        {
            if (super == null)
            {
                return Index(target, i, coalesce);
            }
            return Expression.MakeIndex(target, _SuperIndexUInt, new Expression[] { super, Expression.Constant(i) });
        }


        public static Expression Index(Expression target, uint i, bool coalesce = false)
        {

            return Expression.MakeIndex(target, _IndexUInt, new Expression[] { Expression.Constant(i) });
        }


        public static Expression Index(Expression target, Expression super, Expression property, bool coalesce = false)
        {
            if (super == null)
            {
                return Index(target, property, coalesce);
            }
            if (property.Type == typeof(KeyString))
            {
                if (coalesce)
                {
                    return Expression.Call(target, _SuperPropertyOrUndefinedKeyString, super, property);
                }
                return Expression.MakeIndex(target, _SuperIndexKeyString, new Expression[] { super, property });
            }
            if (property.Type == typeof(uint))
            {
                if (coalesce)
                {
                    return Expression.Call(target, _SuperPropertyOrUndefinedUInt, super, property);
                }
                return Expression.MakeIndex(target, _SuperIndexUInt, new Expression[] { super, property });
            }
            if (property.Type == typeof(int))
            {
                if (coalesce)
                {
                    return Expression.Call(target, _SuperPropertyOrUndefinedUInt, super, Expression.Convert(property, typeof(uint)));
                }
                return Expression.MakeIndex(target, _SuperIndexUInt, new Expression[] { super, Expression.Convert(property, typeof(uint)) });
            }
            if (coalesce)
            {
                return Expression.Call(target, _SuperPropertyOrUndefined, super, Expression.Convert(property, typeof(uint)));
            }
            return Expression.MakeIndex(target, _SuperIndex, new Expression[] { super, property });
        }


        public static Expression Index(Expression target, Expression property, bool coalesce = false)
        {
            if (property.Type == typeof(KeyString))
            {
                if(coalesce)
                {
                    return Expression.Call(target, _PropertyOrUndefinedKeyString, property);
                }
                return Expression.MakeIndex(target, _IndexKeyString, new Expression[] { property });
            }
            if (property.Type == typeof(uint))
            {
                if (coalesce)
                {
                    return Expression.Call(target, _PropertyOrUndefinedUInt, property);
                }
                return Expression.MakeIndex(target, _IndexUInt, new Expression[] { property });
            }
            if (property.Type == typeof(int))
            {
                if (coalesce)
                {
                    return Expression.Call(target, _PropertyOrUndefinedUInt, Expression.Convert(property, typeof(uint)));
                }
                return Expression.MakeIndex(target, _IndexUInt, new Expression[] { Expression.Convert(property, typeof(uint)) });
            }
            if (coalesce)
            {
                return Expression.Call(target, _PropertyOrUndefined, property);
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
            = type.PublicMethod(nameof(Core.JSValue.StaticEquals), typeof(JSValue), typeof(JSValue));


        private static MethodInfo _Equals
            = type.PublicMethod(nameof(Core.JSValue.Equals), typeof(JSValue));

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
            = type.PublicMethod(nameof(Core.JSValue.Less), typeof(JSValue));

        public static Expression Less(Expression target, Expression value)
        {
            return Expression.Call(ValueOf(target), _Less, ValueOf(value));
        }

        private static MethodInfo _LessOrEqual
            = type.PublicMethod(nameof(Core.JSValue.LessOrEqual), typeof(JSValue));

        public static Expression LessOrEqual(Expression target, Expression value)
        {
            return Expression.Call(ValueOf(target), _LessOrEqual, ValueOf(value));
        }

        private static MethodInfo _Greater
            = type.PublicMethod(nameof(Core.JSValue.Greater), typeof(JSValue));
        public static Expression Greater(Expression target, Expression value)
        {
            return Expression.Call(ValueOf(target), _Greater, ValueOf(value));
        }

        private static MethodInfo _GreaterOrEqual
            = type.PublicMethod(nameof(Core.JSValue.GreaterOrEqual), typeof(JSValue));
        public static Expression GreaterOrEqual(Expression target, Expression value)
        {
            return Expression.Call(ValueOf(target), _GreaterOrEqual, ValueOf(value));
        }

        public static Expression ValueOf(Expression target) {
            return Expression.Call(target, _ValueOf);
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
            type.PublicMethod(nameof(JSValue.GetAllKeys), typeof(bool), typeof(bool));

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
            type.PublicMethod(nameof(JSValue.ForceConvert), typeof(Type));

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

        public static Expression Coalesce(Expression target, Expression def)
        {
            return Expression.Condition(
                JSValueBuilder.IsNullOrUndefined(target), def, target);
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
