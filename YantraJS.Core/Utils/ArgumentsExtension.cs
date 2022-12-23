using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.ExpHelper;
using YantraJS.Expressions;

namespace YantraJS.Core
{
    internal interface IJSValueConverter
    {
        YExpression FromJSValue(string name, YExpression value, YExpression defaultValue);
    }

    internal class JSValueConverter<T> : IJSValueConverter
    {
        private readonly Func<JSValue,string,T> converter;
        private readonly Func<JSValue, T, T> defConverter;

        public JSValueConverter(
            Func<JSValue,string, T> converter,
            Func<JSValue, T, T> defConverter)
        {
            this.converter = converter;
            this.defConverter = defConverter;
        }

        public YExpression FromJSValue(string name, YExpression value, YExpression defaultValue)
        {
            return defaultValue == null
                ? YExpression.Invoke(YExpression.Constant(converter),value, YExpression.Constant(name))
                : YExpression.Invoke(YExpression.Constant(defConverter), value, defaultValue);
        }
    }

    internal static class ArgumentsExtension
    {
        private static bool HasValue(this JSValue v)
        {
            return v != null && !v.IsNullOrUndefined;
        }

        private static Dictionary<Type, IJSValueConverter> methods = new Dictionary<Type, IJSValueConverter>();

        private static MethodInfo GetAsGeneric = typeof(ArgumentsExtension).GetMethod(nameof(GetAs));

        private static Type nullableType = typeof(Nullable<>);

        static ArgumentsExtension()
        {
            //var type = typeof(ArgumentsExtension);
            //foreach(var method in type.GetMethods())
            //{
            //    if(method.IsGenericMethod || method.IsGenericMethodDefinition)
            //    {
            //        continue;
            //    }
            //    var ps = method.GetParameters();
            //    if (ps.Length != 2)
            //        continue;
            //    if (ps[0].ParameterType != ArgumentsBuilder.refType)
            //        continue;
            //    if (ps[1].ParameterType != typeof(int))
            //        continue;
            //    var rt = Nullable.GetUnderlyingType(method.ReturnType) ?? method.ReturnType;
            //    methods[rt] = method;
            //}

            Add<int>(
                (x, name) => x.HasValue() ? x.IntValue : throw new ArgumentException($"{name} is required"),
                (x, def) => x.HasValue() ? x.IntValue : def);
            Add<int?>(
                (x, name) => x.HasValue() ? x.IntValue : null,
                (x, def) => x.HasValue() ? x.IntValue : def);
            Add<bool>(
                (x, name) => x.HasValue() ? x.BooleanValue : throw new ArgumentException($"{name} is required"),
                (x, def) => x.HasValue() ? x.BooleanValue : def);
            Add<bool?>(
                (x, name) => x.HasValue() ? x.BooleanValue : null,
                (x, def) => x.HasValue() ? x.BooleanValue : def);
            Add<long>(
                (x, name) => x.HasValue() ? x.BigIntValue : throw new ArgumentException($"{name} is required"),
                (x, def) => x.HasValue() ? x.BigIntValue : def);
            Add<long?>(
                (x, name) => x.HasValue() ? x.BigIntValue : null,
                (x, def) => x.HasValue() ? x.BigIntValue : def);
            Add<double>(
                (x, name) => x.HasValue() ? x.DoubleValue: throw new ArgumentException($"{name} is required"),
                (x, def) => x.HasValue() ? x.DoubleValue : def);
            Add<double?>(
                (x, name) => x.HasValue() ? x.DoubleValue : null,
                (x, def) => x.HasValue() ? x.DoubleValue : def);
            Add<float>(
                (x, name) => x.HasValue() ? (float)x.DoubleValue : throw new ArgumentException($"{name} is required"),
                (x, def) => x.HasValue() ? (float)x.DoubleValue : def);
            Add<float?>(
                (x, name) => x.HasValue() ? (float?)x.DoubleValue : null,
                (x, def) => x.HasValue() ? (float?)x.DoubleValue : def);
            Add<string>(
                (x, name) => x.HasValue() ? x.ToString() : throw new ArgumentException($"{name} is required"),
                (x, def) => x.HasValue() ? x.ToString() : def);
            Add<DateTime>(
                (x, name) => x.HasValue() && x is JSDate d? d.DateTime : throw new ArgumentException($"{name} is required"),
                (x, def) => x.HasValue() && x is JSDate d? d.DateTime : def);
            Add<DateTime?>(
                (x, name) => x.HasValue() && x is JSDate d ? d.DateTime : null,
                (x, def) => x.HasValue() && x is JSDate d ? d.DateTime : def);
            Add<DateTimeOffset>(
                (x, name) => x.HasValue() && x is JSDate d ? d.value : throw new ArgumentException($"{name} is required"),
                (x, def) => x.HasValue() && x is JSDate d ? d.value : def);
            Add<DateTimeOffset?>(
                (x, name) => x.HasValue() && x is JSDate d ? d.value : null,
                (x, def) => x.HasValue() && x is JSDate d ? d.value : def);
        }

        private static IJSValueConverter Add<T>(
            Func<JSValue,string,T> force,
            Func<JSValue,T,T> convertOrDefault)
        {
            return new JSValueConverter<T>(force, convertOrDefault);
        }

        public static YExpression GetArgument(
            YExpression args,
            int index,
            Type type,
            YExpression defaultValue,
            string name)
        {
            var isNullable = Nullable.GetUnderlyingType(type) != null;
            if(methods.TryGetValue(type, out var method))
            {
                return method.FromJSValue(name, ArgumentsBuilder.GetAt(args, index), defaultValue);
            }
            var m = GetAsGeneric.MakeGenericMethod(type);
            return YExpression.Coalesce(
                            YExpression.Call(null, m, args, YExpression.Constant(index)),
                            JSExceptionBuilder.New($"{name} is required"));
        }

        public static T GetAs<T>(in Arguments a, int index)
        {
            var v = a[index];
            return v is ClrProxy js
                ? (T)js.value
                : v.ConvertTo<T>(out T v1)
                    ? v1
                    : default;
        }
    }
}
