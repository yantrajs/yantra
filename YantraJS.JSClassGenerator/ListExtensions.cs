using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace YantraJS.JSClassGenerator
{
    internal static class ListExtensions
    {
        public static string ClrProxyMarshal(this string target, ITypeSymbol type, string value) {
            if (type.Name == "JSValue")
            {
                return target;
            }
            return $"ClrProxy.Marshal({target})";
        }

        public static string ToJSValueFromClr(this string name, ITypeSymbol type, string parameter)
        {
            switch(type.Name)
            {
                case "JSValue":
                    return name;
                case "Int32":
                case "int":
                    return $"JSValueToClrConverter.ToInt({name}, \"{parameter}\")";
                case "int?":
                    return $"JSValueToClrConverter.ToNullableInt({name}, \"{parameter}\")";
                case "long":
                    return $"JSValueToClrConverter.ToLong({name}, \"{parameter}\")";
                case "long?":
                    return $"JSValueToClrConverter.ToNullableLong({name}, \"{parameter}\")";
                case "short":
                    return $"JSValueToClrConverter.ToShort({name}, \"{parameter}\")";
                case "short?":
                    return $"JSValueToClrConverter.ToNullableShort({name}, \"{parameter}\")";
                case "byte":
                    return $"JSValueToClrConverter.ToByte({name}, \"{parameter}\")";
                case "byte?":
                    return $"JSValueToClrConverter.ToNullableByte({name}, \"{parameter}\")";
                case "sbyte":
                    return $"JSValueToClrConverter.ToSByte({name}, \"{parameter}\")";
                case "sbyte?":
                    return $"JSValueToClrConverter.ToNullableSByte({name}, \"{parameter}\")";
                case "double":
                    return $"JSValueToClrConverter.ToDouble({name}, \"{parameter}\")";
                case "double?":
                    return $"JSValueToClrConverter.ToNullableDouble({name}, \"{parameter}\")";
                case "float":
                    return $"JSValueToClrConverter.ToFloat({name}, \"{parameter}\")";
                case "float?":
                    return $"JSValueToClrConverter.ToNullableFloat({name}, \"{parameter}\")";
                case "Boolean":
                case "bool":
                    return $"JSValueToClrConverter.ToBoolean({name}, \"{parameter}\")";
                case "bool?":
                    return $"JSValueToClrConverter.ToNullableBoolean({name}, \"{parameter}\")";
                case "string":
                case "String":
                    return $"JSValueToClrConverter.ToString({name}, \"{parameter}\")";
            }
            return $"JSValueToClrConverter.GetAsOrThrow<{type.Name}>({name}, \"{parameter}\")";
        }

        public static bool IsJSFunction(this IMethodSymbol method)
        {
            return method.Parameters.Length == 1
                && method.Parameters[0] is IParameterSymbol p
                && p.RefKind == RefKind.In
                && p.Type.Name == "Arguments";
        }

        public static unsafe string ToCamelCase(this string @this)
        {
            var length = @this.Length;
            if (length == 0)
            {
                return string.Empty;
            }
            var d = new char[length];
            fixed (char* start = @this)
            {
                char* startOffset = start;
                int i;
                for (i = 0; i < length; i++)
                {
                    var ch = *(startOffset++);
                    d[i] = Char.ToLower(ch);
                    if (!Char.IsUpper(ch))
                    {
                        i++;
                        break;
                    }
                }
                for (; i < length; i++)
                {
                    d[i] = *(startOffset++);
                }
            }
            return new string(d);
        }
        public static string GetOrCreateName(this List<string> list, string name, string className = "Names")
        {
            if (!list.Contains(name))
            {
                list.Add(name);
            }
            return className + "." + name;
        }
    }
}
