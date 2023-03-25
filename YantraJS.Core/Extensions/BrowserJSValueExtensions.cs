#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using YantraJS.Core;
using YantraJS.Core.Clr;

#if !NETSTANDARD2_1
namespace System.Diagnostics.CodeAnalysis
{
    public class NotNullWhenAttribute : Attribute
    {

        public NotNullWhenAttribute(bool value)
        {

        }

    }
}
#endif


namespace YantraJS.Core
{
    public static class BrowserJSValueExtensions
    {

        public static bool TryGetProperty(
            this JSValue? value,
            KeyString name,
            [NotNullWhen(true)]
            out JSValue? property)
        {
            if (value == null)
            {
                property = null;
                return false;
            }
            var v = value.GetValue(name.Key, value, false);
            if (v.IsNull || v.IsUndefined)
            {
                property = null;
                return false;
            }
            property = v;
            return true;
        }

        public static T Get<T>(
            in this Arguments a,
            int index,
            string name = "Argument")
        {
            var value = a[index] ?? throw new ArgumentException($"{name} is required");
            if (value is ClrProxy proxy)
            {
                return (T)proxy.Target;
            }
            if (value.ConvertTo<T>(out var item))
            {
                return item;
            }
            throw new InvalidCastException();
        }

        public static string GetString(
            in this Arguments a,
            int index,
            string name = "Argument")
        {
            var value = a[index] ?? throw new ArgumentException($"{name} is required");
            return value.ToString();
        }
    }
}
