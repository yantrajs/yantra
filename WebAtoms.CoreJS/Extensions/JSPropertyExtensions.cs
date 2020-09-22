using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Extensions
{
    internal static class JSPropertyExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSValue GetValue(this JSValue target, JSProperty p)
        {
            if (p.IsEmpty)
                return JSUndefined.Value;
            return p.IsValue ? p.value : p.get.f(target, JSArguments.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSValue ToJSValue(this JSProperty px)
        {
            var ctx = JSContext.Current;
            var t = JSBoolean.True;
            var f = JSBoolean.False;
            JSObject obj;
            if (px.IsValue)
            {
                obj = new JSObject(
                    JSProperty.Property(KeyStrings.configurable, px.IsConfigurable ? t : f),
                    JSProperty.Property(KeyStrings.enumerable, px.IsEnumerable ? t : f),
                    JSProperty.Property(KeyStrings.writable, !px.IsReadOnly ? t : f),
                    JSProperty.Property(KeyStrings.value, px.value)
                    );
            } else
            {
                obj = new JSObject(
                    JSProperty.Property(KeyStrings.configurable, px.IsConfigurable ? t : f),
                    JSProperty.Property(KeyStrings.enumerable, px.IsEnumerable ? t : f),
                    JSProperty.Property(KeyStrings.@get, px.get),
                    JSProperty.Property(KeyStrings.@set, px.set)
                    );
            }
            return obj;
        }
    }
}
