using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Core.Date;
using YantraJS.ExpHelper;
using YantraJS.Expressions;

namespace YantraJS.Core
{
    public static class JSValueToClrConverter
    {
        private static bool HasValue(this JSValue value)
        {
            return value == null ? false : !value.IsNullOrUndefined;
        }

        public static int ToInt(JSValue value, string name)
        {
            return value.HasValue() ? value.IntValue : throw new ArgumentException($"{name} is required");
        }
        public static int? ToNullableInt(JSValue value, string name)
        {
            return value.IsNullOrUndefined ? null :value.IntValue;
        }

        public static short ToShort(JSValue value, string name)
        {
            return value.HasValue() ? (short)value.IntValue : throw new ArgumentException($"{name} is required");
        }
        public static short? ToNullableShort(JSValue value, string name)
        {
            return value.IsNullOrUndefined ? null : (short)value.IntValue;
        }

        public static byte ToByte(JSValue value, string name)
        {
            return value.HasValue() ? (byte)value.IntValue : throw new ArgumentException($"{name} is required");
        }
        public static byte? ToNullableByte(JSValue value, string name)
        {
            return value.IsNullOrUndefined ? null : (byte)value.IntValue;
        }
        public static sbyte ToSByte(JSValue value, string name)
        {
            return value.HasValue() ? (sbyte)value.IntValue : throw new ArgumentException($"{name} is required");
        }
        public static sbyte? ToNullableSByte(JSValue value, string name)
        {
            return value.IsNullOrUndefined ? null : (sbyte)value.IntValue;
        }

        public static double ToDouble(JSValue value, string name)
        {
            return value.HasValue() ? value.DoubleValue: throw new ArgumentException($"{name} is required");
        }
        public static double? ToNullableDouble(JSValue value, string name)
        {
            return value.IsNullOrUndefined ? null : value.DoubleValue;
        }
        public static float ToFloat(JSValue value, string name)
        {
            return value.HasValue() ? (float)value.DoubleValue : throw new ArgumentException($"{name} is required");
        }
        public static float? ToNullableFloat(JSValue value, string name)
        {
            return value.IsNullOrUndefined ? null : (float)value.DoubleValue;
        }
        public static decimal ToDecimal(JSValue value, string name)
        {
            return value.HasValue() ? (decimal)value.DoubleValue : throw new ArgumentException($"{name} is required");
        }
        public static decimal? ToNullableDecimal(JSValue value, string name)
        {
            return value.IsNullOrUndefined ? null : (decimal)value.DoubleValue;
        }
        public static DateTime ToDateTime(JSValue value, string name)
        {
            return value.HasValue()
                ? (value is JSDate date
                    ? date.DateTime
                    : DateTime.Parse(value.ToString()))
                : throw new ArgumentException($"{name} is required");
        }
        public static DateTime? ToNullableDateTime(JSValue value, string name)
        {
            return value.HasValue()
                ? (value is JSDate date
                    ? date.DateTime
                    : DateTime.Parse(value.ToString()))
                : null;
        }
        public static DateTimeOffset ToDateTimeOffset(JSValue value, string name)
        {
            return value.HasValue()
                ? (value is JSDate date
                    ? date.DateTime
                    : DateTime.Parse(value.ToString()))
                : throw new ArgumentException($"{name} is required");
        }
        public static DateTimeOffset? ToNullableDateTimeOffset(JSValue value, string name)
        {
            return value.HasValue()
                ? (value is JSDate date
                    ? date.DateTime
                    : DateTime.Parse(value.ToString()))
                : null;
        }
   

        private static Dictionary<Type, MethodInfo> methods = new Dictionary<Type, MethodInfo>();

        private static MethodInfo GetAsGeneric = typeof(JSValueToClrConverter).GetMethod(nameof(GetAs));

        private static Type nullableType = typeof(Nullable<>);

        static JSValueToClrConverter()
        {
            foreach(var method in typeof(JSValueToClrConverter).GetMethods())
            {
                if (!method.Name.StartsWith("To"))
                    continue;
                methods[method.ReturnType] = method;
            }
        }

        public static YExpression GetArgument(
            YExpression args,
            int index,
            Type type,
            YExpression defaultValue,
            string name)
        {
            if(methods.TryGetValue(type, out var method))
            {
                if (defaultValue == null)
                {
                    return YExpression.Call(null, method, ArgumentsBuilder.GetAt(args, index), YExpression.Constant(name));
                }
                return YExpression.Condition(
                    YExpression.Binary(
                        ArgumentsBuilder.Length(args),
                        YOperator.Greater,
                        YExpression.Constant(index)),
                    YExpression.Call(null, method, ArgumentsBuilder.GetAt(args, index), YExpression.Constant(name)),
                    defaultValue);
            }
            if (typeof(JSValue).IsAssignableFrom(type))
            {
                return ArgumentsBuilder.GetAt(args, index);
            }
            var m = GetAsGeneric.MakeGenericMethod(type);
            return YExpression.Coalesce(
                            YExpression.Call(null, m, args, YExpression.Constant(index)),
                            JSExceptionBuilder.New($"{name} is required"));
        }

        public static YExpression Get(YExpression target, Type type, string name)
        {
            if (typeof(JSValue).IsAssignableFrom(type))
            {
                return target;
            }
            if (methods.TryGetValue(type, out var method))
            {
                return YExpression.Call(null, method, target, YExpression.Constant(name));
            }
            var m = GetAsGeneric.MakeGenericMethod(type);
            return YExpression.Coalesce(
                            YExpression.Call(null, m, target),
                            JSExceptionBuilder.New($"{name} is required"));
        }

        public static T GetAs<T>(JSValue value)
        {
            return value.ConvertTo<T>(out T v1)
                    ? v1
                    : default;
        }
    }
}
