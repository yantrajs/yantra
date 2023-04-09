#nullable enable
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.JSClassGenerator
{
    internal class JSExportInfo
    {
        public string? Name;
        public string? Length;
        public bool IsConstructor;
    }

    internal static class ITypeSymbolExtensions
    {

        public static JSExportInfo? GetExportAttribute(this IMethodSymbol method)
        {
            foreach(var a in method.GetAttributes())
            {
                if(a.AttributeClass?.Name?.StartsWith("JSExport") ?? false)
                {
                    var e = new JSExportInfo { };
                    e.Name = (a.ConstructorArguments.Length > 0 ? a.ConstructorArguments[0].Value?.ToString() ?? null : null)
                        ?? method.Name.ToCamelCase();
                    foreach(var kvp in a.NamedArguments)
                    {
                        if(kvp.Key == "Length")
                        {
                            e.Length = kvp.Value.Value?.ToString();
                        }
                        if(kvp.Key == "IsConstructor")
                        {
                            e.IsConstructor = true;
                        }
                    }
                    return e;
                }
            }
            return null;
        }

        public static bool IsConstructor(this IMethodSymbol method)
        {
            return method.Name == ".ctor";
        }

        public static string ToClrName(this ITypeSymbol type)
        {
            return type.ToDisplayString();
            //if(type.Name == "Nullable")
            //{
            //    var inner = type.ToDisplayString();
            //    return inner;
            //}
            //if (type.NullableAnnotation == NullableAnnotation.Annotated)
            //{
            //    var inner = type.OriginalDefinition.SpecialClrName();
            //    return inner + "?";
            //}
            //return type.SpecialClrName();
        }

        public static string SpecialClrName(this ITypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_Object:
                    return "object";
                case SpecialType.System_Void:
                    return "void";
                case SpecialType.System_Boolean:
                    return "bool";
                case SpecialType.System_Char:
                    return "char";
                case SpecialType.System_SByte:
                    return "sbyte";
                case SpecialType.System_Byte:
                    return "byte";
                case SpecialType.System_Int16:
                    return "short";
                case SpecialType.System_UInt16:
                    return "ushort";
                case SpecialType.System_Int32:
                    return "int";
                case SpecialType.System_UInt32:
                    return "uint";
                case SpecialType.System_Int64:
                    return "long";
                case SpecialType.System_UInt64:
                    return "ulong";
                case SpecialType.System_Decimal:
                    return "decimal";
                case SpecialType.System_Single:
                    return "float";
                case SpecialType.System_Double:
                    return "double";
                case SpecialType.System_String:
                    return "string";
                case SpecialType.System_DateTime:
                    return "DateTime";
            }
            return type.Name;
        }

    }
}
