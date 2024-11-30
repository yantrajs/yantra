using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Primitive;
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

        public static string ToString(JSValue value, string name)
        {
            return value.HasValue() ? value.ToString() : throw new JSException($"{name} is required");
        }


        public static JSNumber ToJSNumber(this JSValue value, string name)
        {
            return value is JSNumber n
                ? n
                : (value is JSPrimitiveObject po
                    ? po.value.ToJSNumber(name)
                    : throw new JSException($"{name} is not a number"));
        }
        public static bool ToBoolean(JSValue value, string name)
        {
            return value.HasValue() ? value.BooleanValue: throw new JSException($"{name} is required");
        }
        public static bool? ToNullableBoolean(JSValue value, string name)
        {
            return value.IsNullOrUndefined ? null : value.BooleanValue;
        }

        public static long ToLong(JSValue value, string name)
        {
            return value.HasValue() ? value.BigIntValue: throw new JSException($"{name} is required");
        }

        public static long? ToNullableLong(JSValue value, string name)
        {
            return value.IsNullOrUndefined ? null : value.BigIntValue;
        }
        public static int ToInt(JSValue value, string name)
        {
            return value.HasValue() ? value.IntValue : throw new JSException($"{name} is required");
        }
        public static int? ToNullableInt(JSValue value, string name)
        {
            return value.IsNullOrUndefined ? null :value.IntValue;
        }

        public static short ToShort(JSValue value, string name)
        {
            return value.HasValue() ? (short)value.IntValue : throw new JSException($"{name} is required");
        }
        public static short? ToNullableShort(JSValue value, string name)
        {
            return value.IsNullOrUndefined ? null : (short)value.IntValue;
        }

        public static byte ToByte(JSValue value, string name)
        {
            return value.HasValue() ? (byte)value.IntValue : throw new JSException($"{name} is required");
        }
        public static byte? ToNullableByte(JSValue value, string name)
        {
            return value.IsNullOrUndefined ? null : (byte)value.IntValue;
        }
        public static sbyte ToSByte(JSValue value, string name)
        {
            return value.HasValue() ? (sbyte)value.IntValue : throw new JSException($"{name} is required");
        }
        public static sbyte? ToNullableSByte(JSValue value, string name)
        {
            return value.IsNullOrUndefined ? null : (sbyte)value.IntValue;
        }

        public static double ToDouble(JSValue value, string name)
        {
            return value.HasValue() ? value.DoubleValue: throw new JSException($"{name} is required");
        }
        public static double? ToNullableDouble(JSValue value, string name)
        {
            return value.IsNullOrUndefined ? null : value.DoubleValue;
        }
        public static float ToFloat(JSValue value, string name)
        {
            return value.HasValue() ? (float)value.DoubleValue : throw new JSException($"{name} is required");
        }
        public static float? ToNullableFloat(JSValue value, string name)
        {
            return value.IsNullOrUndefined ? null : (float)value.DoubleValue;
        }
        public static decimal ToDecimal(JSValue value, string name)
        {
            return value.HasValue() ? (decimal)value.DoubleValue : throw new JSException($"{name} is required");
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

        private static MethodInfo GetAsOrThrowGeneric = typeof(JSValueToClrConverter).GetMethod(nameof(GetAsOrThrow));

        private static Type nullableType = typeof(Nullable<>);

        static JSValueToClrConverter()
        {
            foreach(var method in typeof(JSValueToClrConverter).GetMethods())
            {
                if (!method.Name.StartsWith("To"))
                    continue;
                if (!method.IsStatic)
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
            return Get(ArgumentsBuilder.GetAt(args,index), type, defaultValue, $"{name} is required");
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
            var m = GetAsOrThrowGeneric.MakeGenericMethod(type);
            return YExpression.Call(null, m, target, YExpression.Constant($"{name} is required"));
        }

        public static YExpression Get(YExpression target, Type type, YExpression defaultValue, string name)
        {
            if (defaultValue == null)
            {
                return Get(target, type, name);
            }
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
                            defaultValue);
        }


        public static Func<JSValue, string, T> ToFastClrDelegate<T>()
        {
            var type = typeof(T);
            if (methods.TryGetValue(type, out var m))
            {
                return m.CreateDelegate<Func<JSValue, string, T>>();
            }
            return (v,n) => GetAsOrThrow<T>(v, n);
        }

        public static T ToFastClrValue<T>(this JSValue value)
        {
            var type = typeof(T);
            if (typeof(JSValue).IsAssignableFrom(type))
            {
                return (T)(object)value;
            }

            if(methods.TryGetValue(type, out var m))
            {
                var f = m.CreateDelegate<Func<JSValue, string, T>>();
                return f(value, "");
            }
            if (value.ConvertTo<T>(out var v))
                return v;
            throw new JSException($"Failed to convert JSValue to {type.Name}");
        }

        public static T GetAs<T>(JSValue value)
        {
            return value.ConvertTo<T>(out T v1)
                    ? v1
                    : default;
        }

        public static T GetAsOrThrow<T>(JSValue value, string error)
        {
            return value.ConvertTo<T>(out T v1)
                    ? v1
                    : throw new JSException(error);
        }

    }
}
