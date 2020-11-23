using Esprima;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using YantraJS.Extensions;
using YantraJS.Utils;

namespace YantraJS.Core
{
    public static class JSStringPrototype
    {

        private static JSString AsJSString(this JSValue v, 
            [CallerMemberName] string helper = null)
        {
            if (v.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError($"String.prototype.{helper} called on null or undefined");
            if (v is JSString str)
                return str;
            return new JSString(v.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string AsString(this JSValue v,
            [CallerMemberName] string helper = null)
        {
            if (v.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError($"String.prototype.{helper} called on null or undefined");
            return v.ToString();
        }

        [Constructor(Length = 1)]
        public static JSValue Constructor(in Arguments a)
        {
            if (a.Length == 0)
                return new JSString(StringSpan.Empty);

            return new JSString(a.Get1().ToString());

        }

        [Prototype("charAt", Length =1)]
        public static JSValue CharAt(in Arguments a)
        {
            var text = AsString(a.This);
            //int at = a.TryGetAt(0, out var n) ? (!n.IsNullOrUndefined ? n.IntValue : 0) : 0;
            var at = a.TryGetAt(0, out var n) ? n.DoubleValue : 0;
            if (double.IsNaN(at))
                at = 0;

            if (at < 0 || at >= text.Length)
                return JSString.Empty;
          
            return new JSString(new string(text[(int)(uint)at], 1));
        }

        [Prototype("substring")]
        public static JSValue Substring(in Arguments a) 
        {
            var @this = a.This.AsString();
            int start = a.GetIntAt(0, 0);
            int end = a.GetIntAt(1, int.MaxValue);
            var si = Math.Max(Math.Min(start, end), 0);
            var ei = Math.Max(Math.Max(start, end), 0);
            if (si < 0)
            {
                si += @this.Length;
            }
            if(ei < 0)
            {
                ei += @this.Length;
            }
            si = Math.Min(Math.Max(si, 0), @this.Length);
            ei = Math.Min(Math.Max(ei, 0), @this.Length);
            if (end <= start)
                return new JSString(StringSpan.Empty);

            return new JSString(@this.Substring(si, ei - si));
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

        [Prototype("charCodeAt", Length =1)]
        internal static JSValue CharCodeAt(in Arguments a)
        {
            var text = AsString(a.This);
            var at = a.TryGetAt(0, out var n) ? n.DoubleValue : 0;
            if (at < 0 || at >= text.Length)
                return JSNumber.NaN;

            return new JSNumber(text[(int)(uint)at]);
        }

        [Prototype("codePointAt", Length =1)]
        internal static JSValue CodePointAt(in Arguments a)
        {
            var text = AsString(a.This);
            var at = a.TryGetAt(0, out var n) ? n.DoubleValue : 0;
            if (at < 0 || at >= text.Length)
                return JSNumber.NaN;
            int firstCodePoint = text[(int)(uint)at];
            if (firstCodePoint < 0xD800 || firstCodePoint > 0xDBFF || at + 1 == text.Length)
                return new JSNumber(firstCodePoint);
            int secondCodePoint = text[(int)(uint)(at + 1)];
            if (secondCodePoint < 0xDC00 || secondCodePoint > 0xDFFF)
                return new JSNumber(firstCodePoint);
            var output = (double)((firstCodePoint - 0xD800) * 1024 + (secondCodePoint - 0xDC00) + 0x10000);
            return new JSNumber(output);

        }

        [Prototype("concat", Length = 1)]
        internal static JSValue Concat(in Arguments a)
        {
            var @this = a.This.AsString();
            if (a.Length == 0)
                return a.This;
            StringBuilder sb = new StringBuilder();
            sb.Append(@this);
            for (int i = 0; i < a.Length; i++)
            {
                sb.Append(a.GetAt(i));
            }
                return new JSString( sb.ToString() );
        }

        [Prototype("endsWith")]
        internal static JSValue EndsWith(in Arguments a)
        {
            var @this = a.This.AsString();
            var f = a.Get1();
            return @this.EndsWith(f.ToString()) ? JSBoolean.True : JSBoolean.False;
        }

        [Prototype("startsWith")]
        internal static JSValue StartsWith(in Arguments a)
        {
            var @this = a.This.AsString();
            var text = a.Get1();
            return @this.StartsWith(text.ToString()) ? JSBoolean.True : JSBoolean.False;
        }

        [Prototype("includes")]
        internal static JSValue Includes(in Arguments a)
        {
            var @this = a.This.AsString();
            var (text, param) = a.Get2();
            return @this.IndexOf(text.ToString(), param.IntValue) >= 0 ? JSBoolean.True : JSBoolean.False;
        }

        [Prototype("indexOf")]
        internal static JSValue IndexOf(in Arguments a)
        {
            var @this = a.This.AsString();
            var (text, start) = a.Get2();
            var startIndex = start.IsUndefined ? 0 : start.IntValue;
            var index = @this.IndexOf(text.ToString(), startIndex);
            return new JSNumber(index);
        }

        [Prototype("lastIndexOf")]
        internal static JSValue LastIndexOF(in Arguments a)
        {
            var @this = a.This.AsString();
            var (text, param) = a.Get2();
            if (param.IsUndefined)
                return new JSNumber(@this.LastIndexOf(text.ToString()));
            else
                return new JSNumber(@this.LastIndexOf(text.ToString(),param.IntValue));
        }

        [Prototype("match")]
        internal static JSValue Match(in Arguments a)
        {
            var @this = a.This.AsString();
            var reg = a.Get1();
            var r = Regex.Match(@this, reg.ToString()).Value;
            return new JSString(r);
        }

        /*[Prototype("matchAll")]
        internal static JSValue MatchAll(in Arguments a)
        {
            return
        }*/

        [Prototype("normalize")]
        internal static JSValue Normalize(in Arguments a)
        {
            var @this = a.This.AsString();
            var form = a.Get1().ToString();
            switch (form) 
            {
                case "NFC":
                    return new JSString(@this.Normalize(NormalizationForm.FormC));
                case "NFD":
                    return new JSString(@this.Normalize(NormalizationForm.FormD));
                case "NFKC":
                    return new JSString(@this.Normalize(NormalizationForm.FormKC));
                case "NFKD":
                    return new JSString(@this.Normalize(NormalizationForm.FormKD));
                default:
                    return new JSString(@this.Normalize(NormalizationForm.FormC));

            }
        }

        [Prototype("padEnd")]
        internal static JSValue PadEnd(in Arguments a)
        {
            var @this = a.This.AsString();
            var (s, c) = a.Get2();
            var size = s.IntValue;
            var ch = c.ToString().ToCharArray()[0];
            return new JSString(@this.PadRight(s.IntValue, ch));
        }

        [Prototype("padStart")]
        internal static JSValue PadStart(in Arguments a)
        {
            var @this = a.This.AsString();
            var (s, c) = a.Get2();
            var ch = c.ToString().ToCharArray()[0];
            return new JSString(@this.PadLeft(s.IntValue, ch));
        }

        [Prototype("repeat")]
        internal static JSValue Repeat(in Arguments a)
        {
            var @this = a.This.AsString();
            var c = a.Get1();
            var count = c.IsUndefined || !c.IsNumber ? 0 : c.IntValue;
            for (var i = 0; i < count; i++)
            {
                @this += @this;
            }
            return new JSString(@this);
        }

        [Prototype("replace")]
        internal static JSValue Replace(in Arguments a)
        {
            var @this = a.This.AsString();
            var (f, s) = a.Get2();
            return new JSString(@this.Replace(f.ToString(), s.ToString()));
        }

        /*[Prototype("replaceAll")]
        internal static JSValue ReplaceAll(in Arguments a)
        {
            var @this = a.This.AsString();
            var (f, s) = a.Get2();
            return new JSString(@this.ReplaceAll(f.ToString(), s.ToString()));
        }*/

        [Prototype("search")]
        internal static JSValue Search(in Arguments a)
        {
            var @this = a.This.AsString();
            var search = a.Get1();
            var reg = Regex.Match(@this, search.ToString());
            var index = @this.IndexOf(reg.ToString().ToCharArray()[0]);
            return new JSNumber(index);
        }

        [Prototype("slice")]
        internal static JSValue Slice(in Arguments a)
        {
            var @this = a.This.AsString();
            var (f, s) = a.Get2();
            return new JSString(@this.Slice(f.IntValue ,s.IntValue));
        }

        [Prototype("split")]
        internal static JSValue Split(in Arguments a)
        {
            var @this = a.This.AsString();
            var (_separator, limit) = a.Get2();
            JSArray array;
            if (!_separator.BooleanValue)
            {
                array = new JSArray(@this.Length);
                foreach(var ch in @this)
                {
                    array[array._length++] = new JSString(ch);
                }
                return array;
            }
            var separator = _separator.ToString();
            var tokens = @this.Split(new string[] { separator }, limit.AsInt32OrDefault(int.MaxValue), StringSplitOptions.None);
            array = new JSArray(tokens.Length);
            foreach(var token in tokens)
            {
                array[array._length++] = new JSString(token);
            }
            return array;
        }

        [Prototype("toLocaleLowerCase")]
        internal static JSValue ToLocaleLowerCase(in Arguments a)
        {
            var @this = a.This.AsString();
            return new JSString(@this.ToLower());
        }

        [Prototype("toLocaleUpperCase")]
        internal static JSValue ToLocaleUpperCase(in Arguments a)
        {
            var @this = a.This.AsString();
            return new JSString(@this.ToUpper());
        }

        [Prototype("toLowerCase")]
        internal static JSValue ToLowerCase(in Arguments a)
        {
            var @this = a.This.AsString();
            return new JSString(@this.ToLowerInvariant());
        }

        [Prototype("toUpperCase")]
        internal static JSValue ToUpperCase(in Arguments a)
        {
            var @this = a.This.AsString();
            return new JSString(@this.ToUpperInvariant());
        }

        [Prototype("trim")]
        internal static JSValue Trim(in Arguments a)
        {
            var @this = a.This.AsString();
            return new JSString(@this.Trim());
        }

        [Prototype("trimEnd")]
        internal static JSValue TrimEnd(in Arguments a)
        {
            var @this = a.This.AsString();
            return new JSString(@this.TrimEnd());
        }

        [Prototype("trimStart")]
        internal static JSValue TrimStart(in Arguments a)
        {
            var @this = a.This.AsString();
            return new JSString(@this.TrimStart());
        }

        [Prototype("valueOf")]
        internal static JSValue ValueOf(in Arguments a)
        {
            var @this = a.This.AsString();
            return new JSString(@this);
        }
    }
}
