using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
{
    public static class JSStringPrototype
    {

        private static JSString AsJSString(this JSValue v, 
            [CallerMemberName] string helper = null)
        {
            if (v.IsUndefined || v.IsNull)
                throw JSContext.Current.NewTypeError($"String.prototype.{helper} called on null or undefined");
            if (v is JSString str)
                return str;
            return new JSString(v.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string AsString(this JSValue v,
            [CallerMemberName] string helper = null)
        {
            if (v.IsUndefined || v.IsNull)
                throw JSContext.Current.NewTypeError($"String.prototype.{helper} called on null or undefined");
            return v.ToString();
        }

        [Prototype("charAt")]
        public static JSValue CharAt(in Arguments a)
        {
            var text = AsString(a.This);
            var at = a.TryGetAt(0, out var n) ? n.IntValue : 0;
            return new JSString(new string(text[at], 1));
        }

        [Prototype("substring")]
        public static JSValue Substring(in Arguments a) 
        {
            var j = a.This as JSString;
            if (j == null)
                return JSUndefined.Value;
            if (!a.TryGetAt(0, out var start))
                return a.This;
            if (!a.TryGetAt(1, out var length))
                return new JSString(j.value.Substring(start.IntValue));
            return new JSString(j.value.Substring(start.IntValue, length.IntValue));
        }

        [Prototype("substr")]
        public static JSValue Substr(in Arguments a)
        {
            return Substring(a);
        }

        [Prototype("toString")]
        public static JSValue ToString(in Arguments a)
        {
            return a.This;
        }



        [GetProperty("length")]
        internal static JSValue GetLength(in Arguments a)
        {
            return new JSNumber(a.This.ToString().Length);
        }

        [SetProperty("length")]
        internal static JSValue SetLength(in Arguments a)
        {
            return a.Get1();
        }

    }
}
