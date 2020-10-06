using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
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

        [Prototype("charCodeAt")]
        internal static JSValue CharCodeAt(JSValue t, params JSValue[] a)
        {
            var text = AsString(t);
            var at = a.TryGetAt(0, out var n) ? n.IntValue : 0;
            return new JSString(new string(text[at], 1));
        }

        [Prototype("codePointAt")]
        internal static JSValue CodePointAt(JSValue t, params JSValue[] a)
        {
            return null;
        }

        [Prototype("concat")]
        internal static JSValue Concat(JSValue t, params JSValue[] a)
        {
            var j = t as JSString;
            if (j == null)
                return JSUndefined.Value;
            if (!a.TryGetAt(0, out var n))
                return JSUndefined.Value;
            var at = AsString(a[0]);
            return j.AddValue(at);
        }

        [Prototype("endsWith")]
        internal static JSValue EndsWith(JSValue t, params JSValue[] a)
        {
            var j = t as JSString;
            var s = a[0] as JSString;
            var n = a.TryGetAt(1, out var i) ? i.IntValue : -1;
            if (j == null || s == null)
                return JSUndefined.Value;
            n = Math.Min(Math.Max(0, n),j.Length);
            if (s.Length > n)
                return JSBooleanPrototype.False;
            var ar = new JSValue[2];
            ar[0].AddValue(n-s.Length);
            ar[1].AddValue(s.Length);
            if (Substring(j, ar) == s)
                return JSBooleanPrototype.True;
            else
                return JSBooleanPrototype.False;
        }

        [Prototype("startsWith")]
        internal static JSValue StartsWith(JSValue t, params JSValue[] a)
        {
            var j = t as JSString;
            var s = a[0] as JSString;
            var n = a.TryGetAt(1, out var i) ? i.IntValue : -1;
            if (j == null || s == null)
                return JSUndefined.Value;
            n = Math.Min(Math.Max(0, n), j.Length);
            if (s.Length > n)
                return JSBooleanPrototype.False;
            var ar = new JSValue[2];
            ar[0].AddValue(n);
            ar[1].AddValue(s.Length);
            if (Substring(j, ar) == s)
                return JSBooleanPrototype.True;
            else
                return JSBooleanPrototype.False;
        }

        [Prototype("includes")]
        internal static JSValue Includes(JSValue t, params JSValue[] a)
        {
            var j = t as JSString;
            var s = a[0] as JSString;
            var n = a.TryGetAt(1, out var i) ? i.IntValue : -1;
            if (j == null || s == null)
                return JSUndefined.Value;
            else
                return ()
        }

        [Prototype("indexOf")]
        internal static JSValue IndexOf(JSValue t, params JSValue[] a)
        {
            var j = t as JSArray;
            var s = a[0] as JSArray;

            if (j == null || s == null|| j.Length == 0 || s.Length == 0)
                return JSUndefined.Value;
            if (s.Length > j.Length)
                return j.AddValue(-1);
            if (!a.TryGetAt(1, out var index))
            {
                var count = new UInt32();
                var f = false;
                count = 0;
                foreach(var k in j.AllElements)
                {
                    if(count < s.Length)
                    foreach (var l in s.AllElements)
                    {
                        if (l == k)
                        {
                            f = true;
                            count++;
                        }
                        else
                        {
                            f = false;
                            count = 0;
                            break;
                        }
                    }
                    
                }
                return 
            } else {
                return
            }
        }

        [Prototype("lastIndexOf")]
        internal static JSValue LastIndexOF(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("match")]
        internal static JSValue Match(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("matchAll")]
        internal static JSValue MatchAll(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("normalize")]
        internal static JSValue Normalize(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("padEnd")]
        internal static JSValue PadEnd(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("padStart")]
        internal static JSValue PadStart(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("repeat")]
        internal static JSValue Repeat(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("replace")]
        internal static JSValue Replace(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("replaceAll")]
        internal static JSValue ReplaceAll(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("search")]
        internal static JSValue Search(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("slice")]
        internal static JSValue Slice(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("split")]
        internal static JSValue Split(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("toLocaleLowerCase")]
        internal static JSValue ToLocaleLowerCase(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("toLocaleUpperCase")]
        internal static JSValue ToLocaleUpperCase(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("toLowerCase")]
        internal static JSValue ToLowerCase(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("toUpperCase")]
        internal static JSValue ToUpperCase(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("trim")]
        internal static JSValue Trim(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("trimEnd")]
        internal static JSValue TrimEnd(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("trimstart")]
        internal static JSValue TrimStart(JSValue t, params JSValue[] a)
        {
            return
        }

        [Prototype("valueOf")]
        internal static JSValue ValueOf(JSValue t, params JSValue[] a)
        {
            return
        }
    }
}
