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
            var typeName = type.ToClrName();
            switch(typeName)
            {
                case "JSValue":
                case "YantraJS.Core.JSValue":
                    return name;
                case "JSNumber":
                case "YantraJS.Core.JSNumber":
                case "YantraJS.Core.JSNumber?":
                    return $"JSValueToClrConverter.ToJSNumber({name}, \"{parameter}\")";
                case "JSObject":
                case "YantraJS.Core.JSObject":
                    return $"{name} is JSObject obj{parameter} ? obj{parameter} : throw new JSException(\"{parameter} is not an object\")";
                case "JSFunction":
                case "YantraJS.Core.JSFunction":
                case "JSFunction?":
                case "YantraJS.Core.JSFunction?":
                    return $"{name} is JSFunction obj{parameter} ? obj{parameter} : throw new JSException(\"{parameter} is not a function\")";
                case "Int32":
                case "int":
                    return $"JSValueToClrConverter.ToInt({name}, \"{parameter}\")";
                case "int?":
                    return $"JSValueToClrConverter.ToNullableInt({name}, \"{parameter}\")";
                case "long":
                case "Int64":
                    return $"JSValueToClrConverter.ToLong({name}, \"{parameter}\")";
                case "long?":
                    return $"JSValueToClrConverter.ToNullableLong({name}, \"{parameter}\")";
                case "short":
                    return $"JSValueToClrConverter.ToShort({name}, \"{parameter}\")";
                case "short?":
                    return $"JSValueToClrConverter.ToNullableShort({name}, \"{parameter}\")";
                case "byte":
                case "Byte":
                    return $"JSValueToClrConverter.ToByte({name}, \"{parameter}\")";
                case "byte?":
                    return $"JSValueToClrConverter.ToNullableByte({name}, \"{parameter}\")";
                case "sbyte":
                case "SByte":
                    return $"JSValueToClrConverter.ToSByte({name}, \"{parameter}\")";
                case "sbyte?":
                    return $"JSValueToClrConverter.ToNullableSByte({name}, \"{parameter}\")";
                case "double":
                case "Double":
                    return $"JSValueToClrConverter.ToDouble({name}, \"{parameter}\")";
                case "double?":
                    return $"JSValueToClrConverter.ToNullableDouble({name}, \"{parameter}\")";
                case "Single":
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
            return $"JSValueToClrConverter.GetAsOrThrow<{typeName}>({name}, \"{parameter}\")";
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
            switch(name)
            {
                case "case":
                case "catch":
                case "finally":
                case "return":
                case "throw":
                case "is":
                case "for":
                    name = "@" + name;
                    break;
            }

            if (!list.Contains(name))
            {
                list.Add(name);
            }
            return className + "." + name;
        }
    }
}
