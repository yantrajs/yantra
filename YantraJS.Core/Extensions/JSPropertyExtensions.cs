using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Extensions
{
    internal static class JSPropertyExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSValue GetValue(this JSValue target, in JSProperty p)
        {
            if (p.IsEmpty)
                return JSUndefined.Value;
            return !p.IsProperty
                ? p.value
                : p.get.f(new Arguments(target));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSValue ToJSValue(in this JSProperty px)
        {
            var t = JSBoolean.True;
            var f = JSBoolean.False;
            JSObject obj;
            if (px.IsValue)
            {
                obj = JSObject.NewWithProperties()
                    .AddProperty(KeyStrings.configurable, px.IsConfigurable ? t : f)
                    .AddProperty(KeyStrings.enumerable, px.IsEnumerable ? t : f)
                    .AddProperty(KeyStrings.writable, !px.IsReadOnly ? t : f)
                    .AddProperty(KeyStrings.value, px.value);
            } else
            {
                obj = JSObject.NewWithProperties()
                    .AddProperty(KeyStrings.configurable, px.IsConfigurable ? t : f)
                    .AddProperty(KeyStrings.enumerable, px.IsEnumerable ? t : f)
                    .AddProperty(KeyStrings.@get, px.get)
                    .AddProperty(KeyStrings.@set, px.set);
            }
            return obj;
        }
    }
}
