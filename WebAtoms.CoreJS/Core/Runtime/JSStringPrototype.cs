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
    public partial class JSString
    {
        private static JSString AsJSString(JSValue v, 
            [CallerMemberName] string helper = null)
        {
            switch(v)
            {
                case JSNull _:
                case JSUndefined __:
                    throw JSContext.Current.TypeError($"String.prototype.{helper} called on null or undefined");
                case JSString @string:
                    return @string;
            }
            return new JSString(v.ToString());
        }

        private static string AsString(JSValue v,
            [CallerMemberName] string helper = null)
        {
            switch (v)
            {
                case JSNull _:
                case JSUndefined __:
                    throw JSContext.Current.TypeError($"String.prototype.{helper} called on null or undefined");
                case JSString @string:
                    return @string.value;
            }
            return v.ToString();
        }

        [Prototype("charAt")]
        public static JSValue CharAt(JSValue t, JSValue[] a)
        {
            var text = AsString(t);
            var at = a.TryGetAt(0, out var n) ? n.IntValue : 0;
            return new JSString(new string(text[at], 1));
        }

        [Prototype("substring")]
        public static JSValue Substring(JSValue t,params JSValue[] a) 
        {
            var j = t as JSString;
            if (j == null)
                return JSUndefined.Value;
            if (!a.TryGetAt(0, out var start))
                return start;
            if (!a.TryGetAt(1, out var length))
                return new JSString(j.value.Substring(start.IntValue));
            return new JSString(j.value.Substring(start.IntValue, length.IntValue));
        }

        [Prototype("substr")]
        public static JSValue Substr(JSValue t,params JSValue[] a)
        {
            return Substring(t, a);
        }

        [Prototype("toString")]
        public static JSValue ToString(JSValue t,params JSValue[] a)
        {
            return t;
        }



        [GetProperty("length")]
        internal static JSValue GetLength(JSValue t,params JSValue[] a)
        {
            return new JSNumber(((JSString)t).value.Length);
        }

        [SetProperty("length")]
        internal static JSValue SetLength(JSValue t,params JSValue[] a)
        {
            return a[0];
        }

    }
}
