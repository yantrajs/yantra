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
            var t = ctx.True;
            var f = ctx.False;
            JSObject obj;
            if (px.IsValue)
            {
                obj = new JSObject(
                    JSProperty.Property(JSProperty.KeyConfigurable, px.IsConfigurable ? t : f),
                    JSProperty.Property(JSProperty.KeyEnumerable, px.IsEnumerable ? t : f),
                    JSProperty.Property(JSProperty.KeyWritable, !px.IsReadOnly ? t : f),
                    JSProperty.Property(JSProperty.KeyValue, px.value)
                    );
            } else
            {
                obj = new JSObject(
                    JSProperty.Property(JSProperty.KeyConfigurable, px.IsConfigurable ? t : f),
                    JSProperty.Property(JSProperty.KeyEnumerable, px.IsEnumerable ? t : f),
                    JSProperty.Property(JSProperty.KeyGet, px.get),
                    JSProperty.Property(JSProperty.KeySet, px.set)
                    );
            }
            return obj;
        }
    }
}
